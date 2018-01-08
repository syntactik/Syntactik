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
namespace Syntactik.DOM
{
    /// <summary>
    /// Member of the module: <see cref="Document"/> or <see cref="AliasDefinition"/>.
    /// </summary>
    public abstract class ModuleMember : Pair
    {
        private PairCollection<NamespaceDefinition> _namespaces;

        /// <summary>
        /// Parent <see cref="Module"/> of the <see cref="ModuleMember"/>.
        /// </summary>
        public virtual Module Module => (Parent as Module);

        /// <summary>
        /// Creates an instance of <see cref="ModuleMember"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="assignment"></param>
        /// <param name="value"></param>
        protected ModuleMember(string name = null, AssignmentEnum assignment = AssignmentEnum.None, string value = null):base(name, assignment, value)
        {
        }

        public virtual PairCollection<NamespaceDefinition> NamespaceDefinitions
        {
            get => _namespaces ?? (_namespaces = new PairCollection<NamespaceDefinition>(this));
            set
            {
                if (value != _namespaces)
                {
                    value?.InitializeParent(this);
                    _namespaces = value;
                }
            }
        }
    }
}
