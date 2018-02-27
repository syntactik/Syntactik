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
namespace Syntactik.Compiler
{
    static class ErrorCodes
    {
        public const string SCE0000 = "Fatal error - '{0}'";
        public const string SCE0001 = "Error reading from '{0}': '{1}'.";
        public const string SCE0002 = "File '{0}' was not found.";
        public const string SCE0003 = "Namespace prefix '{0}' is not defined.";
        public const string SCE0004 = "Alias '{0}' is not defined.";
        public const string SCE0005 = "Alias Definition '{0}' has circular reference.";
        public const string SCE0007 = "ParserError - '{0}'.";
        public const string SCE0008 = "Duplicate document name - '{0}'.";
        public const string SCE0009 = "Document '{0}' must have{1} one root element.";
        public const string SCE0010 = "Parameters can't be declared in documents.";
        public const string SCE0011 = "Duplicate argument name - '{0}'.";
        public const string SCE0012 = "Duplicate alias definition name - '{0}'.";
        public const string SCE0013 = "Argument '{0}' is missing.";
        public const string SCE0014 = "Value argument is expected.";
        public const string SCE0015 = "Block argument is expected.";
        public const string SCE0016 = "Invalid usage of the value alias.";
        public const string SCE0017 = "Can not use block alias as value.";
        public const string SCE0018 = "XML validation error - '{0}'.";
        public const string SCE0019 = "Array item is expected.";
        public const string SCE0020 = "Array item is not expected.";
        public const string SCE0021 = "Default parameter must be the only parameter.";
        public const string SCE0022 = "Argument can be defined only in an alias' block.";
        public const string SCE0023 = "Default block argument is missing.";
        public const string SCE0024 = "Unexpected argument.";
        public const string SCE0025 = "Unexpected default block argument.";
        public const string SCE0026 = "Default value argument is missing.";
        public const string SCE0027 = "Unexpected default value argument.";
        public const string SCE0029 = "Invalid escape sequence - '{0}'.";
        public const string SCE0030 = "Alias or Parameter expected after ':='.";
        public const string SCE0031 = "Invalid assignment '{0}'.";
        public const string SCE0032 = "{0}.";
        public const string SCE0033 = "Duplicate namespace definition name - '{0}'.";
        public const string SCE0100 = "Invalid XML element name.";
        public const string SCE0101 = "Invalid name.";
        public const string SCE0102 = "Invalid namespace prefix name.";
        public const string SCE0103 = "Unexpected start of object {{ or array [.";
        public const string SCE0104 = "Double quotes must be used to define strings in JSON.";


        
        public static string Format(string name, params object[] args)
        {
            return string.Format(GetString(name), args);
        }

        private static string GetString(string name)
        {
            return (string)typeof(ErrorCodes).GetField(name).GetValue(null);
        }
    }
}
