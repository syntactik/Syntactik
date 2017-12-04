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
    public class Module : Pair
    {
        #region Fields

        protected PairCollection<ModuleMember> _members;
        protected PairCollection<NamespaceDefinition> _namespaceDefinitions;

        public string FileName;
        protected Document _moduleDocument;

        #endregion

        #region Properties
        public int IndentMultiplicity { get; set; }
        public char IndentSymbol { get; set; }

        public virtual PairCollection<ModuleMember> Members
        {
            get { return _members ?? (_members = new PairCollection<ModuleMember>(this)); }
            set
            {
                if (value != _members)
                {
                    value?.InitializeParent(this);
                    _members = value;
                }
            }
        }

        public virtual PairCollection<NamespaceDefinition> NamespaceDefinitions
        {
            get { return _namespaceDefinitions ?? (_namespaceDefinitions = new PairCollection<NamespaceDefinition>(this)); }
            set
            {
                if (value != _namespaceDefinitions)
                {
                    value?.InitializeParent(this);
                    _namespaceDefinitions = value;
                }
            }
        }
        public virtual Document ModuleDocument => _moduleDocument;

        #endregion



        #region  Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnModule(this);
        }

        public Module()
        {
            _delimiter = DelimiterEnum.C;
        }

        public override void AppendChild(Pair child)
        {
            Value = null;
            PairValue = null;

            var item = child as ModuleMember;
            if (item != null)
            {
                Members.Add(item);
                return;
            }

            var ns = child as NamespaceDefinition;
            if (ns != null)
            {
                NamespaceDefinitions.Add(ns);
                return;
            }

            var entity = child as Entity;
            if (entity != null)
            {
                AddEntity(entity);
            }
            else
            {
                base.AppendChild(child);
            }
        }

        protected void AddEntity(Entity entity)
        {
            if (_moduleDocument == null) CreateModuleDocument();
            _moduleDocument.AppendChild(entity);

        }

        protected void CreateModuleDocument()
        {
            _moduleDocument = new Mapped.Document
            {
                Name = Name,
                NameInterval = new Interval(new CharLocation(1,1,1), new CharLocation(1, 1, 1)),
                Delimiter = DelimiterEnum.None
            };
            Members.Add(_moduleDocument);
        }
        #endregion
    }
}
