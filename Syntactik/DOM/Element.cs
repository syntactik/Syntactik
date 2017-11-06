namespace Syntactik.DOM
{
    public class Element : Entity, INsNode, IContainer
    {
        // Fields
        protected PairCollection<Entity> _entities;
        protected string _nsPrefix;

        // Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnElement(this);
        }

        public override void AppendChild(Pair child)
        {
            Value = null;
            PairValue = null;

            var item = child as Entity;
            if (item != null)
            {
                Entities.Add(item);
            }
            else
            {
                base.AppendChild(child);
            }
        }

        // Properties
        public virtual PairCollection<Entity> Entities
        {
            get { return _entities ?? (_entities = new PairCollection<Entity>(this)); }
            set
            {
                if (value == _entities) return;

                value?.InitializeParent(this);
                _entities = value;
            }
        }

        public virtual string NsPrefix
        {
            get { return _nsPrefix; }
            set { _nsPrefix = value; }
        }
    }
}
