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
    /// <summary>
    /// Generates XML from Syntactik DOM.
    /// </summary>
    public class XmlGenerator : AliasResolvingVisitor
    {
        /// <summary>
        /// <see cref="XmlWriter"/> is used to generate XML text.
        /// </summary>
        protected XmlWriter XmlTextWriter;
        
        /// <summary>
        /// Delegate is called to create <see cref="XmlWriter"/> for each <see cref="Document"/>.
        /// </summary>
        protected readonly Func<string, Encoding,  XmlWriter> WriterDelegate;
        
        /// <summary>
        /// Delegate is called to create <see cref="XmlReader"/> to validate each generated XML document.
        /// </summary>
        protected readonly Func<string, XmlReaderSettings, XmlReader> _readerDelegate;
        
        /// <summary>
        /// True if document element is added. Used to identify document element in the visitor.
        /// </summary>
        protected bool DocumentElementAdded;
        /// <summary>
        /// Stack of <see cref="ChoiceInfo"/>.
        /// </summary>
        protected readonly Stack<ChoiceInfo> ChoiceStack = new Stack<ChoiceInfo>();

        /// <summary>
        /// Current <see cref="ModuleMember"/> (<see cref="Document"/> or <see cref="DOM.AliasDefinition"/>).
        /// </summary>
        protected ModuleMember CurrentModuleMember;

        private readonly bool _generateComments;

        /// <summary>
        /// <see cref="LocationMap"/> is used to map XML validation errors to Syntactik code.
        /// </summary>
        public List<LexicalInfo> LocationMap { get; protected set; }

        /// <summary>
        /// This constructor should be used if output depends on the name of the document.
        /// </summary>
        /// <param name="writerDelegate">Delegate will be called for each Document. The name of the document will be sent in the string argument.</param>
        /// <param name="readerDelegate">Delegate should return XmlReader for each generated document.</param>
        /// <param name="context">Compilation context</param>
        /// <param name="generateComments">if true then comments will be generated.</param>
        public XmlGenerator(Func<string, Encoding, XmlWriter> writerDelegate, Func<string, XmlReaderSettings, XmlReader> readerDelegate, CompilerContext context, bool generateComments = false):base(context)
        {
            WriterDelegate = writerDelegate;
            _readerDelegate = readerDelegate;
            _generateComments = generateComments;
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
        public override void Visit(DOM.Document document)
        {
            CurrentDocument = (Document) document;
            CurrentModuleMember = document;
            ChoiceStack.Push(CurrentDocument.ChoiceInfo);
            Encoding encoding = Encoding.UTF8;
            if (_generateComments)
            {
                encoding = GetEncoding(document);
            }
            //Generate XML file
            using (XmlTextWriter = WriterDelegate(document.Name, encoding))
            {
                WriteStartDocument(document);
                DocumentElementAdded = false;
                LocationMap = new List<LexicalInfo>();
                base.Visit(document);
                XmlTextWriter.WriteEndDocument();

            }

            //Validate XML file
            var validator = new SourceMappedXmlValidator(LocationMap, Context.Parameters.XmlSchemaSet, _readerDelegate);
            validator.ValidationErrorEvent += error => Context.Errors.Add(error);
            
            //var fileName = Path.Combine(_context.Parameters.OutputDirectory, node.Name + ".xml");
            validator.ValidateGeneratedFile(document.Name);

            CurrentDocument = null;
            CurrentModuleMember = null;
        }
        /// <summary>
        /// Gets encoding of xml-document from Syntactik comments.
        /// </summary>
        /// <param name="document">Syntactik document with comments.</param>
        /// <returns>Xml encoding.</returns>
        protected Encoding GetEncoding(DOM.Document document)
        {
            var leadingComments = document.Entities.TakeWhile(e => e is Comment);
            var declaration = leadingComments.FirstOrDefault(c => c.Value.StartsWith("XmlDeclaration:"));
            if (declaration == null) return Encoding.UTF8;

            var m = new Regex(@"(?:[.]*encoding\s*=\s*"")([^""]*)(?:"")").Match(declaration.Value);
            if (!m.Success || m.Groups.Count <= 1) return Encoding.UTF8;

            try
            {
                return Encoding.GetEncoding(m.Groups[1].Value);
            }
            catch (Exception)
            {
                return Encoding.UTF8;
            }
        }

        /// <summary>
        /// Writes start of xml document.
        /// </summary>
        /// <param name="document">Instance of <see cref="DOM.Document"/></param>
        protected virtual void WriteStartDocument(DOM.Document document)
        {
            XmlTextWriter.WriteStartDocument();
            if (!_generateComments) return;
            var leadingComments = document.Entities.TakeWhile(e => e is Comment);
            var pInstructions =
                leadingComments.Where(c => c.Value.StartsWith("ProcessingInstruction")).Select(c => c.Value);
            foreach (var instruction in pInstructions)
            {
                var m = new Regex(@"(?:ProcessingInstruction\s*)(.*):\s*(.*)").Match(instruction);
                if (m.Success && m.Groups.Count > 2)
                {
                    XmlTextWriter.WriteProcessingInstruction(m.Groups[1].Value, m.Groups[2].Value);
                }
            }
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
            var choiceInfo = JsonGenerator.FindChoiceInfo(choice, pair);
            if (choice.ChoiceNode != pair)
            {
                ChoiceStack.Push(choiceInfo);
            }
            ChoiceStack.Push(choiceInfo.Children[0]);
            if (((Element)choiceInfo.Children[0].ChoiceNode).Entities.Count > 0)
                Visit(((Element)choiceInfo.Children[0].ChoiceNode).Entities);

            ChoiceStack.Pop();
            if (choice.ChoiceNode != pair)
                ChoiceStack.Pop();
            return true;

        }

        /// <inheritdoc />
        protected override string ResolveChoiceValue(Pair pair, out ValueType valueType)
        {
            var choice = ChoiceStack.Peek();
            var choiceInfo = JsonGenerator.FindChoiceInfo(choice, pair);
            if (choice.ChoiceNode != pair)
            {
                ChoiceStack.Push(choiceInfo);
            }
            string result = string.Empty;
            valueType = ValueType.OpenString;
            if (choice.Children != null)
            {
                ChoiceStack.Push(choiceInfo.Children[0]);
                result = ResolvePairValue((IMappedPair) choiceInfo.Children[0].ChoiceNode, out valueType);
                ChoiceStack.Pop();
            }
            if (choice.ChoiceNode != pair)
                ChoiceStack.Pop();
            return result;
        }

        /// <inheritdoc />
        public override void Visit(Element element)
        {
            //Getting namespace and prefix
            NamespaceResolver.GetPrefixAndNs(element, CurrentDocument,
                ScopeContext.Peek(),
                out var prefix, out var ns);

            if (string.IsNullOrEmpty(element.NsPrefix)) prefix = null; 
            //Starting Element
            if (!string.IsNullOrEmpty(element.Name))
            {
                if (element.Assignment != AssignmentEnum.CCC)
                {
                    //not text node
                    XmlTextWriter.WriteStartElement(prefix, element.Name, ns);
                    AddLocationMapRecord(CurrentModuleMember.Module.FileName, (IMappedPair) element);

                    //Write all namespace declarations in the root element
                    if (!DocumentElementAdded)
                    {
                        WritePendingNamespaceDeclarations(ns);
                        DocumentElementAdded = true;
                    }
                }
            }
            else
            {
                if (element.Parent.Assignment == AssignmentEnum.CCC && (element.Assignment == AssignmentEnum.C || element.Assignment == AssignmentEnum.CC || element.Assignment == AssignmentEnum.E || element.Assignment == AssignmentEnum.EE))
                {
                    // This is item of explicit array (:::)
                    WriteExplicitArrayItem(element, prefix, ns);
                }
            }

            if (!ResolveValue(element) && !EnterChoiceContainer(element, element.Entities))
                base.Visit(element);

            //End Element
            if (!string.IsNullOrEmpty(element.Name))
            {
                if (element.Assignment != AssignmentEnum.CCC) //not text node and not explicit array
                    XmlTextWriter.WriteEndElement();
            }
            else
            {
                if (element.Parent.Assignment == AssignmentEnum.CCC && (element.Assignment == AssignmentEnum.C || element.Assignment == AssignmentEnum.CC || element.Assignment == AssignmentEnum.E || element.Assignment == AssignmentEnum.EE))
                    XmlTextWriter.WriteEndElement();
            }
        }

        private void WriteExplicitArrayItem(Element element, string prefix, string ns)
        {
            if (string.IsNullOrEmpty(element.NsPrefix)) prefix = null;
            XmlTextWriter.WriteStartElement(prefix, element.Parent.Name, ns);
            AddLocationMapRecord(CurrentModuleMember.Module.FileName, (IMappedPair) element);
        }

        /// <summary>
        /// Adds record about <see cref="IMappedPair"/> in the <see cref="LocationMap"/>.
        /// </summary>
        /// <param name="fileName">Module file name.</param>
        /// <param name="pair">Mapped pair.</param>
        protected virtual void AddLocationMapRecord(string fileName, IMappedPair pair)
        {
            LocationMap.Add(new LexicalInfo(fileName, pair.NameInterval.Begin.Line,
                pair.NameInterval.Begin.Column, pair.NameInterval.Begin.Index));
        }

        /// <inheritdoc />
        public override void Visit(Alias alias)
        {
            var aliasDef = ((DOM.Mapped.Alias)alias).AliasDefinition;
            var prevCurrentModuleMember = CurrentModuleMember;
            CurrentModuleMember = aliasDef;
            if (aliasDef.IsValueNode)
            {
                OnValue(ResolveValueAlias((DOM.Mapped.Alias)alias, out var valueType), valueType);
            }
            AliasContext.Push((DOM.Mapped.Alias) alias);
            if (!EnterChoiceContainer((DOM.Mapped.Alias) alias, aliasDef.Entities, aliasDef))
                Visit(aliasDef.Entities.Where(e => !(e is DOM.Attribute)));
            AliasContext.Pop();
            CurrentModuleMember = prevCurrentModuleMember;
        }

        /// <inheritdoc />
        public override void OnValue(string value, ValueType type)
        {
            XmlTextWriter.WriteString(value);
        }

        /// <inheritdoc />
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
        /// <param name="character">Character.</param>
        /// <returns>True of the character is allowed.</returns>
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

        /// <inheritdoc />
        public override void Visit(DOM.Attribute attribute)
        {
            string prefix = string.Empty, ns = string.Empty;
            if (!string.IsNullOrEmpty(attribute.NsPrefix))
            {
                NamespaceResolver.GetPrefixAndNs(attribute, CurrentDocument,
                    ScopeContext.Peek(),
                    out prefix, out ns);
            }
            XmlTextWriter.WriteStartAttribute(prefix, attribute.Name, ns);
            if (!ResolveValue(attribute) )
            {
                EnterChoiceContainer(attribute, new PairCollection<Entity>().AddRange(((DOM.Mapped.Attribute)attribute).InterpolationItems?.OfType<Entity>()));
            }
            XmlTextWriter.WriteEndAttribute();
            
            AddLocationMapRecord(CurrentModuleMember.Module.FileName, (IMappedPair) attribute);
        }

        /// <inheritdoc />
        public override void Visit(Comment comment)
        {
            if (_generateComments &&
                 !comment.Value.StartsWith("XmlDeclaration:") &&
                 !comment.Value.StartsWith("ProcessingInstruction"))
                    XmlTextWriter.WriteComment(comment.Value);
        }

        private void WritePendingNamespaceDeclarations(string uri)
        {
            NsInfo nsInfo = NamespaceResolver.GetNsInfo(CurrentDocument);
            if (nsInfo == null) return;

            foreach (var ns in nsInfo.Namespaces)
            {
                if (ns.Value == uri) continue;
                XmlTextWriter.WriteAttributeString("xmlns", ns.Name, null, ns.Value);

            }
        }
    }
}
