using System;

namespace Syntactik.DOM
{
    [Serializable]
    public class Scope : Entity, INsNode, IContainer
    {
        // Fields
        protected PairCollection<Entity> _entities;
        protected string _nsPrefix;

        // Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnScope(this);
        }

        public override void AppendChild(Pair child)
        {
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
                if (value != _entities)
                {
                    value?.InitializeParent(this);
                    _entities = value;
                }
            }
        }

        public virtual string NsPrefix
        {
            get { return _nsPrefix; }
            set { _nsPrefix = value; }
        }
    }
}
