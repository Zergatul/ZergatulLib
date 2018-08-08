using System;

namespace Zergatul.Network.Http
{
    public class ChunkedEncodingException : Exception
    {
        public ChunkedEncodingException(string message)
            : base(message)
        {

        }
    }
}