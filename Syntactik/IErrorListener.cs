using Syntactik.DOM;

namespace Syntactik
{
    public interface IErrorListener
    {
        void SyntaxError(int code, Interval interval, params object[] args);
    }
}
