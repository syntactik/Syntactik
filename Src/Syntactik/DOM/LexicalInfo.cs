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

namespace Syntactik.DOM
{
    /// <summary>
    /// Represents immutable location of the character in the project.
    /// </summary>
    public class LexicalInfo : CharLocation, IEquatable<LexicalInfo>, IComparable<LexicalInfo>
    {
        /// <summary>
        /// Singleton instance of <see cref="LexicalInfo"/> representing "Empty" value.
        /// </summary>
        public new static readonly LexicalInfo Empty = new LexicalInfo(null, -1, -1, -1);

        private readonly string _filename;
        private string _fullPath;

        /// <summary>
        /// Creates instance of the class.
        /// </summary>
        /// <param name="filename">Source code file name.</param>
        /// <param name="line">Line number (starts from 1).</param>
        /// <param name="column">Column number (starts from 1).</param>
        /// <param name="index">Index of character in the file (starts from 0).</param>
        public LexicalInfo(string filename, int line, int column, int index)
            : base(line, column, index)
        {
            _filename = filename;
        }

        /// <summary>
        /// Creates instance of the class.
        /// </summary>
        /// <param name="filename">Source code file name.</param>
        public LexicalInfo(string filename) : this(filename, -1, -1, -1)
        {
        }

        /// <summary>
        /// Creates instance of the class.
        /// </summary>
        /// <param name="other">Another instance of the <see cref="LexicalInfo"/> which the new instance will be copied from.</param>
        public LexicalInfo(LexicalInfo other) : this(other.FileName, other.Line, other.Column, other.Index)
        {
        }
        /// <summary>
        /// Source code file name.
        /// </summary>
        public string FileName => _filename;

        /// <summary>
        /// Fill path to the file. Different from the <see cref="FileName"/> if it has relative path.
        /// </summary>
        public string FullPath
        {
            get
            {
                if (null != _fullPath) return _fullPath;
                _fullPath = SafeGetFullPath(_filename);
                return _fullPath;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _filename + base.ToString();
        }

        private static string SafeGetFullPath(string fileName)
        {
            try
            {
                return System.IO.Path.GetFullPath(fileName);
            }
            catch (Exception)
            {
                // ignored
            }
            return fileName;
        }

        /// <inheritdoc />
        public int CompareTo(LexicalInfo other)
        {
            int result = String.CompareOrdinal(_filename, other._filename);
            if (result != 0) return result;

            return base.CompareTo(other); 
        }

        /// <inheritdoc />
        public bool Equals(LexicalInfo other)
        {
            return CompareTo(other) == 0;
        }
    }
}
