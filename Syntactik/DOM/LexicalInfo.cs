#region license
// Copyright © 2016, 2017 Maxim Trushin (dba Syntactik, trushin@gmail.com, maxim.trushin@syntactik.com)
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
    public class LexicalInfo : CharLocation, IEquatable<LexicalInfo>, IComparable<LexicalInfo>
    {
        public new static readonly LexicalInfo Empty = new LexicalInfo(null, -1, -1, -1);

        private readonly string _filename;

        private string _fullPath;

        public LexicalInfo(string filename, int line, int column, int index)
            : base(line, column, index)
        {
            _filename = filename;
        }

        public LexicalInfo(string filename) : this(filename, -1, -1, -1)
        {
        }

        public LexicalInfo(LexicalInfo other) : this(other.FileName, other.Line, other.Column, other.Index)
        {
        }

        public string FileName => _filename;

        public string FullPath
        {
            get
            {
                if (null != _fullPath) return _fullPath;
                _fullPath = SafeGetFullPath(_filename);
                return _fullPath;
            }
        }

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

        public int CompareTo(LexicalInfo other)
        {
            int result = String.CompareOrdinal(_filename, other._filename);
            if (result != 0) return result;

            return base.CompareTo(other); 
        }

        public bool Equals(LexicalInfo other)
        {
            return CompareTo(other) == 0;
        }
    }
}
