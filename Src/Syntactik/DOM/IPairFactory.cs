using Syntactik.IO;

namespace Syntactik.DOM
{
    public interface IPairFactory
    {
        Pair CreateMappedPair(ICharStream input, int nameQuotesType, Interval nameInterval, DelimiterEnum delimiter, 
            Interval delimiterInterval, int valueQuotesType, Interval valueInterval, int valueIndent);

        void AppendChild(Pair parent, Pair child);

        void EndPair(Pair pair, Interval endInterval, bool endedByEof = false);

        /// <summary>
        /// Method is called when parser finds comment
        /// </summary>
        /// <param name="input"></param>
        /// <param name="commentType">1 - single line comment</param>
        /// <param name="commentInterval">2 - multi-line comment</param>
        Comment ProcessComment(ICharStream input, int commentType, Interval commentInterval);
    }
}