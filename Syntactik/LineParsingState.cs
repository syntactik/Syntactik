using Pair = Syntactik.DOM.Pair;

namespace Syntactik
{
    public enum ParserStateEnum
    {
        Indent = 1, //New line started. Indent is checked to calculate the current pair.
        PairDelimiter = 2,
        Name = 4,
        Delimiter = 8,
        Value = 16,
        IndentMLS = 32 //Indent for multiline string
    }

    public struct LineParsingState
    {
        public int Indent;
        public ParserStateEnum State;
        public bool ChainingStarted;
        public Pair CurrentPair; //current pair in the current block.
        public bool Inline; //If true then at least one pair is defined in this line already

        public void Reset()
        {
            CurrentPair = null;
            State = ParserStateEnum.Indent;
            ChainingStarted = false;
            Inline = false;
        }
    }
}