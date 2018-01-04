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

namespace Syntactik.DOM
{
    /// <summary>
    /// Depth first implementation of DOM-visitor.
    /// </summary>
    public class SyntactikDepthFirstVisitor : IDomVisitor
    {
        /// <inheritdoc />
        public virtual void Visit(Alias alias)
        {
            Visit(alias.Arguments);
            Visit(alias.Entities);
        }

        /// <inheritdoc />
        public virtual void Visit(AliasDefinition aliasDefinition)
        {
            Visit(aliasDefinition.NamespaceDefinitions);
            Visit(aliasDefinition.Entities);
        }

        /// <inheritdoc />
        public virtual void Visit(Argument argument)
        {
            Visit(argument.Entities);
        }

        /// <inheritdoc />
        public virtual void Visit(Attribute attribute)
        {
        }

        /// <inheritdoc />
        public virtual void Visit(CompileUnit compileUnit)
        {
            Visit(compileUnit.Modules);
        }

        /// <inheritdoc />
        public virtual void Visit(Document document)
        {
            Visit(document.NamespaceDefinitions);
            Visit(document.Entities);
        }

        /// <inheritdoc />
        public virtual void Visit(Element element)
        {
            Visit(element.Entities);
        }

        /// <inheritdoc />
        public virtual void Visit(Module module)
        {
            Visit(module.NamespaceDefinitions);
            Visit(module.Members);
        }

        /// <inheritdoc />
        public virtual void Visit(NamespaceDefinition namespaceDefinition)
        {
        }

        /// <inheritdoc />
        public virtual void Visit(Scope scope)
        {
            Visit(scope.Entities);
        }

        /// <inheritdoc />
        public virtual void Visit(Parameter parameter)
        {
            Visit(parameter.Entities);
        }

        /// <inheritdoc />
        public virtual void Visit(Comment comment)
        {
        }

        /// <summary>
        /// Method is wrapping call to Accept method of every node.
        /// </summary>
        /// <param name="pair">Current visiting pair.</param>
        protected virtual void OnPair(Pair pair)
        {
            pair.Accept(this);
        }

        /// <summary>
        /// Tells visitor to visit a pair.
        /// </summary>
        /// <param name="pair"><see cref="Pair"/> to visit.</param>
        public void Visit(Pair pair)
        {
            if (pair == null) return;
            OnPair(pair);
        }

        /// <summary>
        /// Tells visitor to visit each pair in collection.
        /// </summary>
        /// <param name="items">Collection of pairs to visit.</param>
        public void Visit<T>(IEnumerable<T> items) where T : Pair
        {
            if (items == null) return;

            foreach (var pair in items)
                OnPair(pair);
        }
    }
}
