using System.Collections.Generic;
using Zergatul.Cryptography;

namespace Zergatul.Tests
{
    public class StaticRandom : AbstractRandom, ISecureRandom
    {
        private IEnumerator<byte> _enumerator;

        public StaticRandom(IEnumerable<byte> data)
        {
            this._enumerator = data.GetEnumerator();
            this._enumerator.MoveNext();
        }

        public override void GetBytes(byte[] data, int offset, int count)
        {
            for (int i = offset; i < offset + count; i++)
            {
                data[i] = _enumerator.Current;
                _enumerator.MoveNext();
            }
        }
    }
}