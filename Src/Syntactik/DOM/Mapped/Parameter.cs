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
        public Interval DelimiterInterval { get; }

        /// <inheritdoc />
        public ValueType ValueType { get; }

        /// <inheritdoc />
        public virtual bool IsValueNode { get; private set; }

        /// <inheritdoc />
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
        public Parameter(string name = null, DelimiterEnum delimiter = DelimiterEnum.None, string value = null,
            Interval nameInterval = null, Interval valueInterval = null, Interval delimiterInterval = null,
            int nameQuotesType = 0, int valueQuotesType = 0, ValueType valueType = ValueType.None, List<object> interpolationItems = null,
            int valueIndent = 0, bool isValueNode = false
        ) : base(name, delimiter, value)
        {
            ValueInterval = valueInterval;
            DelimiterInterval = delimiterInterval;
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
            IsValueNode = Parent?.Delimiter == DelimiterEnum.EC || Parent?.Delimiter == DelimiterEnum.CE;
        }

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
