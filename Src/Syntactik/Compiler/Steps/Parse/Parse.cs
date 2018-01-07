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
using System.IO;
using Syntactik.DOM;
using Syntactik.IO;

namespace Syntactik.Compiler.Steps
{
    public class Parse : ICompilerStep
    {
        protected CompilerContext _context;

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
                foreach (var input in _context.Parameters.Input)
                {
                    try
                    {
                        using (var reader = input.Open())
                            DoParse(input.Name, reader);
                    }
                    catch (Exception ex)
                    {
                        _context.AddError(CompilerErrorFactory.InputError(input.Name, ex));
                    }
                }
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }

        protected virtual void DoParse(string fileName, TextReader reader)
        {
            try
            {
                var module = CreateModule(fileName);
                _context.CompileUnit.AppendChild(module);
                Parser parser = GetParser(module, new InputStream(reader.ReadToEnd()));
                var errorListener = new ErrorListener(_context, fileName);
                parser.ErrorListeners.Add(errorListener);
                parser.ParseModule();
            }
            catch (Exception ex)
            {
                _context.AddError(CompilerErrorFactory.FatalError(ex));
            }
        }

        protected virtual Module CreateModule(string fileName)
        {
            return new DOM.Mapped.Module(name: Path.GetFileNameWithoutExtension(fileName), fileName: fileName);
        }

        protected virtual Parser GetParser(Module module, ICharStream input)
        {
            if (!(module is DOM.Mapped.Module m)) throw new ArgumentException("Invalid module type.");

            if (m.TargetFormat == DOM.Mapped.Module.TargetFormats.Json)
            {
                return new Parser(input, new PairFactoryForJson(_context, module), module);
            }
            else
            {
                return new Parser(input, new PairFactoryForXml(_context, module), module);
            }
        }
    }
}
