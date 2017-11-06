using System.Collections.Generic;

namespace Syntactik.DOM.Mapped
{
    public interface IChoiceNode
    {
        List<object> ChoiceObjects { get; }
        bool IsChoice { get; }
    }

}