#region license
// Copyright � 2017 Maxim O. Trushin (trushin@gmail.com)
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
    /// <summary>
    /// Implementation of <see cref="IPairFactory"/> that creates <see cref="Pair"/> for JSON-modules.
    /// </summary>
    public class PairFactoryForJson : IPairFactory
    {
        private readonly CompilerContext _context;
        private readonly DOM.Module _module;

        /// <summary>
        /// Creates instance of <see cref="PairFactoryForJson"/>.
        /// </summary>
        /// <param name="context"><see cref="CompilerContext"/> used to report errors.</param>
        /// <param name="module">Current module.</param>
        public PairFactoryForJson(CompilerContext context, DOM.Module module)
        {
            _context = context;
            _module = module;
        }

        /// <inheritdoc />
        public Pair CreateMappedPair(ITextSource textSource, int nameQuotesType, Interval nameInterval, AssignmentEnum assignment,
                                Interval assignmentInterval, int valueQuotesType, Interval valueInterval, int valueIndent)
        {
            Pair pair;
            var name = PairFactoryForXml.GetNameText(textSource, nameQuotesType, nameInterval);
            var value = PairFactoryForXml.GetValue(textSource, assignment, valueQuotesType, valueInterval,
                    valueIndent, _context, (Module) _module);

            if (nameQuotesType > 0)
            {
                if (assignment == AssignmentEnum.None)
                {
                    value = PairFactoryForXml.GetValue(textSource, assignment, nameQuotesType, nameInterval,
                                    0, _context, (Module)_module);
                    valueQuotesType = nameQuotesType;
                }
                pair = new Element(
                    name, 
                    nameQuotesType : nameQuotesType,
                    nameInterval : nameInterval,
                    assignment : assignment,
                    assignmentInterval : assignmentInterval,
                    value : value.Item1,
                    valueQuotesType : valueQuotesType,
                    valueInterval : valueInterval,
                    interpolationItems : value.Item2,
                    valueIndent : valueIndent,
                    valueType: GetValueType(assignment, value.Item1, valueQuotesType)
                );
                
            } else if (name.StartsWith("@"))
            {
                pair = new DOM.Mapped.Attribute(
                    name.Substring(1),
                    nameInterval : nameInterval,
                    assignment : assignment,
                    assignmentInterval : assignmentInterval,
                    value : value.Item1,
                    valueQuotesType : valueQuotesType,
                    valueInterval : valueInterval,
                    interpolationItems : value.Item2,
                    valueIndent : valueIndent,
                    valueType: GetValueType(assignment, value.Item1, valueQuotesType)
                );
            } else if (name.StartsWith("!$"))
            {
                pair = new DOM.Mapped.AliasDefinition
                (
                    VerifyName(name.Substring(2), nameInterval, _module),
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval,
                    value: value.Item1,
                    valueQuotesType: valueQuotesType,
                    valueInterval: valueInterval,
                    interpolationItems: value.Item2,
                    valueIndent: valueIndent,
                    valueType: GetValueType(assignment, value.Item1, valueQuotesType)
                );
            } else if (name.StartsWith("!#"))
            {
                pair = new DOM.Mapped.NamespaceDefinition
                (
                    VerifyNsName(name.Substring(2), nameInterval, _module),
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval,
                    value: value.Item1,
                    valueQuotesType: valueQuotesType,
                    valueInterval: valueInterval,
                    interpolationItems: value.Item2,
                    valueIndent: valueIndent,
                    valueType: GetValueType(assignment, value.Item1, valueQuotesType)
                );
            } else if (name.StartsWith("!%"))
            {
                pair = new DOM.Mapped.Parameter
                (
                    VerifyNsName(name.Substring(2), nameInterval, _module),
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval,
                    value: value.Item1,
                    valueQuotesType: valueQuotesType,
                    valueInterval: valueInterval,
                    interpolationItems: value.Item2,
                    valueIndent: valueIndent,
                    valueType: GetValueType(assignment, value.Item1, valueQuotesType)
                );
            } else if (name.StartsWith("!"))
            {
                pair = new DOM.Mapped.Document
                (
                    VerifyName(name.Substring(1), nameInterval, _module),
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval,
                    value: value.Item1,
                    valueQuotesType: valueQuotesType,
                    valueInterval: valueInterval,
                    interpolationItems: value.Item2,
                    valueIndent: valueIndent,
                    valueType: GetValueType(assignment, value.Item1, valueQuotesType)
                );
            } else if (name.StartsWith("$"))
            {
                pair = new DOM.Mapped.Alias
                (
                    VerifyName(name.Substring(1), nameInterval, _module),
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval,
                    value: value.Item1,
                    valueQuotesType: valueQuotesType,
                    valueInterval: valueInterval,
                    interpolationItems: value.Item2,
                    valueIndent: valueIndent,
                    valueType: GetValueType(assignment, value.Item1, valueQuotesType)
                );
            } else if (name.StartsWith("%"))
            {
                pair = new DOM.Mapped.Argument
                (
                    VerifyName(name.Substring(1), nameInterval, _module),
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval,
                    value: value.Item1,
                    valueQuotesType: valueQuotesType,
                    valueInterval: valueInterval,
                    interpolationItems: value.Item2,
                    valueIndent: valueIndent,
                    valueType: GetValueType(assignment, value.Item1, valueQuotesType)
                );
                
            }
            //else if (name.StartsWith("#"))
            //{
            //    pair = new DOM.Mapped.Scope
            //    {
            //        NsPrefix = VerifyScopeName(name.Substring(1), nameInterval, _module),
            //        NameInterval = nameInterval,
            //        Assignment = assignment,
            //        AssignmentInterval = assignmentInterval
            //    };
            //    SetValueType((IMappedPair) pair, assignment, value.Item1, valueQuotesType);
            //}
            else
            {
                pair = new Element
                (
                    name,
                    nameInterval: nameInterval,
                    assignment: assignment,
                    assignmentInterval: assignmentInterval,
                    value: value.Item1,
                    valueQuotesType: valueQuotesType,
                    valueInterval: valueInterval,
                    interpolationItems: value.Item2,
                    valueIndent: valueIndent,
                    valueType: GetValueType(assignment, 
                        (assignment == AssignmentEnum.None? PairFactoryForXml.GetValue(textSource, assignment, nameQuotesType, nameInterval,
                            0, _context, (Module)_module).Item1:value.Item1),
                        assignment == AssignmentEnum.None ? nameQuotesType: valueQuotesType)
                );
            }
            
            return pair;
        }

        private ValueType GetValueType(AssignmentEnum assignment, string value, int valueQuotesType)
        {
            switch (assignment)
            {
                case AssignmentEnum.CE:
                    return ValueType.PairValue;
                case AssignmentEnum.EC:
                    return ValueType.Concatenation;
                case AssignmentEnum.ECC:
                    return ValueType.LiteralChoice;
                case AssignmentEnum.C:
                case AssignmentEnum.CC:
                case AssignmentEnum.CCC:
                    return ValueType.None;
            }
            if (value == null) return ValueType.None;

            if (valueQuotesType == '\'')
            {
                return ValueType.SingleQuotedString;
            }
            if (valueQuotesType == '"')
            {
                return ValueType.DoubleQuotedString;
            }
            if (assignment == AssignmentEnum.E)
                return GetJsonValueType(value, ValueType.FreeOpenString); 
            if (assignment == AssignmentEnum.EE || assignment == AssignmentEnum.None)
                return GetJsonValueType(value, ValueType.OpenString);
            return ValueType.None;
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

        /// <inheritdoc />
        public void AppendChild(Pair parent, Pair child)
        {
            try
            {
                var mp = (IMappedPair) parent;
                if (child is DOM.Argument && parent is Element && parent.Assignment == AssignmentEnum.None 
                        && mp.NameInterval == null && mp.BlockType == BlockType.JsonObject)
                {
                    child.InitializeParent(parent);
                    parent.Parent.AppendChild(child); //Argument in JSON block
                }
                else
                    parent.AppendChild(child);
            }
            catch (Exception e)
            {
                _context.Errors.Add(CompilerErrorFactory.CantAppendChild(((IMappedPair)child).NameInterval, _module.FileName, e.Message));
            }
        }

        /// <inheritdoc />
        public void EndPair(Pair pair, Interval endInterval, bool endedByEof)
        {
        }

        /// <inheritdoc />
        public DOM.Comment ProcessComment(ITextSource textSource, int commentType, Interval commentInterval)
        {
            return null;
        }

        /// <inheritdoc />
        public Pair ProcessBrackets(Pair pair, int bracket, Interval interval)
        {
            if (bracket == '{' || bracket == '[')
            {
                if (!(pair is IContainer) && !(pair is Module)) {
                    _context.Errors.Add(CompilerErrorFactory.InvalidInlineJsonDeclaration(interval, _module.FileName));
                    return pair;
                }
                var newPair = new Element(assignmentInterval: interval) { BlockType = bracket == '{' ? BlockType.JsonObject : BlockType.JsonArray};
                AppendChild(pair, newPair);
                return newPair;
            }
            return pair;
        }
    }

    static class PairFactoryHelper
    {
        public static bool IsJsonBlock(this BlockType blocktype)
        {
            return blocktype == BlockType.JsonArray || blocktype == BlockType.JsonObject;
        }
    }
}