using System;

namespace Zergatul.Security.OpenSsl
{
    public class OpenSslException : Exception
    {
        public ulong Code { get; private set; }
        public override string Message { get; }

        public OpenSslException()
        {
            this.Code = Native.ERR_get_error();
            if (this.Code != 0)
            {
                this.Message = Native.ERR_error_string(this.Code);
            }
        }

        public OpenSslException(string message)
            : base(message)
        {

        }
    }
}