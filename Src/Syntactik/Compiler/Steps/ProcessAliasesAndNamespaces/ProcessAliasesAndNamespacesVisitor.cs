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

using Syntactik.DOM;
using Syntactik.DOM.Mapped;
using Alias = Syntactik.DOM.Mapped.Alias;
using AliasDefinition = Syntactik.DOM.Mapped.AliasDefinition;
using Document = Syntactik.DOM.Mapped.Document;
using Element = Syntactik.DOM.Mapped.Element;
using Parameter = Syntactik.DOM.Mapped.Parameter;
using Scope = Syntactik.DOM.Mapped.Scope;

namespace Syntactik.Compiler.Steps
{
    class ProcessAliasesAndNamespacesVisitor : DOM.SyntactikDepthFirstVisitor
    {
        protected CompilerContext _context;
        private readonly NamespaceResolver _namespaceResolver;

        public ProcessAliasesAndNamespacesVisitor(CompilerContext context)
        {
            _context = context;
            _namespaceResolver = (NamespaceResolver)context.Properties["NamespaceResolver"];
        }

        public override void Visit(DOM.Module module)
        {
            _namespaceResolver.EnterModule(module);
            base.Visit(module);
        }

        public override void Visit(DOM.Document document)
        {
            _namespaceResolver.EnterDocument((Document) document);
            ProcessInterpolation((IPairWithInterpolation)document);
            base.Visit(document);
            Visit(document.PairValue);
        }

        public override void Visit(DOM.Alias alias)
        {
            _namespaceResolver.ProcessAlias((Alias)alias);
            _namespaceResolver.AddAlias((Alias)alias);
            ProcessInterpolation((IPairWithInterpolation)alias);
            base.Visit(alias);
            Visit(alias.PairValue);
        }

        public override void Visit(DOM.Parameter parameter)
        {
            _namespaceResolver.ProcessParameter((Parameter) parameter);
            ProcessInterpolation((IPairWithInterpolation)parameter);
            base.Visit(parameter);
            Visit(parameter.PairValue);
        }

        public override void Visit(DOM.AliasDefinition aliasDefinition)
        {
            _namespaceResolver.EnterAliasDef((AliasDefinition) aliasDefinition);
            ProcessInterpolation((IPairWithInterpolation)aliasDefinition);
            base.Visit(aliasDefinition);
            Visit(aliasDefinition.PairValue);
        }

        public override void Visit(DOM.Element element)
        {
            _namespaceResolver.ProcessNsPrefix((IMappedPair) element);
            ProcessInterpolation((IPairWithInterpolation) element);
            base.Visit(element);
            Visit(element.PairValue);
        }

        /// <summary>
        /// Process Aliases and Parameters specified in the string interpolation or concatenation.
        /// </summary>
        /// <param name="pair"></param>
        private void ProcessInterpolation(IPairWithInterpolation pair)
        {
            if (pair.InterpolationItems == null || pair.InterpolationItems.Count == 0) return;
            foreach (var item in pair.InterpolationItems)
            {
                if (item is Alias alias)
                {
                    _namespaceResolver.ProcessAlias(alias);
                    _namespaceResolver.AddAlias(alias);
                    continue;
                }

                if (item is DOM.Parameter param)
                {
                    _namespaceResolver.ProcessParameter((Parameter) param);
                    continue;
                }

                if (item is Element element) Visit((Pair) element);
            }
        }

        public override void Visit(DOM.Attribute attribute)
        {
            _namespaceResolver.ProcessNsPrefix((IMappedPair)attribute);
            ProcessInterpolation((IPairWithInterpolation)attribute);
            base.Visit(attribute);
            Visit(attribute.PairValue);
        }

        public override void Visit(DOM.Argument argument)
        {
            ProcessInterpolation((IPairWithInterpolation)argument);
            base.Visit(argument);
            Visit(argument.PairValue);
        }

        public override void Visit(DOM.NamespaceDefinition namespaceDefinition)
        {
            ProcessInterpolation((IPairWithInterpolation)namespaceDefinition);
            base.Visit(namespaceDefinition);
            Visit(namespaceDefinition.PairValue);
        }

        public override void Visit(DOM.Scope scope)
        {
            _namespaceResolver.ProcessNsPrefix((IMappedPair)scope);
            if (!string.IsNullOrEmpty(scope.Name))
            {
                var mapped = (Scope) scope;
                var pair = new Element
                (
                    scope.Name,
                    nameQuotesType: mapped.NameQuotesType,
                    nameInterval: mapped.NameInterval,
                    delimiter: scope.Delimiter,
                    delimiterInterval: mapped.DelimiterInterval,
                    value: scope.Value,
                    valueQuotesType: mapped.ValueQuotesType,
                    valueInterval: mapped.ValueInterval,
                    interpolationItems: mapped.InterpolationItems,
                    valueIndent: mapped.ValueIndent,
                    valueType: mapped.ValueType
                );

                pair.Entities.AddRangeOverrideParent(scope.Entities);
                scope.Entities.Clear();
                scope.Entities.Add(pair);
                ((Scope)scope).OverrideName(null);
            }
            base.Visit(scope);
            Visit(scope.PairValue);
        }
    }
}
