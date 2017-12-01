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