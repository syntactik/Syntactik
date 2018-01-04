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
using Syntactik.IO;

namespace Syntactik.DOM
{
    /// <summary>
    /// Represents location of the character in the <see cref="Module"/> (file).
    /// </summary>
    public class CharLocation: IEquatable<CharLocation>, IComparable<CharLocation>
    {
        /// <summary>
        /// Creates instance of the class.
        /// </summary>
        /// <param name="line">Line number (starts from 1).</param>
        /// <param name="column">Column number (starts from 1).</param>
        /// <param name="index">Index of character in the file (starts from 0).</param>
        public CharLocation(int line, int column, int index)
        {
            Index = index;
            Line = line;
            Column = column;
        }

        /// <summary>
        /// Creates instance of the class.
        /// </summary>
        /// <param name="input">Instance of <see cref="ICharStream"/> used for initialization.</param>
        public CharLocation(ICharStream input)
        {
            Index = input.Index;
            Line = input.Line;
            Column = input.Column;
        }


        /// <summary>
        /// Singleton instance of <see cref="CharLocation"/> representing "Empty" value.
        /// </summary>
        public static readonly CharLocation Empty = new CharLocation(0, 0, -1);

        /// <summary>
        /// Index of character in the file (starts from 0).
        /// </summary>
        public int Index;
        /// <summary>
        /// Line number (starts from 1).
        /// </summary>
        public int Line;
        /// <summary>
        /// Column number (starts from 1).
        /// </summary>
        public int Column;

        /// <inheritdoc />
        public int CompareTo(CharLocation other)
        {
            int num = Line.CompareTo(other.Line);
            if (num != 0)
            {
                return num;
            }
            return Column.CompareTo(other.Column);
        }

        /// <inheritdoc />
        public bool Equals(CharLocation other)
        {
            return other != null && Line == other.Line && Column == other.Column;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({Line},{Column},{Index})";
        }
    }
}
