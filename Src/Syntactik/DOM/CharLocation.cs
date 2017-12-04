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
using Syntactik.IO;

namespace Syntactik.DOM
{
    public class CharLocation
    {
        public CharLocation(int line, int column, int index)
        {
            Index = index;
            Line = line;
            Column = column;
        }

        public CharLocation(ICharStream input)
        {
            Index = input.Index;
            Line = input.Line;
            Column = input.Column;
        }

        static CharLocation()
        {
            Empty = new CharLocation(0, 0, -1);
        }

        public static CharLocation Empty;

        public int Index;
        public int Line;
        public int Column;

        protected int CompareTo(CharLocation other)
        {
            int num = Line.CompareTo(other.Line);
            if (num != 0)
            {
                return num;
            }
            return Column.CompareTo(other.Column);
        }

        public override string ToString()
        {
            return $"({Line},{Column},{Index})";
        }
    }
}
