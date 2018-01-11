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
using System.Xml.Schema;

namespace Syntactik.Compiler
{
    /// <summary>
    /// Compiler parameters.
    /// </summary>
    public class CompilerParameters
    {
        /// <summary>
        /// Output directory. Used if <see cref="CompilerPipeline"/> produces output files.
        /// </summary>
        public string OutputDirectory { get; set; }
        /// <summary>
        /// <see cref="CompilerPipeline"/> is an ordered list of <see cref="ICompilerStep"/> implementations
        /// that should be executed in sequence.
        /// </summary>
        public CompilerPipeline Pipeline { get; set; }
        /// <summary>
        /// List of compiler inputs.
        /// </summary>
        public List<ICompilerInput> Input { get; } = new List<ICompilerInput>();
        /// <summary>
        /// Instance of <see cref="XmlSchemaSet"/> used to validate output XML.
        /// </summary>
        public XmlSchemaSet XmlSchemaSet { get; } = new XmlSchemaSet();
    }
}
