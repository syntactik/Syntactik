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
    public class Element: DOM.Element, IMappedPair, IPairWithInterpolation, IChoiceNode
    {
        public Interval NameInterval { get; set; }
        public Interval ValueInterval { get; set; }
        public Interval DelimiterInterval { get; set; }
        public ValueType ValueType { get; set; }
        public virtual bool IsValueNode => ValueType != ValueType.None && ValueType != ValueType.Object;
        public List<object> InterpolationItems { get; set; }
        public int ValueIndent { get; set; }
        public List<object> ChoiceObjects => !IsChoice ? null : InterpolationItems;

        public bool IsChoice { get; private set; }

        public static Tuple<string, string> GetNameAndNs(string name, int nameQuotesType)
        {
            if (nameQuotesType > 0) return new Tuple<string, string>("", name);
            var match = Regex.Match(name, @"^(?:([^.]*)\.)?(.*)$");
            if (match.Success) return new Tuple<string, string>(match.Groups[1].Value, match.Groups[2].Value);
            return new Tuple<string, string>("", name);
        }

        public override void InitializeParent(Pair parent)
        {
            if (parent.Delimiter == DelimiterEnum.CC || parent.Delimiter == DelimiterEnum.ECC)
            {
                IsChoice = true;
            }
            base.InitializeParent(parent);
        }

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
