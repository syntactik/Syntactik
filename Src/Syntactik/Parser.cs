#region license

// Copyright © 2017 Maxim O. Trushin (trushin@gmail.com)
//
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
using Module = Syntactik.DOM.Module;
using Pair = Syntactik.DOM.Pair;

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
        private readonly Module _module;
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
        /// <param name="root"><see cref="Module"/> object is used as root of DOM structure.</param>
        /// <param name="processJsonBrackets">If true, parser will process {} and [] brackets.</param>
        public Parser(ICharStream input, IPairFactory pairFactory, Module root, bool processJsonBrackets = false)
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
        public virtual Module ParseModule()
        {
            ResetState();

            while (_input.Next != -1)
                ParseLine();
            if (_lineState.State == ParserStateEnum.IndentMLS) ParseMlStringIndent();
            ExitNonBlockPair();
            while (_pairStack.Peek().Indent >= 0) EndPair(new Interval(_input), true);
            if (_wsaStack.Count > 0)
                ReportSyntaxError(1, new Interval(_input), "Closing parenthesis");

            _module.IndentMultiplicity = _indentMultiplicity;
            _module.IndentSymbol = _indentSymbol;

            return (Module) _pairStack.Peek().Pair;
        }

        private void EndPair(Interval interval, bool endedByEof = false)
        {
            _pairFactory.EndPair(_pairStack.Pop().Pair, interval, endedByEof);
        }

        /// <summary>
        /// Parses one line.
        /// WSA region is treated as one line.
        /// </summary>
        private void ParseLine()
        {
            if (_lineState.State != ParserStateEnum.IndentMLS && _lineState.State != ParserStateEnum.Value)
                _lineState.Reset();
            while (_input.Next != -1)
            {
                switch (_lineState.State)
                {
                    case ParserStateEnum.Indent:
                    {
                        ParseIndent();
                        if (_input.Next != -1)
                            _lineState.State = ParserStateEnum.PairDelimiter;
                        break;
                    }
                    case ParserStateEnum.Name:
                        ParseName();
                        break;
                    case ParserStateEnum.Assignment:
                        ParseAssignment();
                        break;
                    case ParserStateEnum.Value:
                        ParseValue();
                        if (_lineState.State != ParserStateEnum.IndentMLS)
                            _lineState.State = ParserStateEnum.PairDelimiter;
                        break;
                    case ParserStateEnum.PairDelimiter:
                        ParsePairDelimiter();
                        _lineState.State = ParserStateEnum.Name;
                        break;
                    case ParserStateEnum.IndentMLS:
                        if (ParseMlStringIndent())
                        {
                            if (!ParseMlValue())
                                _lineState.State = ParserStateEnum.Indent;
                        }
                        else
                            _lineState.State = ParserStateEnum.PairDelimiter;
                        break;
                }
                if (_wsaStack.Count != 0) continue;
                if (_lineState.State != ParserStateEnum.Value && !ConsumeEol()) continue;
                ExitInlinePair();
                if (_input.Next == -1 && _lineState.State != ParserStateEnum.Indent)
                {
                    if (_lineState.State == ParserStateEnum.IndentMLS || _lineState.State == ParserStateEnum.Value)
                        ParseMlStringIndent();
                    _lineState.Reset();
                    ParseIndent();
                }
                break;
            }
        }

        private void ExitInlinePair()
        {
            if (_lineState.CurrentPair == MappedPair.EmptyPair)
            {
                _lineState.CurrentPair = null;
            }
            if (_lineState.State == ParserStateEnum.IndentMLS || _lineState.State == ParserStateEnum.Value) return;
            while (_pairStack.Count > 0)
            {
                var pi = _pairStack.Peek();

                if (_lineState.CurrentPair != null)
                {
                    //Report end of pair
                    var newPair = AppendCurrentPair();
                    _pairFactory.EndPair(newPair,
                        new Interval(
                            GetPairEnd(
                                (IMappedPair) newPair)) /*endedByEof is always false because this method is called after ConsumeEol consuming eol.*/);
                    _lineState.CurrentPair = null;
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
                return;
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
        }

        /// <summary>
        /// Parses a next line of a multiline string value.
        /// </summary>
        /// <returns>Return false if multi line value ended.</returns>
        private bool ParseMlValue()
        {
            var p = _lineState.CurrentPair;
            var valueStart = GetValueStart(p);
            while (true)
            {
                if (_input.Next == valueStart)
                {
                    //Quoted ML string ended
                    _input.Consume();
                    p.ValueInterval =
                        new Interval(((IMappedPair) p).ValueInterval.Begin, new CharLocation(_input));
                    AssignValueToCurrentPair(((IMappedPair) p).ValueInterval.Begin,
                        ((IMappedPair) p).ValueInterval.End);
                    var newPair = AppendCurrentPair();
                    //Report end of pair
                    _pairFactory.EndPair(newPair, new Interval(GetPairEnd((IMappedPair) newPair)));
                    _lineState.CurrentPair = null;
                    EnsureNothingElseBeforeEol();
                    return false;
                }

                if (_input.Next == '\\' && valueStart == '"') //escape symbol in DQS
                {
                    _input.Consume();
                }

                if (_input.Next.IsNewLineCharacter())
                {
                    p.ValueInterval =
                        new Interval(((IMappedPair) p).ValueInterval.Begin, new CharLocation(_input));
                    return true;
                }

                if (_input.Next == -1)
                {
                    p.ValueInterval =
                        new Interval(((IMappedPair) p).ValueInterval.Begin, new CharLocation(_input));
                    if (valueStart > 0)
                    {
                        ReportMLSSyntaxError(1, new Interval(_input), valueStart);
                        AssignValueToCurrentPair(((IMappedPair) p).ValueInterval.Begin,
                            ((IMappedPair) p).ValueInterval.End, true);
                    }
                    else
                    {
                        AssignValueToCurrentPair(((IMappedPair) p).ValueInterval.Begin,
                            ((IMappedPair) p).ValueInterval.End);
                    }
                    return false;
                }
                _input.Consume();
                if (((IMappedPair) p).ValueInterval == null || ((IMappedPair) p).ValueInterval.Begin.Index == -1)
                {
                    p.ValueInterval = new Interval(new CharLocation(_input), new CharLocation(_input));
                }
            }
        }

        private Pair AppendCurrentPair()
        {
            var pair = _lineState.CurrentPair;
            var newPair = _pairFactory.CreateMappedPair((ITextSource) _input, pair.NameQuotesType,
                ((IMappedPair) pair).NameInterval, pair.Assignment, ((IMappedPair) pair).AssignmentInterval,
                pair.ValueQuotesType, ((IMappedPair) pair).ValueInterval,
                _lineState.Indent + (_indentMultiplicity > 0 ? _indentMultiplicity : 1));

            _pairFactory.AppendChild(_pairStack.Peek().Pair, newPair);
            _lineState.ChainingStarted = false;
            return newPair;
        }

        private void AppendAndPushCurrentPair()
        {
            var newPair = AppendCurrentPair();
            _pairStack.Push(new PairIndentInfo {Pair = newPair, Indent = _lineState.Indent, BlockIndent = -1});
            _lineState.CurrentPair = null;
        }

        /// <summary>
        /// Consumes till EOL or EOF. Reports any symbols other than spaces as unexpected.
        ///  </summary>
        private void EnsureNothingElseBeforeEol()
        {
            var c = _input.Next;
            while (c != -1 && !c.IsNewLineCharacter())
            {
                if (_input.ConsumeSpaces() || _input.ConsumeComments(_pairFactory, _pairStack.Peek().Pair))
                {
                }
                else
                {
                    _input.Consume();
                    ReportUnexpectedCharacter(c);
                }
                c = _input.Next;
            }
        }

        /// <summary>
        /// Returns first symbol of quoted multiline string.
        /// If the value is open ml string the it returns -2. 
        /// </summary>
        /// <param name="mappedPair"></param>
        /// <returns></returns>
        private static int GetValueStart(MappedPair mappedPair)
        {
            if (mappedPair.ValueQuotesType == (int) QuotesEnum.Double) return '"';
            if (mappedPair.ValueQuotesType == (int) QuotesEnum.Single) return '\'';
            return -2;
        }

        private void ParseFreeOpenString()
        {
            _input.ConsumeSpaces();
            var begin = CharLocation.Empty;
            var end = CharLocation.Empty;
            while (_input.Next != -1)
            {
                if (_input.Next.IsNewLineCharacter())
                {
                    if (begin.Index == -1)
                    {
                        begin = new CharLocation(_input.Line, _input.Column + 1, _input.Index + 1);
                        end = CharLocation.Empty;
                    }
                    if (_wsaStack.Count < 1 && !_lineState.Inline)
                    {
                        _lineState.State = ParserStateEnum.IndentMLS;
                        AssignValueToCurrentPair(begin, end);
                        return;
                    }
                    break;
                }
                if (!_input.Next.IsSpaceCharacter()) //Ignoring leading and trailing spaces
                {
                    _input.Consume();
                    if (begin.Index == -1) begin = new CharLocation(_input);
                    end = new CharLocation(_input);
                }
                else
                {
                    _input.Consume();
                }
            }
            AssignValueToCurrentPair(begin, end);
        }


        private void ParseOpenString()
        {
            var c = _input.Next;
            var begin = CharLocation.Empty;
            var end = CharLocation.Empty;
            while (true)
            {
                if (c.IsEndOfOpenString(_processJsonBrackets) || c == -1)
                {
                    AssignValueToCurrentPair(begin, end);
                    return;
                }

                if (c.IsNewLineCharacter())
                {
                    if (begin.Index == -1)
                    {
                        begin = new CharLocation(_input.Line, _input.Column + 1, _input.Index + 1);
                        end = CharLocation.Empty;
                    }
                    if (_wsaStack.Count < 1 && !_lineState.Inline)
                    {
                        _lineState.State = ParserStateEnum.IndentMLS;
                        AssignValueToCurrentPair(begin, end);
                    }
                    else
                    {
                        AssignValueToCurrentPair(begin, end);
                        return;
                    }
                    break;
                }
                _input.Consume();

                if (!c.IsSpaceCharacter())
                {
                    if (begin.Index == -1) begin = new CharLocation(_input);
                    end = new CharLocation(_input);
                }
                c = _input.Next;
            }
        }

        private void AssignValueToCurrentPair(CharLocation begin, CharLocation end, bool missingQuote = false)
        {
            var pair = _lineState.CurrentPair;
            pair.ValueInterval =
                begin == CharLocation.Empty ? Interval.Empty : new Interval(begin, end);
            pair.MissingValueQuote = missingQuote;
        }

        /// <summary>
        /// Parses single line or multiline single quoted value.
        /// The function can be called only if the next symbol is a double quote.
        /// </summary>
        private void ParseSQValue()
        {
            _lineState.CurrentPair.ValueQuotesType = (int) QuotesEnum.Single;
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
            _lineState.CurrentPair.ValueQuotesType = (int) QuotesEnum.Double;
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
            else if (assignment == AssignmentEnum.E || assignment == AssignmentEnum.EE || assignment == AssignmentEnum.None
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
                _lineState.CurrentPair.AssignmentInterval =
                    new Interval(begin, new CharLocation(_input));
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
                _lineState.CurrentPair.AssignmentInterval =
                    new Interval(begin, new CharLocation(_input));
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
                    _pairFactory.AppendChild(_pairStack.Peek().Pair, _pairFactory.CreateMappedPair((ITextSource) _input, pair.NameQuotesType,
                        ((IMappedPair) pair).NameInterval, pair.Assignment, ((IMappedPair) pair).AssignmentInterval,
                        pair.ValueQuotesType, ((IMappedPair) pair).ValueInterval,
                        _lineState.Indent + (_indentMultiplicity > 0 ? _indentMultiplicity : 1)));

                    _lineState.ChainingStarted = false;
                    EndPair(new Interval(_input));
                }
                else
                {
                    var newPair = AppendCurrentPair();
                    //Report end of pair
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
                if (c == -1)
                {
                    _lineState.State = ParserStateEnum.PairDelimiter;
                    return false;
                }

                if (_input.ConsumeSpaces())
                {
                }
                else if (c.IsNewLineCharacter())
                {
                    if (_wsaStack.Count > 0)
                    {
                        _input.ConsumeNewLine();
                        ConsumeLeadingSpaces();
                    }
                    else
                    {
                        _lineState.State = ParserStateEnum.PairDelimiter;
                        return false;
                    }
                }
                else if (c == ')' || _processJsonBrackets && (c == '}' || c == ']'))
                {
                    if (_wsaStack.Count > 0)
                    {
                        _lineState.State = ParserStateEnum.PairDelimiter;
                        return false;
                    }
                    _input.Consume();
                    ReportUnexpectedCharacter(c);
                }
                else if (c == '(' || _processJsonBrackets && (c == '{' || c == '['))
                {
                    ExitNonBlockPair();
                    _input.Consume();
                    _wsaStack.Push(new WsaInfo(_pairStack.Peek().Pair, c));
                    _pairFactory.ProcessBrackets(_pairStack.Peek().Pair, c, new Interval(_input));
                    return false;
                }
                else
                {
                    _lineState.State = ParserStateEnum.PairDelimiter;
                    return false;
                }
                c = _input.Next;
            }
            return true;
        }

        private void ConsumeLeadingSpaces()
        {
            var c = _input.Next;
            if (c == '\t' || c == ' ')
            {
                if (_indentSymbol == 0) _indentSymbol = (char) c;
            }
            else return;

            int indentSum = c;
            var indent = 1;
            while (true)
            {
                _input.Consume();
                c = _input.Next;
                if (!c.IsSpaceCharacter()) break;
                indent++;
                indentSum += c;
            }

            if (indentSum != _indentSymbol * indent)
                ReportInvalidIndentation(new Interval(new CharLocation(_input.Line, 1, _input.Index - indent),
                    new CharLocation(_input)));
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
                else if (c == '"')
                {
                    ParseDQName();
                    _lineState.State = ParserStateEnum.Assignment;
                    break;
                }
                else if (c == '(' || c == ')' || c == ',' || _processJsonBrackets && (c == '{' || c == '[' || c == ']' || c == '}'))
                {
                    _input.Consume();
                    ReportUnexpectedCharacter(c);
                }
                else if (c == '=' || c == ':')
                {
                    _lineState.CurrentPair = new MappedPair
                    {
                        Name = string.Empty,
                        NameQuotesType = 0,
                        NameInterval = new Interval(new CharLocation(_input),
                            new CharLocation(_input.Line, _input.Column, _input.Index - 1)),
                        Assignment = AssignmentEnum.None
                    };
                    _lineState.State = ParserStateEnum.Assignment;
                    break;
                }
                else if (_input.ConsumeSpaces() || _input.ConsumeComments(_pairFactory, _pairStack.Peek().Pair))
                {
                }
                else if (c.IsNewLineCharacter())
                {
                    if (_wsaStack.Count > 0)
                    {
                        _input.ConsumeNewLine();
                        ConsumeLeadingSpaces();
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
            var end = CharLocation.Empty;
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
                    end = new CharLocation(_input);
                }

                c = _input.Next;
            }
            if (!_lineState.ChainingStarted)
            {
                _lineState.CurrentPair = new MappedPair
                {
                    NameInterval = begin == CharLocation.Empty ? Interval.Empty : new Interval(begin, end),
                    Assignment = AssignmentEnum.None,
                    NameQuotesType = 0
                };
            }
            else
            {
                _lineState.CurrentPair = new MappedPair
                {
                    NameInterval = new Interval(begin, end),
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
                    NameQuotesType = 2
                };
            }
            else
            {
                _lineState.CurrentPair = new MappedPair
                {
                    NameInterval = new Interval(begin, new CharLocation(_input)),
                    Assignment = AssignmentEnum.None,
                    NameQuotesType = 2
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
                NameQuotesType = 1
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
                    _wsaStack.Push(new WsaInfo(_pairStack.Peek().Pair, c));
                    _pairFactory.ProcessBrackets(_pairStack.Peek().Pair, c, new Interval(_input));
                }
                else if (c == ')' || _processJsonBrackets && (c == '}' || c == ']'))
                {
                    _lineState.Inline = true;

                    _input.Consume();
                    if (_wsaStack.Count > 0 && _wsaStack.Peek().ClosingBracket == c)
                    {
                        if (_lineState.CurrentPair != null)
                        {
                            ExitPair();
                        }
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
                        _lineState.CurrentPair = MappedPair.EmptyPair;
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
                         && (_wsaStack.Count == 0 || _pairStack.Peek().Pair != _wsaStack.Peek().Pair)) //comma can't close block that started before current wsa block
                        
                    )
                    {
                        var commaEndsEmptyPair = _lineState.CurrentPair == MappedPair.EmptyPair;
                        ExitPair();
                        if (!commaEndsEmptyPair && _wsaStack.Count > 0
                            && _wsaStack.Peek().Bracket == '{' && _pairStack.Count > 1)
                        {
                            EndPair(new Interval(_input));
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
                        ConsumeLeadingSpaces();
                    }
                    else
                    {
                        break;
                    }
                }
                else // new pair is starting
                {
                    var p = _lineState.CurrentPair;
                    if (p == null) break;
                    _lineState.Inline = true;
                    if (p.Assignment == AssignmentEnum.E ||
                        p.Assignment == AssignmentEnum.EE ||
                        p.Assignment == AssignmentEnum.None)
                    {
                        ExitPair();

                        ReportSyntaxError(1, new Interval(_input),
                            _wsaStack.Count > 0 ? "Comma or closing parenthesis" : "Comma");
                    }
                    break;
                }
                c = _input.Next;
            }
        }

        private void ExitNonBlockPair()
        {
            if (_lineState.CurrentPair == MappedPair.EmptyPair)
            {
                _lineState.CurrentPair = null;
            }

            if (_lineState.CurrentPair != null)
            {
                var newPair = AppendCurrentPair();
                //Report end of pair
                _pairFactory.EndPair(newPair, new Interval(GetPairEnd((IMappedPair) newPair)), _input.Next == -1);
                _lineState.CurrentPair = null;
            }
        }

        private void ReportUnexpectedCharacter(int c)
        {
            ReportSyntaxError(0,
                new Interval(_input),
                ((char) c).ToString());
        }

        private void ReportInvalidIndentation(Interval interval)
        {
            var proxy = new ProxyErrorListener(_errorListeners);
            proxy.OnError(2, interval);
        }

        private void ReportInvalidIndentationSize(Interval interval)
        {
            var proxy = new ProxyErrorListener(_errorListeners);
            proxy.OnError(6, interval);
        }

        private void ReportMixedIndentation(Interval interval)
        {
            var proxy = new ProxyErrorListener(_errorListeners);
            proxy.OnError(5, interval);
        }

        private void ReportInvalidIndentationMultiplicity(Interval interval)
        {
            var proxy = new ProxyErrorListener(_errorListeners);
            proxy.OnError(4, interval);
        }

        private void ReportBlockIndentationMismatch(Interval interval)
        {
            var proxy = new ProxyErrorListener(_errorListeners);
            proxy.OnError(3, interval);
        }


        private void ReportSyntaxError(int code, Interval interval, params object[] args)
        {
            var proxy = new ProxyErrorListener(_errorListeners);
            proxy.OnError(code, interval, args);
        }

        private void ReportMLSSyntaxError(int code, Interval interval, int start)
        {
            var proxy = new ProxyErrorListener(_errorListeners);

            string quoteName = start == '"' ? "Double quote" : "Single quote";

            proxy.OnError(code, interval, quoteName);
        }

        /// <summary>
        /// Parsing and processes line indent.
        /// </summary>
        private void ParseIndent()
        {
            if (_wsaStack.Count > 0) //Do not calculate indent for inline pair or wsa mode
            {
                _input.ConsumeSpaces();
                return;
            }
            int indentSum = 0;
            var begin = -1;
            var end = -2;
            while (true)
            {
                if (_input.Next == '\t' || _input.Next == ' ')
                {
                    indentSum += _input.Next;
                    _input.Consume();
                    if (begin == -1) begin = _input.Index;
                    end = _input.Index;
                }
                else if (_input.ConsumeNewLine() || _input.ConsumeComments(_pairFactory, _pairStack.Peek().Pair))
                {
                    begin = -1;
                    end = -2;
                    indentSum = 0;
                    if (_input.Next == -1) break;
                }
                else
                {
                    break;
                }
            }
            ProcessIndent(begin, end, indentSum);
        }

        /// <summary>
        /// Consumes indent.
        /// Returns true if indent is greater than current indent
        /// </summary>
        /// <returns></returns>
        private bool ParseMlStringIndent()
        {
            var begin = -1;
            var end = -2;
            var p = _lineState.CurrentPair;
            int currentIndent = _lineState.Indent;
            var indentBeforeComments = -1;
            var indentCounter = 0;
            int indentSum = 0;

            while (true)
            {
                if (_input.Next == '\t' || _input.Next == ' ')
                {
                    if (_indentSymbol == 0) //First indent defines indent standard for the whole file.
                    {
                        _indentSymbol = (char) _input.Next;
                        _indentMultiplicity = 1;
                    }
                    if (_indentMultiplicity == 0) _indentMultiplicity = 1;

                    indentCounter++;
                    if (indentCounter <= currentIndent + _indentMultiplicity)
                    {
                        indentSum += _input.Next;
                        _input.Consume();
                        if (begin == -1) begin = _input.Index;
                        end = _input.Index;
                    }
                    else
                    {
                        _input.Consume();
                        if (p.ValueInterval == null ||
                            p.ValueInterval.Begin.Index == -1)
                            p.ValueInterval =
                                new Interval(new CharLocation(_input), new CharLocation(_input));
                    }
                }
                else if (_input.Next == -1)
                {
                    break;
                }
                else if (_input.Next.IsNewLineCharacter())
                {
                    _input.ConsumeNewLine();
                    indentSum = 0;
                    indentCounter = 0;
                    begin = -1;
                    end = -2;
                }
                else if (end - begin < currentIndent && _input.ConsumeComments(_pairFactory, _pairStack.Peek().Pair))
                {
                    indentBeforeComments = end - begin + 1;
                }
                else
                {
                    break;
                }
            }
            var indent = indentBeforeComments < 0 ? end - begin + 1 : indentBeforeComments;

            if (_input.Next != -1 && (indent > currentIndent))
            {
                CheckIndentErrors(indent, indentSum);
                return true;
            }
            var valueStart = GetValueStart(p);
            Pair newPair;
            if (valueStart > 0)
            {
                _lineState.CurrentPair.MissingValueQuote = true;
                newPair = AppendCurrentPair();
                var valueEnd = p.ValueInterval.End;
                ReportMLSSyntaxError(1, new Interval(valueEnd, valueEnd), valueStart);
            }
            else
            {
                if (indent == currentIndent && _input.Next == '=' && _input.La(2) == '=' && _input.La(3) == '=')
                {
                    _input.Consume();
                    _input.Consume();
                    _input.Consume();
                    var cp = _lineState.CurrentPair;
                    cp.ValueInterval = new Interval(cp.ValueInterval.Begin, new CharLocation(_input));
                }
                newPair = AppendCurrentPair();
            }
            //Report end of pair
            if (_input.Next != -1)
                _pairFactory.EndPair(newPair, new Interval(GetPairEnd((IMappedPair) newPair)));
            else
            {
                _pairFactory.EndPair(newPair,
                    indent <= currentIndent ? new Interval(GetPairEnd((IMappedPair) newPair)) : new Interval(_input),
                    _lineState.State == ParserStateEnum.Value ||
                    indent > currentIndent); //Special case used in completion. Value context. True- means value is ended by EOF but not by dedent.
            }


            _lineState.CurrentPair = null;
            indent = end - begin + 1;
            _lineState.Indent = indent;

            while (_pairStack.Peek().Indent >= indent) EndPair(new Interval(_input));

            if (_input.Next != -1 && //ignore indent mismatch in the EOF
                _pairStack.Peek().BlockIndent != indent)
                ReportBlockIndentationMismatch(new Interval(new CharLocation(_input.Line, 1, _input.Index - indent),
                    new CharLocation(_input)));

            CheckIndentErrors(indent, indentSum);
            return false;
        }

        private void ProcessIndent(int begin, int end, int indentSum)
        {
            var indent = end - begin + 1;

            if (_indentSymbol == 0 && indent > 0) //First indent defines indent standard for the whole file.
            {
                _indentSymbol = ((ITextSource) _input).GetChar(begin);
            }

            while (_pairStack.Peek().Indent >= indent) EndPair(new Interval(_input));

            if (_pairStack.Peek().BlockIndent == -1) _pairStack.Peek().BlockIndent = indent;
            else
            {
                if (_input.Next != -1 && //ignore indent mismatch in the EOF
                    _pairStack.Peek().BlockIndent != indent)
                    ReportBlockIndentationMismatch(new Interval(new CharLocation(_input.Line, 1, _input.Index - indent),
                        new CharLocation(_input)));
            }
            if (_indentMultiplicity == 0 && _input.Next != -1 && indent > 0)
            {
                _indentMultiplicity = indent;
            }
            CheckIndentErrors(indent, indentSum);
            _lineState.Indent = indent;
        }

        private void CheckIndentErrors(int indent, int indentSum)
        {
            if (_input.Next == -1) return;
            // Multiplicity of the indent symbols must be the same for the whole document
            if (_indentMultiplicity > 0 && indent % _indentMultiplicity > 0)
                ReportInvalidIndentationMultiplicity(new Interval(
                    new CharLocation(_input.Line, 1, _input.Index - indent),
                    new CharLocation(_input)));
            //Indent must be increased exactly with number of symbols defined by indent multiplicity
            if (_indentMultiplicity > 0 && indent > _lineState.Indent &&
                indent != _lineState.Indent + _indentMultiplicity &&
                indent % _indentMultiplicity == 0)
                ReportInvalidIndentationSize(new Interval(new CharLocation(_input.Line, 1, _input.Index - indent),
                    new CharLocation(_input)));
            //Indent must consists of the either tab or space but both are not allowed.
            if ((indent > 0) && indentSum != _indentSymbol * indent)
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

        private CharLocation GetPairEnd(IMappedPair child)
        {
            if (child.ValueInterval != null) return child.ValueInterval.End;
            if (child.AssignmentInterval != null) return child.AssignmentInterval.End;
            return child.NameInterval.End;
        }
    }
}