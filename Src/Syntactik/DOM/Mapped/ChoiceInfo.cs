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
using Syntactik.Compiler;

namespace Syntactik.DOM.Mapped
{
    /// <summary>
    /// Stores info about pair with choice assignment. 
    /// </summary>
    public class ChoiceInfo
    {
        /// <summary>
        /// Parent <see cref="ChoiceInfo"/> of the choice object.
        /// </summary>
        public ChoiceInfo Parent { get; }

        /// <summary>
        /// Node that defines the choice object.
        /// </summary>
        public Pair ChoiceNode { get; }

        /// <summary>
        /// List of <see cref="ChoiceInfo"/> of direct or indirect descendant choices.
        /// </summary>
        public List<ChoiceInfo> Children { get; private set; }

        /// <summary>
        /// List of compilation error occurred in the current choice node.
        /// </summary>
        public List<CompilerError> Errors
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds child <see cref="ChoiceInfo"/>.
        /// </summary>
        /// <param name="child">Child <see cref="ChoiceInfo"/>.</param>
        public void AddChild(ChoiceInfo child)
        {
            if (Children == null) Children = new List<ChoiceInfo>();
            Children.Add(child);
        }

        /// <summary>
        /// Adds an error to the list of choice errors.
        /// </summary>
        /// <param name="error"><see cref="CompilerError"/> to be added to the list of errors.</param>
        public void AddError(CompilerError error)
        {
            if (Errors == null) Errors = new List<CompilerError>();
            Errors.Add(error);
        }

        /// <summary>
        /// Creates instance of <see cref="ChoiceInfo"/>.
        /// </summary>
        /// <param name="parent">Parent <see cref="ChoiceInfo"/> of the created choice object.</param>
        /// <param name="choiceNode">Node that defines the created choice.</param>
        public ChoiceInfo(ChoiceInfo parent, Pair choiceNode)
        {
            Parent = parent;
            ChoiceNode = choiceNode;
        }
    }
}
