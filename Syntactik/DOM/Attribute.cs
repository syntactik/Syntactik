namespace Syntactik.DOM
{
    public class Attribute : Entity, INsNode
    {
        protected string _nsPrefix;

        // Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnAttribute(this);
        }

        public virtual string NsPrefix
        {
            get { return _nsPrefix; }
            set { _nsPrefix = value; }
        }
    }
}