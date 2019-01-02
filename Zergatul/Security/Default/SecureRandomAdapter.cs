
using System;
using Zergatul.Cryptography;

namespace Zergatul.Security.Default
{
    class SecureRandomAdapter : ISecureRandom
    {
        private SecureRandom _random;

        public SecureRandomAdapter(SecureRandom random)
        {
            if (random == null)
                throw new ArgumentNullException(nameof(random));

            _random = random;
        }

        public void GetBytes(byte[] data)
        {
            _random.GetNextBytes(data);
        }

        public void GetBytes(byte[] data, int offset, int count)
        {
            byte[] buffer = new byte[count];
            _random.GetNextBytes(buffer);
            Array.Copy(buffer, 0, data, offset, count);
        }

        public ulong GetUInt64()
        {
            throw new NotImplementedException();
        }
    }
}