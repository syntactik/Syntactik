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
using System.Text.RegularExpressions;
using System.Xml;
using Syntactik.DOM;
using Syntactik.DOM.Mapped;
using Syntactik.IO;
using Element = Syntactik.DOM.Mapped.Element;
using Module = Syntactik.DOM.Mapped.Module;
using ValueType = Syntactik.DOM.Mapped.ValueType;

namespace Syntactik.Compiler.Steps
{
    public class PairFactoryForJson : IPairFactory
    {
        private readonly CompilerContext _context;
        private readonly DOM.Module _module;

        public PairFactoryForJson(CompilerContext context, DOM.Module module)
        {
            _context = context;
            _module = module;
        }

        public Pair CreateMappedPair(ITextSource textSource, int nameQuotesType, Interval nameInterval, DelimiterEnum delimiter,
                                Interval delimiterInterval, int valueQuotesType, Interval valueInterval, int valueIndent)
        {
            Pair pair;
            var name = PairFactoryForXml.GetNameText(textSource, nameQuotesType, nameInterval);
            var value = PairFactoryForXml.GetValue(textSource, delimiter, valueQuotesType, valueInterval,
                    valueIndent, _context, (Module) _module);

            if (nameQuotesType > 0)
            {
                if (delimiter == DelimiterEnum.None)
                {
                    value = PairFactoryForXml.GetValue(textSource, delimiter, nameQuotesType, nameInterval,
                    0, _context, (Module)_module);
                    valueQuotesType = nameQuotesType;
                }
                pair = new Element
                {
                    Name = name,
                    NameQuotesType = nameQuotesType,
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };

                SetValueType((Element) pair, delimiter, value.Item1, valueQuotesType);
            } else if (name.StartsWith("@"))
            {
                pair = new DOM.Mapped.Attribute()
                {
                    Name = name.Substring(1),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };

            } else if (name.StartsWith("!$"))
            {
                pair = new DOM.Mapped.AliasDefinition
                {
                    Name = VerifyName(name.Substring(2), nameInterval, _module),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
            } else if (name.StartsWith("!#"))
            {
                pair = new DOM.Mapped.NamespaceDefinition
                {
                    Name = VerifyNsName(name.Substring(2), nameInterval, _module),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
            } else if (name.StartsWith("!%"))
            {
                pair = new DOM.Mapped.Parameter
                {
                    Name = VerifyNsName(name.Substring(2), nameInterval, _module),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
            } else if (name.StartsWith("!"))
            {
                pair = new DOM.Mapped.Document
                {
                    Name = VerifyName(name.Substring(1), nameInterval, _module),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
            } else if (name.StartsWith("$"))
            {
                pair = new DOM.Mapped.Alias()
                {
                    Name = VerifyName(name.Substring(1), nameInterval, _module),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
            } else if (name.StartsWith("%"))
            {
                pair = new DOM.Mapped.Argument()
                {
                    Name = VerifyName(name.Substring(1), nameInterval, _module),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };

                SetValueType((IMappedPair) pair, delimiter, value.Item1, valueQuotesType);
            } else if (name.StartsWith("#"))
            {
                pair = new DOM.Mapped.Scope
                {
                    NsPrefix = VerifyScopeName(name.Substring(1), nameInterval, _module),
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval
                };
                SetValueType((IMappedPair) pair, delimiter, value.Item1, valueQuotesType);
            }
            else
            {
                pair = new Element
                {
                    Name = name,
                    NameQuotesType = nameQuotesType,
                    NameInterval = nameInterval,
                    Delimiter = delimiter,
                    DelimiterInterval = delimiterInterval,
                    Value = value.Item1,
                    ValueQuotesType = valueQuotesType,
                    ValueInterval = valueInterval,
                    InterpolationItems = value.Item2,
                    ValueIndent = valueIndent
                };
                if (delimiter == DelimiterEnum.None)
                {
                    value = PairFactoryForXml.GetValue(textSource, delimiter, nameQuotesType, nameInterval,
                    0, _context, (Module)_module);
                    valueQuotesType = nameQuotesType;
                }
            }
            SetValueType((IMappedPair) pair, delimiter, value.Item1, valueQuotesType);
            return pair;
        }

        private void SetValueType(IMappedPair pair, DelimiterEnum delimiter, string value, int valueQuotesType)
        {
            switch (delimiter)
            {
                case DelimiterEnum.CE:
                    pair.ValueType = ValueType.PairValue;
                    return;
                case DelimiterEnum.EC:
                    pair.ValueType = ValueType.Concatenation;
                    return;
                case DelimiterEnum.ECC:
                    pair.ValueType = ValueType.LiteralChoice;
                    return;
                case DelimiterEnum.C:
                case DelimiterEnum.CC:
                case DelimiterEnum.CCC:
                    pair.ValueType = ValueType.Object;
                    return;
            }
            if (value == null) return;

            if (valueQuotesType == 1)
            {
                pair.ValueType = ValueType.SingleQuotedString;
                return;
            }
            if (valueQuotesType == 2)
            {
                pair.ValueType = ValueType.DoubleQuotedString;
                return;
            }
            if (delimiter == DelimiterEnum.E)
                pair.ValueType = GetJsonValueType(value, ValueType.FreeOpenString); 
            if (delimiter == DelimiterEnum.EE || delimiter == DelimiterEnum.None)
                pair.ValueType = GetJsonValueType(value, ValueType.OpenString);
        }

        private ValueType GetJsonValueType(string value, ValueType defaultType)
        {
            if (value == "true" || value == "false") return ValueType.Boolean;
            if (value == "null") return ValueType.Null;
            if (Regex.Match(value, @"^-?(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+-]?\d+)?$").Success) return ValueType.Number;
            return defaultType;
        }

        private string VerifyName(string name, Interval nameInterval, DOM.Module module)
        {
            try
            {
                XmlConvert.VerifyNCName(name);
            }
            catch (Exception)
            {
                _context.Errors.Add(CompilerErrorFactory.InvalidName(nameInterval, module.FileName));
            }
            return name;
        }
        private string VerifyNsName(string name, Interval nameInterval, DOM.Module module)
        {
            if (Regex.Match(name, @"^[a-zA-Z_][a-zA-Z0-9_\-]*$").Success) return name;

            _context.Errors.Add(CompilerErrorFactory.InvalidNsName(nameInterval, module.FileName));

            return name;
        }
        private string VerifyScopeName(string name, Interval nameInterval, DOM.Module module)
        {
            if (string.IsNullOrEmpty(name)) return name;
            return VerifyNsName(name, nameInterval, module);
        }
        public void AppendChild(Pair parent, Pair child)
        {
            try
            {
                parent.AppendChild(child);
            }
            catch (Exception e)
            {
                _context.Errors.Add(CompilerErrorFactory.CantAppendChild(((IMappedPair)child).NameInterval, _module.FileName, e.Message));
            }
        }

        public void EndPair(Pair pair, Interval endInterval, bool endedByEof)
        {
        }

        public DOM.Comment ProcessComment(ITextSource textSource, int commentType, Interval commentInterval)
        {
            return null;
        }
    }
}