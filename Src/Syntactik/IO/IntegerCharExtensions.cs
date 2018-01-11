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
    /// Auxiliary static class that provides extension methods to process Unicode characters in the semantics of
    /// the Syntactik parser.
    /// </summary>
    public static class IntegerCharExtensions
    {
        /// <summary>
        /// Returns true if the character can be used in indentation.
        /// </summary>
        /// <param name="c">Target character.</param>
        /// <returns>True if the character can be used in indentation.</returns>
        public static bool IsIndentCharacter(this int c)
        {
            return c == '\t' || c == ' ';
        }

        /// <summary>
        /// Returns true if the character represents whitespace.
        /// </summary>
        /// <param name="c">Target character.</param>
        /// <returns>True if character represents whitespace.</returns>
        public static bool IsSpaceCharacter(this int c)
        {
            return c == ' ' || c == '\t';
        }

        /// <summary>
        /// Returns true if the character can't be included in the open string.
        /// </summary>
        /// <param name="c">Target character.</param>
        /// <returns>True if the character can't be included in the open string.</returns>
        public static bool IsEndOfOpenString(this int c)
        {
            if (c > 61) return false;
            return c == '=' || c == ':' || c == ',' || c == '\'' || c == '"' || c == ')' || c == '(';
        }

        /// <summary>
        /// Returns true if the character can't be included in the open name.
        /// </summary>
        /// <param name="c">Target character.</param>
        /// <returns>True if the character can't be included in the open name.</returns>
        public static bool IsEndOfOpenName(this int c)
        {
            if (c > 61) return false;
            return c == '=' || c == ':' ||c == '\r' || c == '\n' || c == ',' || c =='\'' || c == '"' || c == ')' || c == '(';
        }

        /// <summary>
        /// Returns true if the character represents newline.
        /// </summary>
        /// <param name="c">Target character.</param>
        /// <returns>True if the character represents newline.</returns>
        public static bool IsNewLineCharacter(this int c)
        {
            return c == '\r' || c == '\n';
        }
    }
}
