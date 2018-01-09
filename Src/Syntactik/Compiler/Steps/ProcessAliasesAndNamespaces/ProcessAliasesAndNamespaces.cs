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

namespace Syntactik.Compiler.Steps
{
    /// <summary>
    /// <see cref="ICompilerStep"/> that uses <see cref="ProcessAliasesAndNamespacesVisitor"/> and 
    /// <see cref="NamespaceResolver"/> to collect info about namespaces and aliases.
    /// </summary>
    public class ProcessAliasesAndNamespaces : ICompilerStep    
    {
        CompilerContext _context;

        /// <inheritdoc />
        public void Dispose()
        {
            _context = null;
        }

        private NamespaceResolver NamespaceResolver => (NamespaceResolver) _context.Properties["NamespaceResolver"];

        /// <inheritdoc />
        public void Initialize(CompilerContext context)
        {
            _context = context;
            context.Properties.Add("NamespaceResolver", new NamespaceResolver(context));
        }

        /// <inheritdoc />
        public void Run()
        {
            try
            {
                foreach (var module in _context.CompileUnit.Modules)
                {
                    DoProcessAliasesAndNamespaces(module, _context);
                }
                NamespaceResolver.ResolveAliasesAndDoChecks();
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }

        private void DoProcessAliasesAndNamespaces(DOM.Module module, CompilerContext context)
        {
            try
            {
                var visitor = new ProcessAliasesAndNamespacesVisitor(context);
                visitor.Visit(module);
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }
    }
}
