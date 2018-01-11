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
    /// <summary>
    /// An immutable inclusive interval.
    /// </summary>
    public class Interval
    {
        /// <summary>
        /// Creates instance of the class.
        /// </summary>
        /// <param name="begin">Starting position of the interval (inclusive).</param>
        /// <param name="end">Ending position of the interval (inclusive).</param>
        public Interval(CharLocation begin, CharLocation end)
        {
            Begin = begin;
            End = end;
        }

        /// <summary>
        /// Creates instance of the class.
        /// </summary>
        /// <param name="input">Uses current position of the <see cref="ICharStream"/> to calculate start and end of the interval.</param>
        public Interval(ICharStream input) 
        {
            Begin = new CharLocation(input);
            End = new CharLocation(input);
        }

        /// <summary>
        /// Creates instance of the class.
        /// </summary>
        /// <param name="charLocation">Uses <see cref="CharLocation"/> to set start and end of the interval.</param>
        public Interval(CharLocation charLocation)
        {
            Begin = charLocation;
            End = charLocation;
        }

        /// <summary>
        /// Singleton instance of <see cref="Interval"/> representing "Empty" value.
        /// </summary>
        public static Interval Empty = new Interval(CharLocation.Empty, CharLocation.Empty);

        /// <summary>
        /// Starting position of the interval (inclusive).
        /// </summary>
        public readonly CharLocation Begin;

        /// <summary>
        /// Ending position of the interval (inclusive).
        /// </summary>
        public readonly CharLocation End;
    }
}
