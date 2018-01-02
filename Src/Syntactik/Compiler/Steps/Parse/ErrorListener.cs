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
using System.Collections.Generic;
using Syntactik.DOM;

namespace Syntactik.Compiler
{
    public class ErrorListener : IErrorListener
    {
        private readonly CompilerContext _context;
        private readonly string _fileName;

        public ErrorListener(CompilerContext context, string fileName)
        {
            _context = context;
            _fileName = fileName;
        }
        public List<string> Errors { get; } = new List<string>();

        public void OnSyntaxError(int code, Interval interval, params object[] args)
        {
            _context.AddError(CompilerErrorFactory.ParserError(Syntactik.ParsingErrors.Format(code, args), _fileName, interval.Begin.Line,
                interval.Begin.Column));
        }
    }
}
