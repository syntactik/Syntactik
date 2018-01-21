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
    /// Represents an Alias.
    /// </summary>
    public class Alias : Entity, IContainer
    {
        private PairCollection<Argument> _arguments;
        private PairCollection<Entity> _entities;

        /// <summary>
        /// Collection of child pairs.
        /// </summary>
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
        /// Collection of arguments.
        /// </summary>
        public virtual PairCollection<Argument> Arguments
        {
            get => _arguments ?? (_arguments = new PairCollection<Argument>(this));
            set
            {
                if (value != _arguments)
                {
                    value?.InitializeParent(this);
                    _arguments = value;
                }
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="Alias"/>.
        /// </summary>
        /// <param name="name">Alias name.</param>
        /// <param name="assignment">Pair assignment.</param>
        /// <param name="value">Alias value.</param>
        public Alias(string name = null, AssignmentEnum assignment = AssignmentEnum.None, string value = null) : base(name, assignment, value)
        {
        }

        /// <summary>
        /// Method is a part the <see href="https://en.wikipedia.org/wiki/Visitor_pattern">visitor pattern</see> implementation.
        /// </summary>
        /// <param name="visitor">Visitor object.</param>
        public override void Accept(IDomVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Adds another pair as a child.
        /// </summary>
        /// <param name="child">Child pair to be added</param>
        public override void AppendChild(Pair child)
        {
            if (Assignment == AssignmentEnum.CE && !(child is Comment))
            {
                base.AppendChild(child);
            }
            else if (child is Argument item)
            {
                Arguments.Add(item);
            }
            else
            {
                Entities.Add((Entity)child);
            }
        }
    }
}
