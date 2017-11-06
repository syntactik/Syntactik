using Pair = Syntactik.DOM.Pair;

namespace Syntactik
{
    internal class PairIndentInfo
    {
        public Pair Pair;
        public int Indent = -1; //indent length
        public int BlockIndent = -1;
    }
}
