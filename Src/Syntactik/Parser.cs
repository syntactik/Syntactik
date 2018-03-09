#region license

// Copyright © 2017 Maxim O. Trushin (trushin@gmail.com)
// This file is part of Syntactik.
// Syntactik is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// Syntactik is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Syntactik.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System.Collections.Generic;
using Syntactik.DOM;
using Syntactik.DOM.Mapped;
using Syntactik.IO;

namespace Syntactik
{
    /// <summary>
    /// Parses stream of chars, calls methods of <see cref="IPairFactory"/> to create DOM structure of Syntactik document.
    /// </summary>
    public class Parser
    {
        private readonly ICharStream _input;
        private IList<IErrorListener> _errorListeners;
        private Stack<WsaInfo> _wsaStack; //WSA mode stack.
        private readonly Stack<PairIndentInfo> _pairStack;
        private LineParsingState _lineState;
        private char _indentSymbol;
        private int _indentMultiplicity;
        private readonly IPairFactory _pairFactory;
        private readonly DOM.Module _module;
        private readonly bool _processJsonBrackets;

        /// <summary>
        /// List of error listeners.
        /// </summary>
        public virtual IList<IErrorListener> ErrorListeners =>
            _errorListeners ?? (_errorListeners = new List<IErrorListener>());

        /// <summary>
        /// Creates instance of the <see cref="Parser"/>.
        /// </summary>
        /// <param name="input">Stream of characters for parsing.</param>
        /// <param name="pairFactory">Pair factory is used to create DOM structure of the Syntactik document.</param>
        /// <param name="root"><see cref="DOM.Module"/> object is used as root of DOM structure.</param>
        /// <param name="processJsonBrackets">If true, parser will process {} and [] brackets.</param>
        public Parser(ICharStream input, IPairFactory pairFactory, DOM.Module root, bool processJsonBrackets = false)
        {
            _input = input;
            _pairFactory = pairFactory;
            _pairStack = new Stack<PairIndentInfo>();
            _pairStack.Push(new PairIndentInfo {Pair = root});
            _module = root;
            _processJsonBrackets = processJsonBrackets;
        }

        /// <summary>
        /// Starts parsing.
        /// </summary>
        /// <returns>Root object of the parsed DOM structure.</returns>
        public virtual DOM.Module ParseModule()
        {
            ResetState();
            while (_input.Next != -1) ParseLine();
            if (_lineState.State == ParserStateEnum.IndentMLS || _lineState.State == ParserStateEnum.Value) ParseMlStringIndent(); //Ending unfinished strings
            ExitNonBlockPair();
            while (_pairStack.Peek().Indent >= 0) EndPair(new Interval(_input), true); //Ending all pairs with flag endedByEof = true
            if (_wsaStack.Count > 0) ReportSyntaxError(1, new Interval(_input), "Closing parenthesis"); //Reporting unclosed parenthesis
            _module.IndentMultiplicity = _indentMultiplicity;
            _module.IndentSymbol = _indentSymbol;
            return (DOM.Module) _pairStack.Peek().Pair;
        }

        /// <summary>
        /// Parses one line.
        /// WSA region is treated as one line.
        /// </summary>
        private void ParseLine()
        {
            _lineState.Reset();
            while (_input.Next != -1)
            {
                switch (_lineState.State)
                {
                    case ParserStateEnum.Indent:
                        ParseIndent();
                        break;
                    case ParserStateEnum.Name:
                        ParseName();
                        break;
                    case ParserStateEnum.Assignment:
                        ParseAssignment();
                        break;
                    case ParserStateEnum.Value:
                        ParseValue();
                        break;
                    case ParserStateEnum.PairDelimiter:
                        ParsePairDelimiter();
                        break;
                    case ParserStateEnum.IndentMLS:
                        ParseMlStringIndent();
                        break;
                }
                if (_wsaStack.Count != 0) continue; //don't end line if this is wsa
                if (_lineState.State == ParserStateEnum.Value //Value state must be processed even if this is eol.
                    || _lineState.State == ParserStateEnum.IndentMLS) //ML string continues on the next line.
                    continue; 
                if (!ConsumeEol()) continue; //line is not ended.
                //line ended with eol
                ExitAllInlinePairs();//Exit all inline pair cause line is ended
                if (_input.Next == -1) 
                {
                    //This is part of "ended by eof logic"
                    //Code will run to this point only if file ends with EOL
                    if (_lineState.State == ParserStateEnum.IndentMLS || _lineState.State == ParserStateEnum.Value)
                    {
                        ParseMlStringIndent(); //Ending unfinished strings
                        _lineState.State = ParserStateEnum.PairDelimiter;
                    }
                    else
                        ParseIndent(); //Ending all pairs.
                }
                break;
            }
        }

        //Ends all inline pairs
        private void ExitAllInlinePairs()
        {
            if (_lineState.CurrentPair == MappedPair.EmptyPair) _lineState.CurrentPair = null;
            if (_lineState.State == ParserStateEnum.IndentMLS) return; //inline pair can't have ML String
            while (_pairStack.Count > 0)
            {
                var pi = _pairStack.Peek();
                if (_lineState.CurrentPair != null)
                {
                    var newPair = AppendCurrentPair();
                    _pairFactory.EndPair(newPair, new Interval(GetPairEnd((IMappedPair) newPair)));
                }
                if (pi.Pair.Assignment == AssignmentEnum.E || pi.Pair.Assignment == AssignmentEnum.EE ||
                    pi.Pair.Assignment == AssignmentEnum.CE || pi.Pair.Assignment == AssignmentEnum.None)
                {
                    EndPair(new Interval(_input));
                }
                else
                {
                    if (pi.Indent <= _lineState.Indent) break;
                    EndPair(new Interval(_input));
                }
            }
            _lineState.ChainingStarted = false;
        }

        private bool ConsumeEol()
        {
            if (_input.Next == '\r') _input.Consume();
            if (_input.Next != '\n') return false;
            _input.Consume();
            return true;
        }

        private void ParseValue()
        {
            var p = _lineState.CurrentPair;
            if (p.Assignment != AssignmentEnum.E && p.Assignment != AssignmentEnum.EE)
            {
                //No literal value
                _lineState.State = ParserStateEnum.PairDelimiter;
                return;
            }
            _input.ConsumeSpaces();
            if (_input.ConsumeComments(_pairFactory, _pairStack.Peek().Pair))
            {
                AssignValueToCurrentPair(CharLocation.Empty, CharLocation.Empty);
            }
            else if (_input.Next == '\'')
            {
                ParseSQValue();
            }
            else if (_input.Next == '"')
            {
                ParseDQValue();
            }
            else if (p.Assignment == AssignmentEnum.E && _wsaStack.Count == 0)
            {
                ParseFreeOpenString();
            }
            else
                ParseOpenString();

            if (_lineState.State != ParserStateEnum.IndentMLS)
                _lineState.State = ParserStateEnum.PairDelimiter;
        }

        /// <summary>
        /// Parses a next line of a multiline string value.
        /// </summary>
        private void ParseMlValue()
        {
            var currentPair = _lineState.CurrentPair;
            var startQuote = currentPair.ValueQuotesType;
            while (true)
            {
                if (_input.Next == startQuote)
                {
                    //Quoted ML string ended
                    _input.Consume();
                    currentPair.ValueInterval = new Interval(currentPair.ValueInterval.Begin, new CharLocation(_input));
                    AssignValueToCurrentPair(currentPair.ValueInterval.Begin, currentPair.ValueInterval.End);
                    var newPair = AppendCurrentPair();
                    _pairFactory.EndPair(newPair, new Interval(GetPairEnd((IMappedPair) newPair)));
                    EnsureNothingElseBeforeEol();
                    _lineState.State = ParserStateEnum.PairDelimiter;
                    return;
                }

                if (_input.Next == '\\' && startQuote == '"') //escape symbol in DQS
                {
                    _input.Consume();
                }

                if (_input.Next.IsNewLineCharacter())
                {
                    currentPair.ValueInterval = new Interval(currentPair.ValueInterval.Begin, new CharLocation(_input));
                    return;
                }

                if (_input.Next == -1)
                {
                    currentPair.ValueInterval = new Interval(currentPair.ValueInterval.Begin, new CharLocation(_input));
                    if (startQuote > 0)
                    {
                        ReportMLSSyntaxError(1, new Interval(_input), startQuote);
                        AssignValueToCurrentPair(currentPair.ValueInterval.Begin, currentPair.ValueInterval.End, true);
                    }
                    else
                    {
                        AssignValueToCurrentPair(currentPair.ValueInterval.Begin, currentPair.ValueInterval.End);
                    }
                    _lineState.State = ParserStateEnum.PairDelimiter;
                    return;
                }
                _input.Consume();
                if (currentPair.ValueInterval == null || currentPair.ValueInterval.Begin.Index == -1)
                    currentPair.ValueInterval = new Interval(new CharLocation(_input), new CharLocation(_input));
            }
        }

        private void EndPair(Interval interval, bool endedByEof = false)
        {
            _pairFactory.EndPair(_pairStack.Pop().Pair, interval, endedByEof);
        }

        //Creates new pair and appends it to parent
        private Pair AppendCurrentPair()
        {
            var pair = _lineState.CurrentPair;
            var newPair = _pairFactory.CreateMappedPair((ITextSource) _input, pair.NameQuotesType,
                ((IMappedPair) pair).NameInterval, pair.Assignment, ((IMappedPair) pair).AssignmentInterval,
                pair.ValueQuotesType, ((IMappedPair) pair).ValueInterval,
                _lineState.Indent + (_indentMultiplicity > 0 ? _indentMultiplicity : 1));

            _pairFactory.AppendChild(_pairStack.Peek().Pair, newPair);
            _lineState.ChainingStarted = false;
            _lineState.CurrentPair = null;
            return newPair;
        }

        private void AppendAndPushCurrentPair()
        {
            var newPair = AppendCurrentPair();
            _pairStack.Push(new PairIndentInfo {Pair = newPair, Indent = _lineState.Indent, BlockIndent = -1});
        }

        /// <summary>
        /// Consumes till EOL or EOF. Reports any symbols other than spaces as unexpected.
        ///  </summary>
        private void EnsureNothingElseBeforeEol()
        {
            var c = _input.Next;
            while (c != -1 && !c.IsNewLineCharacter())
            {
                if (!(_input.ConsumeSpaces() || _input.ConsumeComments(_pairFactory, _pairStack.Peek().Pair)))
                {
                    _input.Consume();
                    ReportUnexpectedCharacter(c);
                }
                c = _input.Next;
            }
        }

        private void ParseFreeOpenString()
        {
            _input.ConsumeSpaces();
            CharLocation begin = CharLocation.Empty;
            int endLine = 0, endColumn = 0, endIndex = -1;
            while (_input.Next != -1)
            {
                if (_input.Next.IsNewLineCharacter())
                {
                    if (begin.Index == -1)
                    {
                        begin = new CharLocation(_input.Line, _input.Column + 1, _input.Index + 1);
                    }
                    if (_wsaStack.Count < 1 && !_lineState.Inline) //ML String is not allowed for inline pairs and in WSA
                    {
                        _lineState.State = ParserStateEnum.IndentMLS;
                        AssignValueToCurrentPair(begin, new CharLocation(endLine, endColumn, endIndex));
                        return;
                    }
                    break;
                }
                if (!_input.Next.IsSpaceCharacter()) //Ignoring leading and trailing spaces
                {
                    _input.Consume();
                    if (begin.Index == -1) begin = new CharLocation(_input);
                    endLine = _input.Line;
                    endColumn = _input.Column;
                    endIndex = _input.Index;
                }
                else
                {
                    _input.Consume();
                }
            }
            AssignValueToCurrentPair(begin, new CharLocation(endLine, endColumn, endIndex));
        }


        private void ParseOpenString()
        {
            var c = _input.Next;
            CharLocation begin = CharLocation.Empty;
            int endLine = 0, endColumn = 0, endIndex = -1;
            while (true)
            {
                if (c.IsEndOfOpenString(_processJsonBrackets) || c == -1)
                {
                    AssignValueToCurrentPair(begin, new CharLocation(endLine, endColumn, endIndex));
                    return;
                }

                if (c.IsNewLineCharacter())
                {
                    if (begin.Index == -1)
                    {
                        begin = new CharLocation(_input.Line, _input.Column + 1, _input.Index + 1);
                        endLine = 0;
                        endColumn = 0;
                        endIndex = -1;
                    }
                    if (_wsaStack.Count < 1 && !_lineState.Inline) //ML String is not allowed for inline pairs and in WSA
                    {
                        _lineState.State = ParserStateEnum.IndentMLS;
                        AssignValueToCurrentPair(begin, new CharLocation(endLine, endColumn, endIndex));
                    }
                    else
                    {
                        AssignValueToCurrentPair(begin, new CharLocation(endLine, endColumn, endIndex));
                        return;
                    }
                    break;
                }
                _input.Consume();

                if (!c.IsSpaceCharacter())
                {
                    if (begin.Index == -1) begin = new CharLocation(_input);
                    endLine = _input.Line;
                    endColumn = _input.Column;
                    endIndex = _input.Index;
                }
                c = _input.Next;
            }
        }

        private void AssignValueToCurrentPair(CharLocation begin, CharLocation end, bool missingQuote = false)
        {
            var pair = _lineState.CurrentPair;
            pair.ValueInterval = begin == CharLocation.Empty ? Interval.Empty : new Interval(begin, end);
            pair.MissingValueQuote = missingQuote;
        }

        /// <summary>
        /// Parses single line or multiline single quoted value.
        /// The function can be called only if the next symbol is a double quote.
        /// </summary>
        private void ParseSQValue()
        {
            _lineState.CurrentPair.ValueQuotesType = '\'';
            _input.Consume(); // Consume starting '
            var begin = new CharLocation(_input);
            var c = _input.Next;

            while (true)
            {
                if (c == '\'')
                {
                    _input.Consume();
                    AssignValueToCurrentPair(begin, new CharLocation(_input));
                    break;
                }

                if (c.IsNewLineCharacter())
                {
                    if (_wsaStack.Count < 1 && !_lineState.Inline)
                    {
                        AssignValueToCurrentPair(begin, new CharLocation(_input));
                        _lineState.State = ParserStateEnum.IndentMLS;
                        break;
                    }

                    ReportSyntaxError(1, new Interval(_input), "Single quote");
                    AssignValueToCurrentPair(begin, new CharLocation(_input), true);
                    break;
                }

                if (c == -1)
                {
                    ReportSyntaxError(1, new Interval(_input), "Single quote");
                    AssignValueToCurrentPair(begin, new CharLocation(_input), true);
                    break;
                }
                _input.Consume();
                c = _input.Next;
            }
        }

        private void ParseDQValue()
        {
            _lineState.CurrentPair.ValueQuotesType = '"';
            _input.Consume(); // Consume starting "
            var begin = new CharLocation(_input);
            var c = _input.Next;
            while (true)
            {
                if (c == '"')
                {
                    _input.Consume();
                    AssignValueToCurrentPair(begin, new CharLocation(_input));
                    break;
                }
                if (c == '\\')
                {
                    _input.Consume();
                    c = _input.Next;
                }
                if (c.IsNewLineCharacter())
                {
                    if (_wsaStack.Count < 1 && !_lineState.Inline)
                    {
                        AssignValueToCurrentPair(begin, new CharLocation(_input));
                        _lineState.State = ParserStateEnum.IndentMLS;
                        break;
                    }
                    ReportSyntaxError(1, new Interval(_input), "Double quote");
                    AssignValueToCurrentPair(begin, new CharLocation(_input), true);
                    break;
                }

                if (c == -1)
                {
                    ReportSyntaxError(1, new Interval(_input), "Double quote");
                    AssignValueToCurrentPair(begin, new CharLocation(_input), true);
                    break;
                }
                _input.Consume();
                c = _input.Next;
            }
        }

        /// <summary>
        /// Recognizes the following assignments:
        ///  `:`  `::`  `=`  `==`  `=:`  `=::`  `:::` `:=`
        /// </summary>
        /// <returns>True if assignment found.</returns>
        private void ParseAssignment()
        {
            if (!ConsumeTillAssignment())
            {
                _lineState.State = ParserStateEnum.PairDelimiter;
                return;
            }
            var assignment = GetAssignment();

            if (assignment == AssignmentEnum.CE)
            {
                AppendAndPushCurrentPair();
                _lineState.ChainingStarted = true;
                _lineState.State = ParserStateEnum.PairDelimiter;
            }
            else if (assignment == AssignmentEnum.E || assignment == AssignmentEnum.EE ||
                     assignment == AssignmentEnum.None
            ) //Assignments followed by literal or chained pair
            {
                _lineState.State = ParserStateEnum.Value;
            }
            else // Assignments followed by block
            {
                AppendAndPushCurrentPair();
                _lineState.ChainingStarted = false;
                _lineState.State = ParserStateEnum.PairDelimiter;
                _lineState.CurrentPair = null;
                _lineState.Inline = true;
            }
        }

        /// <summary>
        /// This function is called only if next character is : or =
        /// </summary>
        /// <returns></returns>
        private AssignmentEnum GetAssignment()
        {
            AssignmentEnum assignment;
            if (_input.Next == ':')
            {
                _input.Consume();
                var begin = new CharLocation(_input);

                if (_input.Next == ':')
                {
                    _input.Consume();
                    if (_input.Next == ':')
                    {
                        _input.Consume();
                        assignment = AssignmentEnum.CCC;
                    }
                    else assignment = AssignmentEnum.CC;
                }
                else
                {
                    if (_input.Next == '=')
                    {
                        _input.Consume();
                        assignment = AssignmentEnum.CE;
                    }
                    else assignment = AssignmentEnum.C;
                }
                _lineState.CurrentPair.AssignmentInterval = new Interval(begin, new CharLocation(_input));
            }
            else // =
            {
                _input.Consume();
                var begin = new CharLocation(_input);
                _lineState.CurrentPair.AssignmentInterval = new Interval(_input);
                if (_input.Next == '=')
                {
                    _input.Consume();
                    assignment = AssignmentEnum.EE;
                }
                else if (_input.Next == ':') // =:
                {
                    _input.Consume();
                    if (_input.Next == ':') // =::
                    {
                        _input.Consume();
                        assignment = AssignmentEnum.ECC;
                    }
                    else
                    {
                        assignment = AssignmentEnum.EC;
                    }
                }
                else
                {
                    assignment = AssignmentEnum.E;
                }
                _lineState.CurrentPair.AssignmentInterval = new Interval(begin, new CharLocation(_input));
            }
            _lineState.CurrentPair.Assignment = assignment;
            EnsureAssignmentEnds();
            return assignment;
        }

        private void ExitPair()
        {
            if (_lineState.CurrentPair == MappedPair.EmptyPair)
            {
                _lineState.CurrentPair = null;
                return;
            }
            var pair = _lineState.CurrentPair;
            if (pair != null)
            {
                if (_lineState.ChainingStarted)
                {
                    AppendCurrentPair();
                    EndPair(new Interval(_input));
                }
                else
                {
                    var newPair = AppendCurrentPair();
                    _pairFactory.EndPair(newPair, new Interval(GetPairEnd((IMappedPair) newPair)));
                }
            }
            else if (_pairStack.Peek().Indent == _lineState.Indent)
            {
                EndPair(new Interval(_input));
            }
            _lineState.CurrentPair = null;
        }

        private void EnsureAssignmentEnds()
        {
            var c = _input.Next;
            while (c == ':' || c == '=')
            {
                _input.Consume();
                ReportUnexpectedCharacter(c);
                c = _input.Next;
            }
        }

        /// <summary>
        /// Consumes input till assignment is met or till end of pair is found.
        /// Everything else is reported as unexpected character.
        /// </summary>
        /// <returns>If was able to find assignment.</returns>
        private bool ConsumeTillAssignment()
        {
            var c = _input.Next;
            while (c != ':' && c != '=')
            {
                if (c == -1) return false;

                if (_input.ConsumeSpaces())
                {
                }
                else if (c.IsNewLineCharacter())
                {
                    if (_wsaStack.Count > 0)
                    {
                        _input.ConsumeNewLine();
                        ConsumeLeadingSpacesInWsa();
                    }
                    else return false;
                }
                else if (c == ')' || _processJsonBrackets && (c == '}' || c == ']'))
                {
                    if (_wsaStack.Count > 0) return false;
                    _input.Consume();
                    ReportUnexpectedCharacter(c);
                }
                else return false;
                c = _input.Next;
            }
            return true;
        }

        private void ConsumeLeadingSpacesInWsa()
        {
            while (_input.Next.IsSpaceCharacter()) _input.Consume();
            
        }

        /// <summary>
        /// ParseName is called after Indent and PairAssignment.
        /// 
        /// </summary>
        /// <returns>true if eof is not reached</returns>
        private void ParseName()
        {
            var c = _input.Next;
            while (c != -1)
            {
                if (c == '\'')
                {
                    ParseSQName();
                    _lineState.State = ParserStateEnum.Assignment;
                    break;
                }
                if (c == '"')
                {
                    ParseDQName();
                    _lineState.State = ParserStateEnum.Assignment;
                    break;
                }
                if (c == '=' || c == ':')
                {
                    _lineState.CurrentPair = new MappedPair
                    {
                        Name = string.Empty,
                        NameQuotesType = 0,
                        NameInterval = new Interval(new CharLocation(_input), new CharLocation(_input.Line, _input.Column, _input.Index - 1)),
                        Assignment = AssignmentEnum.None
                    };
                    _lineState.State = ParserStateEnum.Assignment;
                    break;
                }
                if (_input.ConsumeSpaces() || _input.ConsumeComments(_pairFactory, _pairStack.Peek().Pair))
                {
                }
                else if (c.IsNewLineCharacter())
                {
                    if (_wsaStack.Count > 0)
                    {
                        _input.ConsumeNewLine();
                        ConsumeLeadingSpacesInWsa();
                    }
                    else
                        break;
                }
                else
                {
                    ParseOpenName();
                    _lineState.State = ParserStateEnum.Assignment;
                    break;
                }
                c = _input.Next;
            }
        }

        /// <summary>
        /// Open name ends with : = or end of line
        /// </summary>
        /// <returns>true if eof is not reached</returns>
        private void ParseOpenName()
        {
            var c = _input.Next;
            var begin = CharLocation.Empty;
            int endLine = 0, endColumn = 0, endIndex = -1;
            while (c != -1)
            {
                if (c.IsEndOfOpenName(_processJsonBrackets))
                {
                    break;
                }
                _input.Consume();

                if (!c.IsSpaceCharacter())
                {
                    if (begin.Index == -1) begin = new CharLocation(_input);
                    endLine = _input.Line;
                    endColumn = _input.Column;
                    endIndex = _input.Index;
                }

                c = _input.Next;
            }
            if (!_lineState.ChainingStarted)
            {
                _lineState.CurrentPair = new MappedPair
                {
                    NameInterval = begin == CharLocation.Empty ? Interval.Empty : new Interval(begin, new CharLocation(endLine, endColumn, endIndex)),
                    Assignment = AssignmentEnum.None,
                    NameQuotesType = 0
                };
            }
            else
            {
                _lineState.CurrentPair = new MappedPair
                {
                    NameInterval = new Interval(begin, new CharLocation(endLine, endColumn, endIndex)),
                    Assignment = AssignmentEnum.None,
                    NameQuotesType = 0
                };
            }
        }

        private void ParseDQName()
        {
            _input.Consume(); // Consume starting "
            var begin = new CharLocation(_input);
            var c = _input.Next;

            while (true)
            {
                if (c == '"')
                {
                    _input.Consume();
                    break;
                }
                if (c.IsNewLineCharacter() || c == -1)
                {
                    ReportSyntaxError(1, new Interval(_input), "Double quote");
                    break;
                }
                _input.Consume();
                c = _input.Next;
            }
            if (!_lineState.ChainingStarted)
            {
                _lineState.CurrentPair = new MappedPair
                {
                    NameInterval = new Interval(begin, new CharLocation(_input)),
                    Assignment = AssignmentEnum.None,
                    NameQuotesType = '"'
                };
            }
            else
            {
                _lineState.CurrentPair = new MappedPair
                {
                    NameInterval = new Interval(begin, new CharLocation(_input)),
                    Assignment = AssignmentEnum.None,
                    NameQuotesType = '"'
                };
            }
        }

        private void ParseSQName()
        {
            _input.Consume(); // Consume starting "
            var begin = new CharLocation(_input);
            var c = _input.Next;

            while (true)
            {
                if (c == '\'')
                {
                    _input.Consume();
                    break;
                }
                if (c.IsNewLineCharacter() || c == -1)
                {
                    ReportSyntaxError(1, new Interval(_input), "Single quote");
                    break;
                }
                _input.Consume();
                c = _input.Next;
            }
            _lineState.CurrentPair = new MappedPair
            {
                NameInterval = new Interval(begin, new CharLocation(_input)),
                Assignment = AssignmentEnum.None,
                NameQuotesType = '\''
            };
        }

        /// <summary>
        /// Processes interval between pairs in one line.
        /// It can include spaces, tabs, comma and parentheses.
        /// </summary>
        private void ParsePairDelimiter()
        {
            var c = _input.Next;
            while (c != -1)
            {
                if (_input.ConsumeSpaces())
                {
                }
                else if (_input.ConsumeComments(_pairFactory, _pairStack.Peek().Pair))
                {
                }
                else if (c == '(' || _processJsonBrackets && (c == '{' || c == '['))
                {
                    _lineState.Inline = true;
                    if (_lineState.CurrentPair != null)
                        ReportSyntaxError(1, new Interval(_input),
                            _wsaStack.Count > 0 ? "Comma or closing parenthesis" : "Comma");

                    ExitNonBlockPair();
                    _input.Consume();
                    var pair = _pairStack.Peek().Pair;
                    var bracketPair = _pairFactory.ProcessBrackets(pair, c, new Interval(_input));
                    if (bracketPair != pair) //Add pair create by pair factory for the bracket
                        _pairStack.Push(new PairIndentInfo
                        {
                            Pair = bracketPair,
                            Indent = _lineState.Indent,
                            BlockIndent = _pairStack.Peek().BlockIndent
                        });
                    _wsaStack.Push(new WsaInfo(bracketPair, c));
                }
                else if (c == ')' || _processJsonBrackets && (c == '}' || c == ']'))
                {
                    _input.Consume();
                    if (_wsaStack.Count > 0 && _wsaStack.Peek().ClosingBracket == c)
                    {
                        _lineState.Inline = true;
                        if (_lineState.CurrentPair != null) ExitPair();
                        _lineState.ChainingStarted = false;
                        var wsaStartPair = _wsaStack.Pop().Pair;
                        var p1 = _pairStack.Peek().Pair;
                        while (_pairStack.Count > 1 && p1 != wsaStartPair)
                        {
                            EndPair(new Interval(_input));
                            if (_pairStack.Count > 1)
                                p1 = _pairStack.Peek().Pair;
                        }

                        _pairFactory.EndPair(null, new Interval(_input)); //report end of parents

                        if ((c == '}' || c == ']') && _pairStack.Count > 1) //JSON brackets end current block
                        {
                            EndPair(new Interval(_input));
                        }
                        _lineState.CurrentPair = MappedPair.EmptyPair; // Need comma after closing bracket and before another statement.
                    }
                    else
                    {
                        ReportUnexpectedCharacter(c);
                    }
                }
                else if (c == ',')
                {
                    if (
                        _lineState.CurrentPair != null || //comma ends any pair which is in parsing state.
                        (_pairStack.Peek().Indent == _lineState.Indent //in inline block
                         && (_wsaStack.Count == 0 || _pairStack.Peek().Pair != _wsaStack.Peek().Pair)
                        ) //comma can't close block that started before current wsa block
                    )
                    {
                        var commaEndsEmptyPair = _lineState.CurrentPair == MappedPair.EmptyPair;
                        ExitPair();
                        if (!commaEndsEmptyPair && _wsaStack.Count > 0
                            && _wsaStack.Peek().Bracket == '{' && _pairStack.Count > 1)
                        {
                            EndPair(new Interval(_input)); //comma ends current block in {}. Needed for JSON
                        }
                        _input.Consume();
                    }
                    else
                    {
                        ReportUnexpectedCharacter(c);
                        _input.Consume();
                    }
                }
                else if (c.IsNewLineCharacter())
                {
                    if (_wsaStack.Count > 0)
                    {
                        _input.ConsumeNewLine();
                        ConsumeLeadingSpacesInWsa();
                    }
                    else break;
                }
                else // new pair is starting
                {
                    var p = _lineState.CurrentPair;
                    if (p == null) break;
                    _lineState.Inline = true;
                    if (p.Assignment == AssignmentEnum.E || p.Assignment == AssignmentEnum.EE || p.Assignment == AssignmentEnum.None)
                    {
                        ExitPair();
                        ReportSyntaxError(1, new Interval(_input), _wsaStack.Count > 0 ? "Comma or closing parenthesis" : "Comma");
                    }
                    break;
                }
                c = _input.Next;
            }
            _lineState.State = ParserStateEnum.Name;
        }

        private void ExitNonBlockPair()
        {
            if (_lineState.CurrentPair == MappedPair.EmptyPair) _lineState.CurrentPair = null;
            if (_lineState.CurrentPair == null) return;
            var newPair = AppendCurrentPair();
            _pairFactory.EndPair(newPair, new Interval(GetPairEnd((IMappedPair) newPair)), _input.Next == -1);
        }

        private void ReportUnexpectedCharacter(int c) => ReportSyntaxError(0, new Interval(_input), ((char) c).ToString());

        private void ReportInvalidIndentationSize(Interval interval) => new ProxyErrorListener(_errorListeners).OnError(6, interval);

        private void ReportMixedIndentation(Interval interval) => new ProxyErrorListener(_errorListeners).OnError(5, interval);

        private void ReportInvalidIndentationMultiplicity(Interval interval) => new ProxyErrorListener(_errorListeners).OnError(4, interval);

        private void ReportBlockIndentationMismatch(Interval interval) => new ProxyErrorListener(_errorListeners).OnError(3, interval);

        private void ReportSyntaxError(int code, Interval interval, params object[] args) => new ProxyErrorListener(_errorListeners).OnError(code, interval, args);

        private void ReportMLSSyntaxError(int code, Interval interval, int start) =>
            new ProxyErrorListener(_errorListeners).OnError(code, interval, start == '"' ? "Double quote" : "Single quote");

        /// <summary>
        /// Parses and processes line indent.
        /// </summary>
        private void ParseIndent()
        {
            if (_wsaStack.Count > 0) //Do not calculate indent for inline pair or wsa mode
            {
                _input.ConsumeSpaces();
                _lineState.State = ParserStateEnum.PairDelimiter;
                return;
            }
            var indent = GetIndent(0, null, out var _);
            while (_pairStack.Peek().Indent >= indent) EndPair(new Interval(_input)); //Ending pairs with bigger indent
            if (_pairStack.Peek().BlockIndent == -1) _pairStack.Peek().BlockIndent = indent;
            else
            {
                if (_input.Next != -1 /*ignore indent mismatch in the EOF*/ && _pairStack.Peek().BlockIndent != indent)
                    ReportBlockIndentationMismatch(new Interval(new CharLocation(_input.Line, 1, _input.Index - indent),
                        new CharLocation(_input)));
            }
            _lineState.Indent = indent;
            _lineState.State = ParserStateEnum.PairDelimiter;
        }


        /// <summary>
        /// Consumes indent of the multiline string line.
        /// Returns true if indent is greater than current indent (ml string continues)
        /// </summary>
        /// <returns></returns>
        private void ParseMlStringIndent()
        {
            var currentPair = _lineState.CurrentPair;
            var indent = GetIndent(_lineState.Indent, currentPair, out var endedByComment);

            if (_input.Next != -1 &&
                indent > _lineState.Indent && // Indent is greater than current indent (ml string continues).
                !endedByComment //Dedent in comments ends ML string
            )
            {
                ParseMlValue();
                return;
            }

            // At this point we know that ml string is ended.
            var valueStart = currentPair.ValueQuotesType;
            if (valueStart > 0)
            {
                //Quoted string ended by indentation (missing quote)
                currentPair.MissingValueQuote = true;
                //newPair = AppendCurrentPair();
                ReportMLSSyntaxError(1, new Interval(currentPair.ValueInterval.End, currentPair.ValueInterval.End),
                    valueStart);
            }
            else
            {
                //Multiline Open string
                if (indent == _lineState.Indent && _input.Next == '=' && _input.La(2) == '=' && _input.La(3) == '=')
                {
                    //Checking if it is ended with ===
                    _input.Consume();_input.Consume();_input.Consume(); //Consuming ===
                    currentPair.ValueInterval = new Interval(currentPair.ValueInterval.Begin, new CharLocation(_input));
                }
            }
            var newPair = AppendCurrentPair(); //Creating the new pair and appending it to parent
            //Reporting end of pair
            if (_input.Next != -1)
                _pairFactory.EndPair(newPair, new Interval(GetPairEnd((IMappedPair) newPair)));
            else
            {
                _pairFactory.EndPair(newPair,
                    indent <= _lineState.Indent
                        ? new Interval(GetPairEnd((IMappedPair) newPair))
                        : new Interval(_input),
                    _lineState.State == ParserStateEnum.Value ||
                    indent > _lineState.Indent); //Special case used in completion. Value context. True- means value is ended by EOF but not by dedent.
            }
            _lineState.Indent = indent; //Saving new value of indent in the lineState
            //Ending pair with bigger indents.
            while (_pairStack.Peek().Indent >= indent) EndPair(new Interval(_input));
            //Checking for BlockIndentationMismatch error
            if (_input.Next != -1 && //ignore indent mismatch in the EOF
                _pairStack.Peek().BlockIndent != indent)
                ReportBlockIndentationMismatch(new Interval(new CharLocation(_input.Line, 1, _input.Index - indent), new CharLocation(_input)));
            _lineState.State = ParserStateEnum.PairDelimiter;
        }

        private int GetIndent(int currentIndent, MappedPair mlPair, out bool endedByComment)
        {
            int begin = -1, end = -2, indentCounter = 1, indentSum = 0;
            endedByComment = false;
            while (_input.Next != -1)
            {
                if (_input.Next.IsSpaceCharacter())
                {
                    if (_indentSymbol == 0) //First indent defines indent standard for the whole file.
                    {
                        _indentSymbol = (char) _input.Next;
                        if (mlPair != null) _indentMultiplicity = 1; //Calculating indent for mlPair.
                    }
                    if (mlPair == null || indentCounter <= currentIndent + _indentMultiplicity)
                    {
                        indentCounter++;
                        indentSum += _input.Next;
                        _input.Consume();
                        if (begin == -1) begin = _input.Index;
                        end = _input.Index;
                    }
                    else
                    {
                        _input.Consume();
                        if (mlPair.ValueInterval == null || mlPair.ValueInterval.Begin.Index == -1)
                            mlPair.ValueInterval = new Interval(new CharLocation(_input), new CharLocation(_input));
                    }
                }
                else if (_input.ConsumeNewLine() ||
                         mlPair == null && _input.ConsumeComments(_pairFactory, _pairStack.Peek().Pair))
                {
                    indentSum = 0;
                    indentCounter = 1;
                    begin = -1;
                    end = -2;
                }
                else if (mlPair != null && end - begin < currentIndent &&
                         _input.ConsumeComments(_pairFactory, _pairStack.Peek().Pair))
                {
                    //consuming less indented comments and storing comment indent (for mlPair only)
                    endedByComment = true;
                }
                else break;
            }
            var indent = end - begin + 1;
            if (_indentMultiplicity == 0 && _input.Next != -1 && indent > 0) _indentMultiplicity = indent;
            CheckIndentErrors(indent, indentSum);
            return indent;
        }

        private void CheckIndentErrors(int indent, int indentSum)
        {
            if (_input.Next == -1) return;
            // Multiplicity of the indent symbols must be the same for the whole document
            if (_indentMultiplicity > 0 && indent % _indentMultiplicity > 0)
                ReportInvalidIndentationMultiplicity(
                    new Interval(new CharLocation(_input.Line, 1, _input.Index - indent), new CharLocation(_input)));
            //Indent must be increased exactly with number of symbols defined by indent multiplicity
            if (_indentMultiplicity > 0 && indent > _lineState.Indent &&
                indent != _lineState.Indent + _indentMultiplicity && indent % _indentMultiplicity == 0)
                ReportInvalidIndentationSize(new Interval(new CharLocation(_input.Line, 1, _input.Index - indent),
                    new CharLocation(_input)));
            //Indent must consists of the either tab or space but both are not allowed.
            if (indent > 0 && indentSum != _indentSymbol * indent)
                ReportMixedIndentation(new Interval(new CharLocation(_input.Line, 1, _input.Index - indent),
                    new CharLocation(_input)));
        }

        private void ResetState()
        {
            _input.Reset();
            _wsaStack = new Stack<WsaInfo>();
            _lineState = new LineParsingState();
            _indentMultiplicity = 0;
        }

        private static CharLocation GetPairEnd(IMappedPair child)
        {
            if (child.ValueInterval != null) return child.ValueInterval.End;
            if (child.AssignmentInterval != null) return child.AssignmentInterval.End;
            return child.NameInterval.End;
        }
    }
}