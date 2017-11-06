﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Syntactik.IO;
using NUnit.Framework;
using Syntactik.DOM;

namespace Syntactik.Tests
{
    public class TestUtils
    {
        public static string AssemblyDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void DoTest()
        {
            var code = LoadTestCode();

            PrintCode(code);

            var parser = new Parser(new InputStream(code), new PairFactory(), new DOM.Module { Name = "Module" });
            var errorListener = new ErrorListener();
            parser.ErrorListeners.Add(errorListener);

            var domText = PrintModule(parser.ParseModule(""));

            var recordedDom = GetRecordedDomText(domText);

            //Parser Errors
            string serialParserErrors;
            var recordedParserErros = LoadParserErrors(errorListener, out serialParserErrors);
            if (recordedParserErros != null)
            {
                Console.WriteLine("Parser Errors:");
                Console.WriteLine(serialParserErrors);
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
                if (errorListener.Errors.Count > 0)
                    PrintErrors(errorListener.Errors, "Parser Errors:");

                Assert.AreEqual(false, errorListener.Errors.Count > 0, "ParserErrorListener has errors");
            }
        }

        private static void PrintErrors(List<string> errors, string title)
        {
            Console.WriteLine(title);

            foreach (var error in errors)
            {
                Console.WriteLine();
                Console.WriteLine(error);
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
            Console.WriteLine("\nDOM:");

            var printer = new DomPrinter();
            printer.Visit(pair);
            Console.WriteLine(printer.Text);
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

        public static string LoadParserErrors(ErrorListener errorListener, out string serialParserErrors)
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

        private static string SerializeErrors(List<string> errors)
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