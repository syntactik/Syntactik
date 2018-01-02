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
using Pair = Syntactik.DOM.Pair;

namespace Syntactik
{
    public enum ParserStateEnum
    {
        Indent = 1, //New line started. Indent is checked to calculate the current pair.
        PairDelimiter = 2,
        Name = 4,
        Delimiter = 8,
        Value = 16,
        IndentMLS = 32 //Indent for multi-line string
    }

    public struct LineParsingState
    {
        public int Indent;
        public ParserStateEnum State;
        public bool ChainingStarted;
        public Pair CurrentPair; //current pair in the current block.
        public bool Inline; //If true then at least one pair is defined in this line already

        public void Reset()
        {
            CurrentPair = null;
            State = ParserStateEnum.Indent;
            ChainingStarted = false;
            Inline = false;
        }
    }
}