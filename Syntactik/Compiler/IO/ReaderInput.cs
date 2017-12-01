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

namespace Syntactik.Compiler.IO
{
    /// <summary>
    /// TextReader based compiler input.
    /// </summary>
    [Serializable]
    public class ReaderInput : ICompilerInput
    {
        private readonly string _name;

        private readonly System.IO.TextReader _reader;

        public ReaderInput(string name, System.IO.TextReader reader)
        {
            if (null == name)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (null == reader)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            _name = name;
            _reader = reader;
        }

        public string Name => _name;

        public System.IO.TextReader Open()
        {
            return _reader;
        }
    }
}
