using Syntactik.IO;

namespace Syntactik.DOM
{
    public class CharLocation
    {
        public CharLocation(int line, int column, int index)
        {
            Index = index;
            Line = line;
            Column = column;
        }

        public CharLocation(ICharStream input)
        {
            Index = input.Index;
            Line = input.Line;
            Column = input.Column;
        }

        static CharLocation()
        {
            Empty = new CharLocation(0, 0, -1);
        }

        public static CharLocation Empty;

        public int Index;
        public int Line;
        public int Column;

        protected int CompareTo(CharLocation other)
        {
            int num = Line.CompareTo(other.Line);
            if (num != 0)
            {
                return num;
            }
            return Column.CompareTo(other.Column);
        }

        public override string ToString()
        {
            return $"({Line},{Column},{Index})";
        }
    }
}
