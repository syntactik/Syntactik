using System;
using System.Text;

namespace Syntactik.DOM
{
    public enum DelimiterEnum
    {
        None, 
        E,   // =
        EE,  // ==
        C,   // :
        CC,  // ::
        CCC, // :::
        EC,  // =:
        ECC, // =::
        CE, // :=
    }

    public enum QuotesEnum
    {
        None,
        Single,
        Double
    }
    /// <summary>
    /// Pair has either literal (property Value), pair (property PairValue) or block (implemented by descendants) value.
    /// </summary>
    public abstract class Pair{
        protected string _value;
        protected Pair _pairValue;
        protected Pair _parent;
        protected string _name;
        protected DelimiterEnum _delimiter;
        protected int _nameQuotesType;
        protected int _valueQuotesType;

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual Pair PairValue
        {
            get { return _pairValue; }
            set
            {
                _pairValue = value;
                _value = null;
            }
        }

        /// <summary>
        /// Represent a literal value of a pair.
        /// </summary>
        public virtual string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _pairValue = null;
            }
        }
        public virtual bool HasValue()
        {
            return PairValue != null || Value != null;
        }


        public virtual DelimiterEnum Delimiter
        {
            get { return _delimiter; }
            set { _delimiter = value; }
        }

        /// <summary>
        /// 0 - no quotes
        /// 1 - single quotes
        /// 2 - double quotes
        /// </summary>
        public virtual int NameQuotesType
        {
            get { return _nameQuotesType; }
            set { _nameQuotesType = value; }
        }

        /// <summary>
        /// 0 - no quotes
        /// 1 - single quotes
        /// 2 - double quotes
        /// </summary>
        public virtual int ValueQuotesType
        {
            get { return _valueQuotesType; }
            set { _valueQuotesType = value; }
        }

        public virtual void InitializeParent(Pair parent)
        {
            _parent = parent;
        }
        public virtual Pair Parent => _parent;

        // Methods
        public abstract void Accept(IDomVisitor visitor);
        public virtual void AppendChild(Pair child)
        {
            throw new NotSupportedException(new StringBuilder("Cannot add ").Append(child.GetType().Name).Append(" in ").Append(GetType().Name).ToString());
        }

        public static string DelimiterToString(DelimiterEnum delimiter)
        {
            switch (delimiter)
            {
                case DelimiterEnum.C:
                    return ":";
                case DelimiterEnum.CC:
                    return "::";
                case DelimiterEnum.CCC:
                    return ":::";
                case DelimiterEnum.E:
                    return "=";
                case DelimiterEnum.EC:
                    return "=:";
                case DelimiterEnum.ECC:
                    return "=::";
                case DelimiterEnum.EE:
                    return "==";
                case DelimiterEnum.CE:
                    return ":=";
            }
            return "";
        }
    }
}