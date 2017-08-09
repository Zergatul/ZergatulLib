using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Tests
{
    public class StaticRandom : AbstractRandom, ISecureRandom
    {
        private byte[] _data;
        private int _index;

        public StaticRandom(byte[] data)
        {
            this._data = data;
            this._index = 0;
        }

        public override void GetBytes(byte[] data, int offset, int count)
        {
            Array.Copy(_data, _index, data, offset, count);
            _index += count;
        }
    }
}