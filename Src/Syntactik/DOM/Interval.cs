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
    public class Interval
    {
        public Interval(CharLocation begin, CharLocation end)
        {
            Begin = begin;
            End = end;
        }

        static Interval()
        {
            Empty = new Interval(CharLocation.Empty, CharLocation.Empty);
        }

        public Interval(ICharStream input) 
        {
            Begin = new CharLocation(input);
            End = new CharLocation(input);
        }

        public Interval(CharLocation charLocation)
        {
            Begin = charLocation;
            End = charLocation;
        }

        public static Interval Empty;

        public readonly CharLocation Begin;
        public readonly CharLocation End;
    }
}
