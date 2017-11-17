using System.Collections.Generic;

namespace Syntactik.DOM.Mapped
{
    public class Alias: DOM.Alias, IMappedPair, IPairWithInterpolation
    {
        public AliasDefinition AliasDefinition { get; set; }
        public Interval NameInterval { get; set; }
        public Interval ValueInterval { get; set; }
        public Interval DelimiterInterval { get; set; }
        public ValueType ValueType { get; set; }
        public virtual bool IsValueNode { get; set; }
        public List<object> InterpolationItems { get; set; }
        public int ValueIndent { get; set; }
        public override void InitializeParent(Pair parent)
        {
            base.InitializeParent(parent);
            IsValueNode = Parent.Delimiter == DelimiterEnum.EC || Parent.Delimiter == DelimiterEnum.CE;
        }
        public override void AppendChild(Pair child)
        {
            if (Delimiter == DelimiterEnum.EC)
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
