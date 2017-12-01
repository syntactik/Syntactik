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

using NUnit.Framework;
using static Syntactik.Compiler.Tests.TestUtils;

namespace Syntactik.Compiler.Tests
{
    [TestFixture, Category("Compiler")]
    public class CompilerTestFixture
    {

        [Test, CompilerErrorRecorded]
        public void AliasDefWithDefaultAndBlockParameter()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void AliasDefWithDefaultAndValueParameter()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void AliasHasCircularReference()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void AliasParameterWithDefaultValue()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void ModuleDocument()
        {
            PerformCompilerTest();
        }
        [Test, RecordedTest]
        public void ModuleDocumentWithComment()
        {
            PerformCompilerTest(true);
        }
        [Test, CompilerErrorRecorded]
        public void ModuleDocumentDuplicate()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void ModulesWithNsDocumentAndNsAlias()
        {
            PerformCompilerTest();
        }
        
        [Test, RecordedTest]
        public void MultipleFilesWithSchemaCompilation()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void NestedAliases()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void NestedAliasesWithParameters()
        {
            PerformCompilerTest();
        }
        
        [Test, CompilerErrorRecorded]
        public void MissingAlias()
        {
            PerformCompilerTest();
        }
        
        [Test, RecordedTest]
        public void MixedContentInXml()
        {
            PerformCompilerTest();
        }


        [Test, CompilerErrorRecorded]
        public void NamespaceIsNotDefined()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void NamespaceScope()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void NamespaceDefAfterComment()
        {
            PerformCompilerTest();
        }


        [Test, CompilerErrorRecorded]
        public void DuplicateDocumentName()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void EmptyModule()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void EmptyParameters()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void DuplicateAliasDefinition()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void DuplicateNamespaceDefinition()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void AliasInsideDqs()
        {
            PerformCompilerTest();
        }


        [Test, RecordedTest]
        public void AliasWithArguments()
        {
            PerformCompilerTest();
        }


        [Test, RecordedTest]
        public void AliasParameterWithDefaultBlock()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void ExtraRootElementInDocument()
        {
            PerformCompilerTest();
        }
        
        [Test, RecordedTest]
        public void FoldedOpenString()
        {
            PerformCompilerTest();
        }
        [Test, RecordedTest]
        public void FoldedDQS()
        {
            PerformCompilerTest();
        }
        [Test, RecordedTest]
        public void FreeOpenString()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void MultilineSQS()
        {
            PerformCompilerTest();
        }
        [Test, CompilerErrorRecorded]
        public void InvalidAppendChild()
        {
            PerformCompilerTest();
        }
        [Test, CompilerErrorRecorded]
        public void InvalidArgumentValue()
        {
            PerformCompilerTest();
        }
        [Test, CompilerErrorRecorded]
        public void InvalidInterpolation()
        {
            PerformCompilerTest();
        }
        [Test, CompilerErrorRecorded]
        public void InvalidNamespaceDefinition()
        {
            PerformCompilerTest();
        }
        [Test, CompilerErrorRecorded]
        public void IncorrectName()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void InvalidPairValue()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void JsonArray()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void JsonExplicitArray()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void JsonExplicitArray2()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void JsonArrayInAlias()
        {
            PerformCompilerTest();
        }
        
        [Test, CompilerErrorRecorded]
        public void JsonArrayInXmlDocument()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void JsonArrayItemInObject()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void JsonArrayWithValues()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void JsonArrayWithValuesInAlias()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void JsonArrayWithValuesInParameters()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void JsonEmptyArrayAndObject()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void JsonLiteralsInSqs()
        {
            PerformCompilerTest();
        }
        [Test, RecordedTest]
        public void JsonStringAsDocument()
        {
            PerformCompilerTest();
        }
        [Test, CompilerErrorRecorded]
        public void JsonPropertyInArray()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void LineComments()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void LiteralAliasInArray()
        {
            PerformCompilerTest();
        }
        [Test, RecordedTest]
        public void LiteralChoiceDelimiter()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void ParameterInDocument()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void SchemaValidation()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void SchemaValidationConstraints()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void SchemaValidationXsdMissing()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void Scope1()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void SchemaValidationTypeAttribute()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void DoubleQuoteEscape()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void StringConcatenation()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void AliasWithIncorrectBlock()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void AliasWithIncorrectType()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void AliasWithMissedArgument()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void AliasWithMissedDefaultBlockParameter()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void AliasWithMissedDefaultValueParameter()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void AliasWithUnexpectedDefaultBlockParameter()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void AliasWithUnexpectedDefaultValueParameter()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void AliasWithUnexpectedArgument()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void ArgumentInTheElementBlock()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void AliasWithAttributes()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void AliasWithDefaultBlockParameter()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void AliasWithDefaultValueParameter()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void ArrayExplicitInXml()
        {
            PerformCompilerTest();
        }


        [Test, RecordedTest]
        public void ArgumentWithObjectValue()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void AliasWithDuplicateArguments()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void AliasWithIncorrectArgumentType()
        {
            PerformCompilerTest();
        }
        
        [Test, RecordedTest]
        public void ChoiceDelimiter()
        {
            PerformCompilerTest();
        }
        [Test, RecordedTest]
        public void ChoiceDelimiterOptionalArgument()
        {
            PerformCompilerTest();
        }
        [Test, RecordedTest]
        public void DotEscapedInId()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void TwoModulesWithDocumentAndAlias()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void UnclosedQuotedString()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void UndeclaredNamespace()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void UnexpectedAliasWithArrayItem()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void UnexpectedChoiceArgument()
        {
            PerformCompilerTest();
        }

        [Test, CompilerErrorRecorded]
        public void UnresolvedAliasInsideDqs()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void ValueAliasStartsBlock()
        {
            PerformCompilerTest();
        }
        [Test, CompilerErrorRecorded]
        public void ValueAliasWithMissedArgument()
        {
            PerformCompilerTest();
        }
        [Test, RecordedTest]
        public void XmlDeclarationAndProcessing()
        {
            PerformCompilerTest(true);
        }
    }
}
