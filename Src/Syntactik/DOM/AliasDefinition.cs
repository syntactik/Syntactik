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
    /// Represents an AliasDefinition.
    /// </summary>
    public class AliasDefinition : ModuleMember, IContainer
    {
        private PairCollection<Entity> _entities;

        /// <summary>
        /// Creates a new instance of <see cref="AliasDefinition"/>.
        /// </summary>
        /// <param name="name">Pair name.</param>
        /// <param name="assignment">Pair assignment.</param>
        /// <param name="value">Pair value.</param>
        public AliasDefinition(string name, AssignmentEnum assignment, string value) : base(name, assignment, value)
        {
        }


        /// <inheritdoc />
        public override void Accept(IDomVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override void AppendChild(Pair child)
        {
            if (Assignment == AssignmentEnum.CE && !(child is Comment))
            {
                base.AppendChild(child);
                return;
            }

            if (child is Entity entity && !(child is Argument))
            {
                Entities.Add(entity);
                return;
            }

            if (child is NamespaceDefinition ns)
            {
                NamespaceDefinitions.Add(ns);
                return;
            }

            base.AppendChild(child);
        }

        /// <inheritdoc />
        public virtual PairCollection<Entity> Entities
        {
            get => _entities ?? (_entities = new PairCollection<Entity>(this));
            set
            {
                if (value != _entities)
                {
                    value?.InitializeParent(this);
                    _entities = value;
                }
            }
        }
    }
}
