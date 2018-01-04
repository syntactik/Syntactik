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
namespace Syntactik.DOM
{
    /// <summary>
    /// Base interface for <see href="https://en.wikipedia.org/wiki/Visitor_pattern#Classic_visitor">visitor</see>.
    /// </summary>
    public interface IDomVisitor
    {
        /// <summary>
        /// Visit method for <see cref="Alias"/>.
        /// </summary>
        /// <param name="alias">Calling object.</param>
        void Visit(Alias alias);
        /// <summary>
        /// Visit method for <see cref="AliasDefinition"/>.
        /// </summary>
        /// <param name="aliasDefinition">Calling object.</param>
        void Visit(AliasDefinition aliasDefinition);
        /// <summary>
        /// Visit method for <see cref="Argument"/>.
        /// </summary>
        /// <param name="argument">Calling object.</param>
        void Visit(Argument argument);
        /// <summary>
        /// Visit method for <see cref="Attribute"/>.
        /// </summary>
        /// <param name="attribute">Calling object.</param>
        void Visit(Attribute attribute);
        /// <summary>
        /// Visit method for <see cref="CompileUnit"/>.
        /// </summary>
        /// <param name="compileUnit">Calling object.</param>
        void Visit(CompileUnit compileUnit);
        /// <summary>
        /// Visit method for <see cref="Document"/>.
        /// </summary>
        /// <param name="document">Calling object.</param>
        void Visit(Document document);
        /// <summary>
        /// Visit method for <see cref="Element"/>.
        /// </summary>
        /// <param name="element">Calling object.</param>
        void Visit(Element element);
        /// <summary>
        /// Visit method for <see cref="Module"/>.
        /// </summary>
        /// <param name="module">Calling object.</param>
        void Visit(Module module);
        /// <summary>
        /// Visit method for <see cref="NamespaceDefinition"/>.
        /// </summary>
        /// <param name="namespaceDefinition">Calling object.</param>
        void Visit(NamespaceDefinition namespaceDefinition);
        /// <summary>
        /// Visit method for <see cref="Scope"/>.
        /// </summary>
        /// <param name="scope">Calling object.</param>
        void Visit(Scope scope);
        /// <summary>
        /// Visit method for <see cref="Parameter"/>.
        /// </summary>
        /// <param name="parameter">Calling object.</param>
        void Visit(Parameter parameter);
        /// <summary>
        /// Visit method for <see cref="Comment"/>.
        /// </summary>
        /// <param name="comment">Calling object.</param>
        void Visit(Comment comment);
    }
}