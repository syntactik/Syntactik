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
    public class SyntactikDepthFirstVisitor : IDomVisitor
    {

        public virtual void OnAlias(Alias alias)
        {
            Visit(alias.Arguments);
            Visit(alias.Entities);
        }

        public virtual void OnAliasDefinition(AliasDefinition aliasDefinition)
        {
            Visit(aliasDefinition.NamespaceDefinitions);
            Visit(aliasDefinition.Entities);
        }

        public virtual void OnArgument(Argument argument)
        {
            Visit(argument.Entities);
        }

        public virtual void OnAttribute(Attribute attribute)
        {
        }

        public virtual void OnCompileUnit(CompileUnit compileUnit)
        {
            Visit(compileUnit.Modules);
        }
        public virtual void OnDocument(Document document)
        {
            Visit(document.NamespaceDefinitions);
            Visit(document.Entities);
        }

        public virtual void OnElement(Element element)
        {
            Visit(element.Entities);
        }

        public virtual void OnModule(Module module)
        {
            Visit(module.NamespaceDefinitions);
            Visit(module.Members);
        }

        public virtual void OnNamespaceDefinition(NamespaceDefinition namespaceDefinition)
        {
        }

        public virtual void OnScope(Scope scope)
        {
            Visit(scope.Entities);
        }

        protected virtual void OnPair(Pair pair)
        {
            pair.Accept(this);
        }

        public virtual void OnParameter(Parameter parameter)
        {
            Visit(parameter.Entities);
        }

        public virtual void OnComment(Comment comment)
        {
        }

        public void Visit(Pair pair)
        {
            if (pair == null) return;
            OnPair(pair);
        }

        public void Visit<T>(IEnumerable<T> items) where T : Pair
        {
            if (items == null) return;

            foreach (var pair in items)
                OnPair(pair);
        }
    }
}
