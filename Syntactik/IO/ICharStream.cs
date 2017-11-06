namespace Syntactik.IO
{
    public interface ICharStream
    {
        void Consume();
        void Reset();
        int La(int i);
        int Next { get; }
        int Index { get; }
        int Line { get; }
        int Column { get; }
        int Length { get; }
        
    }
}
