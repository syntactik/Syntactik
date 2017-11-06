using System;
using System.Linq;

namespace Syntactik.DOM.Mapped
{
    public class Module : DOM.Module
    {
        public enum TargetFormats
        {
            Undefined = 0,
            Xml,
            Json
        }

        private TargetFormats _targetFormat;
        public TargetFormats TargetFormat
        {
            get
            {
                if (_targetFormat != TargetFormats.Undefined) return _targetFormat;

                if (FileName != null && FileName.EndsWith(".s4j")) return _targetFormat = TargetFormats.Json;

                return _targetFormat = TargetFormats.Xml;
            }
            set { _targetFormat = value; }
        }

        public override void AppendChild(Pair child)
        {
            if (child is NamespaceDefinition)
            {
                if (_moduleDocument != null && _moduleDocument.Entities.Any(e => !(e is Comment)) 
                        || _moduleDocument == null && Members.Count > 0)
                    throw new ApplicationException("Namespaces must be defined first");
            }
            base.AppendChild(child);
        }
    }
}
