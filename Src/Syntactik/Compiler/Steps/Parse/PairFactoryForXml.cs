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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Syntactik.DOM;
using Syntactik.DOM.Mapped;
using Syntactik.IO;
using Alias = Syntactik.DOM.Mapped.Alias;
using AliasDefinition = Syntactik.DOM.Mapped.AliasDefinition;
using Argument = Syntactik.DOM.Mapped.Argument;
using Attribute = Syntactik.DOM.Mapped.Attribute;
using Comment = Syntactik.DOM.Comment;
using Document = Syntactik.DOM.Mapped.Document;
using Element = Syntactik.DOM.Mapped.Element;
using Module = Syntactik.DOM.Module;
using NamespaceDefinition = Syntactik.DOM.Mapped.NamespaceDefinition;
using Parameter = Syntactik.DOM.Mapped.Parameter;
using Scope = Syntactik.DOM.Mapped.Scope;
using ValueType = Syntactik.DOM.Mapped.ValueType;

namespace Syntactik.Compiler.Steps
{
    public class PairFactoryForXml : IPairFactory
    {
        private readonly CompilerContext _context;
        private readonly Module _module;

        public PairFactoryForXml(CompilerContext context, Module module)
        {
            _context = context;
            _module = module;
        }

        public Pair CreateMappedPair(ITextSource textSource, int nameQuotesType, Interval nameInterval,
            DelimiterEnum delimiter,
            Interval delimiterInterval, int valueQuotesType, Interval valueInterval, int valueIndent)
        {
            IMappedPair pair;
            var nameText = GetNameText(textSource, nameQuotesType, nameInterval);
            var value = GetValue(textSource, delimiter, valueQuotesType, valueInterval, valueIndent, _context, _module);
            if (nameQuotesType > 0)
            {
                pair = new Element
                {
                    Name = VerifyElementName(nameText, nameInterval, _module),
                    NameQuotesType = nameQuotesType,
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
            }
            else if (nameText.StartsWith("@"))
            {
                var tuple = Element.GetNameAndNs(nameText.Substring(1), nameQuotesType);
                var ns = string.IsNullOrEmpty(tuple.Item1) ? null : tuple.Item1;
                pair = new Attribute
                {
                    NsPrefix = ns,
                    Name = VerifyName(tuple.Item2, nameInterval, _module),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
            }
            else if (nameText.StartsWith("!$"))
            {
                pair = new AliasDefinition
                {
                    Name = VerifyName(nameText.Substring(2), nameInterval, _module),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
            }
            else if (nameText.StartsWith("!#"))
            {
                pair = new NamespaceDefinition
                {
                    Name = VerifyNsName(nameText.Substring(2), nameInterval, _module),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
            }
            else if (nameText.StartsWith("!%"))
            {
                pair = new Parameter
                {
                    Name = VerifyNsName(nameText.Substring(2), nameInterval, _module),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
            }
            else if (nameText.StartsWith("!"))
            {
                pair = new Document
                {
                    Name = VerifyName(nameText.Substring(1), nameInterval, _module),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
            }
            else if (nameText.StartsWith("$"))
            {
                pair = new Alias
                {
                    Name = VerifyName(nameText.Substring(1), nameInterval, _module),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
            }
            else if (nameText.StartsWith("%"))
            {
                pair = new Argument
                {
                    Name = VerifyName(nameText.Substring(1), nameInterval, _module),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
            }
            else if (nameText.StartsWith("#"))
            {
                var tuple = Element.GetNameAndNs(nameText.Substring(1), nameQuotesType);
                var ns = string.IsNullOrEmpty(tuple.Item1) ? null : tuple.Item1;

                if (ns == null)
                {
                    pair = new Scope
                    {
                        NsPrefix = VerifyScopeName(nameText.Substring(1), nameInterval, _module),
                        NameInterval = nameInterval,
                        Delimiter = delimiter,
                        DelimiterInterval = delimiterInterval,
                        Value = value.Item1,
                        ValueQuotesType = valueQuotesType,
                        ValueInterval = valueInterval,
                        InterpolationItems = value.Item2,
                        ValueIndent = valueIndent
                    };
                }
                else
                {
                    pair = new Scope
                    {
                        NsPrefix = VerifyScopeName(ns, nameInterval, _module),
                        Name = VerifyElementName(tuple.Item2, nameInterval, _module),
                        NameInterval = nameInterval,
                        Delimiter = delimiter,
                        DelimiterInterval = delimiterInterval,
                        Value = value.Item1,
                        ValueQuotesType = valueQuotesType,
                        ValueInterval = valueInterval,
                        InterpolationItems = value.Item2,
                        ValueIndent = valueIndent
                    };
                }
            }
            else
            {
                var tuple = Element.GetNameAndNs(nameText, nameQuotesType);
                var ns = string.IsNullOrEmpty(tuple.Item1) ? null : tuple.Item1;

                pair = new Element
                {
                    NsPrefix = VerifyScopeName(ns, nameInterval, _module),
                    Name = VerifyElementName(tuple.Item2, nameInterval, _module),
                    NameQuotesType = nameQuotesType,
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
            }
            SetValueType(pair, delimiter, value.Item1, valueQuotesType);
            return (Pair) pair;
        }

        internal static string GetNameText(ITextSource input, int nameQuotesType, Interval nameInterval)
        {
            if (nameQuotesType == 0)
                return input.GetText(nameInterval.Begin.Index, nameInterval.End.Index);
            var c = input.GetChar(nameInterval.End.Index);
            if (nameQuotesType == 1)
                return c == '\''
                    ? input.GetText(nameInterval.Begin.Index + 1, nameInterval.End.Index - 1)
                    : input.GetText(nameInterval.Begin.Index + 1, nameInterval.End.Index);

            return c == '"'
                ? input.GetText(nameInterval.Begin.Index + 1, nameInterval.End.Index - 1)
                : input.GetText(nameInterval.Begin.Index + 1, nameInterval.End.Index);
        }

        private string VerifyElementName(string name, Interval nameInterval, Module module)
        {
            if (string.IsNullOrEmpty(name)) return name;
            try
            {
                XmlConvert.VerifyNCName(name);
            }
            catch (XmlException)
            {
                _context.Errors.Add(CompilerErrorFactory.InvalidXmlElementName(nameInterval, module.FileName));
            }
            return name;
        }

        private string VerifyName(string name, Interval nameInterval, Module module)
        {
            try
            {
                XmlConvert.VerifyNCName(name);
            }
            catch (Exception)
            {
                _context.Errors.Add(CompilerErrorFactory.InvalidName(nameInterval, module.FileName));
            }
            return name;
        }

        private string VerifyNsName(string name, Interval nameInterval, Module module)
        {
            if (Regex.Match(name, @"^[a-zA-Z_][a-zA-Z0-9_\-]*$").Success) return name;
            _context.Errors.Add(CompilerErrorFactory.InvalidNsName(nameInterval, module.FileName));
            return name;
        }

        private string VerifyScopeName(string name, Interval nameInterval, Module module)
        {
            if (string.IsNullOrEmpty(name)) return name;
            return VerifyNsName(name, nameInterval, module);
        }

        private void SetValueType(IMappedPair pair, DelimiterEnum delimiter, string value, int valueQuotesType)
        {
            switch (delimiter)
            {
                case DelimiterEnum.CE:
                    pair.ValueType = ValueType.PairValue;
                    return;
                case DelimiterEnum.EC:
                    pair.ValueType = ValueType.Concatenation;
                    return;
                case DelimiterEnum.ECC:
                    pair.ValueType = ValueType.LiteralChoice;
                    return;
                case DelimiterEnum.C:
                case DelimiterEnum.CC:
                case DelimiterEnum.CCC:
                    pair.ValueType = ValueType.Object;
                    return;
            }
            if (value == null) return;

            if (valueQuotesType == 1)
            {
                pair.ValueType = ValueType.SingleQuotedString;
                return;
            }
            if (valueQuotesType == 2)
            {
                pair.ValueType = ValueType.DoubleQuotedString;
                return;
            }
            if (delimiter == DelimiterEnum.E)
            {
                pair.ValueType = GetJsonValueType(value, ValueType.FreeOpenString);
                return;
            }

            if (delimiter == DelimiterEnum.EE)
            {
                pair.ValueType = GetJsonValueType(value, ValueType.OpenString);
            }
        }

        private ValueType GetJsonValueType(string value, ValueType defaultType)
        {
            if (value == "true" || value == "false") return ValueType.Boolean;
            if (value == "null") return ValueType.Null;
            if (Regex.Match(value, @"^-?(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+-]?\d+)?$").Success) return ValueType.Number;
            return defaultType;
        }

        internal static Tuple<string, List<object>> GetValue(ITextSource input, DelimiterEnum delimiter,
            int valueQuotesType, Interval valueInterval, int valueIndent, CompilerContext context, Module module)
        {
            if (valueInterval == null)
            {
                if (delimiter == DelimiterEnum.E || delimiter == DelimiterEnum.EE)
                    return new Tuple<string, List<object>>(string.Empty, null);
                return new Tuple<string, List<object>>(null, null);
            }
            if (valueInterval.Begin.Index == -1)
            {
                return new Tuple<string, List<object>>(string.Empty, null);
            }
            if (valueQuotesType == (int) QuotesEnum.Single)
            {
                if (!ValueIsMissingQuote(input, valueQuotesType, valueInterval))
                {
                    return new Tuple<string, List<object>>(GetValueFromValueInterval(input, delimiter,
                        valueQuotesType,
                        valueInterval.Begin.Index + 1, valueInterval.End.Index - 1, valueIndent), null);
                }

                return new Tuple<string, List<object>>(GetValueFromValueInterval(input, delimiter,
                    valueQuotesType,
                    valueInterval.Begin.Index + 1, valueInterval.End.Index, valueIndent), null);
            }
            if (valueQuotesType == (int) QuotesEnum.Double)
            {
                var ii = GetInterpolationItems(input, valueInterval, context, module);
                string value = (string) (ii.Count == 1 && ii[0] is string ? ii[0] : string.Empty);
                return new Tuple<string, List<object>>(value, ii);
            }
            return new Tuple<string, List<object>>(GetValueFromValueInterval(input, delimiter,
                valueQuotesType,
                valueInterval.Begin.Index, valueInterval.End.Index, valueIndent), null);
        }

        private static bool ValueIsMissingQuote(ITextSource input, int valueQuotesType, Interval valueInterval)
        {
            var c = input.GetChar(valueInterval.End.Index);
            return valueQuotesType == (int) QuotesEnum.Single && c != '\'' ||
                   valueQuotesType == (int) QuotesEnum.Double && c != '"';
        }

        public static string GetValueFromValueInterval(ITextSource charStream, DelimiterEnum delimiter,
            int valueQuotesType, int begin, int end, int valueIndent)
        {
            var sb = new StringBuilder();
            //Splitting text. Getting array of text lines
            var lines = charStream.GetText(begin, end).Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

            bool folded = lines.Length > 1 && delimiter == DelimiterEnum.EE &&
                          (valueQuotesType == (int) QuotesEnum.None || valueQuotesType == (int) QuotesEnum.Double);

            var first = true;
            var firstEmptyLine = true; //If true then previous line was not empty therefor newline shouldn't be added
            var checkIfFirstLineIsEmpty = true;

            foreach (var item in lines)
            {
                var line = TrimEndOfFoldedStringLine(item, folded);
                if (checkIfFirstLineIsEmpty) //ignoring first empty line for open strings
                {
                    checkIfFirstLineIsEmpty = false;
                    if (valueQuotesType == (int) QuotesEnum.None && line == string.Empty)
                    {
                        first = false;
                        continue;
                    }
                }

                if (first)
                {
                    sb.Append(line);
                    first = false;
                    continue;
                } //adding first line without appending new line symbol

                if (line.Length <= valueIndent) //this is just empty line
                {
                    if (folded) //Folded string
                    {
                        if (firstEmptyLine)
                        {
                            firstEmptyLine = false;
                            continue; //Ignore first empty line for folded string
                        }
                    }
                    sb.AppendLine();
                    continue;
                }

                var lineIndent = line.TakeWhile(c => c == ' ' || c == '\t').Count();
                if (lineIndent < valueIndent) // this is multiline string terminator === or indent mismatch
                {
                    line = line.Substring(lineIndent); //Removing indents
                    if (line.TrimEnd() == "===") sb.AppendLine();
                    break; // this is multiline string terminator ===
                }

                line = line.Substring(valueIndent); //Removing indents
                if (sb.Length == 0) // If it is first line to be added just add it. No new line or spacing needed.
                {
                    sb.Append(line);
                    continue;
                }
                if (folded && firstEmptyLine) sb.Append(" ");
                if (!folded || !firstEmptyLine) sb.AppendLine();
                firstEmptyLine = true; //reseting the flag for folded string logic

                sb.Append(line);
            }
            return sb.ToString();
        }

        private static string TrimEndOfFoldedStringLine(string line, bool ignoreTrailing)
        {
            if (ignoreTrailing)
                return line.TrimEnd(' ', '\t'); //ignoring trailing whitespace for open strings
            return line;
        }

        internal static List<object> GetInterpolationItems(ITextSource input, Interval valueInterval,
            CompilerContext context, Module module)
        {
            var intItems = new List<object>();

            var i = ValueIsMissingQuote(input, 2, valueInterval) ? 0 : -1;
            var value = input.GetText(valueInterval.Begin.Index + 1, valueInterval.End.Index + i);
            var matches = Regex.Matches(value,
                //Escape characters
                @"(\\(?:[""\\\/bfnrt]|u[a-fA-F0-9]{0,4}|" +
                //Alias or parameter with brackets
                @"(?:\$|!%)\([ \t]*(?:(?:_|[A-Z]|[a-z]|[\xC0-\xD6]|[\xD8-\xF6]|[\xF8-\u02FF]|[\u0370-\u037D]|[\u037F-\u1FFF]|[\u200C-\u200D]|[\u2070-\u218F]|[\u2C00-\u2FEF]|[\u3001-\uD7FF]|[\uF900-\uFDCF]|[\uFDF0-\uFFFD])" +
                @"(?:[A-Z]|[a-z]|[\xC0-\xD6]|[\xD8-\xF6]|[\xF8-\u02FF]|[\u0370-\u037D]|[\u037F-\u1FFF]|[\u200C-\u200D]|[\u2070-\u218F]|[\u2C00-\u2FEF]|[\u3001-\uD7FF]|[\uF900-\uFDCF]|[\uFDF0-\uFFFD]|" +
                @"-|\.|[0-9]|\xB7|[\u0300-\u036F]|[\u203F-\u2040])*)[ \t]*\)?|" +
                //Alias or parameter without brackets
                @"(?:\$|!%)(?:(?:_|[A-Z]|[a-z]|[\xC0-\xD6]|[\xD8-\xF6]|[\xF8-\u02FF]|[\u0370-\u037D]|[\u037F-\u1FFF]|[\u200C-\u200D]|[\u2070-\u218F]|[\u2C00-\u2FEF]|[\u3001-\uD7FF]|[\uF900-\uFDCF]|[\uFDF0-\uFFFD])" +
                @"(?:[A-Z]|[a-z]|[\xC0-\xD6]|[\xD8-\xF6]|[\xF8-\u02FF]|[\u0370-\u037D]|[\u037F-\u1FFF]|[\u200C-\u200D]|[\u2070-\u218F]|[\u2C00-\u2FEF]|[\u3001-\uD7FF]|[\uF900-\uFDCF]|[\uFDF0-\uFFFD]|" +
                @"-|\.|[0-9]|\xB7|[\u0300-\u036F]|[\u203F-\u2040])*)" +
                //Incorrect escape, indent, new line, other characters
                @"|.?))|(\r?\n[ \t]*)|([^\\\0-\x1F\x7F]+)", RegexOptions.Singleline);
            var line = valueInterval.Begin.Line;
            var column = valueInterval.Begin.Column;
            foreach (var match in matches)
            {
                if (((Match) match).Value.StartsWith(@"\$"))
                    intItems.Add(GetAliasInterpolationItem((Match) match, line, column + 1, context, module));
                else if (((Match) match).Value.StartsWith(@"\!%"))
                    intItems.Add(GetParameterInterpolationItem((Match) match, line, column + 1, context, module));
                else if (((Match) match).Value.StartsWith(@"\"))
                    intItems.Add(GetEscapeInterpolationItem((Match) match, line, column + 1, context, module));
                else if (((Match) match).Value.StartsWith("\n") || ((Match) match).Value.StartsWith("\r"))
                    intItems.Add(GetEolEscapeInterpolationItem((Match) match));
                else
                    intItems.Add(((Match) match).Value);

                if (((Match) match).Value.StartsWith("\n"))
                {
                    line++;
                    column = ((Match) match).Value.Length;
                }
                else if (((Match) match).Value.StartsWith("\r"))
                {
                    line++;
                    column = ((Match) match).Value.Length + 1;
                }
                else
                    column += ((Match) match).Value.Length;
            }

            return intItems;
        }

        private static EscapeMatch GetEscapeInterpolationItem(Match match, int line, int column,
            CompilerContext context, Module module)
        {
            var escape = match.Value;
            if (escape.Length < 2)
            {
                context.Errors.Add(CompilerErrorFactory.ParserError("Invalid escape sequence.", module.FileName, line, column));
            }
            else
                switch (escape[1])
                {
                    //'b'|'f'|'n'|'r'|'t'|'v'
                    case '"':
                    case '\\':
                    case '/':
                    case 'b':
                    case 'f':
                    case 'n':
                    case 'r':
                    case 't':
                    case 'v':
                        break;
                    case 'u':
                        if (match.Length < 6)
                            context.Errors.Add(CompilerErrorFactory.ParserError("Invalid escape sequence.", module.FileName, line, column));
                        break;
                    default:
                        context.Errors.Add(CompilerErrorFactory.ParserError("Invalid escape sequence.", module.FileName, line, column));
                        break;
                }

            return new EscapeMatch {Value = match.Value};
        }

        private static EscapeMatch GetEolEscapeInterpolationItem(Match match)
        {
            return new EolEscapeMatch {Value = match.Value};
        }

        private static Parameter GetParameterInterpolationItem(Match match, int line, int column,
            CompilerContext context, Module module)
        {
            if (match.Value.StartsWith(@"\!%(") && !match.Value.EndsWith(@")"))
            {
                context.Errors.Add(CompilerErrorFactory.ParserError("Missing closing parenthesis.", module.FileName, line, column + match.Length));
            }
            var parameter = new Parameter
            {
                Name = match.Value.TrimStart('\\', '!', '%', '(', '\t', ' ').TrimEnd(')', '\t', ' '),
                NameInterval = new Interval(new CharLocation(line, column, -1),
                    new CharLocation(line, column + match.Value.Length, -1)),
                Delimiter = DelimiterEnum.None,
                ValueType = ValueType.Empty,
                IsValueNode = true
            };
            return parameter;
        }

        private static Alias GetAliasInterpolationItem(Match match, int line, int column, CompilerContext context,
            Module module)
        {
            if (match.Value.StartsWith(@"\$(") && !match.Value.EndsWith(@")"))
            {
                context.Errors.Add(CompilerErrorFactory.ParserError("Missing closing parenthesis.", module.FileName, line, column + match.Length));
            }
            var alias = new Alias
            {
                Name = match.Value.TrimStart('\\', '$', '(', '\t', ' ').TrimEnd(')', '\t', ' '),
                NameInterval = new Interval(new CharLocation(line, column, -1),
                    new CharLocation(line, column + match.Value.Length, -1)),
                Delimiter = DelimiterEnum.None,
                Value = null,
                ValueType = ValueType.Empty,
                IsValueNode = true
            };
            return alias;
        }

        public void AppendChild(Pair parent, Pair child)
        {
            try
            {
                parent.AppendChild(child);
            }
            catch (Exception e)
            {
                _context.Errors.Add(CompilerErrorFactory.CantAppendChild(((DOM.Mapped.IMappedPair)child).NameInterval, _module.FileName, e.Message));
            }
        }

        public void EndPair(Pair pair, Interval endInterval, bool endedByEof)
        {
        }

        public Comment ProcessComment(ITextSource textSource, int commentType, Interval interval)
        {
            var value = GetValueFromValueInterval(textSource, DelimiterEnum.None, 0, interval.Begin.Index + 3,
                interval.End.Index - (commentType == 2 ? 3 : 0), 0);
            return new DOM.Mapped.Comment {CommentType = commentType, ValueInterval = interval, Value = value};
        }
    }
}