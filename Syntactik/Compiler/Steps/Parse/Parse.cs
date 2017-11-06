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
                parser.ParseModule(fileName);
            }
            catch (Exception ex)
            {
                _context.AddError(CompilerErrorFactory.FatalError(ex));
            }
        }

        protected virtual Module CreateModule(string fileName)
        {
            return new DOM.Mapped.Module { Name = Path.GetFileNameWithoutExtension(fileName), Value = null, FileName = fileName };
        }

        protected virtual Parser GetParser(Module module, ICharStream input)
        {
            var m = module as DOM.Mapped.Module;
            if (m == null) throw new ArgumentException("Invalid module type.");

            if (m.TargetFormat == DOM.Mapped.Module.TargetFormats.Json)
            {
                return new Parser(input, new ReportingPairFactoryForJson(_context, module), module);
            }
            else
            {
                return new Parser(input, new ReportingPairFactoryForXml(_context, module), module);
            }
        }
    }
}
