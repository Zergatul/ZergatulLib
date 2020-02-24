using System;

namespace Zergatul.Security.Zergatul.SymmetricCipher
{
    public class BlockCipherException : Exception
    {
        public int Code { get; }

        public BlockCipherException(string message, int code)
            : base(message)
        {
            Code = code;
        }

        internal BlockCipherException(ErrorCode error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            throw new NotImplementedException();
            //ErrorCode = error.Code;
            //Message = error.Message;
        }
    }
}