using System.Collections.Generic;

namespace Syntactik.DOM.Mapped
{
    public class Parameter: DOM.Parameter, IMappedPair, IPairWithInterpolation
    {
        public Interval NameInterval { get; set; }
        public Interval ValueInterval { get; set; }
        public Interval DelimiterInterval { get; set; }
        public ValueType ValueType { get; set; }
        public virtual bool IsValueNode => ValueType != ValueType.None && ValueType != ValueType.Object;
        public AliasDefinition AliasDefinition { get; set; }
        public List<object> InterpolationItems { get; set; }
        public int ValueIndent { get; set; }

        public override void InitializeParent(Pair parent)
        {
            base.InitializeParent(parent);
            if (Parent.Delimiter == DelimiterEnum.EC || //Parameter is inside of string concatenation
                Parent.PairValue == this && ValueType == ValueType.None) //Parameter is assigned as Pair Value
                ValueType = ValueType.Empty;
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
