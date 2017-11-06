namespace Syntactik.DOM
{
    public class Comment: Entity
    {
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnComment(this);
        }
    }
}
