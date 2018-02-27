using System;
using System.Linq;
using System.Text;
using Syntactik.DOM;
using Syntactik.IO;

namespace TestEditor
{
    class PairFactory : IPairFactory
    {
        public delegate void EndPairEventHandler(Pair pair, Interval endInterval, bool endedByEof);

        public event EndPairEventHandler OnEndPair;

        public delegate Comment ProcessCommentHandler(int commentType, Interval commentInterval);

        public event ProcessCommentHandler OnProcessComment;

        public Pair CreateMappedPair(ITextSource textSource, int nameQuotesType, Interval nameInterval,
            AssignmentEnum assignment,
            Interval assignmentInterval, int valueQuotesType, Interval valueInterval, int valueIndent)
        {
            var name = GetName(textSource, nameQuotesType, nameInterval);
            if (nameQuotesType > 0)
                return new Syntactik.DOM.Mapped.Element
                (
                    name,
                    nameQuotesType: nameQuotesType,
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval,
                    value: GetValue(textSource, assignment, valueQuotesType, valueInterval, valueIndent),
                    valueQuotesType: valueQuotesType,
                    valueInterval: valueInterval,
                    interpolationItems: null,
                    valueIndent: valueIndent
                );
            if (name.StartsWith("@"))
                return new Syntactik.DOM.Mapped.Attribute
                (
                    name.Substring(1),
                    null,
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval,
                    value: GetValue(textSource, assignment, valueQuotesType, valueInterval, valueIndent),
                    valueQuotesType: valueQuotesType,
                    valueInterval: valueInterval,
                    interpolationItems: null,
                    valueIndent: valueIndent
                );
            if (name.StartsWith("!$"))
                return new Syntactik.DOM.Mapped.AliasDefinition
                (
                    name.Substring(2),
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval,
                    value: GetValue(textSource, assignment, valueQuotesType, valueInterval, valueIndent),
                    valueQuotesType: valueQuotesType,
                    valueInterval: valueInterval,
                    interpolationItems: null,
                    valueIndent: valueIndent
                );
            if (name.StartsWith("!#"))
                return new Syntactik.DOM.Mapped.NamespaceDefinition
                (
                    name.Substring(2),
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval,
                    value: GetValue(textSource, assignment, valueQuotesType, valueInterval, valueIndent),
                    valueQuotesType: valueQuotesType,
                    valueInterval: valueInterval,
                    interpolationItems: null,
                    valueIndent: valueIndent
                );
            if (name.StartsWith("!%"))
                return new Syntactik.DOM.Mapped.Parameter
                (
                    name.Substring(2),
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval,
                    value: GetValue(textSource, assignment, valueQuotesType, valueInterval, valueIndent),
                    valueQuotesType: valueQuotesType,
                    valueInterval: valueInterval,
                    interpolationItems: null,
                    valueIndent: valueIndent
                );
            if (name.StartsWith("!"))
                return new Syntactik.DOM.Mapped.Document
                (
                     name.Substring(1),
                     nameInterval: nameInterval,
                     assignment: assignment,
                     assignmentInterval: assignmentInterval,
                     value: GetValue(textSource, assignment, valueQuotesType, valueInterval, valueIndent),
                     valueQuotesType: valueQuotesType,
                     valueInterval: valueInterval,
                     interpolationItems: null,
                     valueIndent: valueIndent
                );
            if (name.StartsWith("$"))
                return new Syntactik.DOM.Mapped.Alias
                (
                    name.Substring(1),
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval,
                    value: GetValue(textSource, assignment, valueQuotesType, valueInterval, valueIndent),
                    valueQuotesType: valueQuotesType,
                    valueInterval: valueInterval,
                    interpolationItems: null,
                    valueIndent: valueIndent
                );
            if (name.StartsWith("%"))
                return new Syntactik.DOM.Mapped.Argument
                (
                    name.Substring(1),
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval,
                    value: GetValue(textSource, assignment, valueQuotesType, valueInterval, valueIndent),
                    valueQuotesType: valueQuotesType,
                    valueInterval: valueInterval,
                    interpolationItems: null,
                    valueIndent: valueIndent
                );
            if (name.StartsWith("#"))
                return new Syntactik.DOM.Mapped.Scope
                (
                    nsPrefix: name.Substring(1),
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval
                );
            var tuple = Syntactik.DOM.Mapped.Element.GetNameAndNs(name, nameQuotesType);
            return new Syntactik.DOM.Mapped.Element
            (
                tuple.Item2,
                string.IsNullOrEmpty(tuple.Item1) ? null : tuple.Item1,
                nameQuotesType: nameQuotesType,
                nameInterval: nameInterval,
                assignment: assignment,
                assignmentInterval: assignmentInterval,
                value: GetValue(textSource, assignment, valueQuotesType, valueInterval, valueIndent),
                valueQuotesType: valueQuotesType,
                valueInterval: valueInterval,
                interpolationItems: null,
                valueIndent: valueIndent
            );
        }

        private static string GetName(ITextSource input, int nameQuotesType, Interval nameInterval)
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

        private string GetValue(ITextSource input, AssignmentEnum assignment,
            int valueQuotesType, Interval valueInterval, int valueIndent)
        {
            if (valueInterval == null)
            {
                if (assignment == AssignmentEnum.E || assignment == AssignmentEnum.EE)
                    return string.Empty;
                return null;
            }
            if (valueInterval.Begin.Index == -1)
            {
                return string.Empty;
            }
            if (valueQuotesType == (int)QuotesEnum.Single || valueQuotesType == (int)QuotesEnum.Double)
            {
                var c = input.GetChar(valueInterval.End.Index);
                var missingValueQuote = valueQuotesType == (int)QuotesEnum.Single && c != '\'' ||
                                        valueQuotesType == (int)QuotesEnum.Double && c != '"';
                if (!missingValueQuote)
                {
                    return GetValueFromValueInterval(input, assignment, valueQuotesType,
                        valueInterval.Begin.Index + 1, valueInterval.End.Index - 1, valueIndent);
                }

                return GetValueFromValueInterval(input, assignment, valueQuotesType,
                    valueInterval.Begin.Index + 1, valueInterval.End.Index, valueIndent);
            }
            return GetValueFromValueInterval(input, assignment, valueQuotesType,
                valueInterval.Begin.Index, valueInterval.End.Index, valueIndent);
        }

        public static string GetValueFromValueInterval(ITextSource charStream, AssignmentEnum assignment,
            int valueQuotesType, int begin, int end, int valueIndent)
        {
            var sb = new StringBuilder();
            //Splitting text. Getting array of text lines
            var lines = charStream.GetText(begin, end).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            bool folded = lines.Length > 1 && assignment == AssignmentEnum.EE &&
                          (valueQuotesType == (int)QuotesEnum.None || valueQuotesType == (int)QuotesEnum.Double);

            var first = true;
            var firstEmptyLine = true; //If true then previous line was not empty therefor newline shouldn't be added
            var checkIfFirstLineIsEmpty = true;

            foreach (var item in lines)
            {
                var line = TrimEndOfFoldedStringLine(item, folded);
                if (checkIfFirstLineIsEmpty) //ignoring first empty line for open strings
                {
                    checkIfFirstLineIsEmpty = false;
                    if (valueQuotesType == (int)QuotesEnum.None && line == string.Empty)
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
                if (lineIndent < valueIndent)
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
                sb.Append(line); //Removing indents                    
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

        public Comment ProcessComment(ITextSource textSource, int commentType, Interval commentInterval)
        {
            return OnProcessComment?.Invoke(commentType, commentInterval);
        }

        public void ProcessBrackets(Pair currentPair, int bracket, Interval endInterval)
        {
        }
    }
}
