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
using Syntactik.DOM.Mapped;
using System;
using System.Xml.Schema;
using Syntactik.Compiler.Steps;
using Syntactik.DOM;
using Alias = Syntactik.DOM.Mapped.Alias;
using AliasDefinition = Syntactik.DOM.Mapped.AliasDefinition;
using Argument = Syntactik.DOM.Mapped.Argument;
using Document = Syntactik.DOM.Mapped.Document;
using LexicalInfo = Syntactik.DOM.LexicalInfo;
using NamespaceDefinition = Syntactik.DOM.Mapped.NamespaceDefinition;
using Parameter = Syntactik.DOM.Mapped.Parameter;


namespace Syntactik.Compiler
{
    /// <summary>
    /// This class factory creates instance of class <see cref="CompilerError"/>. 
    /// </summary>
    public static class CompilerErrorFactory
    {
        /// <summary>
        /// Creates input/output related error.
        /// </summary>
        /// <param name="inputName">Unique name of the input, a file name, for example.</param>
        /// <param name="ex">Inner exception.</param>
        /// <returns>An instance of <see cref="CompilerError"/>.</returns>
        public static CompilerError InputError(string inputName, Exception ex)
        {
            return InputError(new LexicalInfo(inputName), ex);
        }

        internal static CompilerError InputError(LexicalInfo lexicalInfo, Exception error)
        {
            return Instantiate("MCE0001", lexicalInfo, error, lexicalInfo.FileName, error.Message);
        }

        /// <summary>
        /// Creates a fatal error.
        /// </summary>
        /// <param name="ex">Inner exception.</param>
        /// <returns>An instance of <see cref="CompilerError"/>.</returns>
        public static CompilerError FatalError(Exception ex)
        {
            return new CompilerError("MCE0000", ex, ex.Message);
        }

        /// <summary>
        /// Creates a fatal error.
        /// </summary>
        /// <param name="ex">Inner exception.</param>
        /// <param name="message">Description of the error.</param>
        /// <returns>An instance of <see cref="CompilerError"/>.</returns>
        public static CompilerError FatalError(Exception ex, string message)
        {
            return new CompilerError("MCE0000", ex, message);
        }

        private static CompilerError Instantiate(string code, LexicalInfo location, Exception ex, params object[] args)
        {
            return new CompilerError(code, location, ex, args);
        }

        private static CompilerError Instantiate(string code, LexicalInfo location, bool isParserError, params object[] args)
        {
            return new CompilerError(code, location, isParserError, Array.ConvertAll(args, DisplayStringFor));
        }

        internal static object DisplayStringFor(object o)
        {
            if (o == null) return "";
            return  o.ToString();
        }

        internal static Exception FileNotFound(string fileName)
        {
            return Instantiate("MCE0002", new LexicalInfo(fileName), false, fileName);
        }

        internal static CompilerError NsPrefixNotDefined(Interval interval, string prefix, string fileName)
        {
            return Instantiate("MCE0003", new LexicalInfo(fileName, interval.Begin.Line, interval.Begin.Column, interval.Begin.Index), false, prefix);
        }

        internal static CompilerError AliasIsNotDefined(Alias alias, string fileName)
        {
            return Instantiate("MCE0004", new LexicalInfo(fileName, alias.NameInterval.Begin.Line, alias.NameInterval.Begin.Column, alias.NameInterval.Begin.Index), false, alias.Name);
        }

        internal static CompilerError AliasDefHasCircularReference(NsInfo aliasDefNsInfo)
        {
            var aliasDef = (AliasDefinition) aliasDefNsInfo.ModuleMember;
            return Instantiate("MCE0005", new LexicalInfo(aliasDef.Module.FileName,
                aliasDef.NameInterval.Begin.Line, aliasDef.NameInterval.Begin.Column, aliasDef.NameInterval.Begin.Index), false, aliasDef.Name);
        }

        internal static CompilerError ParserError(string message, string fileName, int line, int column)
        {
            return Instantiate("MCE0007", new LexicalInfo(fileName, line, column, 0), true, message);
        }

        internal static CompilerError DuplicateDocumentName(Document document, string fileName)
        {
            return Instantiate("MCE0008", new LexicalInfo(fileName, document.NameInterval.Begin.Line, document.NameInterval.Begin.Column, document.NameInterval.Begin.Index), false, document.Name);
        }
        internal static CompilerError DuplicateNsDefName(NamespaceDefinition nsDefinition, string fileName)
        {
            return Instantiate("MCE0033", new LexicalInfo(fileName, nsDefinition.NameInterval.Begin.Line, nsDefinition.NameInterval.Begin.Column, nsDefinition.NameInterval.Begin.Index), false, nsDefinition.Name);
        }

        internal static CompilerError DocumentMustHaveOneRootElement(Document document, string fileName, string only)
        {
            return Instantiate("MCE0009", new LexicalInfo(fileName, document.NameInterval.Begin.Line, document.NameInterval.Begin.Column, document.NameInterval.Begin.Index), false, document.Name, only);
        }
        internal static CompilerError ParametersCantBeDeclaredInDocuments(Parameter node, string fileName)
        {
            return Instantiate("MCE0010", new LexicalInfo(fileName, node.NameInterval.Begin.Line, node.NameInterval.Begin.Column, node.NameInterval.Begin.Index), false);
        }

        internal static CompilerError DuplicateArgumentName(Argument argument, string fileName)
        {
            return Instantiate("MCE0011", new LexicalInfo(fileName, argument.NameInterval.Begin.Line, argument.NameInterval.Begin.Column, argument.NameInterval.Begin.Index), false, argument.Name);
        }

        internal static CompilerError DuplicateAliasDefName(AliasDefinition aliasDef, string fileName)
        {
            return Instantiate("MCE0012", new LexicalInfo(fileName, aliasDef.NameInterval.Begin.Line, aliasDef.NameInterval.Begin.Column, aliasDef.NameInterval.Begin.Index), false, aliasDef.Name);
        }

        internal static CompilerError ArgumentIsMissing(Alias alias, string argumentName, string fileName)
        {
            return Instantiate("MCE0013", new LexicalInfo(fileName, alias.NameInterval.Begin.Line, alias.NameInterval.Begin.Column, alias.NameInterval.Begin.Index), false, argumentName);
        }

        internal static CompilerError ValueArgumentIsExpected(Argument argument, string fileName)
        {
            return Instantiate("MCE0014", new LexicalInfo(fileName, argument.NameInterval.Begin.Line, argument.NameInterval.Begin.Column, argument.NameInterval.Begin.Index), false);
        }

        internal static CompilerError BlockArgumentIsExpected(Argument argument, string fileName)
        {
            return Instantiate("MCE0015", new LexicalInfo(fileName, argument.NameInterval.Begin.Line, argument.NameInterval.Begin.Column, argument.NameInterval.Begin.Index), false);
        }

        internal static CompilerError InvalidUsageOfValueAlias(Alias alias, string fileName)
        {
            return Instantiate("MCE0016", new LexicalInfo(fileName, alias.NameInterval.Begin.Line, alias.NameInterval.Begin.Column, alias.NameInterval.Begin.Index), false);
        }

        internal static CompilerError CantUseBlockAliasAsValue(Alias alias, string fileName)
        {
            return Instantiate("MCE0017", new LexicalInfo(fileName, alias.NameInterval.Begin.Line, alias.NameInterval.Begin.Column, alias.NameInterval.Begin.Index), false);
        }

        internal static CompilerError XmlSchemaValidationError(XmlSchemaValidationException ex)
        {
            return Instantiate("MCE0018", new LexicalInfo(ex.SourceUri, ex.LineNumber, ex.LinePosition, 0), false, ex.Message);
        }

        internal static CompilerError XmlSchemaValidationError(XmlSchemaException ex, LexicalInfo location)
        {
            return Instantiate("MCE0018", new LexicalInfo(location.FileName, location.Line, location.Column, 0), false, ex.Message);
        }

        internal static CompilerError ArrayItemIsExpected(IMappedPair node, string fileName)
        {
            return Instantiate("MCE0019", new LexicalInfo(fileName, node.NameInterval.Begin.Line, node.NameInterval.Begin.Column, node.NameInterval.Begin.Index), false);
        }

        internal static CompilerError PropertyIsExpected(IMappedPair node, string fileName)
        {
            return Instantiate("MCE0020", new LexicalInfo(fileName, node.NameInterval.Begin.Line, node.NameInterval.Begin.Column, node.NameInterval.Begin.Index), false);
        }

        internal static CompilerError DefaultParameterMustBeOnly(Parameter node, string fileName)
        {
            return Instantiate("MCE0021", new LexicalInfo(fileName, node.NameInterval.Begin.Line, node.NameInterval.Begin.Column, node.NameInterval.Begin.Index), false);
        }

        internal static CompilerError ArgumentMustBeDefinedInAlias(Argument node, string fileName)
        {
            return Instantiate("MCE0022", new LexicalInfo(fileName, node.NameInterval.Begin.Line, node.NameInterval.Begin.Column, node.NameInterval.Begin.Index), false);
        }

        internal static CompilerError DefaultBlockArgumentIsMissing(Alias alias, string fileName)
        {
            return Instantiate("MCE0023", new LexicalInfo(fileName, alias.NameInterval.Begin.Line, alias.NameInterval.Begin.Column, alias.NameInterval.Begin.Index), false);
        }

        internal static CompilerError UnexpectedArgument(Argument argument, string fileName)
        {
            return Instantiate("MCE0024", new LexicalInfo(fileName, argument.NameInterval.Begin.Line, argument.NameInterval.Begin.Column, argument.NameInterval.Begin.Index), false);
        }

        internal static CompilerError UnexpectedDefaultBlockArgument(IMappedPair entity, string fileName)
        {
            return Instantiate("MCE0025", new LexicalInfo(fileName, entity.NameInterval.Begin.Line, entity.NameInterval.Begin.Column, entity.NameInterval.Begin.Index), false);
        }

        internal static CompilerError DefaultValueArgumentIsMissing(Alias alias, string fileName)
        {
            return Instantiate("MCE0026", new LexicalInfo(fileName, alias.NameInterval.Begin.Line, alias.NameInterval.Begin.Column, alias.NameInterval.Begin.Index), false);
        }

        internal static CompilerError UnexpectedDefaultValueArgument(Alias alias, string fileName)
        {
            return Instantiate("MCE0027", new LexicalInfo(fileName, alias.NameInterval.Begin.Line, alias.NameInterval.Begin.Column, alias.NameInterval.Begin.Index), false);
        }

        internal static CompilerError InvalidEscapeSequence(string sequence, LexicalInfo location)
        {
            return Instantiate("MCE0029", new LexicalInfo(location.FileName, location.Line, location.Column, 0), false, sequence);
        }

        internal static CompilerError AliasOrParameterExpected(Interval nameInterval, string fileName)
        {
            return Instantiate("MCE0030", new LexicalInfo(fileName, nameInterval.Begin.Line, nameInterval.Begin.Column, nameInterval.Begin.Index), false);
        }
        internal static CompilerError InvalidAssignment(Interval nameInterval, string fileName, string assignment)
        {
            return Instantiate("MCE0031", new LexicalInfo(fileName, nameInterval.Begin.Line, nameInterval.Begin.Column, nameInterval.Begin.Index), false, assignment);
        }
        internal static CompilerError CantAppendChild(Interval nameInterval, string fileName, string message)
        {
            return Instantiate("MCE0032", new LexicalInfo(fileName, nameInterval.Begin.Line, nameInterval.Begin.Column, nameInterval.Begin.Index), true, message);
        }
        internal static CompilerError InvalidXmlElementName(Interval nameInterval, string fileName)
        {
            return Instantiate("MCE0100", new LexicalInfo(fileName, nameInterval.Begin.Line, nameInterval.Begin.Column, nameInterval.Begin.Index), true);
        }

        internal static CompilerError InvalidName(Interval nameInterval, string fileName)
        {
            return Instantiate("MCE0101", new LexicalInfo(fileName, nameInterval.Begin.Line, nameInterval.Begin.Column, nameInterval.Begin.Index), true);
        }
        internal static CompilerError InvalidNsName(Interval nameInterval, string fileName)
        {
            return Instantiate("MCE0102", new LexicalInfo(fileName, nameInterval.Begin.Line, nameInterval.Begin.Column, nameInterval.Begin.Index), true);
        }

    }
}