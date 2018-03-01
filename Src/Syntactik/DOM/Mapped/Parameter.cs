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
    /// Represents <see cref="DOM.Parameter"/> mapped to the source code.
    /// </summary>
    public class Parameter: DOM.Parameter, IMappedPair, IPairWithInterpolation
    {
        /// <inheritdoc />
        public Interval NameInterval { get; }

        /// <inheritdoc />
        public int NameQuotesType { get; }

        /// <inheritdoc />
        public Interval ValueInterval { get; }

        /// <inheritdoc />
        public int ValueQuotesType { get; }

        /// <inheritdoc />
        public Interval AssignmentInterval { get; }

        /// <inheritdoc />
        public ValueType ValueType { get; }

        /// <inheritdoc />
        public BlockType BlockType { get; set; }

        /// <inheritdoc />
        public virtual bool IsValueNode { get; private set; }

        /// <summary>
        /// List of interpolation objects.
        /// </summary>
        public List<object> InterpolationItems { get; private set; }

        /// <inheritdoc />
        public int ValueIndent { get; }

        /// <summary>
        /// <see cref="AliasDefinition"/> where the parameter is defined.
        /// </summary>
        public AliasDefinition AliasDefinition { get; protected internal set; }

        /// <summary>
        /// Creates a new instance of <see cref="Parameter"/>.
        /// </summary>
        /// <param name="name">Pair name.</param>
        /// <param name="assignment">Pair assignment.</param>
        /// <param name="value">Pair value.</param>
        /// <param name="nameInterval">Name <see cref="Interval"/>.</param>
        /// <param name="valueInterval">Value <see cref="Interval"/>.</param>
        /// <param name="assignmentInterval">Assignment <see cref="Interval"/>.</param>
        /// <param name="nameQuotesType">Name quotes type.</param>
        /// <param name="valueQuotesType">Value quotes type.</param>
        /// <param name="valueType">Type of value.</param>
        /// <param name="interpolationItems">List of interpolation objects.</param>
        /// <param name="valueIndent">Indent of value in the source code.</param>
        /// <param name="isValueNode">True if node is associated with or has literal value.</param>
        public Parameter(string name = null, AssignmentEnum assignment = AssignmentEnum.None, string value = null,
            Interval nameInterval = null, Interval valueInterval = null, Interval assignmentInterval = null,
            int nameQuotesType = 0, int valueQuotesType = 0, ValueType valueType = ValueType.None, List<object> interpolationItems = null,
            int valueIndent = 0, bool isValueNode = false
        ) : base(name, assignment, value)
        {
            ValueInterval = valueInterval;
            AssignmentInterval = assignmentInterval;
            NameInterval = nameInterval;
            NameQuotesType = nameQuotesType;
            ValueQuotesType = valueQuotesType;
            ValueType = valueType;
            InterpolationItems = interpolationItems;
            ValueIndent = valueIndent;
            IsValueNode = isValueNode;
        }

        /// <inheritdoc />
        public override void InitializeParent(Pair parent)
        {
            base.InitializeParent(parent);
            IsValueNode = Parent?.Assignment == AssignmentEnum.EC || Parent?.Assignment == AssignmentEnum.CE ||
                //JSON value
                Parent?.Assignment == AssignmentEnum.C && Parent is IMappedPair mp && mp.BlockType == BlockType.Default 
                && Parent?.Parent is IMappedPair pmp && pmp.BlockType == BlockType.JsonObject; 
        }

        /// <inheritdoc />
        public override void AppendChild(Pair child)
        {
            if (Assignment == AssignmentEnum.EC)
            {
                if (InterpolationItems == null) InterpolationItems = new List<object>();
                InterpolationItems.Add(child);
                child.InitializeParent(this);
            }
            else if (BlockType == BlockType.Default && Parent is IMappedPair mp && mp.BlockType == BlockType.JsonObject &&
                     (child is DOM.Alias || child is DOM.Parameter))
            {
                PairValue = child;
                child.InitializeParent(this);
            }
            else
                base.AppendChild(child);
        }
    }
}
