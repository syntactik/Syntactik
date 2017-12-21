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
using System;

namespace Syntactik.DOM
{
    /// <summary>
    /// Root DOM object of the compilation session.
    /// </summary>
    [Serializable]
    public class CompileUnit : Pair
    {
        // Fields
        private PairCollection<Module> _modules;


        /// <summary>
        /// List of compiled modules.
        /// </summary>
        public virtual PairCollection<Module> Modules
        {
            get { return _modules ?? (_modules = new PairCollection<Module>(this)); }
            set
            {
                if (value == _modules) return;
                value?.InitializeParent(this);
                _modules = value;
            }
        }

        /// <summary>
        /// Method Accept is part of implementation of Visitor pattern.
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnCompileUnit(this);
        }

        /// <summary>
        /// Adds a Module as a child of CompilationUnit.
        /// </summary>
        /// <param name="child"></param>
        public override void AppendChild(Pair child)
        {
            var item = child as Module;
            if (item != null)
            {
                Modules.Add(item);
            }
            else
            {
                base.AppendChild(child);
            }
        }
    }
}
