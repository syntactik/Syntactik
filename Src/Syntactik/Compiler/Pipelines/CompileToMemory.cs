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
using Syntactik.Compiler.Steps;

namespace Syntactik.Compiler.Pipelines
{
    /// <summary>
    /// Compiler pipeline that stores generated output in memory. See <see cref="P:Syntactik.Compiler.CompilerContext.InMemoryOutputObjects" />
    /// </summary>
    public class CompileToMemory: CompilerPipeline
    {
        /// <summary>
        /// Creates instance of the pipeline.
        /// </summary>
        /// <param name="generateComments">If true generates XML comments.</param>
        public CompileToMemory(bool generateComments = false)
        {
            Steps.Add(new Parse());
            Steps.Add(new ProcessAliasesAndNamespaces());
            Steps.Add(new ValidateDocuments());
            Steps.Add(new CompileDocumentsToMemory(generateComments));
        }
    }
}
