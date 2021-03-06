﻿#region license
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
namespace Syntactik.IO
{
    /// <summary>
    /// A source of characters or strings.
    /// </summary>
    public interface ITextSource
    {
        /// <summary>
        /// Returns a string.
        /// </summary>
        /// <param name="begin">Start index.</param>
        /// <param name="end">End index.</param>
        /// <returns>Result string</returns>
        string GetText(int begin, int end);
        /// <summary>
        /// Returns a character.
        /// </summary>
        /// <param name="index">Index of character in the string, stream etc.</param>
        /// <returns>Result character.</returns>
        char GetChar(int index);
    }
}
