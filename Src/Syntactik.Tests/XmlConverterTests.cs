using System.Collections.Specialized;
using NUnit.Framework;
using Syntactik.Tests;
using static Syntactik.Tests.TestUtils;


namespace SyntactikMDAddin.Tests
{
    [TestFixture]
    public class XmlConverterTests
    {
        [Test, RecordTest]
        public void AttributeAndText()
        {
            DoXmlConverterTest();
        }

        [Test, RecordedTest]
        public void Comment1()
        {
            DoXmlConverterTest();
        }
        [Test, RecordedTest]
        public void NamespaceResolution1()
        {
            DoXmlConverterTest();
        }
        [Test, RecordedTest]
        public void NamespaceResolution2()
        {
            var declaredNamespaces = new ListDictionary { { "ipo", "http://www.example.com/IPO" } };
            DoXmlConverterTest(declaredNamespaces);
        }
        [Test, RecordedTest]
        public void NamespaceResolution3()
        {
            DoXmlConverterTest();
        }
        [Test, RecordedTest]
        public void NamespaceResolution4()
        {
            DoXmlConverterTest();
        }
        [Test, RecordedTest]
        public void ProcessingInstruction()
        {
            DoXmlConverterTest();
        }
        [Test, RecordedTest]
        public void TextNode1()
        {
            DoXmlConverterTest();
        }
    }
}