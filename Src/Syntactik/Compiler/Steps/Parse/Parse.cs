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
    /// <summary>
    /// <see cref="ICompilerStep"/> parsing Syntactik modules.
    /// </summary>
    public class Parse : ICompilerStep
    {
        private CompilerContext _context;

        /// <inheritdoc />
        public void Dispose()
        {
            _context = null;
        }

        /// <inheritdoc />
        public void Initialize(CompilerContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
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

        /// <summary>
        /// Do parsing of the single <see cref="Module"/>.
        /// </summary>
        /// <param name="fileName">Module file name.</param>
        /// <param name="reader">Source code reader.</param>
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

        /// <summary>
        /// Used to create instance of the <see cref="Module"/> for each <see cref="ICompilerInput"/> in <see cref="CompilerParameters"/>.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>Instance of the <see cref="Module"/>.</returns>
        protected virtual Module CreateModule(string fileName)
        {
            return new DOM.Mapped.Module(Path.GetFileNameWithoutExtension(fileName), fileName);
        }

        /// <summary>
        /// Creates instance of <see cref="Parser"/> to be used by this step.
        /// </summary>
        /// <param name="module">Instance of <see cref="Module"/>.</param>
        /// <param name="input">Input stream for the parser.</param>
        /// <returns>Instance of <see cref="Parser"/>.</returns>
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
