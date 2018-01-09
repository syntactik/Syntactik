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
using System.IO;

namespace Syntactik.Compiler.IO
{
    /// <summary>
    /// TextReader based compiler input.
    /// </summary>
    public class ReaderInput : ICompilerInput
    {
        private readonly TextReader _reader;

        /// <summary>
        /// Creates an instance of <see cref="ReaderInput"/>.
        /// </summary>
        /// <param name="name">File name associated with the input.</param>
        /// <param name="reader">Underlying <see cref="TextReader"/></param>
        public ReaderInput(string name, TextReader reader)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        /// <summary>
        /// File name associated with the input.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Opens input for reading.
        /// </summary>
        /// <returns><see cref="TextReader"/> as source of string input.</returns>
        public TextReader Open()
        {
            return _reader;
        }
    }
}
