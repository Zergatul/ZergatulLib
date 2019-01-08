using System;

namespace Zergatul.IO.Compression
{
    public class DeflateStreamException : Exception
    {
        public DeflateStreamException()
            : base()
        {

        }

        public DeflateStreamException(Exception innerException)
            : base("", innerException)
        {

        }
    }
}