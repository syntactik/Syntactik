using System;

namespace Syntactik.DOM
{
    [Serializable]
    public class CompileUnit : Pair
    {
        // Fields
        private PairCollection<Module> _modules;


        // Properties
        public virtual PairCollection<Module> Modules
        {
            get { return _modules ?? (_modules = new PairCollection<Module>(this)); }
            set
            {
                if (value == _modules) return;
                value?.InitializeParent(this);
                _modules = value;
            }
        }

        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnCompileUnit(this);
        }

        public override void AppendChild(Pair child)
        {
            var item = child as Module;
            if (item != null)
            {
                Modules.Add(item);
            }
            else
            {
                base.AppendChild(child);
            }
        }
    }
}
