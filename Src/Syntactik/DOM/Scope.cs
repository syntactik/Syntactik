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
    /// Represents a Scope.
    /// </summary>
    public class Scope : Entity, INsNode, IContainer
    {
        private PairCollection<Entity> _entities;
        internal string _nsPrefix;

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

        /// <summary>
        /// Creates an instance of <see cref="Scope"/>.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="nsPrefix">Namespace prefix.</param>
        /// <param name="assignment">Pair assignment.</param>
        /// <param name="value">Element value.</param>
        public Scope(string name, string nsPrefix, AssignmentEnum assignment, string value) : base(name, assignment, value)
        {
            _nsPrefix = nsPrefix;
        }

        /// <summary>
        /// Namespace prefix of the attribute.
        /// </summary>
        public virtual string NsPrefix => _nsPrefix;


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

            if (child is Entity item && !(child is Argument))
            {
                Entities.Add(item);
            }
            else
            {
                base.AppendChild(child);
            }
        }
    }
}
