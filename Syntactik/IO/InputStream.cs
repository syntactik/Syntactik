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

namespace Syntactik.IO
{
    public class InputStream: ICharStream, ITextSource, IDisposable
    {
        private int _index;
        private int _line;
        private int _column;
        private readonly int _length;
        protected string Data;
        private int _next;
        public const int Eof = -1;


        public InputStream(string input)
        {
            Data = input;
            _length = Data.Length;
            _line = 1;
            _column = 0;
            _index = -1;
            _next = _length > 0 ? Data[0] : -1;
        }

        public InputStream(string input, int length)
        {
            Data = input;
            _length = length;
            _line = 1;
            _column = 0;
            _index = -1;
            _next = _length > 0 ? Data[0] : -1;
        }

        public void Consume()
        {
            if (_index >= _length)
            {
                throw new InvalidOperationException("The input has reached the EOF.");
            }

            _index++;
            _next = _index + 2 <= _length ? char.ConvertToUtf32(Data, _index + 1) : -1;


            if (Data[_index] == 10)
            {
                _line++;
                _column = 0;
            }
            else
            {
                _column++;
            }

            //If this Unicode symbol takes two bytes ...
            if (_index + 1 < _length && char.IsHighSurrogate(Data, _index + 1))
                _index++;

        }

        /// <summary>
        /// Returns character from the stream without consuming it.
        /// </summary>
        /// <param name="i">
        /// if i = 0 then result is undefined.
        /// if i = -1 then the function returns the previously read character. 
        /// If i = -2 then the function returns the character prior to the previously read character, etc.
        /// If i = 1 then the function returns the current character which is the next character to be consumed, etc. 
        /// </param>
        /// <returns></returns>
        public int La(int i)
        {
            if (i > 0)
            {
                //var j = _index + i;
                var j = _index;
                do
                {
                    j++;
                    if (j >= _length) return Eof;
                    if (char.IsHighSurrogate(Data, j))
                        j++;
                    i--;
                } while (i > 0);
                return j >= _length ? Eof : Data[j];
            }

            if (i == 0)
            {
                return 0;
            }

            i += 1;

            if (_index + i < 0)
            {
                return Eof;
            }
            i += _index; 
            return i  >= _length ? Eof : Data[i];
        }

        public int Index => _index;

        public int Length => _length;

        public int Line => _line;

        public int Column => _column;

        public string GetText(int begin, int end)
        {
            if (begin < 0 || end < begin) return string.Empty;
            return Data.Substring(begin, end - begin + 1);
        }

        public char GetChar(int index)
        {
            return Data[index];
        }

        public void Reset()
        {
            _index = -1;
            _next = _length > 0 ? Data[0] : -1;
        }

        public int Next => _next;

        public void Dispose()
        {
            Data = null;
        }
    }
}
