using System;

namespace Zergatul.FileFormat.Pdf
{
    public class ImplementationLimitException : Exception
    {
        public ImplementationLimitException(string message)
            : base(message)
        {

        }
    }
}