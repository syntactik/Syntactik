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
    /// <summary>
    /// Generates JSON from Syntactik DOM.
    /// </summary>
    public class JsonGenerator : AliasResolvingVisitor
    {
        /// <summary>
        /// Enumerates possible state of block.
        /// </summary>
        public enum BlockStateEnum
        {
            /// <summary>
            /// Block is an object.
            /// </summary>
            Object,
            /// <summary>
            /// Block is an array.
            /// </summary>
            Array,
            /// <summary>
            /// Block represents a value
            /// </summary>
            Value
        }
        /// <summary>
        /// Delegate is called to create JsonWriter for each <see cref="Document"/>.
        /// </summary>
        protected readonly Func<string, JsonWriter> WriterDelegate;
        /// <summary>
        /// JsonWriter is used to generate JSON text.
        /// </summary>
        protected JsonWriter JsonWriter;
        /// <summary>
        /// If true then the current block state (object/array) is not defined. 
        /// </summary>
        protected bool BlockIsStarting;
        /// <summary>
        /// Stack storing states of blocks.
        /// </summary>
        protected Stack<BlockStateEnum> BlockState;
        /// <summary>
        /// Stack of <see cref="ChoiceInfo"/>.
        /// </summary>
        protected readonly Stack<ChoiceInfo> ChoiceStack = new Stack<ChoiceInfo>();


        /// <summary>
        /// This constructor should be used if output depends on the name of the document.
        /// </summary>
        /// <param name="writerDelegate">Delegate will be called for the each Document. The name of the document will be sent in the string argument.</param>
        /// <param name="context">Compilation context.</param>
        public JsonGenerator(Func<string, JsonWriter> writerDelegate, CompilerContext context):base(context)
        {
            WriterDelegate = writerDelegate;
        }

        /// <inheritdoc />
        public override void Visit(DOM.Module module)
        {
            Visit(module.NamespaceDefinitions);
            Visit(module.Members.Where(
                    m => (m is DOM.AliasDefinition) || 
                    ((Document)m).Module.ModuleDocument != m || 
                    ((IContainer)m).Entities.Any(e => !(e is DOM.Comment))) //Skipping module documents having only comments in body
                );
        }

        /// <inheritdoc />
        public override void Visit(Document document)
        {
            CurrentDocument = (DOM.Mapped.Document) document;
            ChoiceStack.Push(CurrentDocument.ChoiceInfo);

            using (JsonWriter = WriterDelegate(document.Name))
            {
                BlockIsStarting = true;
                BlockState = new Stack<BlockStateEnum>();
                base.Visit(document);

                if (BlockState.Count > 0)
                {
                    var blockState = BlockState.Pop();
                    if (blockState == BlockStateEnum.Array)
                        JsonWriter.WriteEndArray();
                    else if (blockState == BlockStateEnum.Object)
                        JsonWriter.WriteEndObject();
                }
                //Empty document. Writing an empty object as a value.
                else
                {
                    if (document.Assignment == AssignmentEnum.CC)
                    {
                        JsonWriter.WriteStartArray();
                        JsonWriter.WriteEndArray();
                    }
                    else if (document.Assignment == AssignmentEnum.E || document.Assignment == AssignmentEnum.EE || document.Assignment == AssignmentEnum.CE)
                    {
                        ResolveValue(document);
                    }
                    else
                    {
                        JsonWriter.WriteStartObject();
                        JsonWriter.WriteEndObject();
                    }
                }
            }
            ChoiceStack.Pop();
            CurrentDocument = null;
        }

        /// <summary>
        /// Tries to resolve a pair as a choice.
        /// </summary>
        /// <param name="pair">Pair being resolved.</param>
        /// <param name="entities">Entities of the pair.</param>
        /// <param name="implementationPair">If pair is an <see cref="Alias"/> then this parameter should get be an <see cref="DOM.AliasDefinition"/>.</param>
        /// <returns>True if pair is choice.</returns>
        protected bool EnterChoiceContainer(Pair pair, PairCollection<Entity> entities, Pair implementationPair = null)
        {
            if (implementationPair == null) implementationPair = pair;
            if (implementationPair.Assignment != AssignmentEnum.CC && implementationPair.Assignment != AssignmentEnum.ECC 
                    || entities == null || entities.Count == 0)
                return false;

            var choice = ChoiceStack.Peek();
            var choiceInfo = FindChoiceInfo(choice, pair);
            if (choice.ChoiceNode != pair)
            {
                ChoiceStack.Push(choiceInfo);
            }
            ChoiceStack.Push(choiceInfo.Children[0]);
            if (((Element) choiceInfo.Children[0].ChoiceNode).Entities.Count > 0)
                Visit(((Element) choiceInfo.Children[0].ChoiceNode).Entities);
            else
            {
                //Empty choice generates empty object
                JsonWriter.WriteStartObject();
                BlockState.Push(BlockStateEnum.Object);
            }

            ChoiceStack.Pop();
            if (choice.ChoiceNode != pair)
                ChoiceStack.Pop();
            return true;

        }

        /// <inheritdoc />
        protected override string ResolveChoiceValue(Pair pair, out ValueType valueType)
        {
            var choice = ChoiceStack.Peek();
            var choiceInfo = FindChoiceInfo(choice, pair);
            if (choice.ChoiceNode != pair)
            {
                ChoiceStack.Push(choiceInfo);
            }
            string result = string.Empty;
            valueType = ValueType.OpenString;
            if (choice.Children != null)
            {
                ChoiceStack.Push(choiceInfo.Children[0]);
                result = ResolveValue((IMappedPair) choiceInfo.Children[0].ChoiceNode, out valueType);
                ChoiceStack.Pop();
            }
            if (choice.ChoiceNode != pair)
                ChoiceStack.Pop();
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

        /// <inheritdoc />
        public override void OnValue(string value, ValueType type)
        {
            if (type == ValueType.Null)
            {
                JsonWriter.WriteNull();
                return;
            }

            if (type == ValueType.Boolean && bool.TryParse(value, out var boolValue))
            {
                JsonWriter.WriteValue(boolValue);
            }
            else
            {
                if (type == ValueType.Number && decimal.TryParse(value, out var numberValue))
                {
                    if (numberValue % 1 == 0)
                        JsonWriter.WriteValue((long) numberValue);
                    else
                        JsonWriter.WriteValue(numberValue);
                }
                    
                else
                    JsonWriter.WriteValue(value);
            }
        }

        /// <inheritdoc />
        public override void Visit(Attribute attribute)
        {
            CheckBlockStart(attribute);

            JsonWriter.WritePropertyName((attribute.NsPrefix != null ? attribute.NsPrefix + "." : "") + attribute.Name);
            ResolveValue(attribute);
        }

        /// <inheritdoc />
        public override void Visit(Element element)
        {
            CheckBlockStart(element);

            if (!string.IsNullOrEmpty(element.Name)&& element.Assignment != AssignmentEnum.None)
                JsonWriter.WritePropertyName((element.NsPrefix != null ? element.NsPrefix + "." : "") + element.Name);

            if (ResolveValue(element)) return; //Element has value therefore it has no block.

            //Working with node's block
            
            var prevBlockStateCount = BlockState.Count;
            var mp = (IMappedPair)element;
            if (BlockState.Peek() == BlockStateEnum.Array && mp.BlockType == BlockType.JsonObject)
            {
                BlockIsStarting = false;
                JsonWriter.WriteStartObject(); //start array
                BlockState.Push(BlockStateEnum.Object);
            }
            else if (!string.IsNullOrEmpty(element.Name) || element.Assignment != AssignmentEnum.None)
                BlockIsStarting = true;

            base.Visit(element);

            BlockIsStarting = false;

            if (BlockState.Count > prevBlockStateCount)
            {
                var blockState = BlockState.Pop();
                if (blockState == BlockStateEnum.Array)
                    JsonWriter.WriteEndArray();
                else if (blockState == BlockStateEnum.Object)
                    JsonWriter.WriteEndObject();
                return;
            }

            //Element has no block and no value. Writing an empty object as a value.
            if (!string.IsNullOrEmpty(element.Name) || ((DOM.Mapped.Element)element).Assignment.IsObjectAssignment())
            {
                if (element.Assignment == AssignmentEnum.CC || ((IMappedPair)element).BlockType == BlockType.JsonArray)
                {
                    JsonWriter.WriteStartArray();
                    JsonWriter.WriteEndArray();
                }
                else
                {
                    JsonWriter.WriteStartObject();
                    JsonWriter.WriteEndObject();
                }
            }
        }

        /// <inheritdoc />
        public override void Visit(Alias alias)
        {
            var aliasDef = ((DOM.Mapped.Alias)alias).AliasDefinition;
            if (aliasDef.IsValueNode)
            {
                CheckBlockStartForValueAlias();
                OnValue(ResolveValueAlias((DOM.Mapped.Alias)alias, out var valueType), valueType);
            }

            AliasContext.Push(new AliasContextInfo((DOM.Mapped.Alias) alias, CurrentModuleMember));
            if (!EnterChoiceContainer((DOM.Mapped.Alias) alias, aliasDef.Entities, aliasDef))
            {
                if (BlockState.Count > 0 && BlockState.Peek() == BlockStateEnum.Array && aliasDef.BlockType == BlockType.JsonObject)
                {
                    if (BlockIsStarting)
                    {
                        JsonWriter.WriteStartArray();
                    }
                    BlockState.Push(BlockStateEnum.Object);
                    JsonWriter.WriteStartObject();
                    Visit(aliasDef.Entities.Where(e => !(e is Attribute)));
                    JsonWriter.WriteEndObject();
                    BlockState.Pop();
                }
                else
                    Visit(aliasDef.Entities.Where(e => !(e is Attribute)));
            }
            AliasContext.Pop();
        }

        /// <summary>
        /// Starts array if the value alias is first item in the array
        /// </summary>
        protected virtual void CheckBlockStartForValueAlias()
        {
            if (!BlockIsStarting) return;
            JsonWriter.WriteStartArray(); //start array
            BlockState.Push(BlockStateEnum.Array);
            BlockIsStarting = false;
        }

        private void CheckBlockStart(Pair node)
        {
            if (!BlockIsStarting) return;

            if (!string.IsNullOrEmpty(node.Name) && (node.Parent as IMappedPair)?.BlockType == BlockType.Default && (node.Parent?.Parent as IMappedPair)?.BlockType == BlockType.JsonObject)
            {
                BlockState.Push(BlockStateEnum.Value);
            }
            //Processing BlockTypeElement
            else if (string.IsNullOrEmpty(node.Name) && node.Assignment == AssignmentEnum.None)
            {
                if (((IMappedPair) node).BlockType == BlockType.JsonArray)
                {
                    JsonWriter.WriteStartArray(); //start array
                    BlockState.Push(BlockStateEnum.Array);
                }
                else
                {
                    JsonWriter.WriteStartObject(); //start object
                    BlockState.Push(BlockStateEnum.Object);
                }
            }
            //This element is the first element of the block. It decides if the block is array or object
            else if (string.IsNullOrEmpty(node.Name) || node.Assignment == AssignmentEnum.None)
            {
                JsonWriter.WriteStartArray(); //start array
                BlockState.Push(BlockStateEnum.Array);
            }
            else
            {
                JsonWriter.WriteStartObject(); //start object
                BlockState.Push(BlockStateEnum.Object);
            }
            BlockIsStarting = false;
        }
    }
}
