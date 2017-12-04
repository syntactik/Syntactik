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
    public static class ErrorCodes
    {
        public static string[] Errors =
        {
            "Unexpected character(s) `{0}`.", // 0
            "{0} is expected.", // 1
            "Invalid indentation.", //2
            "Block indent mismatch.",// 3
            "Invalid indent multiplicity.",// 4
            "Mixed indentation.", //5
            "Invalid indentation size.",// 6
        };

        public static string Format(int code, params object[] args)
        {
            return string.Format(Errors[code], args);
        }
    }
}
