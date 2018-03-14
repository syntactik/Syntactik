namespace Syntactik.DOM.Mapped
{
    /// <summary>
    /// Represents type of the pair value.
    /// </summary>
    public enum ValueType
    {
        /// <summary>
        /// Node has no literal value.
        /// </summary>
        None = 0,
        /// <summary>
        /// Node has literal value but the value is not defined (for example value parameter without default value)
        /// </summary>
        Empty,
        /// <summary>
        /// Value is a double quoted string
        /// </summary>
        DoubleQuotedString,
        /// <summary>
        /// Value is a single quoted string
        /// </summary>
        SingleQuotedString,
        /// <summary>
        /// Value is an open string
        /// </summary>
        OpenString,
        /// <summary>
        /// Value is a free open string
        /// </summary>
        FreeOpenString,
        /// <summary>
        /// Pair has a "pair value".
        /// </summary>
        PairValue,
        /// <summary>
        /// JSON null literal
        /// </summary>
        Null,
        /// <summary>
        /// JSON number literal
        /// </summary>
        Number,
        /// <summary>
        /// JSON boolean literal
        /// </summary>
        Boolean,
        /// <summary>
        /// Concatenation
        /// </summary>
        Concatenation,
        /// <summary>
        /// Literal choice
        /// </summary>
        LiteralChoice
    }
}