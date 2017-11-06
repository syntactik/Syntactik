namespace Syntactik.DOM
{
    public class Alias : Entity, IContainer
    {
        // Fields
        protected PairCollection<Argument> _arguments;
        protected PairCollection<Entity> _entities;
        //Properties
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

        // Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnAlias(this);
        }

        public override void AppendChild(Pair child)
        {
            Value = null;
            PairValue = null;
            var item = child as Argument;
            if (item != null)
            {
                Arguments.Add(item);
            }
            else
            {
                Entities.Add((Entity)child);
            }
        }

        // Properties
        public virtual PairCollection<Argument> Arguments
        {
            get
            {
                return _arguments ?? (_arguments = new PairCollection<Argument>(this));
            }
            set
            {
                if (value != _arguments)
                {
                    value?.InitializeParent(this);
                    _arguments = value;
                }
            }
        }

    }
}
