using System;

namespace Zergatul.Security.Tests
{
    class StaticSecureRandom : SecureRandom
    {
        private readonly byte[] _data;
        private int _index;

        public StaticSecureRandom(byte[] data)
        {
            _data = data;
            _index = 0;
        }

        public override void GetNextBytes(byte[] bytes)
        {
            if (_data.Length - _index >= bytes.Length)
            {
                Array.Copy(_data, _index, bytes, 0, bytes.Length);
                _index += bytes.Length;
            }
            else
                throw new InvalidOperationException();
        }

        public override void SetSeed(byte[] seed)
        {
            throw new NotImplementedException();
        }
    }
}