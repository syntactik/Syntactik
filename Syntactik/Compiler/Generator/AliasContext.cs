﻿#region license
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

namespace Syntactik.Compiler.Generator
{
    public class AliasContext
    {
        public Alias Alias { get; set; }
        public AliasDefinition AliasDefinition { get; set; }
        public NsInfo AliasNsInfo { get; set; }

    }
}
