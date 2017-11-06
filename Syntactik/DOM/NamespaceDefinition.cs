namespace Syntactik.DOM
{
    public class NamespaceDefinition : Pair
    {
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnNamespaceDefinition(this);
        }
    }
}
