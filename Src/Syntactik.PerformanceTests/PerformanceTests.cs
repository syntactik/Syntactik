﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Syntactik.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using YamlDotNet.RepresentationModel;
using Module = Syntactik.DOM.Module;

namespace Syntactik.PerformanceTests
{
    [TestFixture]
    public class PerformanceTests
    {
        public static string AssemblyDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        [Test]
        public void BigFile2()
        {
            TestContext.WriteLine("Starting BigFile2");
            var code = LoadTestCodeRaw();

            var parser = new Parser(new InputStream(code), new PairFactory(), new Module("Module"));
            var t1 = Environment.TickCount;
            var m = parser.ParseModule();
            var t2 = Environment.TickCount;

            TestContext.WriteLine("ParseModule Time: {0}", t2 - t1);
            //Assert.Less(t2 - t1, 400);

            code = LoadTestCodeRaw(".xml");

            XmlDocument xml = new XmlDocument();

            t1 = Environment.TickCount;
            xml.LoadXml(code);
            t2 = Environment.TickCount;
            TestContext.WriteLine("XML Time: {0}", t2 - t1);

            code = LoadTestCodeRaw(".json");

            t1 = Environment.TickCount;
            JObject o = (JObject)JToken.ReadFrom(new JsonTextReader(new StringReader(code)));
            t2 = Environment.TickCount;
            TestContext.WriteLine("JSON Time: {0}", t2 - t1);

            code = LoadTestCodeRaw(".yml");

            var input = new StringReader(code);
            var yaml = new YamlStream();
            t1 = Environment.TickCount;
            yaml.Load(input);
            t2 = Environment.TickCount;
            TestContext.WriteLine("Yaml Time: {0}", t2 - t1);
        }

      


        public static string LoadTestCodeRaw(string ext = null)
        {
            var testCaseName = GetTestCaseName();

            var fileName =
                new StringBuilder(AssemblyDirectory + @"\Scenarios\").Append(testCaseName).Append(ext??".s4x").ToString();

            return File.ReadAllText(fileName);
        }

        private static string GetTestCaseName()
        {
            var trace = new StackTrace();
            return
                trace.GetFrames()
                    .Select(f => f.GetMethod())
                    .First(m => m.CustomAttributes.Any(a => a.AttributeType.FullName == "NUnit.Framework.TestAttribute"))
                    .Name;
        }

    }

}
