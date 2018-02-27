using Syntactik.DOM;

namespace Syntactik
{
    internal struct WsaInfo
    {
        public Pair Pair;
        public int Bracket;

        public WsaInfo(Pair pair, int bracket)
        {
            Pair = pair;
            Bracket = bracket;
        }
    }
}
