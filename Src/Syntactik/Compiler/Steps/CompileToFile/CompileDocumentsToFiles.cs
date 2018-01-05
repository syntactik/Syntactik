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
using System.IO;
using System.Text;
using System.Xml;
using Syntactik.Compiler.Generator;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using Module = Syntactik.DOM.Mapped.Module;

namespace Syntactik.Compiler.Steps
{
    class CompileDocumentsToFiles : ICompilerStep
    {
        CompilerContext _context;
        private readonly bool _generateComments;

        public CompileDocumentsToFiles(bool generateComments = false)
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
                foreach (var module in _context.CompileUnit.Modules)
                {
                    if (_context.Errors.Count > 0) break;
                    DoCompileDocumentsToFile((Module) module, _context);
                }
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }

        private void DoCompileDocumentsToFile(Module module, CompilerContext context)
        {
            try
            {
                Directory.CreateDirectory(context.Parameters.OutputDirectory);

                SyntactikDepthFirstVisitor visitor;
                if (module.TargetFormat == Module.TargetFormats.Xml)
                    visitor = new XmlGenerator(XmlFileWriterDelegate, XmlFileReaderDelegate, context, _generateComments);
                else visitor = new JsonGenerator(JsonFileWriterDelegate, context);

                visitor.Visit(module);
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }

        private XmlReader XmlFileReaderDelegate(string documentName, XmlReaderSettings settings)
        {
            var fileName = Path.Combine(_context.Parameters.OutputDirectory, documentName + ".xml");

            return XmlReader.Create(new XmlTextReader(fileName), settings);
        }

        private XmlWriter XmlFileWriterDelegate(string documentName, Encoding encoding)
        {
            var fileName = Path.Combine(_context.Parameters.OutputDirectory, documentName + ".xml");
            return XmlWriter.Create(
                new XmlTextWriter(fileName, encoding) {Formatting = System.Xml.Formatting.Indented, Namespaces = true},
                new XmlWriterSettings {ConformanceLevel = ConformanceLevel.Document});

        }

        private JsonWriter JsonFileWriterDelegate(string documentName)
        {
            var fileName = Path.Combine(_context.Parameters.OutputDirectory, documentName + ".json");
            if (File.Exists(fileName)) File.Delete(fileName);
            TextWriter writer = new StreamWriter(fileName);
            return new JsonTextWriter(writer) {Formatting = Formatting.Indented};
        }
    }
}
