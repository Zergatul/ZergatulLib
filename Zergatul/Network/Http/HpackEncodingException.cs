using System;

namespace Zergatul.Network.Http
{
    public class HpackEncodingException : Exception
    {
        public HpackEncodingException()
            : base()
        { }

        public HpackEncodingException(string message)
            : base(message)
        { }

        public HpackEncodingException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}