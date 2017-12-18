using System;
using System.Collections.Generic;
using NUnit.Framework;
using Syntactik.DOM;
using Syntactik.DOM.Mapped;
using Syntactik.IO;
using static Syntactik.Tests.TestUtils;
using Module = Syntactik.DOM.Module;

namespace Syntactik.Tests
{
    [TestFixture]
    class ParserTests
    {
        [Test, DomRecorded]
        public void AliasDefinitionWithAttrAndElem()
        {
            DoTest();
        }
        [Test, DomRecorded]
        public void AliasDefinitionWithAttributes()
        {
            DoTest();
        }
        [Test, DomRecorded]
        public void AliasDefinitionWithDefaultBlockParameters()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasDefinitionWithDefaultInlineBlockParameters()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasDefinitionWithDefaultValueParameters()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasDefinitionWithInlineAttributeDefaultParameters()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasDefinitionWithInlineAttributeParameters()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasDefinitionWithInlineParameters()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasDefinitionWithSimpleParameters()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasWithArguments()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasWithDefaultValueParameter()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasWithIncorrectBlock()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasWithInlineArgumentList()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasWithInlineArgumentList2()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasWithInlineArgumentList3()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasWithInlineArgumentList4()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasWithInlineArgumentList5()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void AliasWithInlineArguments()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void DotEscapedInId()
        {
            DoTest();
        }

        [Test, ParserErrorRecorded, DomRecorded]
        public void DoubleQuoteMultilineString()
        {
            DoTest();
        }

        [Test, ParserErrorRecorded, DomRecorded]
        public void DoubleQuoteMultilineStringEof()
        {
            DoTest();
        }

        [Test, ParserErrorRecorded, DomRecorded]
        public void DoubleQuoteMultilineStringEof2()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void ElementList1()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void ElementWithAlias()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void ElementWithAliasAndAliasDefinition()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void ElementWithAliasedAttribute()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void ElementWithAttributes()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void ElementWithAttributesAndOtherElements()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void ElementWithNestedAlias()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void ElementWithNestedAliasAndNestedAliasDefinition()
        {
            DoTest();
        }


        [Test, DomRecorded]
        public void ElementWithNamespace()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void EmptyElement()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void EmptyElementWithNamespace()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void EmptyParameters()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void EmptyValueEof()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void FreeOpenString()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void HybridBlock()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void HybridBlockAliasStmt()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void HybridBlockArgumentStmt()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void HybridBlockParameterStmt()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void InlineAliasDefinition()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void InlineAliasEndOfBlock()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void InlineDocumentDefinition()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void InlineElementBody1()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void InlineElementBody2()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void InlineElementBody3()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void InlineJsonArray()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void JsonArray()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void JsonArrayWithValues()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void JsonArrayWithValuesInParameters()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void JsonArrayItemInObject()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void JsonEmptyArrayAndObject()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void LineComments()
        {
            DoTest();
        }
        [Test, DomRecorded]
        public void ModuleNamespaceOverload()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void NamespaceScope1()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void NamespaceScope2()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void NamespaceScope3()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void OpenStringEndOnDedentAndEof()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void OpenStringEndsWithIndentEof()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void OpenStringMultiline()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void OpenStringMultiline2()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void OpenStringMultiline3()
        {
            DoTest();
        }

        [Test, ParserErrorRecorded, DomRecorded]
        public void SingleQuoteMultiline()
        {
            DoTest();
        }

        [Test, ParserErrorRecorded, DomRecorded]
        public void SingleQuotedStringInline()
        {
            DoTest();
        }

        [Test, DomRecord]
        public void DoubleQuoteEscape()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void OpenStringEmpty()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void ValueAliasDefinition()
        {
            DoTest();
        }


        [Test, DomRecorded]
        public void ElementAtEOF()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void Wsa1()
        {
            DoTest();
        }

        [Test, ParserErrorRecorded, DomRecorded]
        public void Wsa2()
        {
            DoTest();
        }


        /// <summary>
        /// Testing Value Indent property.
        /// </summary>
        [Test]
        public void ValueIndent1()
        {
            var pf = new PairFactory();
            const string code = @"el1:
    ";
            var parser = new Parser(new InputStream(code), pf, new Module { Name = "Module" });
            var m = (Module) parser.ParseModule("");
            var mp = (IMappedPair) m.ModuleDocument.Entities[0];
            Assert.AreEqual(1, mp.ValueIndent);
        }


        /// <summary>
        /// Testing that closing paren is reported as end of pair.
        /// </summary>
        [Test]
        public void OnEndPairEvent()
        {
            var pf = new PairFactory();
            Interval endInterval = null;
            pf.OnEndPair += (pair, interval, endedByEof) =>
            {
                if (endInterval == null)
                    endInterval = interval;
            };
            var code = @"el1:(     )";
            var parser = new Parser(new InputStream(code), pf, new Module {Name = "Module"});

            parser.ParseModule("");
            if (endInterval != null) Assert.AreEqual(')', code[endInterval.End.Index]);
        }

        /// <summary>
        /// Testing that closing paren is reported as end of pair.
        /// </summary>
        [Test]
        public void OnEndPairEvent2()
        {
            var pf = new PairFactory();
            var i = new List<int>();
            pf.OnEndPair += (pair, interval, endedByEof) => { i.Add(interval.Begin.Column); };
            var code = @"(1:(2=3)))";
            var parser = new Parser(new InputStream(code), pf, new Module {Name = "Module"});

            parser.ParseModule("");
            Assert.AreEqual(4, i.Count);
        }

        /// <summary>
        /// Testing parameter endedByEof.
        /// </summary>
        [Test]
        public void OnEndPairEvent3()
        {
            var pf = new PairFactory();
            var ebf = false;
            pf.OnEndPair += (pair, interval, endedByEof) => { ebf = endedByEof; };
            var code = @"el1:
    ";
            var parser = new Parser(new InputStream(code), pf, new Module {Name = "Module"});

            parser.ParseModule("");
            Assert.AreEqual(true, ebf);
        }

        /// <summary>
        /// Testing parameter endedByEof.
        /// </summary>
        [Test]
        public void OnEndPairEvent4()
        {
            var pf = new PairFactory();
            var ebf = false;
            pf.OnEndPair += (pair, interval, endedByEof) => { ebf = endedByEof; };
            var code = @"el1:
";
            var parser = new Parser(new InputStream(code), pf, new Module {Name = "Module"});

            parser.ParseModule("");
            Assert.AreEqual(false, ebf);
        }

        /// <summary>
        /// Testing parameter endedByEof.
        /// </summary>
        [Test]
        public void OnEndPairEvent5()
        {
            var pf = new PairFactory();
            var ebf = false;
            pf.OnEndPair += (pair, interval, endedByEof) => { ebf = endedByEof; };
            var code = @"el1:";
            var parser = new Parser(new InputStream(code), pf, new Module {Name = "Module"});

            parser.ParseModule("");
            Assert.AreEqual(true, ebf);
        }

        /// <summary>
        /// Testing parameter endedByEof.
        /// </summary>
        [Test]
        public void OnEndPairEvent6()
        {
            var pf = new PairFactory();
            var ebf = false;
            pf.OnEndPair += (pair, interval, endedByEof) => { ebf = endedByEof; };
            var code = @"el1";
            var parser = new Parser(new InputStream(code), pf, new Module {Name = "Module"});

            parser.ParseModule("");
            Assert.AreEqual(true, ebf);
        }

        /// <summary>
        /// Testing reporting end of ML value by EOF. This is used in completion to identify context.
        /// </summary>
        [Test]
        public void OnEndPairEvent7()
        {
            var pf = new PairFactory();
            Tuple<Pair, Interval, bool> t = null;

            pf.OnEndPair += (pair, interval, endedByEof) =>
            {
                if (t == null)
                {
                    t = new Tuple<Pair, Interval, bool>(pair, interval, endedByEof);
                }
            };
            var code = @"el1:
	el2 == abc


		";
            var parser = new Parser(new InputStream(code), pf, new Module {Name = "Module"});

            parser.ParseModule("");
            Assert.IsNotNull(t);
            Assert.AreEqual("el2", t.Item1.Name);
            Assert.AreEqual(5, t.Item2.End.Line);
            Assert.AreEqual(2, t.Item2.End.Column);
            Assert.AreEqual(true, t.Item3);
        }

        /// <summary>
        /// Testing that inline pair's end is reported correctly.
        /// </summary>
        [Test]
        public void OnEndPairEvent8()
        {
            var pf = new PairFactory();
            Tuple<Pair, Interval, bool> t = null;

            pf.OnEndPair += (pair, interval, endedByEof) =>
            {
                if (t == null && endedByEof)
                {
                    t = new Tuple<Pair, Interval, bool>(pair, interval, true);
                }
            };
            var code = @"el1:el2
";
            var parser = new Parser(new InputStream(code), pf, new Module { Name = "Module" });

            parser.ParseModule("");
            Assert.IsNull(t);
        }

        /// <summary>
        /// Testing reporting end of inline pair. This is used in completion to identify context.
        /// </summary>
        [Test]
        public void OnEndPairEvent9()
        {
            var pf = new PairFactory();
            Tuple<Pair, Interval, bool> t = null;

            pf.OnEndPair += (pair, interval, endedByEof) =>
            {
                if (t == null && endedByEof)
                {
                    t = new Tuple<Pair, Interval, bool>(pair, interval, true);
                }
            };
            var code = @"el1:el2";
            var parser = new Parser(new InputStream(code), pf, new Module { Name = "Module" });

            parser.ParseModule("");
            Assert.IsNotNull(t);
            Assert.AreEqual("el2", t.Item1.Name);
            Assert.AreEqual(1, t.Item2.End.Line);
            Assert.AreEqual(7, t.Item2.End.Column);
            Assert.AreEqual(true, t.Item3);
        }

        /// <summary>
        /// Testing reporting end of inline pair. This is used in completion to identify context.
        /// </summary>
        [Test]
        public void OnEndPairEvent10()
        {
            var pf = new PairFactory();
            Tuple<Pair, Interval, bool> t = null;

            pf.OnEndPair += (pair, interval, endedByEof) =>
            {
                if (t == null && endedByEof)
                {
                    t = new Tuple<Pair, Interval, bool>(pair, interval, true);
                }
            };
            var code = @"el=";
            var parser = new Parser(new InputStream(code), pf, new Module { Name = "Module" });

            parser.ParseModule("");
            Assert.IsNotNull(t);
            Assert.AreEqual("el", t.Item1.Name);
            Assert.AreEqual(1, t.Item2.End.Line);
            Assert.AreEqual(3, t.Item2.End.Column);
            Assert.AreEqual(true, t.Item3);
        }

        /// <summary>
        /// Testing reporting end of inline pair. This is used in completion to identify context.
        /// </summary>
        [Test]
        public void OnEndPairEvent11()
        {
            var pf = new PairFactory();
            Tuple<Pair, Interval, bool> t = null;

            pf.OnEndPair += (pair, interval, endedByEof) =>
            {
              t = new Tuple<Pair, Interval, bool>(pair, interval, endedByEof);
            };
            var code = "el=text\r\n";
            var parser = new Parser(new InputStream(code), pf, new Module { Name = "Module" });

            parser.ParseModule("");
            Assert.IsNotNull(t);
            Assert.AreEqual("el", t.Item1.Name);
            Assert.AreEqual(1, t.Item2.End.Line);
            Assert.AreEqual(7, t.Item2.End.Column);
            Assert.AreEqual(false, t.Item3);
        }


        /// <summary>
        /// Testing indent before eof is ignored.
        /// </summary>
        [Test]
        public void IndentEof()
        {
            var pf = new PairFactory();
            var code = "line 1\n\t\t\n\t\t";
            var parser = new Parser(new InputStream(code), pf, new Module {Name = "Module"});
            var module = (Module) parser.ParseModule("");
            Assert.AreEqual(0, module.IndentMultiplicity);
            Assert.AreEqual('\t', module.IndentSymbol);
        }
    }
}