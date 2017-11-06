using System.Collections.Generic;

namespace Syntactik.DOM.Mapped
{
    public class MappedPair: Pair, IMappedPair 
    {
        public override void Accept(IDomVisitor visitor)
        {
        }

        public static Pair EmptyPair { get; } = new MappedPair {Name = "EmptyPair"};

        public Interval NameInterval { get; set; }
        public Interval ValueInterval { get; set; }
        public Interval DelimiterInterval { get; set; }
        public ValueType ValueType { get; set; }
        public bool IsValueNode { get; set; }
        public bool MissingNameQuote { get; set; }
        public bool MissingValueQuote { get; set; }
        public List<object> InterpolationItems => null;
        public int ValueIndent { get; } = 0;
    }
}
