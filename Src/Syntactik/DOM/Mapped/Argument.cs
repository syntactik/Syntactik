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
using System.Collections.Generic;

namespace Syntactik.DOM.Mapped
{
    /// <summary>
    /// Represents an Argument mapped to the source code. 
    /// </summary>
    public class Argument: DOM.Argument, IMappedPair, IPairWithInterpolation
    {
        /// <inheritdoc />
        public Interval NameInterval { get; set; }

        /// <inheritdoc />
        public Interval ValueInterval { get; set; }

        /// <inheritdoc />
        public Interval DelimiterInterval { get; set; }

        /// <inheritdoc />
        public ValueType ValueType { get; set; }

        /// <inheritdoc />
        public virtual bool IsValueNode => ValueType != ValueType.None && ValueType != ValueType.Object;

        /// <inheritdoc />
        public List<object> InterpolationItems { get; set; }

        /// <inheritdoc />
        public int ValueIndent { get; set; }

        /// <inheritdoc />
        public override void AppendChild(Pair child)
        {
            if (Delimiter == DelimiterEnum.EC)
            {
                if (InterpolationItems == null) InterpolationItems = new List<object>();
                InterpolationItems.Add(child);
                child.InitializeParent(this);
            }
            else
                base.AppendChild(child);
        }

    }
}
