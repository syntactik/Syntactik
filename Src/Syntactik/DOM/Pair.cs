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
using System.Text;

namespace Syntactik.DOM
{
    /// <summary>
    /// Base class for all DOM classes. 
    /// </summary>
    public abstract class Pair{
        private string _value;
        private Pair _pairValue;
        private Pair _parent;
        private DelimiterEnum _delimiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pair"/> class.
        /// </summary>
        protected Pair()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pair"/> class.
        /// This constructor is used if pair descendant has default value of delimiter.
        /// </summary>
        /// <param name="delimiter"></param>
        protected Pair(DelimiterEnum delimiter)
        {
            _delimiter = delimiter;
        }

        /// <summary>
        /// Name of the pair.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Pair value of the pair.
        /// </summary>
        public virtual Pair PairValue
        {
            get => _pairValue;
            set
            {
                _pairValue = value;
                _value = null;
            }
        }

        /// <summary>
        /// Literal value of the pair.
        /// </summary>
        public virtual string Value
        {
            get => _value;
            set
            {
                _value = value;
                _pairValue = null;
            }
        }

        /// <summary>
        /// If true then the pair has either literal value or pair value.
        /// </summary>
        /// <returns></returns>
        public virtual bool HasValue()
        {
            return PairValue != null || Value != null;
        }

        /// <summary>
        /// Gets or sets a value of pair delimiter.
        /// </summary>
        public virtual DelimiterEnum Delimiter
        {
            get => _delimiter;
            set => _delimiter = value;
        }

        /// <summary>
        /// Stores info about quotes used to define name of the pair.
        /// 0 - no quotes
        /// 1 - single quotes
        /// 2 - double quotes
        /// </summary>
        public virtual int NameQuotesType { get; set; }

        /// <summary>
        /// Stores info about quotes used to define value of the pair.
        /// 0 - no quotes
        /// 1 - single quotes
        /// 2 - double quotes
        /// </summary>
        public virtual int ValueQuotesType { get; set; }

        /// <summary>
        /// This method is called when the pair is added as child to another pair.
        /// </summary>
        /// <param name="parent">Parent of the pair.</param>
        public virtual void InitializeParent(Pair parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Parent of the pair.
        /// </summary>
        public virtual Pair Parent => _parent;

        /// <summary>
        /// Method is a part the <see href="https://en.wikipedia.org/wiki/Visitor_pattern">visitor pattern</see> implementation.
        /// </summary>
        /// <param name="visitor">Visitor object</param>
        public abstract void Accept(IDomVisitor visitor);

        /// <summary>
        /// Adds another pair as a child.
        /// </summary>
        /// <param name="child">Child pair to be added</param>
        public virtual void AppendChild(Pair child)
        {
            throw new NotSupportedException(new StringBuilder("Cannot add ").Append(child.GetType().Name).Append(" in ").Append(GetType().Name).ToString());
        }

        /// <summary>
        /// Auxiliary function to convert delimiter value to its string representation.
        /// </summary>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        internal static string DelimiterToString(DelimiterEnum delimiter)
        {
            switch (delimiter)
            {
                case DelimiterEnum.C:
                    return ":";
                case DelimiterEnum.CC:
                    return "::";
                case DelimiterEnum.CCC:
                    return ":::";
                case DelimiterEnum.E:
                    return "=";
                case DelimiterEnum.EC:
                    return "=:";
                case DelimiterEnum.ECC:
                    return "=::";
                case DelimiterEnum.EE:
                    return "==";
                case DelimiterEnum.CE:
                    return ":=";
            }
            return "";
        }
    }
}