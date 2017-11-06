using System.Collections.Generic;
using Syntactik.Compiler;

namespace Syntactik.DOM.Mapped
{
    public class ChoiceInfo
    {
        public ChoiceInfo Parent { get; }
        public Pair ChoiceNode { get; }
        public List<ChoiceInfo> Children { get; private set; }

        public List<CompilerError> Errors
        {
            get;
            private set;
        }

        public void AddChild(ChoiceInfo child)
        {
            if (Children == null) Children = new List<ChoiceInfo>();
            Children.Add(child);
        }

        public void AddError(CompilerError error)
        {
            if (Errors == null) Errors = new List<CompilerError>();
            Errors.Add(error);
        }

        public ChoiceInfo(ChoiceInfo parent, Pair choiceNode)
        {
            Parent = parent;
            ChoiceNode = choiceNode;
        }
    }
}
