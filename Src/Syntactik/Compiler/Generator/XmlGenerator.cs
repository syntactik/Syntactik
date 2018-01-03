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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Syntactik.Compiler.Steps;
using Syntactik.DOM;
using Syntactik.DOM.Mapped;
using Alias = Syntactik.DOM.Alias;
using Comment = Syntactik.DOM.Comment;
using Document = Syntactik.DOM.Mapped.Document;
using Element = Syntactik.DOM.Element;
using ValueType = Syntactik.DOM.Mapped.ValueType;

namespace Syntactik.Compiler.Generator
{
    public class XmlGenerator : AliasResolvingVisitor
    {
        protected XmlWriter _xmlTextWriter;
        protected readonly Func<string, Encoding,  XmlWriter> _writerDelegate;
        protected readonly Func<string, XmlReaderSettings, XmlReader> _readerDelegate;
        protected bool _rootElementAdded;
        protected readonly Stack<ChoiceInfo> _choiceStack = new Stack<ChoiceInfo>();
        protected ModuleMember _currentModuleMember;
        private readonly bool _generateComments;

        public List<LexicalInfo> LocationMap { get; set; }

        /// <summary>
        /// This constructor should be used if output depends on the name of the document.
        /// </summary>
        /// <param name="writerDelegate">Delegate will be called for each Document. The name of the document will be sent in the string argument.</param>
        /// <param name="readerDelegate">Delegate should return XmlReader for each generated document.</param>
        /// <param name="context"></param>
        /// <param name="generateComments"></param>
        public XmlGenerator(Func<string, Encoding, XmlWriter> writerDelegate, Func<string, XmlReaderSettings, XmlReader> readerDelegate, CompilerContext context, bool generateComments = false):base(context)
        {
            _writerDelegate = writerDelegate;
            _readerDelegate = readerDelegate;
            _generateComments = generateComments;
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

        public override void OnDocument(DOM.Document document)
        {
            _currentDocument = (Document) document;
            _currentModuleMember = document;
            _choiceStack.Push(_currentDocument.ChoiceInfo);
            Encoding encoding = Encoding.UTF8;
            if (_generateComments)
            {
                encoding = GetEncoding(document);
            }
            //Generate XML file
            using (_xmlTextWriter = _writerDelegate(document.Name, encoding))
            {
                WriteStartDocument(document);
                _rootElementAdded = false;
                LocationMap = new List<LexicalInfo>();
                base.OnDocument(document);
                _xmlTextWriter.WriteEndDocument();

            }

            //Validate XML file
            var validator = new SourceMappedXmlValidator(LocationMap, Context.Parameters.XmlSchemaSet, _readerDelegate);
            validator.ValidationErrorEvent += error => Context.Errors.Add(error);
            
            //var fileName = Path.Combine(_context.Parameters.OutputDirectory, node.Name + ".xml");
            validator.ValidateGeneratedFile(document.Name);

            _currentDocument = null;
            _currentModuleMember = null;
        }

        protected Encoding GetEncoding(DOM.Document document)
        {
            var leadingComments = document.Entities.TakeWhile(e => e is Comment);
            var declaration = leadingComments.FirstOrDefault(c => c.Value.StartsWith("XmlDeclaration:"));
            if (declaration != null)
            {
                var m = new Regex(@"(?:[.]*encoding\s*=\s*"")([^""]*)(?:"")").Match(declaration.Value);
                if (m.Success && m.Groups.Count > 1)
                {
                    try
                    {
                        return Encoding.GetEncoding(m.Groups[1].Value);
                    }
                    catch (Exception)
                    {
                        return Encoding.UTF8;
                    }
                }
            }
            return Encoding.UTF8;

        }

        protected virtual void WriteStartDocument(DOM.Document document)
        {
            _xmlTextWriter.WriteStartDocument();
            if (!_generateComments) return;
            var leadingComments = document.Entities.TakeWhile(e => e is Comment);
            var pInstructions =
                leadingComments.Where(c => c.Value.StartsWith("ProcessingInstruction")).Select(c => c.Value);
            foreach (var instruction in pInstructions)
            {
                var m = new Regex(@"(?:ProcessingInstruction\s*)(.*):\s*(.*)").Match(instruction);
                if (m.Success && m.Groups.Count > 2)
                {
                    _xmlTextWriter.WriteProcessingInstruction(m.Groups[1].Value, m.Groups[2].Value);
                }
            }
        }


        protected bool EnterChoiceContainer(Pair pair, PairCollection<Entity> entities, Pair implementationPair = null)
        {
            if (implementationPair == null) implementationPair = pair;
            if (implementationPair.Delimiter != DelimiterEnum.CC && implementationPair.Delimiter != DelimiterEnum.ECC 
                    || entities == null || entities.Count == 0)
                return false;

            var choice = _choiceStack.Peek();
            var choiceInfo = JsonGenerator.FindChoiceInfo(choice, pair);
            if (choice.ChoiceNode != pair)
            {
                _choiceStack.Push(choiceInfo);
            }
            _choiceStack.Push(choiceInfo.Children[0]);
            if (((Element)choiceInfo.Children[0].ChoiceNode).Entities.Count > 0)
                Visit(((Element)choiceInfo.Children[0].ChoiceNode).Entities);

            _choiceStack.Pop();
            if (choice.ChoiceNode != pair)
                _choiceStack.Pop();
            return true;

        }

        protected override string ResolveChoiceValue(Pair pair, out ValueType valueType)
        {
            var choice = _choiceStack.Peek();
            var choiceInfo = JsonGenerator.FindChoiceInfo(choice, pair);
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

        public override void OnElement(Element element)
        {
            //Getting namespace and prefix
            string prefix, ns;
            NamespaceResolver.GetPrefixAndNs(element, _currentDocument,
                () => ScopeContext.Peek(),
                out prefix, out ns);

            if (string.IsNullOrEmpty(element.NsPrefix)) prefix = null; 
            //Starting Element
            if (!string.IsNullOrEmpty(element.Name))
            {
                if (element.Delimiter != DelimiterEnum.CCC)
                {
                    //not text node
                    _xmlTextWriter.WriteStartElement(prefix, element.Name, ns);
                    AddLocationMapRecord(_currentModuleMember.Module.FileName, (IMappedPair) element);

                    //Write all namespace declarations in the root element
                    if (!_rootElementAdded)
                    {
                        WritePendingNamespaceDeclarations(ns);
                        _rootElementAdded = true;
                    }
                }
            }
            else
            {
                if (element.Parent.Delimiter == DelimiterEnum.CCC && (element.Delimiter == DelimiterEnum.C || element.Delimiter == DelimiterEnum.CC))
                {
                    // This is item of explicit array (:::)
                    WriteExplicitArrayItem(element);
                }
            }

            if (!ResolveValue(element) && !EnterChoiceContainer(element, element.Entities))
                base.OnElement(element);

            //End Element
            if (!string.IsNullOrEmpty(element.Name))
            {
                if (element.Delimiter != DelimiterEnum.CCC) //not text node and not explicit array
                    _xmlTextWriter.WriteEndElement();
            }
            else
            {
                if (element.Parent.Delimiter == DelimiterEnum.CCC && (element.Delimiter == DelimiterEnum.C || element.Delimiter == DelimiterEnum.CC))
                    _xmlTextWriter.WriteEndElement();
            }
        }

        private void WriteExplicitArrayItem(Element element)
        {
            string prefix;
            string ns;
            NamespaceResolver.GetPrefixAndNs((INsNode) element.Parent, _currentDocument,
                () => ScopeContext.Peek(),
                out prefix, out ns);
            if (string.IsNullOrEmpty(element.NsPrefix)) prefix = null;
            _xmlTextWriter.WriteStartElement(prefix, element.Parent.Name, ns);
            AddLocationMapRecord(_currentModuleMember.Module.FileName, (IMappedPair) element);
        }

        protected virtual void AddLocationMapRecord(string fileName, IMappedPair pair)
        {
            LocationMap.Add(new LexicalInfo(fileName, pair.NameInterval.Begin.Line,
                pair.NameInterval.Begin.Column, pair.NameInterval.Begin.Index));
        }

        public override void OnAlias(Alias alias)
        {
            var aliasDef = ((DOM.Mapped.Alias)alias).AliasDefinition;
            var prevCurrentModuleMember = _currentModuleMember;
            _currentModuleMember = aliasDef;
            if (aliasDef.IsValueNode)
            {
                ValueType valueType;
                OnValue(ResolveValueAlias((DOM.Mapped.Alias)alias, out valueType), valueType);
            }
            AliasContext.Push((DOM.Mapped.Alias) alias);
            if (!EnterChoiceContainer((DOM.Mapped.Alias) alias, aliasDef.Entities, aliasDef))
                Visit(aliasDef.Entities.Where(e => !(e is DOM.Attribute)));
            AliasContext.Pop();
            _currentModuleMember = prevCurrentModuleMember;
        }

        public override void OnValue(string value, ValueType type)
        {
            _xmlTextWriter.WriteString(value);
        }

        protected override void ResolveDqsEscape(EscapeMatch escapeMatch, StringBuilder sb)
        {
            char c = ResolveDqsEscapeChar(escapeMatch);
            if (IsLegalXmlChar(c))
            {
                sb.Append(c);
            }
        }

        /// <summary>
        /// Whether a given character is allowed by XML 1.0.
        /// </summary>
        public static bool IsLegalXmlChar(int character)
        {
            return
            (
                 character == 0x9 /* == '\t' == 9   */          ||
                 character == 0xA /* == '\n' == 10  */          ||
                 character == 0xD /* == '\r' == 13  */          ||
                (character >= 0x20 && character <= 0xD7FF) ||
                (character >= 0xE000 && character <= 0xFFFD) ||
                (character >= 0x10000 && character <= 0x10FFFF)
            );
        }

        public override void OnAttribute(DOM.Attribute attribute)
        {
            string prefix = string.Empty, ns = string.Empty;
            if (!string.IsNullOrEmpty(attribute.NsPrefix))
            {
                NamespaceResolver.GetPrefixAndNs(attribute, _currentDocument,
                    () => ScopeContext.Peek(),
                    out prefix, out ns);
            }
            _xmlTextWriter.WriteStartAttribute(prefix, attribute.Name, ns);
            if (!ResolveValue(attribute) )
            {
                EnterChoiceContainer(attribute, new PairCollection<Entity>().AddRange(((DOM.Mapped.Attribute)attribute).InterpolationItems?.OfType<Entity>()));
            }
            _xmlTextWriter.WriteEndAttribute();
            
            AddLocationMapRecord(_currentModuleMember.Module.FileName, (IMappedPair) attribute);
        }

        public override void OnComment(Comment comment)
        {
            if (_generateComments &&
                 !comment.Value.StartsWith("XmlDeclaration:") &&
                 !comment.Value.StartsWith("ProcessingInstruction"))
                    _xmlTextWriter.WriteComment(comment.Value);
        }

        private void WritePendingNamespaceDeclarations(string uri)
        {
            NsInfo nsInfo = NamespaceResolver.GetNsInfo(_currentDocument);
            if (nsInfo == null) return;

            foreach (var ns in nsInfo.Namespaces)
            {
                if (ns.Value == uri) continue;
                _xmlTextWriter.WriteAttributeString("xmlns", ns.Name, null, ns.Value);

            }
        }
    }
}
