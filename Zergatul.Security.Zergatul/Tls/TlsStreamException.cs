using System;

namespace Zergatul.Security.Zergatul.Tls
{
    public abstract class TlsStreamException : Exception
    {
        public int ErrorCode { get; }
        public override string Message { get; }

        public TlsStreamException(string message)
            : base(message)
        {

        }

        internal TlsStreamException(ErrorCode error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            ErrorCode = error.Code;
            Message = error.Message;
        }
    }
}