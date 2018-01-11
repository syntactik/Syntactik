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
    /// File based compiler input.
    /// </summary>
    public class FileInput : ICompilerInput
    {
        private readonly string _fname;

        /// <summary>
        /// Creates instance of the class.
        /// </summary>
        /// <param name="fname">Full path name.</param>
        public FileInput(string fname)
        {
            _fname = fname ?? throw new ArgumentNullException(nameof(fname));
        }

        /// <summary>
        /// Full path name.
        /// </summary>
        public string Name => _fname;


        /// <inheritdoc />
        public System.IO.TextReader Open()
        {
            try
            {
                return System.IO.File.OpenText(_fname);
            }
            catch (System.IO.FileNotFoundException)
            {
                throw CompilerErrorFactory.FileNotFound(_fname);
            }
            catch (Exception e)
            {
                throw CompilerErrorFactory.InputError(_fname, e);
            }
        }
    }
}
