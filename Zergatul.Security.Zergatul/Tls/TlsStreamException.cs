using System;

namespace Zergatul.Security.Zergatul.Tls
{
    public abstract class TlsStreamException : Exception
    {
        public TlsStreamException(string message)
            : base(message)
        {

        }
    }
}