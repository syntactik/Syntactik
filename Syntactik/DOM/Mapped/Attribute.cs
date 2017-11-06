using System.Collections.Generic;

namespace Syntactik.DOM.Mapped
{
    public class Attribute: DOM.Attribute, IMappedPair, IPairWithInterpolation
    {
        public Interval NameInterval { get; set; }
        public Interval ValueInterval { get; set; }
        public Interval DelimiterInterval { get; set; }
        public ValueType ValueType { get; set; }
        public virtual bool IsValueNode => true;
        public List<object> InterpolationItems { get; set; }
        public int ValueIndent { get; set; }

        public override void AppendChild(Pair child)
        {
            if (Delimiter == DelimiterEnum.EC || Delimiter == DelimiterEnum.ECC)
            {
                if (InterpolationItems == null) InterpolationItems = new List<object>();
                InterpolationItems.Add(child);
                child.InitializeParent(this);
            }
            else
                base.AppendChild(child);
        }
    }
}
