using System;

namespace Syntactik.DOM.Mapped
{

    [Serializable]
    public enum ValueType
    {
        None = 0, //Node is not Value Node
        Empty, //Node is Value Node but Value is not defined (value parameter for ex.)
        DoubleQuotedString,
        SingleQuotedString,
        OpenString,
        FreeOpenString, // Folded open string (starts with ==)
        PairValue, //Parameter or Alias
        Null, //Json null
        Number, // Json number literal
        Boolean, // Json boolean literal
        Object, //Json empty object {} or empty block (ex: empty block of parameter "%param:" )
        Concatenation,
        LiteralChoice // Alias definition with literal choice
    }
    public interface IMappedPair
    {
        Interval NameInterval { get; set; }
        Interval ValueInterval { get; set; }
        Interval DelimiterInterval { get; set; }
        ValueType ValueType { get; set; }
        bool IsValueNode { get; }
        int ValueIndent { get; }
    }
}
