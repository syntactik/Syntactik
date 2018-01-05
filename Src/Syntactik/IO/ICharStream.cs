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
namespace Syntactik.IO
{
    /// <summary>
    /// Represents base interface for stream of Unicode characters.
    /// </summary>
    public interface ICharStream
    {
        /// <summary>
        /// Consumes one character.
        /// </summary>
        void Consume();
        /// <summary>
        /// Initializes/resets state of the stream. 
        /// </summary>
        void Reset();
        /// <summary>
        /// Implements look-ahead logic that allows to read characters without changing the current position of the stream.
        /// </summary>
        /// <param name="i">
        /// if i = 0 then result is undefined.
        /// if i = -1 then the function returns the previously read character. 
        /// If i = -2 then the function returns the character prior to the previously read character, etc.
        /// If i = 1 then the function returns the current character which is the next character to be consumed, etc. 
        /// </param>
        /// <returns>Result character.</returns>
        int La(int i);
        /// <summary>
        /// Current character. <b>Next</b> symbol to be consumed.
        /// </summary>
        int Next { get; }
        /// <summary>
        /// Index of the last consumed character. -1 if input is in the initial state.
        /// </summary>
        int Index { get; }
        /// <summary>
        /// Current line. Starts from 1.
        /// </summary>
        int Line { get; }
        /// <summary>
        /// Number of consumed character since property Line changed the value. 0 if no characters 
        /// has been consumed in the current line.
        /// </summary>
        int Column { get; }
        /// <summary>
        /// Total number of symbols in the stream.
        /// </summary>
        int Length { get; }
        
    }
}
