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
using System.Collections.Generic;
using System.Linq;

namespace Syntactik.DOM.Mapped
{
    /// <summary>
    /// Represents <see cref="DOM.AliasDefinition"/> mapped to the source code.
    /// </summary>
    public class AliasDefinition : DOM.AliasDefinition, IMappedPair, IPairWithInterpolation
    {
        private List<Parameter> _parameters;

        /// <inheritdoc />
        public AliasDefinition()
        {
            SyncTime = DateTime.Now.Ticks;
        }

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
        internal long SyncTime { get; set; } //This field is used in completion.

        /// <summary>
        /// List of <see cref="DOM.Parameter"/> defined in the <see cref="DOM.AliasDefinition"/>
        /// </summary>
        public List<Parameter> Parameters
        {
            get => _parameters ?? (_parameters = new List<Parameter>());
            set
            {
                if (_parameters != null && value != _parameters)
                {
                    _parameters = value;
                }
            }
        }
        /// <summary>
        /// True if <see cref="DOM.AliasDefinition"/> has a default block parameter.
        /// </summary>
        public bool HasDefaultBlockParameter { get; set; }
        /// <summary>
        /// True if <see cref="DOM.AliasDefinition"/> has a default value parameter.
        /// </summary>
        public bool HasDefaultValueParameter { get; set; }

        /// <summary>
        /// True if <see cref="DOM.AliasDefinition"/> has a circular reference.
        /// </summary>
        public bool HasCircularReference { get; set; }

        /// <inheritdoc />
        public override void AppendChild(Pair child)
        {
            if (child is NamespaceDefinition)
            {
                if (Entities.Any(e => !(e is Comment))) throw new ApplicationException("Namespaces must be defined first");
            }
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
