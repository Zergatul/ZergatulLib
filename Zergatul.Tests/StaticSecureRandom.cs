using System;
using System.Collections.Generic;
using System.Linq;
using Zergatul.Security;

namespace Zergatul.Tests
{
    public class StaticSecureRandom : SecureRandom
    {
        private byte[] _data;
        private int _index;

        public StaticSecureRandom(byte[] data)
        {
            this._data = data;
            this._index = 0;
        }

        public StaticSecureRandom(IEnumerable<byte> data)
            : this(data.ToArray())
        {

        }

        public override void GetNextBytes(byte[] bytes)
        {
            if (_data.Length - _index < bytes.Length)
                throw new InvalidOperationException();

            Array.Copy(_data, _index, bytes, 0, bytes.Length);
            _index += bytes.Length;
        }

        public override void GetNextBytes(byte[] bytes, int offset, int count)
        {
            if (_data.Length - _index < count)
                throw new InvalidOperationException();

            Array.Copy(_data, _index, bytes, offset, count);
            _index += count;
        }


        public override void SetSeed(byte[] seed)
        {
            throw new NotImplementedException();
        }
    }
}
