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

namespace Syntactik.Compiler
{
    /// <summary>
    /// An ordered list of <see cref="ICompilerStep"/> implementations
    /// that should be executed in sequence.
    /// </summary>
    public class CompilerPipeline
    {
        /// <summary>
        /// An ordered list of <see cref="ICompilerStep"/> implementations
        /// that should be executed in sequence.
        /// </summary>
        public List<ICompilerStep> Steps { get; } = new List<ICompilerStep>();

        /// <summary>
        /// Orderly executes compiler steps of the pipeline.
        /// </summary>
        /// <param name="context"></param>
        public virtual void Run(CompilerContext context)
        {
            foreach (ICompilerStep step in Steps)
            {
                step.Initialize(context);
                step.Run();
            }
        }

        /// <summary>
        /// Adds step to the pipeline.
        /// </summary>
        /// <param name="step"></param>
        /// <returns>Current instance of the pipeline.</returns>
        public CompilerPipeline Add(ICompilerStep step)
        {
            Steps.Add(step);
            return this;
        }
    }
}