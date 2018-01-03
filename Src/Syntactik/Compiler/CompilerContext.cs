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
using Syntactik.DOM;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Syntactik.Compiler
{
    /// <summary>
    /// Represent a current state of compilation.
    /// </summary>
    public class CompilerContext
    {
        /// <summary>
        /// Root DOM object of the compilation session. Stores list of all compiled <see cref="Module">modules</see>.
        /// </summary>
        public CompileUnit CompileUnit { get; }

        /// <summary>
        /// Compiler parameters of the session.
        /// </summary>
        public CompilerParameters Parameters { get; }

        /// <summary>
        /// <see cref="Hashtable"/> used to store session variables.
        /// </summary>
        public Hashtable Properties { get; }
        /// <summary>
        /// <see cref="Dictionary{TKey,TValue}"/> used to store output compilation objects.
        /// </summary>
        public Dictionary<string, object> InMemoryOutputObjects { get; set; }
        /// <summary>
        /// List of compiler errors.
        /// </summary>
        public SortedSet<CompilerError> Errors { get; }
        /// <summary>
        /// Creates instance of <see cref="CompilerContext"/>.
        /// </summary>
        /// <param name="parameters">Compiler parameters of the session</param>
        /// <param name="compileUnit">Compiler unit of the session.</param>
        
        public CompilerContext(CompilerParameters parameters, CompileUnit compileUnit)
        {
            Parameters = parameters;
            CompileUnit = compileUnit;
            Properties = new Hashtable();
            Errors = new SortedSet<CompilerError>();            
        }
        /// <summary>
        /// Adds <see cref="CompilerError"/> to the <see cref="CompilerContext"/>.
        /// </summary>
        /// <param name="error"></param>
        public void AddError(CompilerError error)
        {
            if (Errors.Count >= 1000)
                throw new ApplicationException("Number of compiler errors exceeds 1000.");

            Errors.Add(error);
        }
    }
}
