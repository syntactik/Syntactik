namespace Syntactik.DOM
{
    public abstract class ModuleMember : Pair
    {
        private PairCollection<NamespaceDefinition> _namespaces;

        public virtual Module Module => (Parent as Module);

        public virtual PairCollection<NamespaceDefinition> NamespaceDefinitions
        {
            get { return _namespaces ?? (_namespaces = new PairCollection<NamespaceDefinition>(this)); }
            set
            {
                if (value != _namespaces)
                {
                    value?.InitializeParent(this);
                    _namespaces = value;
                }
            }
        }
    }
}
