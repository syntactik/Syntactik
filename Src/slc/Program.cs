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
using System.IO;
using System.Linq;
using System.Reflection;
using Syntactik.Compiler;
using Syntactik.Compiler.IO;
using Syntactik.Compiler.Pipelines;

namespace slc
{
    public class Program
    {
        public static int Main(string[] args)
        {
            AppInfo();
            var result = 1;
            try
            {
                bool recursive;
                List<string> files;
                string outputDirectory;
                ArgumentsParser.Parse(args, out files, out recursive, out outputDirectory);

                var compilerParameters = GetCompilerParameters(files, outputDirectory);

                var compiler = new SyntactikCompiler(compilerParameters);

                var context = compiler.Run();

                if (context.Errors.Count > 0)
                {
                    PrintCompilerErrors(context.Errors);
                }

                result = 0;
            }
            catch (ArgumentsParserException e)
            {
                Console.WriteLine("Fatal error: {0}", e.Message);
                ArgumentsParser.Help();
                AppInfo2();

            }
            catch (Exception e)
            {
                Console.WriteLine("Fatal error: {0}", e.Message);
                Console.WriteLine(e.StackTrace);
            }
            return result;
        }

        private static CompilerParameters GetCompilerParameters(IEnumerable<string> files, string outputDirectory)
        {
            var compilerParameters = new CompilerParameters {OutputDirectory = outputDirectory != string.Empty?outputDirectory: Directory.GetCurrentDirectory(), Pipeline = new CompileToFiles(false) };

            var fileNames = files as string[] ?? files.ToArray();
            var s4xFound = false;
            foreach (var fileName in fileNames)
            {
                if (fileName.EndsWith(".s4x"))
                {
                    s4xFound = true;
                    compilerParameters.Input.Add(new FileInput(fileName));
                    continue;
                }
                if (fileName.EndsWith(".xsd")) compilerParameters.XmlSchemaSet.Add(null, fileName);
            }
            
            if (!s4xFound)
            foreach (var fileName in fileNames)
            {
                if (fileName.EndsWith(".s4j"))
                {
                    compilerParameters.Input.Add(new FileInput(fileName));
                }
                
            }
            return compilerParameters;
        }

        private static void AppInfo()
        {
            var type = typeof(SyntactikCompiler);
            Console.WriteLine("Syntactik Compiler version {0}. Environment: {1}",
                type.Assembly.GetName().Version, RuntimeDisplayName);
        }

        private static void AppInfo2()
        {
            var type = typeof(SyntactikCompiler);
            Console.WriteLine("Syntactik Compiler version {0}. {1}",
                type.Assembly.GetName().Version, Path.GetDirectoryName(type.Assembly.Location));
        }

        public static string RuntimeDisplayName
        {
            get
            {
                var monoRuntime = Type.GetType("Mono.Runtime");
                return (monoRuntime != null)
                    ? (string)monoRuntime.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null)
                    : $"CLR {Environment.Version}";
            }
        }

        public static void PrintCompilerErrors(IEnumerable<CompilerError> errors)
        {
            Console.WriteLine("Compiler Errors:");

            foreach (var error in errors)
            {
                Console.WriteLine();
                Console.Write(error.Code + " " + error.LexicalInfo + ": ");
                Console.WriteLine(error.Message);
                if (error.InnerException != null)
                    Console.WriteLine(error.InnerException.StackTrace);
            }
        }

    }

}
