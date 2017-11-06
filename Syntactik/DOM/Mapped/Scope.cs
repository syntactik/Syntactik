using System.Collections.Generic;

namespace Syntactik.DOM.Mapped
{
    public class Scope : DOM.Scope, IMappedPair
    {
        public Interval NameInterval { get; set; }
        public Interval ValueInterval { get; set; }
        public Interval DelimiterInterval { get; set; }
        public ValueType ValueType { get; set; }
        public virtual bool IsValueNode => ValueType != ValueType.None && ValueType != ValueType.Object;
        public List<object> InterpolationItems { get; set; }
        public int ValueIndent { get; set; }
    }
}
