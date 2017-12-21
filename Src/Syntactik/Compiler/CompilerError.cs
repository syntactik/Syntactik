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
#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Text;
using Syntactik.DOM;

namespace Syntactik.Compiler
{
    /// <summary>
    /// A compilation error.
    /// </summary>
    [Serializable]
    public class CompilerError : ApplicationException, IComparable<CompilerError>
    {
        /// <summary>
        /// If true then the error occured during parsing step.
        /// </summary>
        public bool IsParserError { get; }
        private readonly LexicalInfo _lexicalInfo;
        private readonly string _code;

        /// <summary>
        /// Initializes a new instance of the CompilerError class.
        /// </summary>
        /// <param name="code">String identificator of the error.</param>
        /// <param name="lexicalInfo"></param>
        /// <param name="cause"></param>
        /// <param name="args"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CompilerError(string code, LexicalInfo lexicalInfo, Exception cause, params object[] args) : base(ErrorCodes.Format(code, args), cause)
        {
            if (null == lexicalInfo)
                throw new ArgumentNullException(nameof(lexicalInfo));
            _code = code;
            _lexicalInfo = lexicalInfo;
        }

        public CompilerError(string code, Exception cause, params object[] args) : this(code, LexicalInfo.Empty, cause, args)
        {
        }

        public CompilerError(string code, LexicalInfo lexicalInfo, bool isParserError, params object[] args) : base(ErrorCodes.Format(code, args))
        {
            IsParserError = isParserError;
            if (null == lexicalInfo)
                throw new ArgumentNullException(nameof(lexicalInfo));
            _code = code;
            _lexicalInfo = lexicalInfo;
        }

        public CompilerError(string code, LexicalInfo lexicalInfo, string message, Exception cause) : base(message, cause)
        {
            if (null == lexicalInfo)
                throw new ArgumentNullException(nameof(lexicalInfo));
            _code = code;
            _lexicalInfo = lexicalInfo;
        }

        public CompilerError(LexicalInfo lexicalInfo, string message, Exception cause) : this("MCE0000", lexicalInfo, message, cause)
        {
        }


        public CompilerError(LexicalInfo data, string message) : this(data, message, null)
        {
        }

        public CompilerError(LexicalInfo data, Exception cause) : this(data, cause.Message, cause)
        {
        }

        public CompilerError(string message) : this(LexicalInfo.Empty, message, null)
        {
        }

        public string Code => _code;

        public LexicalInfo LexicalInfo => _lexicalInfo;

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (_lexicalInfo.Line > 0)
            {
                sb.Append(_lexicalInfo);
                sb.Append(": ");
            }
            sb.Append(_code);
            sb.Append(": ");
            sb.Append(_verbose ? base.ToString() : Message);
            return sb.ToString();
        }

        public string ToString(bool verbose)
        {
            var saved = _verbose;
            try
            {
                _verbose = _verbose || verbose;
                return ToString();
            }
            finally
            {
                _verbose = saved;
            }
        }

        public int CompareTo(CompilerError other)
        {
            var result = LexicalInfo.CompareTo(other.LexicalInfo);
            if (result != 0) return result;

            result = String.CompareOrdinal(Code, other.Code);
            if (result != 0) return result;

            return String.CompareOrdinal(Message, other.Message);
        }

        [ThreadStatic]
        private static bool _verbose;
    }
}
