using System;

namespace Zergatul.IO.Compression
{
    public class DeflateDataFormatException : Exception
    {
        public DeflateDataFormatException()
            : base()
        {

        }

        public DeflateDataFormatException(Exception innerException)
            : base("", innerException)
        {

        }
    }
}