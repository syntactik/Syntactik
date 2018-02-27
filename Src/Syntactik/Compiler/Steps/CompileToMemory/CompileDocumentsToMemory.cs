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
using Syntactik.DOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Syntactik.Compiler.Generator;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using Module = Syntactik.DOM.Mapped.Module;

namespace Syntactik.Compiler.Steps
{
    class CompileDocumentsToMemory : ICompilerStep
    {
        CompilerContext _context;
        private StringWriter _stringWriter;
        private readonly bool _generateComments;

        public CompileDocumentsToMemory(bool generateComments = false)
        {
            _generateComments = generateComments;
        }

        public void Dispose()
        {
            _context = null;
        }

        public void Initialize(CompilerContext context)
        {
            _context = context;
        }

        public void Run()
        {
            try
            {
                if (_context.Errors.Count > 0) return;
                _context.InMemoryOutputObjects = new Dictionary<string, object>();
                foreach (var module in _context.CompileUnit.Modules)
                {
                    DoCompileDocumentsToMemory((Module) module, _context);
                }
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }

        private void DoCompileDocumentsToMemory(Module module, CompilerContext context)
        {
            try
            {
                SyntactikDepthFirstVisitor visitor;

                if (module.TargetFormat == Module.TargetFormats.Xml)
                    visitor = new XmlGenerator(XmlMemoryWriterDelegate, XmlFileReaderDelegate, context, _generateComments);
                else visitor = new JsonGenerator(JsonFileWriterDelegate, context);

                visitor.Visit(module);
                
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }

        private XmlWriter XmlMemoryWriterDelegate(string documentName, Encoding encoding)
        {
            _stringWriter = new StringWriter();
            _context.InMemoryOutputObjects[documentName] = _stringWriter;
            return XmlWriter.Create(_stringWriter,
                new XmlWriterSettings {Encoding = encoding, ConformanceLevel = ConformanceLevel.Document, Indent = true, IndentChars = "\t", OmitXmlDeclaration = true});
        }
        private XmlReader XmlFileReaderDelegate(string documentName, XmlReaderSettings setting)
        {

            return XmlReader.Create(new StringReader(_context.InMemoryOutputObjects[documentName].ToString()), setting);
        }

        private JsonWriter JsonFileWriterDelegate(string documentName)
        {
            _stringWriter = new StringWriter();
            _context.InMemoryOutputObjects[documentName] = _stringWriter;
            return new JsonTextWriter(_stringWriter) {Formatting = Formatting.Indented};
        }
    }
}
