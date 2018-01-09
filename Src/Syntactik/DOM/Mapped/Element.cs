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
using System.Text.RegularExpressions;

namespace Syntactik.DOM.Mapped
{
    /// <summary>
    /// Represents an <see cref="DOM.Element"/> mapped to the source code.
    /// </summary>
    public class Element: DOM.Element, IMappedPair, IPairWithInterpolation, IChoiceNode, INsNodeOverridable
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
        public virtual bool IsValueNode => ValueType != ValueType.None && ValueType != ValueType.Object;

        /// <summary>
        /// List of interpolation objects.
        /// </summary>
        public List<object> InterpolationItems { get; private set; }

        /// <inheritdoc />
        public int ValueIndent { get; }

        /// <inheritdoc />
        public List<object> ChoiceObjects => !IsChoice ? null : InterpolationItems;

        /// <inheritdoc />
        public bool IsChoice { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="Element"/>.
        /// </summary>
        /// <param name="name">Pair name.</param>
        /// <param name="nsPrefix">Pair namespace prefix.</param>
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
        public Element(string name = null, string nsPrefix = null, AssignmentEnum assignment = AssignmentEnum.None, string value = null,
            Interval nameInterval = null, Interval valueInterval = null, Interval assignmentInterval = null,
            int nameQuotesType = 0, int valueQuotesType = 0, ValueType valueType = ValueType.None, List<object> interpolationItems = null,
            int valueIndent = 0
            ):base(name, assignment, value, nsPrefix)
        {
            ValueInterval = valueInterval;
            AssignmentInterval = assignmentInterval;
            NameInterval = nameInterval;
            NameQuotesType = nameQuotesType;
            ValueQuotesType = valueQuotesType;
            ValueType = valueType;
            InterpolationItems = interpolationItems;
            ValueIndent = valueIndent;
        }

        /// <summary>
        /// Splits full name on namespace prefix and name.
        /// </summary>
        /// <param name="name">Full name of a pair</param>
        /// <param name="nameQuotesType">Type of quotes used to define the name. 
        /// 0 - no quotes, 1 - single quotes, 2 - double quotes.</param>
        /// <returns>First item of tuple stores namespace prefix. Second item of tuple stores a name.</returns>
        public static Tuple<string, string> GetNameAndNs(string name, int nameQuotesType)
        {
            if (nameQuotesType > 0) return new Tuple<string, string>("", name);
            var match = Regex.Match(name, @"^(?:([^.]*)\.)?(.*)$");
            if (match.Success) return new Tuple<string, string>(match.Groups[1].Value, match.Groups[2].Value);
            return new Tuple<string, string>("", name);
        }

        /// <inheritdoc />
        public override void InitializeParent(Pair parent)
        {
            if (parent?.Assignment == AssignmentEnum.CC || parent?.Assignment == AssignmentEnum.ECC)
            {
                IsChoice = true;
            }
            base.InitializeParent(parent);
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
            else
                base.AppendChild(child);
        }

        /// <inheritdoc />
        public void OverrideNsPrefix(string nsPrefix)
        {
            _nsPrefix = nsPrefix;
        }

    }
}
