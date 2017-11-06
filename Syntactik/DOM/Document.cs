using System;

namespace Syntactik.DOM
{
    [Serializable]
    public class Document : ModuleMember, IContainer
    {
        // Fields
        protected PairCollection<Entity> _entities;

        public Entity DocumentElement;

        // Properties
        public virtual PairCollection<Entity> Entities
        {
            get { return _entities ?? (_entities = new PairCollection<Entity>(this)); }
            set { throw new NotImplementedException(); }
        }

        // Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnDocument(this);
        }

        public override void AppendChild(Pair child)
        {
            Value = null;
            PairValue = null;

            var entity = child as Entity;
            if (entity != null)
            {
                DocumentElement = entity;
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
    }
}
