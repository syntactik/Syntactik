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
    /// Entity is a <see cref="Pair"/> that can be added to <see cref="IContainer"/> node.
    /// </summary>
    public abstract class Entity : Pair
    {
        /// <summary>
        /// Creates an instance of <see cref="Entity"/>.
        /// </summary>
        /// <param name="name">Entity name.</param>
        /// <param name="assignment">Pair assignment.</param>
        /// <param name="value">Entity value.</param>
        protected Entity(string name, AssignmentEnum assignment, string value) : base(name, assignment, value)
        {
        }
    }
}
