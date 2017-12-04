#region license
// Copyright © 2017 Maxim O. Trushin (trushin@gmail.com)
//
// This file is part of Syntactik.
// Syntactik is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Syntactik is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Syntactik.  If not, see <http://www.gnu.org/licenses/>.
#endregion
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
