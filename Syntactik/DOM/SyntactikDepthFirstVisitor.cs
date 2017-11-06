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
