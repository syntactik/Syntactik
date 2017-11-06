using Syntactik.DOM.Mapped;

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

        public override void OnModule(DOM.Module module)
        {
            _namespaceResolver.EnterModule(module);
            base.OnModule(module);
        }

        public override void OnDocument(DOM.Document document)
        {
            _namespaceResolver.EnterDocument((Document) document);
            ProcessInterpolation((IPairWithInterpolation)document);
            base.OnDocument(document);
            Visit(document.PairValue);
        }

        public override void OnAlias(DOM.Alias alias)
        {
            _namespaceResolver.ProcessAlias((Alias)alias);
            _namespaceResolver.AddAlias((Alias)alias);
            ProcessInterpolation((IPairWithInterpolation)alias);
            base.OnAlias(alias);
            Visit(alias.PairValue);
        }

        public override void OnParameter(DOM.Parameter parameter)
        {
            _namespaceResolver.ProcessParameter((Parameter) parameter);
            ProcessInterpolation((IPairWithInterpolation)parameter);
            base.OnParameter(parameter);
            Visit(parameter.PairValue);
        }

        public override void OnAliasDefinition(DOM.AliasDefinition aliasDefinition)
        {
            _namespaceResolver.EnterAliasDef((AliasDefinition) aliasDefinition);
            ProcessInterpolation((IPairWithInterpolation)aliasDefinition);
            base.OnAliasDefinition(aliasDefinition);
            Visit(aliasDefinition.PairValue);
        }

        public override void OnElement(DOM.Element element)
        {
            _namespaceResolver.ProcessNsPrefix((IMappedPair) element);
            ProcessInterpolation((IPairWithInterpolation) element);
            base.OnElement(element);
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
                var alias = item as Alias;
                if (alias != null)
                {
                    _namespaceResolver.ProcessAlias(alias);
                    _namespaceResolver.AddAlias(alias);
                    continue;
                }

                var param = item as DOM.Parameter;
                if (param != null)
                {
                    _namespaceResolver.ProcessParameter((Parameter) param);
                    continue;
                }

                var element = item as Element;
                if (element != null) Visit(element);
            }
        }

        public override void OnAttribute(DOM.Attribute attribute)
        {
            _namespaceResolver.ProcessNsPrefix((IMappedPair)attribute);
            ProcessInterpolation((IPairWithInterpolation)attribute);
            base.OnAttribute(attribute);
            Visit(attribute.PairValue);
        }

        public override void OnArgument(DOM.Argument argument)
        {
            ProcessInterpolation((IPairWithInterpolation)argument);
            base.OnArgument(argument);
            Visit(argument.PairValue);
        }

        public override void OnNamespaceDefinition(DOM.NamespaceDefinition namespaceDefinition)
        {
            ProcessInterpolation((IPairWithInterpolation)namespaceDefinition);
            base.OnNamespaceDefinition(namespaceDefinition);
            Visit(namespaceDefinition.PairValue);
        }

        public override void OnScope(DOM.Scope scope)
        {
            _namespaceResolver.ProcessNsPrefix((IMappedPair)scope);
            if (!string.IsNullOrEmpty(scope.Name))
            {
                var mapped = (Scope) scope;
                var pair = new Element
                {
                    Name = scope.Name,
                    NameQuotesType = scope.NameQuotesType,
                    NameInterval = mapped.NameInterval,
                    Delimiter = scope.Delimiter,
                    DelimiterInterval = mapped.DelimiterInterval,
                    Value = scope.Value,
                    ValueQuotesType = scope.ValueQuotesType,
                    ValueInterval = mapped.ValueInterval,
                    InterpolationItems = mapped.InterpolationItems,
                    ValueIndent = mapped.ValueIndent,
                    ValueType = mapped.ValueType
                };
                pair.Entities.AddRange(scope.Entities);
                scope.Entities.Clear();
                scope.Entities.Add(pair);
                scope.Name = null;
            }
            base.OnScope(scope);
            Visit(scope.PairValue);
        }
    }
}
