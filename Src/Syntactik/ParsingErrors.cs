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
namespace Syntactik
{
    /// <summary>
    /// Contains list of parsing error messages and auxiliary methods to work with them.
    /// </summary>
    public static class ParsingErrors
    {
        /// <summary>
        /// Array which contains all parsing error messages.
        /// Messages can be a <see href="https://msdn.microsoft.com/en-us/library/txafckwd(v=vs.110).aspx">composite format string</see>.
        /// </summary>
        public static string[] Messages =
        {
            "Unexpected character(s) `{0}`.", // 0
            "{0} is expected.", // 1
            "Invalid indentation.", //2
            "Block indent mismatch.",// 3
            "Invalid indent multiplicity.",// 4
            "Mixed indentation.", //5
            "Invalid indentation size.",// 6
        };

        /// <summary>
        /// Formats parsing error message if the message is a <see href="https://msdn.microsoft.com/en-us/library/txafckwd(v=vs.110).aspx">composite 
        /// format string</see>.
        /// </summary>
        /// <param name="code">Parsing error code.</param>
        /// <param name="args">Formatting items. This argument is used if the corresponding message is 
        /// a <see href="https://msdn.microsoft.com/en-us/library/txafckwd(v=vs.110).aspx">composite format string</see>.</param>
        /// <returns>Formatted error message.</returns>
        public static string Format(int code, params object[] args)
        {
            return string.Format(Messages[code], args);
        }
    }
}
