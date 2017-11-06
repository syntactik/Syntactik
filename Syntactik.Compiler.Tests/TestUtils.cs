#region license
// Copyright © 2016, 2017 Maxim Trushin (dba Syntactik, trushin@gmail.com, maxim.trushin@syntactik.com)
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
using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Syntactik.Compiler.Pipelines;
using System;
using System.Text;
using System.Collections.Generic;
using Syntactik.Compiler.IO;

namespace Syntactik.Compiler.Tests
{
    public class TestUtils
    {
        public static void PerformCompilerTest(bool generateComments = false)
        {
            PrintTestScenario();

            var compilerParameters = CreateCompilerParameters(generateComments);
            var compiler = new SyntactikCompiler(compilerParameters);
            var context = compiler.Run();


            //Compiler Error Assertions
            string serialParserErrors;
            var recordedParserErros = LoadSavedCompilerErrors(context.Errors, out serialParserErrors);
            if (recordedParserErros != null)
            {
                Console.WriteLine("Compiler Errors:");
                Console.WriteLine(serialParserErrors);
                Assert.AreEqual(recordedParserErros, serialParserErrors);
            }
            else
            {
                Console.WriteLine("Compiler Errors:");
                Console.WriteLine(serialParserErrors);
                Assert.IsTrue(context.Errors.Count == 0, "Compiler has errors");
            }

            if (IsRecordedTest() || IsRecordTest())
                CompareResultAndRecordedFiles(IsRecordTest());
        }

        public static string LoadSavedCompilerErrors(ICollection<CompilerError> errors, out string serialParserErrors)
        {
            var isCompilerErrorRecordedTest = IsCompilerErrorRecordedTest();
            var isCompilerErrorRecordTest = IsCompilerErrorRecordTest(); //Overwrites existing recording
            string recorded = null;
            serialParserErrors = errors.Count > 0 ? SerializeErrors(errors) : null; 
            if (isCompilerErrorRecordedTest || isCompilerErrorRecordTest)
            {
                if (isCompilerErrorRecordedTest)
                {
                    var testCaseName = GetTestCaseName();
                    var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Recorded\").Append(testCaseName).Append(".error").ToString();
                    if (File.Exists(fileName))
                        recorded = File.ReadAllText(fileName).Replace("\r\n", "\n");
                }
                if (isCompilerErrorRecordTest)
                {
                    SaveCompilerErrors(serialParserErrors);
                    return serialParserErrors;
                }
            }
            return recorded;
        }

        private static void SaveCompilerErrors(string serialLexerErrors)
        {
            var testCaseName = GetTestCaseName();
            var fileName = new StringBuilder(AssemblyDirectory + @"\..\..\Scenarios\Recorded\").Append(testCaseName).Append(".error").ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.WriteAllText(fileName, serialLexerErrors);
        }

        private static bool IsCompilerErrorRecordedTest()
        {
            return TestHasAttribute<CompilerErrorRecordedAttribute>() || TestFixtureHasAttribute<CompilerErrorRecordedAttribute>();
        }

        private static bool IsCompilerErrorRecordTest()
        {
            return TestHasAttribute<CompilerErrorRecordAttribute>() || TestHasAttribute<CompilerErrorRecordedAttribute>() && TestFixtureHasAttribute<CompilerErrorRecordAttribute>();
        }

        private static string SerializeErrors(IEnumerable<CompilerError> errors)
        {
            var sb = new StringBuilder();
            foreach (var error in errors)
            {
                sb.Append(error.Code + " " + Path.GetFileName(error.LexicalInfo.FileName) +
                    $"({error.LexicalInfo.Line},{error.LexicalInfo.Column},{error.LexicalInfo.Index})" +
                    ": ");
                sb.AppendLine(error.Message);
                if (error.InnerException != null)
                    sb.AppendLine(error.InnerException.StackTrace);
            }

            return sb.ToString().Replace("\r\n", "\n");
        }

        /// <summary>
        /// Record result file or compare it with previously recorded result
        /// </summary>
        /// <param name="record">If true then result file is recorded not compared.</param>
        private static void CompareResultAndRecordedFiles(bool record)
        {
            var testCaseName = GetTestCaseName();
            var resultDir = AssemblyDirectory + '\\' + "Scenarios" + '\\' + testCaseName + "\\Result\\";
            var recordedDir = AssemblyDirectory + @"\Scenarios\Recorded\" + testCaseName + @"\Compiler\";

            if (record)
            {
                var recordDir = AssemblyDirectory + @"\..\..\Scenarios\Recorded\" + testCaseName + @"\Compiler\";
                if(Directory.Exists(recordDir)) Directory.Delete(recordDir, true);
                Directory.CreateDirectory(recordDir);
                foreach(var file in Directory.GetFiles(resultDir))
                {
                    var newFile = recordDir + Path.GetFileName(file);
                    File.Copy(file, newFile, true);
                }               
            }
            else
            {
                Assert.IsTrue(Directory.Exists(resultDir), "Directory {0} doesn't exist", resultDir);
                Assert.IsTrue(Directory.Exists(recordedDir), "Directory {0} doesn't exist", recordedDir);

                //Equal number of files
                Assert.AreEqual(Directory.GetFiles(recordedDir).Length, Directory.GetFiles(resultDir).Length, "Number of files {0} in '{1}' should be equal {2}", Directory.GetFiles(resultDir).Length, resultDir, Directory.GetFiles(recordedDir).Length);
                Console.WriteLine();
                Console.WriteLine("Generated Files:");
                foreach (var file in Directory.GetFiles(recordedDir))
                {
                    var recordedFileName = Path.GetFileName(file);
                    var resultFileName = resultDir + recordedFileName;
                    var result = File.ReadAllText(resultFileName).Replace("\r\n", "\n");
                    var recorded = File.ReadAllText(file).Replace("\r\n", "\n");

                    Console.WriteLine($"File {file}:");
                    Console.WriteLine(result);
                    Assert.AreEqual(recorded, result);
                }
            }
            Directory.Delete(resultDir, true);

        }

        private static void PrintTestScenario()
        {
            var testCaseName = GetTestCaseName();

            var dir = new StringBuilder(AssemblyDirectory + @"\Scenarios\").Append(testCaseName).ToString();

            foreach (var fileName in Directory.EnumerateFiles(dir))
            {
                if (!fileName.EndsWith(".s4x") && !fileName.EndsWith(".s4j")) continue;

                Console.WriteLine();
                Console.WriteLine(Path.GetFileName(fileName));
                var code = File.ReadAllText(fileName);
                PrintCode(code);
            }
        }

        private static CompilerParameters CreateCompilerParameters(bool generateComments)
        {
            var compilerParameters = new CompilerParameters {Pipeline = new CompileToFiles(generateComments)};

            var dir = AssemblyDirectory + '\\' + "Scenarios" + '\\' + GetTestCaseName() + '\\';

            compilerParameters.OutputDirectory = dir + "Result" + '\\';

            foreach (var fileName in Directory.EnumerateFiles(dir))
            {
                if (fileName.EndsWith(".s4x") || fileName.EndsWith(".s4j"))
                {
                    compilerParameters.Input.Add(new FileInput(fileName));
                }
                if (fileName.EndsWith(".xsd")) compilerParameters.XmlSchemaSet.Add(null, fileName);
            }

            return compilerParameters;
        }

        public static void PrintCode(string code)
        {
            int line = 1;
            Console.WriteLine("Code:");
            Console.Write("{0}:\t ", line);
            int offset = 0;
            foreach (var c in code)
            {
                if (c == '\r') continue;
                if (c == '\n')
                {
                    Console.Write(" ({0})", offset);
                }

                Console.Write(c);
                offset++;
                if (c == '\n')
                {
                    line++;
                    Console.Write("{0}:\t ", line);
                }
            }
            Console.Write(" ({0})", offset);
            Console.WriteLine();
        }

        public static string AssemblyDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static string GetTestCaseName()
        {
            var trace = new StackTrace();
            return trace.GetFrames().Select(f => f.GetMethod()).First(m => m.CustomAttributes.Any(a => a.AttributeType.FullName == "NUnit.Framework.TestAttribute")).Name;
        }

        private static bool TestHasAttribute<T>()
        {
            var trace = new StackTrace();
            var method = trace.GetFrames().Select(f => f.GetMethod()).First(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(TestAttribute)));
            Debug.Assert(method.DeclaringType != null, "method.DeclaringType != null");
            return method.CustomAttributes.Any(ca => ca.AttributeType == typeof(T));
        }

        private static bool TestFixtureHasAttribute<T>()
        {
            var trace = new StackTrace();
            var method = trace.GetFrames().Select(f => f.GetMethod()).First(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(TestAttribute)));
            Debug.Assert(method.DeclaringType != null, "method.DeclaringType != null");
            return method.DeclaringType.CustomAttributes.Any(ca => ca.AttributeType == typeof(T));
        }

        public static bool IsRecordedTest()
        {
            return TestHasAttribute<RecordedTestAttribute>() || TestFixtureHasAttribute<RecordedTestAttribute>();
        }

        public static bool IsRecordTest()
        {
            return TestHasAttribute<RecordTestAttribute>() || TestHasAttribute<RecordedTestAttribute>() && TestFixtureHasAttribute<RecordTestAttribute>();
        }


    }
}
