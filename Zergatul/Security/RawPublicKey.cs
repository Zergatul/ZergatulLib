using System;

namespace Zergatul.Security
{
    public class RawPublicKey : PublicKey
    {
        public byte[] Data { get; }

        public RawPublicKey(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Data = data;
        }
    }
}