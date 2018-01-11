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
    public abstract class Pair
    {
        private Pair _parent;
        internal string _name;
        internal AssignmentEnum _assignment;
        internal string _value;

        /// <summary>
        /// Name of the pair.
        /// </summary>
        public virtual string Name => _name;

        /// <summary>
        /// Gets or sets a value of pair assignment.
        /// </summary>
        public virtual AssignmentEnum Assignment => _assignment;

        /// <summary>
        /// Pair value of the pair. Use method <see cref="AppendChild"/> to set value of this property.
        /// </summary>
        public virtual Pair PairValue { get; private set; }

        /// <summary>
        /// Literal value of the pair.
        /// </summary>
        public virtual string Value => _value;

        /// <summary>
        /// If true then the pair has either literal value or pair value.
        /// </summary>
        /// <returns>True if the pair has either literal value or pair value.</returns>
        public virtual bool HasValue()
        {
            return PairValue != null || Value != null;
        }

        /// <summary>
        /// Creates an instance of the <see cref="Pair"/>.
        /// </summary>
        /// <param name="name">Pair name.</param>
        /// <param name="assignment"></param>
        /// <param name="value">Pair value.</param>
        protected Pair(string name, AssignmentEnum assignment, string value)
        {
            _name = name;
            _assignment = assignment;
            _value = value;
        }

        /// <summary>
        /// Creates an instance of the <see cref="Pair"/>.
        /// </summary>
        /// <param name="name">Pair name.</param>
        /// <param name="assignment"></param>
        protected Pair(string name, AssignmentEnum assignment)
        {
            _name = name;
            _assignment = assignment;
        }


        /// <summary>
        /// Creates an instance of the <see cref="Pair"/>.
        /// </summary>
        /// <param name="assignment"></param>
        /// <param name="value">Pair value.</param>
        protected Pair(AssignmentEnum assignment, string value)
        {
            _assignment = assignment;
            _value = value;
        }

        /// <summary>
        /// Creates an instance of the <see cref="Pair"/>.
        /// </summary>
        /// <param name="name">Pair name.</param>
        protected Pair(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Creates an instance of the <see cref="Pair"/>.
        /// </summary>
        /// <param name="assignment"></param>
        protected Pair(AssignmentEnum assignment)
        {
            _assignment = assignment;
        }

        /// <summary>
        /// Creates an instance of the <see cref="Pair"/>.
        /// </summary>
        protected Pair()
        {
        }

        /// <summary>
        /// This method is called when the pair is added as child to another pair.
        /// </summary>
        /// <param name="parent">Parent of the pair.</param>
        public virtual void InitializeParent(Pair parent)
        {
            if (_parent != null)
                throw new InvalidOperationException("Parent is already initialized.");
            _parent = parent;
        }

        internal void InitializeOverrideParent(Pair parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Parent of the pair.
        /// </summary>
        public virtual Pair Parent => _parent;

        /// <summary>
        /// Method is a part of the <see href="https://en.wikipedia.org/wiki/Visitor_pattern">visitor pattern</see> implementation.
        /// </summary>
        /// <param name="visitor">Visitor object</param>
        public abstract void Accept(IDomVisitor visitor);

        /// <summary>
        /// Adds another pair as a child. If pair has "pair assignment" <b>:=</b> then the method initializes property PairValue.
        /// </summary>
        /// <param name="child">Child pair to be added</param>
        public virtual void AppendChild(Pair child)
        {
            if (Assignment != AssignmentEnum.CE)
                throw new NotSupportedException(new StringBuilder("Cannot add ").Append(child.GetType().Name)
                    .Append(" in ").Append(GetType().Name).ToString());
            if (PairValue != null) throw new InvalidOperationException("PairValue is already initialized.");
            PairValue = child;
            child.InitializeParent(this);
        }

        /// <summary>
        /// Auxiliary function to convert assignment value to its string representation.
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns></returns>
        internal static string AssignmentToString(AssignmentEnum assignment)
        {
            switch (assignment)
            {
                case AssignmentEnum.C:
                    return ":";
                case AssignmentEnum.CC:
                    return "::";
                case AssignmentEnum.CCC:
                    return ":::";
                case AssignmentEnum.E:
                    return "=";
                case AssignmentEnum.EC:
                    return "=:";
                case AssignmentEnum.ECC:
                    return "=::";
                case AssignmentEnum.EE:
                    return "==";
                case AssignmentEnum.CE:
                    return ":=";
            }
            return "";
        }
    }
}