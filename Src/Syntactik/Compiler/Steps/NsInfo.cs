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

namespace Syntactik.Compiler.Steps
{
    /// <summary>
    /// Collects a list of used namespaces and aliases in the ModuleMember (Document or AliasDef)
    /// </summary>
    public class NsInfo
    {
        public DOM.ModuleMember ModuleMember { get; private set; }
        public bool AliasesResolved { get; internal set; }
        private List<DOM.NamespaceDefinition> _namespaces;
        private List<DOM.Alias> _aliases;

        public List<DOM.NamespaceDefinition> Namespaces
        {
            get
            {
                return _namespaces ?? (_namespaces = new List<DOM.NamespaceDefinition>());
            }

            set
            {
                _namespaces = value;
            }
        }

        public List<DOM.Alias> Aliases
        {
            get
            {

                return _aliases ?? (_aliases = new List<DOM.Alias>());
            }

            set
            {
                _aliases = value;
            }
        }

        public NsInfo(DOM.ModuleMember currentDocument)
        {
            ModuleMember = currentDocument;
        }
    }
}
