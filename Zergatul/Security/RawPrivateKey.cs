using System;

namespace Zergatul.Security
{
    public class RawPrivateKey : PrivateKey
    {
        public byte[] Data { get; }

        public RawPrivateKey(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Data = data;
        }
    }
}