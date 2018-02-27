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
    /// <summary>
    /// Represent a <see cref="Module"/> mapped to the source code.
    /// </summary>
    public class Module : DOM.Module, IMappedPair
    {
        /// <inheritdoc />
        public Interval NameInterval => Interval.Empty;

        /// <inheritdoc />
        public int NameQuotesType => 0;

        /// <inheritdoc />
        public Interval ValueInterval => Interval.Empty;

        /// <inheritdoc />
        public int ValueQuotesType => 0;

        /// <inheritdoc />
        public Interval AssignmentInterval => Interval.Empty;

        /// <inheritdoc />
        public ValueType ValueType => ValueType.None;

        /// <inheritdoc />
        public BlockType BlockType { get; set; }

        /// <inheritdoc />
        public virtual bool IsValueNode => ValueType != ValueType.None && ValueType != ValueType.Object;

        /// <inheritdoc />
        public int ValueIndent => 0;

        /// <summary>
        /// Target format of the module.
        /// </summary>
        public enum TargetFormats
        {
            /// <summary>
            /// Format is not explicitly defined.
            /// </summary>
            Undefined = 0,
            /// <summary>
            /// Format is defined as XML.
            /// </summary>
            Xml,
            /// <summary>
            /// Format is defined as JSON.
            /// </summary>
            Json
        }

        private TargetFormats _targetFormat;

        /// <summary>
        /// Create an instance of <see cref="DOM.Module"/>
        /// </summary>
        /// <param name="name">Module name.</param>
        /// <param name="fileName">Name of file associated with the module.</param>
        public Module(string name, string fileName = null):base(name, fileName)
        {
        }

        /// <summary>
        /// Target format of the module. XML or JSON, for ex.
        /// </summary>
        public TargetFormats TargetFormat
        {
            get
            {
                if (_targetFormat != TargetFormats.Undefined) return _targetFormat;

                if (FileName != null && FileName.EndsWith(".s4j")) return _targetFormat = TargetFormats.Json;

                return _targetFormat = TargetFormats.Xml;
            }
            set => _targetFormat = value;
        }

        /// <inheritdoc />
        public override void AppendChild(Pair child)
        {
            if (child is NamespaceDefinition)
            {
                if (ModuleDocument != null && ModuleDocument.Entities.Any(e => !(e is Comment)) 
                        || ModuleDocument == null && Members.Count > 0)
                    throw new ApplicationException("Namespaces must be defined first");
            }
            base.AppendChild(child);
        }

        /// <inheritdoc />
        protected override void CreateModuleDocument()
        {
            base.CreateModuleDocument();
            ((Document) ModuleDocument).BlockType = BlockType;
        }
    }
}
