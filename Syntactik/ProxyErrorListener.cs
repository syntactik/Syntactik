using System.Collections.Generic;
using Syntactik.DOM;

namespace Syntactik
{
    class ProxyErrorListener: IErrorListener
    {
        private readonly IEnumerable<IErrorListener> _listeners;

        public ProxyErrorListener(IEnumerable<IErrorListener> listeners)
        {
            _listeners = listeners;
        }

        public void SyntaxError(int code, Interval interval, params object[] args)
        {
            if (_listeners == null) return;
            foreach (var errorListener in _listeners)
            {
                errorListener.SyntaxError(code, interval, args);
            }
        }
    }
}
