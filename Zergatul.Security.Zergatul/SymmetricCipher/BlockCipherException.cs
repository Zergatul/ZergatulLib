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
    }
}