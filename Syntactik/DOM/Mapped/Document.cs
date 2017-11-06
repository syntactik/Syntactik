using System;
using System.Collections.Generic;
using System.Linq;

namespace Syntactik.DOM.Mapped
{
    public class Document : DOM.Document, IMappedPair, IPairWithInterpolation
    {
        public Interval NameInterval { get; set; }
        public Interval ValueInterval { get; set; }
        public Interval DelimiterInterval { get; set; }
        public ValueType ValueType { get; set; }
        public virtual bool IsValueNode => ValueType != ValueType.None && ValueType != ValueType.Object;
        public List<object> InterpolationItems { get; set; }
        public int ValueIndent { get; set; }
        public ChoiceInfo ChoiceInfo { get; set; }


        public Document()
        {
            ChoiceInfo = new ChoiceInfo(null, this);
        }
        public override void AppendChild(Pair child)
        {
            if (child is NamespaceDefinition)
            {
                if (Entities.Any(e => !(e is Comment))) throw new ApplicationException("Namespaces must be defined first");
            }
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
