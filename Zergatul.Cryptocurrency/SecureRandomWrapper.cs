using System;
using Zergatul.Cryptography;
using Zergatul.Security;

namespace Zergatul.Cryptocurrency
{
    class SecureRandomWrapper : ISecureRandom
    {
        private SecureRandom _random;

        public SecureRandomWrapper()
        {
            _random = Provider.GetSecureRandomInstance(SecureRandoms.Default);
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