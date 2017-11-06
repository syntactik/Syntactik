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
using Syntactik.DOM;

namespace Syntactik.Compiler
{
    public class SyntactikCompiler
    {
        public CompilerParameters Parameters { get; private set; }

        public SyntactikCompiler()
        {
            Parameters = new CompilerParameters();
        }
        public SyntactikCompiler(CompilerParameters parameters)
        {
            Parameters = parameters;
        }

        public CompilerContext Run(CompileUnit compileUnit = null)
        {
            if (compileUnit == null)
                compileUnit = new CompileUnit();
            var context = new CompilerContext(Parameters, compileUnit);
            Parameters.Pipeline.Run(context);
            return context;
        }
    }
}
