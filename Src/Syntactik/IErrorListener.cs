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

namespace Syntactik
{
    /// <summary>
    /// Base interface for all error listeners.
    /// </summary>
    public interface IErrorListener
    {
        /// <summary>
        /// Method called when parser error occurred.
        /// </summary>
        /// <param name="code">Code of the error</param>
        /// <param name="interval">Interval of source code that caused the error.</param>
        /// <param name="args"><see href="https://msdn.microsoft.com/en-us/library/txafckwd(v=vs.110).aspx">Formatting items</see>.</param>
        void OnError(int code, Interval interval, params object[] args);
    }
}
