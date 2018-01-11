#region license
// Copyright © 2017 Maxim O. Trushin (trushin@gmail.com)
//
// This file is part of Syntactik.
// Syntactik is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Syntactik is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Syntactik.  If not, see <http://www.gnu.org/licenses/>.
#endregion
using System.Collections.Generic;
using System.Text;
using Syntactik.DOM;
using Syntactik.DOM.Mapped;
using Alias = Syntactik.DOM.Alias;
using Element = Syntactik.DOM.Element;
using Pair = Syntactik.DOM.Pair;

namespace Syntactik.Tests
{
    public class DomPrinter: SyntactikDepthFirstVisitor
    {
        private readonly Stack<bool> _valueNodeExpected = new Stack<bool>();
        private readonly StringBuilder _sb = new StringBuilder();
        private int _indent;

        public string Text => _sb.ToString();

        public DomPrinter()
        {
            _valueNodeExpected.Push(false);
        }


        public char QuoteTypeToChar(int quoteType)
        {
            switch (quoteType)
            {
                case  (int)QuotesEnum.Double:
                    return '"';
                case (int)QuotesEnum.Single:
                    return '\'';
                default:
                    return '`';

            }
        }

        public override void Visit(DOM.Module pair)
        {
            PrintNodeName(pair);
            PrintNodeStart(pair);
            base.Visit(pair);
            PrintNodeEnd(pair);
        }

        public override void Visit(DOM.Document pair)
        {
            PrintNodeName(pair);
            PrintNodeStart(pair);
            base.Visit(pair);
            PrintNodeEnd(pair);

        }

        public override void Visit(Element pair)
        {
            PrintNodeName(pair);
            PrintNodeStart(pair);
            base.Visit(pair);
            PrintNodeEnd(pair);
        }



        public override void Visit(DOM.Attribute pair)
        {
            PrintNodeName(pair);
            PrintNodeStart(pair);
            base.Visit(pair);
            PrintNodeEnd(pair);
        }

        public override void Visit(Alias pair)
        {
            PrintNodeName(pair);
            PrintNodeStart(pair);
            base.Visit(pair);
            PrintNodeEnd(pair);
        }
        public override void Visit(DOM.Argument pair)
        {
            PrintNodeName(pair);
            PrintNodeStart(pair);
            base.Visit(pair);
            PrintNodeEnd(pair);
        }
        public override void Visit(DOM.Parameter pair)
        {
            PrintNodeName(pair);
            PrintNodeStart(pair);
            base.Visit(pair);
            PrintNodeEnd(pair);
        }

        public override void Visit(DOM.AliasDefinition aliasDefinition)
        {
            PrintNodeName(aliasDefinition);
            PrintNodeStart(aliasDefinition);
            base.Visit(aliasDefinition);
            PrintNodeEnd(aliasDefinition);
        }

        public override void Visit(DOM.NamespaceDefinition pair)
        {
            PrintNodeName(pair);
            PrintNodeStart(pair);
            base.Visit(pair);
            PrintNodeEnd(pair);
        }

        public override void Visit(DOM.Scope pair)
        {
            PrintNodeName(pair);
            PrintNodeStart(pair);
            base.Visit(pair);
            PrintNodeEnd(pair);
        }

        private void PrintValue(Pair pair)
        {
            _sb.Append(pair.Value.Replace("\r\n", "\n").Replace("\n", "\\n").Replace("\t", "\\t"));
        }

        private void PrintNodeName(Pair pair)
        {
            if (_valueNodeExpected.Peek())
            {
                _sb.Append("\t");
                _sb.Append(pair.GetType().Name);
                _sb.Append(" ");
                _sb.Append(QuoteTypeToChar(((IMappedPair)pair).NameQuotesType));
                PrintNsPrefix(pair);
                _sb.Append(pair.Name);
                _sb.Append(QuoteTypeToChar(((IMappedPair)pair).NameQuotesType));
            }
            else
            {
                var mappedPair = pair as IMappedPair;
                _sb.Append(mappedPair?.NameInterval.Begin.Line.ToString().PadLeft(2, '0') ?? "00");
                _sb.Append(":");
                _sb.Append(mappedPair?.NameInterval.Begin.Column.ToString().PadLeft(2, '0') ?? "00");
                _sb.Append('\t', _indent);
                _sb.Append("\t");
                _sb.Append(pair.GetType().Name);
                _sb.Append(" ");
                _sb.Append(QuoteTypeToChar(((IMappedPair)pair).NameQuotesType));
                PrintNsPrefix(pair);
                _sb.Append(pair.Name);
                _sb.Append(QuoteTypeToChar(((IMappedPair)pair).NameQuotesType));
            }
        }

        private void PrintNsPrefix(Pair pair)
        {
            if (!string.IsNullOrEmpty((pair as INsNode)?.NsPrefix))
            {
                _sb.Append(((INsNode)pair).NsPrefix);
                _sb.Append(".");
            }
        }

        private void PrintNodeEnd(Pair pair)
        {
            _valueNodeExpected.Pop();

            if (pair.Value != null)
            {
                _sb.AppendLine();
            }
            else if (pair.PairValue != null)
            {
            }
            else
            {
                _indent--;
            }
        }

        private void PrintNodeStart(Pair pair)
        {
            if (pair.Value != null)
            {
                _sb.Append(Pair.AssignmentToString(pair.Assignment));
                _sb.Append(" ");
                _sb.Append(QuoteTypeToChar(((IMappedPair)pair).ValueQuotesType));
                PrintValue(pair);
                _sb.Append(QuoteTypeToChar(((IMappedPair)pair).ValueQuotesType));
            }
            else if (pair.PairValue != null)
            {
                _sb.Append(":= ");
                _valueNodeExpected.Push(true);
                Visit(pair.PairValue);
                _valueNodeExpected.Pop();
            }
            else
            {
                _sb.AppendLine(Pair.AssignmentToString(pair.Assignment));
                _indent++;
            }
            _valueNodeExpected.Push(false);
        }
    }
}