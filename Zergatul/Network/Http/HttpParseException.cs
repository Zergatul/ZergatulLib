using System;

namespace Zergatul.Network.Http
{
    public class HttpParseException : Exception
    {
        public HttpParseException()
            : base()
        {

        }

        public HttpParseException(string message)
            : base(message)
        {

        }
    }
}