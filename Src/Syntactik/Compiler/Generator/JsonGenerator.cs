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
using System.Collections.Generic;
using System.Linq;
using Syntactik.DOM;
using Newtonsoft.Json;
using Syntactik.Compiler.Steps;
using Syntactik.DOM.Mapped;
using Alias = Syntactik.DOM.Alias;
using Attribute = Syntactik.DOM.Attribute;
using Document = Syntactik.DOM.Document;
using Element = Syntactik.DOM.Element;
using ValueType = Syntactik.DOM.Mapped.ValueType;

namespace Syntactik.Compiler.Generator
{
    public class JsonGenerator : AliasResolvingVisitor
    {
        public enum BlockState
        {
            Object, //Json block is object
            Array   //Json block is array
        }

        protected readonly Func<string, JsonWriter> _writerDelegate;

        protected JsonWriter _jsonWriter;
        protected bool _blockStart;
        protected Stack<BlockState> _blockState;
        protected readonly Stack<ChoiceInfo> _choiceStack = new Stack<ChoiceInfo>();


        /// <summary>
        /// This constructor should be used if output depends on the name of the document.
        /// </summary>
        /// <param name="writerDelegate">Delegate will be called for the each Document. The name of the document will be sent in the string argument.</param>
        /// <param name="context"></param>
        public JsonGenerator(Func<string, JsonWriter> writerDelegate, CompilerContext context):base(context)
        {
            _writerDelegate = writerDelegate;
            Context = context;
        }

        public override void OnModule(DOM.Module module)
        {
            Visit(module.NamespaceDefinitions);
            Visit(module.Members.Where(
                    m => (m is DOM.AliasDefinition) || 
                    ((Document)m).Module.ModuleDocument != m || 
                    ((IContainer)m).Entities.Any(e => !(e is DOM.Comment))) //Skipping module documents having only comments in body
                );
        }

        public override void OnDocument(Document document)
        {
            _currentDocument = (DOM.Mapped.Document) document;
            _choiceStack.Push(_currentDocument.ChoiceInfo);

            using (_jsonWriter = _writerDelegate(document.Name))
            {
                _blockStart = true;
                _blockState = new Stack<BlockState>();
                base.OnDocument(document);

                if (_blockState.Count > 0)
                {
                    if (_blockState.Pop() == BlockState.Array)
                        _jsonWriter.WriteEndArray();
                    else
                        _jsonWriter.WriteEndObject();
                }
                //Empty document. Writing an empty object as a value.
                else
                {
                    if (document.Delimiter == DelimiterEnum.CC)
                    {
                        _jsonWriter.WriteStartArray();
                        _jsonWriter.WriteEndArray();
                    }
                    else if (document.Delimiter == DelimiterEnum.E || document.Delimiter == DelimiterEnum.EE || document.Delimiter == DelimiterEnum.CE)
                    {
                        ResolveValue(document);
                    }
                    else
                    {
                        _jsonWriter.WriteStartObject();
                        _jsonWriter.WriteEndObject();
                    }
                }
            }
            _choiceStack.Pop();
            _currentDocument = null;
        }

        protected bool EnterChoiceContainer(DOM.Mapped.Alias alias, PairCollection<Entity> entities)
        {
            if (alias.AliasDefinition.Delimiter != DelimiterEnum.CC && alias.AliasDefinition.Delimiter != DelimiterEnum.ECC 
                    || entities == null || entities.Count == 0)
                return false;

            var choice = _choiceStack.Peek();
            var choiceInfo = FindChoiceInfo(choice, alias);
            if (choice.ChoiceNode != alias)
            {
                _choiceStack.Push(choiceInfo);
            }
            _choiceStack.Push(choiceInfo.Children[0]);
            if (((Element) choiceInfo.Children[0].ChoiceNode).Entities.Count > 0)
                Visit(((Element) choiceInfo.Children[0].ChoiceNode).Entities);
            else
            {
                //Empty choice generates empty object
                _jsonWriter.WriteStartObject();
                _blockState.Push(BlockState.Object);
            }

            _choiceStack.Pop();
            if (choice.ChoiceNode != alias)
                _choiceStack.Pop();
            return true;

        }

        protected override string ResolveChoiceValue(Pair pair, out ValueType valueType)
        {
            var choice = _choiceStack.Peek();
            var choiceInfo = FindChoiceInfo(choice, pair);
            if (choice.ChoiceNode != pair)
            {
                _choiceStack.Push(choiceInfo);
            }
            string result = string.Empty;
            valueType = ValueType.OpenString;
            if (choice.Children != null)
            {
                _choiceStack.Push(choiceInfo.Children[0]);
                result = ResolveNodeValue((IMappedPair) choiceInfo.Children[0].ChoiceNode, out valueType);
                _choiceStack.Pop();
            }
            if (choice.ChoiceNode != pair)
                _choiceStack.Pop();
            return result;
        }

        internal static ChoiceInfo FindChoiceInfo(ChoiceInfo choice, Pair pair)
        {
            if (choice.ChoiceNode == pair) return choice;
            if (choice.Children != null)
                foreach (var child in choice.Children)
                {
                    if (child.ChoiceNode == pair) return child;
                }
            return null;
        }

        public override void OnValue(string value, DOM.Mapped.ValueType type)
        {
            if (type == ValueType.Null)
            {
                _jsonWriter.WriteNull();
                return;
            }

            bool boolValue;
            if (type == ValueType.Boolean && bool.TryParse(value, out boolValue))
            {
                _jsonWriter.WriteValue(boolValue);
            }
            else
            {
                decimal numberValue;
                if (type == ValueType.Number && decimal.TryParse(value, out numberValue))
                {
                    if (numberValue % 1 == 0)
                        _jsonWriter.WriteValue((long) numberValue);
                    else
                        _jsonWriter.WriteValue(numberValue);
                }
                    
                else
                    _jsonWriter.WriteValue(value);
            }
        }

        public override void OnAttribute(Attribute pair)
        {
            CheckBlockStart(pair);

            _jsonWriter.WritePropertyName((pair.NsPrefix != null ? pair.NsPrefix + "." : "") + pair.Name);
            ResolveValue(pair);
        }

        public override void OnElement(Element element)
        {
            CheckBlockStart(element);

            if (!string.IsNullOrEmpty(element.Name)&& element.Delimiter != DelimiterEnum.None)
                _jsonWriter.WritePropertyName((element.NsPrefix != null ? element.NsPrefix + "." : "") + element.Name);

            if (ResolveValue(element)) return; //Block has value therefore it has no block.

            //Working with node's block
            _blockStart = true;
            var prevBlockStateCount = _blockState.Count;
            base.OnElement(element);

            _blockStart = false;

            if (_blockState.Count > prevBlockStateCount)
            {
                if (_blockState.Pop() == BlockState.Array)
                    _jsonWriter.WriteEndArray();
                else
                    _jsonWriter.WriteEndObject();
                return;
            }

            //Element hase nor block no value. Writing an empty object as a value.
            if (!string.IsNullOrEmpty(element.Name) || ((DOM.Mapped.Element)element).ValueType == ValueType.Object)
            {
                if (element.Delimiter == DelimiterEnum.CC)
                {
                    _jsonWriter.WriteStartArray();
                    _jsonWriter.WriteEndArray();
                }
                else
                {
                    _jsonWriter.WriteStartObject();
                    _jsonWriter.WriteEndObject();
                }
            }
        }

        public override void OnAlias(Alias alias)
        {
            var aliasDef = ((DOM.Mapped.Alias)alias).AliasDefinition;
            if (aliasDef.IsValueNode)
            {
                CheckBlockStartForAlias();
                ValueType valueType;
                OnValue(ResolveValueAlias((DOM.Mapped.Alias)alias, out valueType), valueType);
            }


            AliasContext.Push((DOM.Mapped.Alias) alias);
            if (!EnterChoiceContainer((DOM.Mapped.Alias) alias, aliasDef.Entities))
                Visit(aliasDef.Entities.Where(e => !(e is Attribute)));
            AliasContext.Pop();
        }

        /// <summary>
        /// Starts array if the value alias is first item in the array
        /// </summary>
        protected virtual void CheckBlockStartForAlias()
        {
            if (!_blockStart) return;
            _jsonWriter.WriteStartArray(); //start array
            _blockState.Push(BlockState.Array);
            _blockStart = false;
        }

        protected virtual void CheckBlockStart(Pair node)
        {
            if (!_blockStart) return;

            //This element is the first element of the block. It decides if the block is array or object
            if (string.IsNullOrEmpty(node.Name) || node.Delimiter == DelimiterEnum.None)
            {
                _jsonWriter.WriteStartArray(); //start array
                _blockState.Push(BlockState.Array);
            }
            else
            {
                _jsonWriter.WriteStartObject(); //start array
                _blockState.Push(BlockState.Object);
            }
            _blockStart = false;
        }
    }
}
