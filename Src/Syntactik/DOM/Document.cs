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

namespace Syntactik.DOM
{
    [Serializable]
    public class Document : ModuleMember, IContainer
    {
        // Fields
        protected PairCollection<Entity> _entities;

        public Entity DocumentElement;

        // Properties
        public virtual PairCollection<Entity> Entities
        {
            get { return _entities ?? (_entities = new PairCollection<Entity>(this)); }
            set { throw new NotImplementedException(); }
        }

        // Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnDocument(this);
        }

        public override void AppendChild(Pair child)
        {
            Value = null;
            PairValue = null;

            var entity = child as Entity;
            if (entity != null)
            {
                DocumentElement = entity;
                Entities.Add(entity);
                return;
            }

            var ns = child as NamespaceDefinition;
            if (ns != null)
            {
                NamespaceDefinitions.Add(ns);
                return;
            }
            base.AppendChild(child);
        }
    }
}
