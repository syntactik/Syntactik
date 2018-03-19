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
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Syntactik.IO;
using NUnit.Framework;
using Syntactik.Converter;
using Syntactik.DOM;
using Module = Syntactik.DOM.Mapped.Module;

namespace Syntactik.Tests
{
    public class TestUtils
    {
        public static string AssemblyDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void DoTest()
        {
            var code = LoadTestCode();
#if (!CI)
            PrintCode(code);
#endif

            var parser = new Parser(new InputStream(code), new PairFactory(), new Module("Module"), true);
            var errorListener = new ErrorListener();
            parser.ErrorListeners.Add(errorListener);

            var domText = PrintModule(parser.ParseModule());

            var recordedDom = GetRecordedDomText(domText);

            //Parser Errors
            var recordedParserErros = LoadParserErrors(errorListener, out var serialParserErrors);
            if (recordedParserErros != null)
            {
#if (!CI)
                TestContext.WriteLine("Parser Errors:");
                TestContext.WriteLine(serialParserErrors);
#endif
            }

            //DOM Assertions
            if (recordedDom != null)
            {
                Assert.AreEqual(recordedDom, domText.Replace("\r\n", "\n"), "DOM assertion failed");
            }

            //ERROR Assertions
            if (recordedParserErros != null)
            {
                Assert.AreEqual(recordedParserErros, serialParserErrors);
            }
            else
            {
#if (!CI)
                if (errorListener.Errors.Count > 0)
                    PrintErrors(errorListener.Errors, "Parser Errors:");
#endif

                Assert.AreEqual(false, errorListener.Errors.Count > 0, "ParserErrorListener has errors");
            }
        }

        public static void DoXmlConverterTest(ListDictionary declaredNamespaces = null, int indent = 0, char indentChar = '\t', int indentMultiplicity = 1, bool insertNewLine = false)
        {

            if (declaredNamespaces == null) declaredNamespaces = new ListDictionary();
            var input = PrintTestScenario(".text");
            string s = ConvertXml(input, declaredNamespaces, indent, indentChar, indentMultiplicity, insertNewLine);
            if (IsRecordedTest() || IsRecordTest())
                CompareResultAndRecordedFiles(s, IsRecordTest(), "cxml");
        }

        public static void DoJsonConverterTest(int indent = 0, char indentChar = '\t', int indentMultiplicity = 1, bool insertNewLine = false)
        {
            var input = PrintTestScenario(".json");
            string s = ConvertJson(input, indent, indentChar, indentMultiplicity, insertNewLine);
            if (IsRecordedTest() || IsRecordTest())
                CompareResultAndRecordedFiles(s, IsRecordTest(), "cjson");
        }

        private static string PrintTestScenario(string extension = ".s4x")
        {
            var testCaseName = GetTestCaseName();

            var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\").Append(testCaseName).Append(extension).ToString();

            Console.WriteLine();
            Console.WriteLine(Path.GetFileName(fileName));
            var code = File.ReadAllText(fileName);
            PrintCode(code);
            return code.TrimEnd('~');
        }

        private static string ConvertXml(string text, ListDictionary declaredNamespaces, int indent = 0, char indentChar = '\t', int indentMultiplicity = 1, bool insertNewLine = false)
        {
            var converter = new XmlToSyntactikConverter(text);
            converter.Convert(indent, indentChar, indentMultiplicity, insertNewLine, declaredNamespaces, out var output);
            return output;
        }

        public static bool IsRecordedTest()
        {
            return TestHasAttribute<RecordedTestAttribute>();
        }

        public static bool IsRecordTest()
        {
            return TestHasAttribute<RecordTestAttribute>();
        }

        private static void CompareResultAndRecordedFiles(string result, bool record, string extension)
        {
            var recordedDir = AssemblyDirectory + @"\Scenarios\" + @"\Recorded\";
            var recordedFileName = recordedDir + GetTestCaseName() + "." + extension;

            Console.WriteLine();
            Console.WriteLine(@"Completion info:");

            if (record)
            {

                SaveTest(result, extension);
                Console.WriteLine(result);
            }
            else
            {
                Assert.IsTrue(Directory.Exists(recordedDir), "Directory {0} doesn't exist", recordedDir);

                result = result.Replace("\r\n", "\n");
                var recorded = File.ReadAllText(recordedFileName).Replace("\r\n", "\n");

                Console.WriteLine(result);
                Assert.AreEqual(recorded, result);
            }
        }

        public static void SaveTest(string result, string extension)
        {
            var recordedDir = AssemblyDirectory + @"\..\..\Scenarios\" + @"\Recorded\";
            var fileName = recordedDir + GetTestCaseName() + "." + extension;
            Directory.CreateDirectory(recordedDir);
            File.WriteAllText(fileName, result);
        }

        private static string ConvertJson(string text, int indent = 0, char indentChar = '\t', int indentMultiplicity = 1, bool insertNewLine = false)
        {
            var converter = new JsonToSyntactikConverter(text);
            string output;
            converter.Convert(indent, indentChar, indentMultiplicity, insertNewLine, out output);
            return output;
        }

        private static string GetTestClassName()
        {
            var trace = new StackTrace();
            var method =
                trace.GetFrames()
                    .Select(f => f.GetMethod())
                    .First(m => m.CustomAttributes.Any(a => a.AttributeType.FullName == "NUnit.Framework.TestAttribute"));
            var name = method.DeclaringType.Name;
            var result = name.Substring(0, name.Length - "Tests".Length);
            return result;
        }

        private static void PrintErrors(IEnumerable<string> errors, string title)
        {
            TestContext.WriteLine(title);

            foreach (var error in errors)
            {
                TestContext.WriteLine();
                TestContext.WriteLine(error);
            }

        }

        private static string GetRecordedDomText(string domText)
        {
            var isDomRecordedTest = IsDomRecordedTest();
            var isDomRecordTest = IsDomRecordTest(); //Overwrites existing recording
            string recordedDom = null;
            if (isDomRecordedTest || isDomRecordTest)
            {
                if (isDomRecordedTest) recordedDom = LoadRecordedDomTest();
                if (recordedDom == null || isDomRecordTest)
                {
                    SaveRecordedDomTest(domText);
                }
            }
            return recordedDom;
        }

        public static string LoadRecordedDomTest(bool scenarioHasFolder = false)
        {
            var testCaseName = GetTestCaseName();
            var sb = new StringBuilder(AssemblyDirectory + @"\Scenarios\Recorded\").Append(testCaseName);
            if (scenarioHasFolder) sb.Append('\\').Append(testCaseName);
            sb.Append(".dom");

            var fileName = sb.ToString();
            if (!File.Exists(fileName)) return null;

            return File.ReadAllText(fileName).Replace("\r\n", "\n");
        }

        public static void SaveRecordedDomTest(string printedTokens, bool scenarioHasFolder = false)
        {
            var testCaseName = GetTestCaseName();
            var sb = new StringBuilder(AssemblyDirectory + @"\..\..\Scenarios\Recorded\").Append(testCaseName);
            if (scenarioHasFolder) sb.Append('\\').Append(testCaseName);
            sb.Append(".dom");
            var fileName = sb.ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.WriteAllText(fileName, printedTokens);
        }

        public static string PrintModule(Pair pair)
        {
#if (!CI)
            TestContext.WriteLine("\nDOM:");
#endif
            var printer = new DomPrinter();
            printer.Visit(pair);
#if (!CI)
            TestContext.WriteLine(printer.Text);
#endif
            return printer.Text;
        }

        private static string LoadTestCode()
        {
            var testCaseName = GetTestCaseName();

            var fileName = AssemblyDirectory + @"\Scenarios\" + testCaseName;

            if (File.Exists(fileName + ".s4j")) fileName += ".s4j";
            else fileName += ".s4x";

            return File.ReadAllText(fileName).Replace("\r\n", "\n");
        }

        static string LoadParserErrors(ErrorListener errorListener, out string serialParserErrors)
        {
            var isParserErrorRecordedTest = IsParserErrorRecordedTest();
            var isParserErrorRecordTest = IsParserErrorRecordTest(); //Overwrites existing recording
            string recorded = null;
            serialParserErrors = null;
            if (isParserErrorRecordedTest || isParserErrorRecordTest)
            {
                serialParserErrors = errorListener.Errors.Count > 0 ? SerializeErrors(errorListener.Errors) : null;
                if (isParserErrorRecordedTest)
                {
                    var testCaseName = GetTestCaseName();
                    var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Recorded\").Append(testCaseName).Append(".error").ToString();
                    if (File.Exists(fileName))
                        recorded = File.ReadAllText(fileName).Replace("\r\n", "\n");
                }
                if (recorded == null || isParserErrorRecordTest)
                {
                    SaveLexerErrors(serialParserErrors);
                    return serialParserErrors;
                }
            }
            return recorded;
        }

        private static void SaveLexerErrors(string serialLexerErrors)
        {
            var testCaseName = GetTestCaseName();
            var fileName = new StringBuilder(AssemblyDirectory + @"\..\..\Scenarios\Recorded\").Append(testCaseName).Append(".error").ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.WriteAllText(fileName, serialLexerErrors);
        }

        private static string SerializeErrors(IEnumerable<string> errors)
        {
            var sb = new StringBuilder();
            foreach (var item in errors)
            {
                sb.AppendLine(item);
            }

            return sb.ToString().Replace("\r\n", "\n");

        }

        private static string GetTestCaseName()
        {
            var trace = new StackTrace();
            var method =
                trace.GetFrames()
                    .Select(f => f.GetMethod())
                    .First(m => m.CustomAttributes.Any(a => a.AttributeType.FullName == "NUnit.Framework.TestAttribute"));

            return method.DeclaringType?.Name.Substring(0, method.DeclaringType.Name.Length - 5) + "/" +  method.Name;
        }

        public static void PrintCode(string code)
        {
            int line = 1;
            TestContext.WriteLine("Code:");
            TestContext.Write("{0}:\t ", line);
            int offset = 0;
            foreach (var c in code)
            {
                if (c == '\r') continue;
                if (c == '\n')
                {
                    TestContext.Write(" ({0})", offset);
                }

                TestContext.Write(c);
                offset++;
                if (c == '\n')
                {
                    line++;
                    TestContext.Write("{0}:\t ", line);
                }
            }
            TestContext.Write(" ({0})", offset);
            TestContext.WriteLine();
        }

        private static bool TestHasAttribute<T>()
        {
            var trace = new StackTrace();
            var method = trace.GetFrames().Select(f => f.GetMethod()).First(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(TestAttribute)));
            return method.DeclaringType != null && (method.CustomAttributes.Any(ca => ca.AttributeType == typeof(T)) ||
                                                    method.DeclaringType.CustomAttributes.Any(ca => ca.AttributeType == typeof(T)));
        }

        public static bool IsDomRecordedTest()
        {
            return TestHasAttribute<DomRecordedAttribute>();
        }

        public static bool IsDomRecordTest()
        {
            return TestHasAttribute<DomRecordAttribute>();
        }

        private static bool IsParserErrorRecordedTest()
        {
            return TestHasAttribute<ParserErrorRecordedAttribute>();
        }

        private static bool IsParserErrorRecordTest()
        {
            return TestHasAttribute<ParserErrorRecordAttribute>();
        }
    }
}
