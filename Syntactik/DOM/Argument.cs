namespace Syntactik.DOM
{
    public class Argument : Entity, IContainer
    {

        // Fields
        protected PairCollection<Entity> _entities;

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

        // Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnArgument(this);
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



    }
}
