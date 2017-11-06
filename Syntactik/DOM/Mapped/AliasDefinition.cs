using System;
using System.Collections.Generic;
using System.Linq;

namespace Syntactik.DOM.Mapped
{
    public class AliasDefinition : DOM.AliasDefinition, IMappedPair, IPairWithInterpolation
    {
        private List<Parameter> _parameters;

        public AliasDefinition()
        {
            SyncTime = DateTime.Now.Ticks;
        }

        public Interval NameInterval { get; set; }
        public Interval ValueInterval { get; set; }
        public Interval DelimiterInterval { get; set; }
        public ValueType ValueType { get; set; }
        public virtual bool IsValueNode => ValueType != ValueType.None && ValueType != ValueType.Object;
        public List<object> InterpolationItems { get; set; }
        public int ValueIndent { get; set; }
        public long SyncTime { get; set; } //This field is used in completion.

        public List<Parameter> Parameters
        {
            get { return _parameters ?? (_parameters = new List<Parameter>()); }
            set
            {
                if (_parameters != null && value != _parameters)
                {
                    _parameters = value;
                }
            }
        }
        public bool HasDefaultBlockParameter { get; set; }
        public bool HasDefaultValueParameter { get; set; }
        public bool HasCircularReference { get; set; }

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
