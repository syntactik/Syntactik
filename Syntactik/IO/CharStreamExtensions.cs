using Syntactik.DOM;

namespace Syntactik.IO
{
    public static class CharStreamExtensions
    {
        public static bool ConsumeNewLine(this ICharStream stream)
        {
            var c = stream.Next;
            if (c == '\r')
            {
                stream.Consume();
                c = stream.Next;
            }

            if (c == '\n')
            {
                stream.Consume();
                return true;
            }

            return false;
        }

        public static bool ConsumeSpaces(this ICharStream stream)
        {
            var c = stream.Next;
            if (!c.IsSpaceCharacter()) return false;

            while (true)
            {
                stream.Consume();
                c = stream.Next;
                if (!c.IsSpaceCharacter()) return true;
            }
        }

        /// <summary>
        /// Consumes comments till EOL or EOF.
        /// </summary>
        public static bool ConsumeComments(this ICharStream stream, IPairFactory pairFactory, Pair parent)
        {
            if (stream.ConsumeSlComment(pairFactory, parent)) return true;
            if (stream.ConsumeMlComment(pairFactory, parent)) return true;
            return false;
        }

        public static bool ConsumeSlComment(this ICharStream stream, IPairFactory pairFactory, Pair parent)
        {
            if (stream.Next != '\'') return false;
            if (stream.La(2) != '\'') return false;
            if (stream.La(3) != '\'') return false;

            stream.Consume();
            var begin = new CharLocation(stream);
            stream.Consume();
            stream.Consume();
            var c = stream.Next;
            while (!c.IsNewLineCharacter() && c != -1)
            {
                stream.Consume();
                c = stream.Next;
            }
           
            var comment = pairFactory.ProcessComment(stream,1, new Interval(begin, new CharLocation(stream)));
            if (comment != null)
            {
                pairFactory.AppendChild(parent, comment);
            }
            return true;
        }

        private static bool FindNextSlComment(ICharStream stream)
        {
            var i = 1;
            while (stream.La(i).IsNewLineCharacter() || stream.La(i).IsSpaceCharacter())
            {
                i++;
            }
            if (stream.La(i++) != '\'') return false;
            if (stream.La(i++) != '\'') return false;
            if (stream.La(i) != '\'') return false;
            i -= 3;
            while (i-- > 0) stream.Consume();

            return true;
        }

        public static bool ConsumeMlComment(this ICharStream stream, IPairFactory pairFactory, Pair parent)
        {
            if (stream.Next != '\"') return false;
            if (stream.La(2) != '\"') return false;
            if (stream.La(3) != '\"') return false;

            stream.Consume();
            var begin = new CharLocation(stream);
            stream.Consume();
            stream.Consume();
            while (!(stream.Next == '\"' && stream.La(2) == '\"' && stream.La(3) == '\"') && stream.Next != -1)
            {
                stream.Consume();
            }

            if (stream.Next == '\"')
            {
                stream.Consume();
                stream.Consume();
                stream.Consume();
            }
            var comment = pairFactory.ProcessComment(stream, 2, new Interval(begin, new CharLocation(stream)));
            if (comment != null)
            {
                pairFactory.AppendChild(parent, comment);
            }
            return true;
        }
    }
}