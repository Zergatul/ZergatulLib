using System;

namespace Zergatul.Network.Http
{
    public class HpackDecodingException : Exception
    {
        public HpackDecodingException()
            : base()
        { }

        public HpackDecodingException(string message)
            : base(message)
        { }

        public HpackDecodingException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}