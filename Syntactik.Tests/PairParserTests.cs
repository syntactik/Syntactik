using NUnit.Framework;
using static Syntactik.Tests.TestUtils;

namespace Syntactik.Tests
{
    /// <summary>
    /// Tests for the basic pair scenarios.
    /// </summary>
    [TestFixture]
    public class PairParserTests
    {
        [Test, DomRecorded, ParserErrorRecorded]
        public void ChainedPairs()
        {
            DoTest();
        }
        [Test, DomRecorded]
        public void ChainedPairs2()
        {
            DoTest();
        }
        [Test, DomRecorded]
        public void Comma()
        {
            DoTest();
        }
        [Test, DomRecorded]
        public void Comments()
        {
            DoTest();
        }
        [Test, DomRecorded]
        public void CommentEndsOpenString()
        {
            DoTest();
        }
        [Test, DomRecorded]
        public void CommentsMultiline()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void EmptyInlinePair()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void EmptyName()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void EmptyValue()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void EmptyValue1()
        {
            DoTest();
        }
        [Test, DomRecorded]
        public void EmptyValue2()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void EmptyValue3()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void EmptyWsaBlock()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void SimplePair()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void SimpleBlock()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void InlineBlock()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void InlineBlock2()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void InvalidIndent()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void InvalidWsaIndent()
        {
            DoTest();
        }
        [Test, DomRecorded]
        public void MultilineBlock()
        {
            DoTest();
        }
        [Test, DomRecorded, ParserErrorRecorded]
        public void InvalidIndentMultiplicity()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void InvalidMixedIndentation()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void MultilineDqString()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void MultilineDqString2()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void MultilineDqString3()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void MultilineFreeOpenString()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void MultilineOpenString()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void MultilineOpenStringIndent()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void MultilineStringIndentMismatch()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void OpenStringComment()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void MultilineSqString()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void MultilineSqString2()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void PairDelimiter()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void SimpleBlock1()
        {
            DoTest();
        }
        [Test, DomRecorded]
        public void SimpleBlock2()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void SimpleBlock3()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void ErrorOpenParenthesis()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void ExtraDelimiter()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void DqName()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void DqName2()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void SqName()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void SqName2()
        {
            DoTest();
        }
        [Test, DomRecorded]
        public void SqString()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void DqString()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void UnclosedDq()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void UnclosedDq2()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void Wsa()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void Wsa2()
        {
            DoTest();
        }

        [Test, DomRecorded, ParserErrorRecorded]
        public void Wsa3()
        {
            DoTest();
        }

        [Test, DomRecorded]
        public void Wsa4()
        {
            DoTest();
        }

    }
}
