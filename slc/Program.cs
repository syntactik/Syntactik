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
