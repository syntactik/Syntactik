namespace Syntactik.IO
{
    public interface ITextSource
    {
        string GetText(int begin, int end);
        char GetChar(int index);
    }
}
