namespace Syntactik.DOM.Mapped
{
    public class Comment : DOM.Comment, IMappedPair
    {
        /// <summary>
        /// 1 - singleline comment
        /// 2 - multiline comment
        /// </summary>
        public int CommentType { get; set; }
        public Interval NameInterval { get; set; }
        public Interval ValueInterval { get; set; }
        public Interval DelimiterInterval { get; set; }
        public ValueType ValueType { get; set; }
        public bool IsValueNode => true;
        public int ValueIndent { get; set; }
    }
}