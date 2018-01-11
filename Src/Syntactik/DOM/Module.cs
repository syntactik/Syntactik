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
    /// DOM object that corresponds to a single source of text, like file, string, stream etc.
    /// Modules that belong to the same <see cref="CompileUnit"/> must have different names.
    /// </summary>
    public class Module : Pair
    {
        private PairCollection<ModuleMember> _members;
        private PairCollection<NamespaceDefinition> _namespaceDefinitions;

        private Document _moduleDocument;

        /// <summary>
        /// Number of indent symbol needed to form a single indent.
        /// </summary>
        public int IndentMultiplicity { get; internal set; }

        /// <summary>
        /// Symbol used for indent (space or tab).
        /// </summary>
        public char IndentSymbol { get; internal set; }

        /// <summary>
        /// Path to the module file.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Collection of <see cref="Document"/> and <see cref="AliasDefinition"/>.
        /// </summary>
        public virtual PairCollection<ModuleMember> Members
        {
            get => _members ?? (_members = new PairCollection<ModuleMember>(this));
            set
            {
                if (value != _members)
                {
                    value?.InitializeParent(this);
                    _members = value;
                }
            }
        }

        /// <summary>
        /// Collection of <see cref="NamespaceDefinition"/>.
        /// </summary>
        public virtual PairCollection<NamespaceDefinition> NamespaceDefinitions
        {
            get => _namespaceDefinitions ?? (_namespaceDefinitions = new PairCollection<NamespaceDefinition>(this));
            set
            {
                if (value != _namespaceDefinitions)
                {
                    value?.InitializeParent(this);
                    _namespaceDefinitions = value;
                }
            }
        }

        /// <summary>
        /// <see cref="Document"/> of module is implicitly declared by added <see cref="Entity"/> as module child.
        /// </summary>
        public virtual Document ModuleDocument => _moduleDocument;


        /// <inheritdoc />
        public override void Accept(IDomVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Creates an instance of <see cref="Module"/>.
        /// </summary>
        /// <param name="name">Module name.</param>
        /// <param name="fileName">Path to the file of module.</param>
        public Module(string name, string fileName = null): base(name, AssignmentEnum.C)
        {
            FileName = fileName;
        }

        /// <inheritdoc />
        public override void AppendChild(Pair child)
        {

            if (child is ModuleMember item)
            {
                Members.Add(item);
                return;
            }

            if (child is NamespaceDefinition ns)
            {
                NamespaceDefinitions.Add(ns);
                return;
            }

            if (child is Entity entity)
            {
                AddEntity(entity);
            }
            else
            {
                base.AppendChild(child);
            }
        }

        /// <summary>
        /// Adds entity to the <see cref="ModuleDocument"/>.
        /// </summary>
        /// <param name="entity"><see cref="Entity"/> to be added to the block of <see cref="ModuleDocument"/>.</param>
        protected virtual void AddEntity(Entity entity)
        {
            if (_moduleDocument == null) CreateModuleDocument();
            _moduleDocument.AppendChild(entity);

        }

        /// <summary>
        /// Creates an empty <see cref="ModuleDocument"/>.
        /// </summary>
        protected virtual void CreateModuleDocument()
        {
            _moduleDocument = new Mapped.Document
            (
                Name,
                nameInterval : new Interval(new CharLocation(1,1,1), new CharLocation(1, 1, 1))
            );
            Members.Add(_moduleDocument);
        }
    }
}
