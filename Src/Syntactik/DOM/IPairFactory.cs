using Syntactik.IO;

namespace Syntactik.DOM
{
    /// <summary>
    /// <see cref="Parser"/> calls <see cref="IPairFactory"/> methods reporting all information necessary to create a Document Object Model (AST). 
    /// <see cref="Parser"/> also calls <see cref="IPairFactory"/> methods to report data that can be useful for code editor, like comments and
    /// boundaries of blocks.
    /// </summary>
    public interface IPairFactory
    {
        /// <summary>
        /// If a pair has a literal value then parser calls this method when name, assignment and value are parsed.
        /// If a pair has a block or pair value then parser calls this method when name and assignment are parsed.
        /// </summary>
        /// <param name="textSource"><see cref="ITextSource"/> is used to get name of the pair and literal values.</param>
        /// <param name="nameQuotesType">Quotes used to define name. 
        /// 0 - no quotes
        /// 1 - single quotes
        /// 2 - double quotes
        /// </param>
        /// <param name="nameInterval"><see cref="Interval"/> used to define name of the pair.</param>
        /// <param name="assignment">Pair assignment.</param>
        /// <param name="assignmentInterval"><see cref="Interval"/> used to define pair assignment.</param>
        /// <param name="valueQuotesType">Quotes used to define literal value. 
        /// 0 - no quotes
        /// 1 - single quotes
        /// 2 - double quotes
        /// </param>
        /// <param name="valueInterval"><see cref="Interval"/> used to define literal value of the pair.</param>
        /// <param name="valueIndent">Indent of the value in the source code.</param>
        /// <returns>Instance of <see cref="Pair"/>.</returns>
        Pair CreateMappedPair(ITextSource textSource, int nameQuotesType, Interval nameInterval, AssignmentEnum assignment, 
            Interval assignmentInterval, int valueQuotesType, Interval valueInterval, int valueIndent);

        /// <summary>
        /// When child pair is create, parser calls this method to add child pair to parent.
        /// </summary>
        /// <param name="parent">Parent <see cref="Pair"/>.</param>
        /// <param name="child">Child <see cref="Pair"/>.</param>
        void AppendChild(Pair parent, Pair child);

        /// <summary>
        /// Parser calls this method to provide location of end of pair. 
        /// </summary>
        /// <param name="pair"><see cref="Pair"/> that the end location info belongs to.</param>
        /// <param name="endInterval">Location of the pair's end.</param>
        /// <param name="endedByEof">True if pair was ended by end of file.</param>
        void EndPair(Pair pair, Interval endInterval, bool endedByEof = false);

        /// <summary>
        /// Method is called when parser finds a comment.
        /// </summary>
        /// <param name="textSource"><see cref="ITextSource"/> is used to get text of the comment.</param>
        /// <param name="commentType">1 - single line comment. 2 - multi-line comment</param>
        /// <param name="commentInterval">Interval of the comment in the source code.</param>
        /// <returns>Instance of <see cref="Comment"/>.</returns>
        Comment ProcessComment(ITextSource textSource, int commentType, Interval commentInterval);

        /// <summary>
        /// Reports valid brackets.
        /// </summary>
        /// <param name="currentPair">Parent pair of current block.</param>
        /// <param name="bracket">Bracket char or open or closing bracket.</param>
        /// <param name="interval"><see cref="Interval"/> of bracket in the source.</param>
        void ProcessBrackets(Pair currentPair, int bracket, Interval interval);
    }
}