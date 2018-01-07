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
namespace Syntactik.DOM.Mapped
{
    /// <summary>
    /// Represent a <see cref="DOM.Comment"/> node mapped to the source code.
    /// </summary>
    public class Comment : DOM.Comment, IMappedPair
    {
        /// <summary>
        /// Type of the comment:
        /// 1 - single-line comment,
        /// 2 - multi-line comment.
        /// </summary>
        public int CommentType { get; }
        /// <summary>
        /// <see cref="Interval"/> used to define name of the pair. Always empty for <see cref="Comment"/>.
        /// </summary>
        public Interval NameInterval { get; }

        /// <inheritdoc />
        public int NameQuotesType { get; }

        /// <inheritdoc />
        public Interval ValueInterval { get; }

        /// <inheritdoc />
        public int ValueQuotesType { get; }

        /// <summary>
        /// <see cref="Interval"/> used to define pair delimiter. Always empty for <see cref="Comment"/>.
        /// </summary>
        public Interval DelimiterInterval { get; }

        /// <summary>
        /// Type of the pair value. Always <see cref="Mapped.ValueType.None"/> for <see cref="Comment"/>.
        /// </summary>
        public ValueType ValueType => ValueType.None;

        /// <summary>
        /// True if pair has a literal value or pair value. Always <b>true</b>> for <see cref="Comment"/>.
        /// </summary>
        public bool IsValueNode => true;

        /// <summary>
        /// Indent of pair value. Always 0 for <see cref="Comment"/>.
        /// </summary>
        public int ValueIndent => 0;

        /// <summary>
        /// Creates a new instance of <see cref="Comment"/>.
        /// </summary>
        public Comment(string name = null, DelimiterEnum delimiter = DelimiterEnum.None, string value = null,
            Interval nameInterval = null, Interval valueInterval = null, Interval delimiterInterval = null,
            int nameQuotesType = 0, int valueQuotesType = 0, int commentType = 0) : base(name, delimiter, value)
        {
            ValueInterval = valueInterval;
            DelimiterInterval = delimiterInterval;
            NameInterval = nameInterval;
            NameQuotesType = nameQuotesType;
            ValueQuotesType = valueQuotesType;
            CommentType = commentType;
        }

    }
}