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

namespace Syntactik.Compiler.Steps
{
    /// <summary>
    /// Collects a list of used namespaces and aliases for <see cref="DOM.ModuleMember"/> (<see cref="Document"/> 
    /// or <see cref="AliasDefinition"/>).
    /// </summary>
    public class NsInfo
    {
        /// <summary>
        /// <see cref="ModuleMember"/> that the collected information is related to.
        /// </summary>
        public ModuleMember ModuleMember { get; }
        /// <summary>
        /// True if <see cref="ModuleMember"/> is <see cref="AliasDefinition"/> and it has a complete information
        /// about namespaces and aliases.
        /// </summary>
        public bool AliasesResolved { get; internal set; }
        private List<NamespaceDefinition> _namespaces;
        private List<Alias> _aliases;

        /// <summary>
        /// Stores info about namespaces used directly or indirectly (trough aliases) in the <see cref="ModuleMember"/>.
        /// </summary>
        public List<NamespaceDefinition> Namespaces
        {
            get => _namespaces ?? (_namespaces = new List<NamespaceDefinition>());

            set => _namespaces = value;
        }

        /// <summary>
        /// Stores info about aliases used directly in the <see cref="ModuleMember"/>.
        /// </summary>
        public List<Alias> Aliases
        {
            get => _aliases ?? (_aliases = new List<Alias>());

            set => _aliases = value;
        }

        /// <summary>
        /// Create an instance of <see cref="NsInfo"/>.
        /// </summary>
        /// <param name="moduleMember"><see cref="ModuleMember"/> that the collected information is related to.</param>
        public NsInfo(ModuleMember moduleMember)
        {
            ModuleMember = moduleMember;
        }
    }
}
