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
using System.Linq;
using Syntactik.DOM;

namespace Syntactik.Compiler.Steps
{
    /// <summary>
    /// <see cref="ICompilerStep"/> that is using data provided by <see cref="NamespaceResolver"/> to validate Syntactik modules.
    /// </summary>
    public class ValidateDocuments: ICompilerStep
    {
        CompilerContext _context;

        /// <inheritdoc />
        public void Dispose()
        {
            _context = null;
        }

        /// <inheritdoc />
        public void Initialize(CompilerContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public void Run()
        {
            try
            {
                if (_context.Errors.Any(e => e.IsParserError)) return; //don't process if parser errors found.
                foreach (var module in _context.CompileUnit.Modules)
                {
                    DoValidateDocuments(module, _context);
                }
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }

        private void DoValidateDocuments(Module module, CompilerContext context)
        {
            try
            {
                SyntactikDepthFirstVisitor visitor = new ValidatingDocumentsVisitor(context);

                visitor.Visit(module);
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }
    }
}
