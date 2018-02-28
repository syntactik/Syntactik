using Syntactik.DOM;

namespace Syntactik
{
    internal class WsaInfo
    {
        public readonly Pair Pair;
        public readonly int Bracket;
        private int _closingBracket;

        public WsaInfo(Pair pair, int bracket)
        {
            Pair = pair;
            Bracket = bracket;
            _closingBracket = 0;
        }

        public int ClosingBracket
        {
            get
            {
                if (_closingBracket == 0)
                {
                    if (Bracket == '(') return _closingBracket = ')';
                    if (Bracket == '{') return _closingBracket = '}';
                    if (Bracket == '[') return _closingBracket = ']';
                }
                return _closingBracket;
            }
        }
    }
}
