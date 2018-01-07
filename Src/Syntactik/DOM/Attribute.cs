﻿#region license
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
        /// Creates a new instance of <see cref="Attribute"/>.
        /// </summary>
        public Attribute(string name, DelimiterEnum delimiter, string value, string nsPrefix) : base(name, delimiter, value)
        {
            _nsPrefix = nsPrefix;
        }

        /// <summary>
        /// Namespace prefix of the attribute.
        /// </summary>
        public virtual string NsPrefix => _nsPrefix;
    }
}