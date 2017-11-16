using System;
using System.Linq;
using System.Text;
using Syntactik.IO;

namespace Syntactik.DOM
{
    public interface IPairFactory
    {
        Pair CreateMappedPair(ICharStream input, int nameQuotesType, Interval nameInterval, DelimiterEnum delimiter, 
                Interval delimiterInterval, int valueQuotesType, Interval valueInterval, int valueIndent);

        void AppendChild(Pair parent, Pair child);

        void EndPair(Pair pair, Interval endInterval, bool endedByEof = false);

        /// <summary>
        /// Method is called when parser finds comment
        /// </summary>
        /// <param name="commentType">1 - single line comment</param>
        /// <param name="commentInterval">2 - multiline comment</param>
        Comment ProcessComment(ICharStream input, int commentType, Interval commentInterval);
    }

    public class PairFactory: IPairFactory
    {
        public delegate void EndPairEventHandler(Pair pair, Interval endInterval, bool endedByEof);

        public event EndPairEventHandler OnEndPair;

        public delegate Comment ProcessCommentHandler(int commentType, Interval commentInterval);

        public event ProcessCommentHandler OnProcessComment;

        public Pair CreateMappedPair(ICharStream input, int nameQuotesType, Interval nameInterval, DelimiterEnum delimiter,
                        Interval delimiterInterval, int valueQuotesType, Interval valueInterval, int valueIndent)
        {
            var name = GetName(input, nameQuotesType, nameInterval);
            if (nameQuotesType > 0)
                return new Mapped.Element()
                    {
                        Name = name,
                        NameQuotesType = nameQuotesType,
                        NameInterval = nameInterval,
                        Delimiter = delimiter,
                        DelimiterInterval = delimiterInterval,
                        Value = GetValue(input, delimiter, valueQuotesType, valueInterval, valueIndent),
                        ValueQuotesType = valueQuotesType,
                        ValueInterval = valueInterval,
                        InterpolationItems = null,
                        ValueIndent = valueIndent
                    };
            if (name.StartsWith("@"))
                return new Mapped.Attribute
                {
                    NsPrefix = null,
                    Name = name.Substring(1),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = GetValue(input, delimiter, valueQuotesType, valueInterval, valueIndent),
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = null,
                    ValueIndent = valueIndent
                };
            if (name.StartsWith("!$"))
                return new Mapped.AliasDefinition
                {
                    Name = name.Substring(2),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = GetValue(input, delimiter, valueQuotesType, valueInterval, valueIndent),
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = null,
                    ValueIndent = valueIndent
                };
            if (name.StartsWith("!#"))
                return new Mapped.NamespaceDefinition
                {
                    Name = name.Substring(2),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = GetValue(input, delimiter, valueQuotesType, valueInterval, valueIndent),
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = null,
                    ValueIndent = valueIndent
                };
            if (name.StartsWith("!%"))
                return new Mapped.Parameter
                {
                    Name = name.Substring(2),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = GetValue(input, delimiter, valueQuotesType, valueInterval, valueIndent),
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = null,
                    ValueIndent = valueIndent
                };
            if (name.StartsWith("!"))
                return new Mapped.Document
                {
                    Name = name.Substring(1),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = GetValue(input, delimiter, valueQuotesType, valueInterval, valueIndent),
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = null,
                    ValueIndent = valueIndent
                };
            if (name.StartsWith("$"))
                return new Mapped.Alias()
                {
                    Name = name.Substring(1),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = GetValue(input, delimiter, valueQuotesType, valueInterval, valueIndent),
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = null,
                    ValueIndent = valueIndent
                };
            if (name.StartsWith("%"))
                return new Mapped.Argument()
                {
                    Name = name.Substring(1),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = GetValue(input, delimiter, valueQuotesType, valueInterval, valueIndent),
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = null,
                    ValueIndent = valueIndent
                };
            if (name.StartsWith("#"))
                return new Mapped.Scope
                {
                    NsPrefix = name.Substring(1),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval
                };
            var tuple = Mapped.Element.GetNameAndNs(name, nameQuotesType);
            return new Mapped.Element()
            {
                NsPrefix = string.IsNullOrEmpty(tuple.Item1) ? null : tuple.Item1,
                Name = tuple.Item2,
                NameQuotesType = nameQuotesType,
                NameInterval = nameInterval,
                Delimiter = delimiter,
                DelimiterInterval = delimiterInterval,
                Value = GetValue(input, delimiter, valueQuotesType, valueInterval, valueIndent),
                ValueQuotesType = valueQuotesType,
                ValueInterval = valueInterval,
                InterpolationItems = null,
                ValueIndent = valueIndent
            };
        }

        private static string GetName(ICharStream input, int nameQuotesType, Interval nameInterval)
        {
            if (nameQuotesType == 0)
                return ((ITextSource) input).GetText(nameInterval.Begin.Index, nameInterval.End.Index);
            var c = ((ITextSource) input).GetChar(nameInterval.End.Index);
            if (nameQuotesType == 1)
                return c == '\'' ? ((ITextSource)input).GetText(nameInterval.Begin.Index + 1, nameInterval.End.Index - 1) : ((ITextSource)input).GetText(nameInterval.Begin.Index + 1, nameInterval.End.Index);

            return c == '"' ? ((ITextSource)input).GetText(nameInterval.Begin.Index + 1, nameInterval.End.Index - 1) : ((ITextSource)input).GetText(nameInterval.Begin.Index + 1, nameInterval.End.Index);

        }

        private string GetValue(ICharStream input, DelimiterEnum delimiter,
                                int valueQuotesType, Interval valueInterval, int valueIndent)
        {
            if (valueInterval == null)
            {
                if (delimiter == DelimiterEnum.E || delimiter == DelimiterEnum.EE)
                    return string.Empty;
                return null;
            }
            if (valueInterval.Begin.Index == -1)
            {
                return string.Empty;
            }
            if (valueQuotesType == (int)QuotesEnum.Single || valueQuotesType == (int)QuotesEnum.Double)
            {
                var c = ((ITextSource)input).GetChar(valueInterval.End.Index);
                var missingValueQuote = valueQuotesType == (int) QuotesEnum.Single && c != '\'' ||
                                        valueQuotesType == (int) QuotesEnum.Double && c != '"';
                if (!missingValueQuote)
                {
                    return GetValueFromValueInterval((ITextSource)input, delimiter, valueQuotesType, 
                        valueInterval.Begin.Index + 1, valueInterval.End.Index - 1, valueIndent);
                }

                return GetValueFromValueInterval((ITextSource)input, delimiter, valueQuotesType,
                    valueInterval.Begin.Index + 1, valueInterval.End.Index, valueIndent);
            }
            return GetValueFromValueInterval((ITextSource)input, delimiter, valueQuotesType,
                valueInterval.Begin.Index, valueInterval.End.Index, valueIndent);
        }

        public static string GetValueFromValueInterval(ITextSource charStream, DelimiterEnum delimiter,
                                int valueQuotesType, int begin, int end, int valueIndent)
        {
            var sb = new StringBuilder();
            //Splitting text. Getting array of text lines
            var lines = charStream.GetText(begin, end).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            bool folded = lines.Length > 1 && delimiter == DelimiterEnum.EE &&
              (valueQuotesType == (int)QuotesEnum.None || valueQuotesType == (int)QuotesEnum.Double);

            var first = true;
            var firstEmptyLine = true; //If true then previous line was not empty therefor newline shouldn't be added
            var checkIfFirstLineIsEmpty = true;

            foreach (var item in lines)
            {
                var line = TrimEndOfFoldedStringLine(item, folded);
                if (checkIfFirstLineIsEmpty)  //ignoring first empty line for open strings
                {
                    checkIfFirstLineIsEmpty = false;
                    if (valueQuotesType == (int)QuotesEnum.None && line == string.Empty)
                    {
                        first = false;
                        continue;
                    }
                }

                if (first) { sb.Append(line); first = false; continue; } //adding first line without appending new line symbol

                if (line.Length <= valueIndent) //this is just empty line
                {
                    if (folded)//Folded string
                    {
                        if (firstEmptyLine)
                        {
                            firstEmptyLine = false;
                            continue; //Ignore first empty line for folded string
                        }
                    }
                    sb.AppendLine(); continue;
                }

                var lineIndent = line.TakeWhile(c => c == ' ' || c == '\t').Count();
                if (lineIndent < valueIndent)
                {
                    line = line.Substring(lineIndent); //Removing indents
                    if (line.TrimEnd() == "===") sb.AppendLine();
                    break; // this is multiline string terminator ===
                }
                    
                line = line.Substring(valueIndent); //Removing indents
                if (sb.Length == 0)// If it is first line to be added just add it. No new line or spacing needed.
                {
                    sb.Append(line);
                    continue;
                }
                if (folded && firstEmptyLine) sb.Append(" ");
                if (!folded || !firstEmptyLine) sb.AppendLine();
                firstEmptyLine = true; //reseting the flag for folded string logic
                sb.Append(line);//Removing indents                    
            }
            return sb.ToString();
        }

        private static string TrimEndOfFoldedStringLine(string line, bool ignoreTrailing)
        {
            if (ignoreTrailing)
                return line.TrimEnd(' ', '\t'); //ignoring trailing whitespace for open strings

            return line;
        }

        public void AppendChild(Pair parent, Pair child)
        {
            parent.AppendChild(child);
        }

        public void EndPair(Pair pair, Interval endInterval, bool endedByEof)
        {
            OnEndPair?.Invoke(pair, endInterval, endedByEof);
        }

        public DOM.Comment ProcessComment(ICharStream input, int commentType, Interval commentInterval)
        {
            return OnProcessComment?.Invoke(commentType, commentInterval);
        }
    }
}
