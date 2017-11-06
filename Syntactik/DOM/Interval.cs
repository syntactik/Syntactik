using Syntactik.IO;

namespace Syntactik.DOM
{
    public class Interval
    {
        public Interval(CharLocation begin, CharLocation end)
        {
            Begin = begin;
            End = end;
        }

        static Interval()
        {
            Empty = new Interval(CharLocation.Empty, CharLocation.Empty);
        }

        public Interval(ICharStream input) 
        {
            Begin = new CharLocation(input);
            End = new CharLocation(input);
        }

        public Interval(CharLocation charLocation)
        {
            Begin = charLocation;
            End = charLocation;
        }

        public static Interval Empty;

        public readonly CharLocation Begin;
        public readonly CharLocation End;
    }
}
