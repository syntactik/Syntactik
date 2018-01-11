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
    /// Represent an Attribute.
    /// </summary>
    public class Attribute : Entity, INsNode
    {
        internal string _nsPrefix;

        /// <inheritdoc />
        public override void Accept(IDomVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Creates an instance of <see cref="Attribute"/>.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <param name="nsPrefix">Namespace prefix.</param>
        /// <param name="assignment">Pair assignment.</param>
        /// <param name="value">Attribute value.</param>
        public Attribute(string name, string nsPrefix, AssignmentEnum assignment, string value) : base(name, assignment, value)
        {
            _nsPrefix = nsPrefix;
        }

        /// <summary>
        /// Namespace prefix of the attribute.
        /// </summary>
        public virtual string NsPrefix => _nsPrefix;
    }
}