namespace Syntactik.DOM
{
    public class AliasDefinition : ModuleMember, IContainer
    {
        // Fields
        protected PairCollection<Entity> _entities;

        // Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnAliasDefinition(this);
        }

        public override void AppendChild(Pair child)
        {
            Value = null;
            PairValue = null;

            var entity = child as Entity;
            if (entity != null)
            {
                Entities.Add(entity);
                return;
            }

            var ns = child as NamespaceDefinition;
            if (ns != null)
            {
                NamespaceDefinitions.Add(ns);
                return;
            }

            base.AppendChild(child);
        }

        // Properties
        public virtual PairCollection<Entity> Entities
        {
            get { return _entities ?? (_entities = new PairCollection<Entity>(this)); }
            set
            {
                if (value != _entities)
                {
                    value?.InitializeParent(this);
                    _entities = value;
                }
            }
        }
    }
}
