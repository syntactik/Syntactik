using System.Collections.Generic;
using Syntactik.DOM;

namespace Syntactik.Tests
{
    public class ErrorListener: IErrorListener
    {
        public List<string> Errors { get; } = new List<string>();

        public void SyntaxError(int code, Interval interval, params object[] args)
        {
            Errors.Add(ErrorCodes.Format(code, args) + $" ({interval.Begin.Line}:{interval.Begin.Column})-({interval.End.Line}:{interval.End.Column})");
        }
    }
}
