using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net
{
    internal class ByteArray
    {
        private List<byte[]> _parts;

        public int Length => _parts.Sum(p => p.Length);

        public byte this[int index]
        {
            get
            {
                int partIndex = 0;
                while (partIndex < _parts.Count && index >= _parts[partIndex].Length)
                {
                    index -= _parts[partIndex].Length;
                    partIndex++;
                }
                if (partIndex >= _parts.Count)
                    throw new IndexOutOfRangeException();
                return _parts[partIndex][index];
            }
        }

        public ByteArray()
        {
            this._parts = new List<byte[]>();
        }

        public ByteArray(byte[] data)
        {
            this._parts = new List<byte[]> { data };
        }

        public void ClearMemory()
        {
            for (int i = 0; i < _parts.Count; i++)
                for (int j = 0; j < _parts[i].Length; j++)
                    _parts[i][j] = 0;
        }

        public ByteArray Truncate(int length)
        {
            if (length == this.Length)
                return this;

            var bytes = new byte[length];
            int position = 0;
            int partIndex = 0;
            while (position < length)
            {
                Array.Copy(_parts[partIndex], 0, bytes, position, System.Math.Min(_parts[partIndex].Length, length - position));
                position += _parts[partIndex].Length;
                partIndex++;
            }
            return new ByteArray(bytes);
        }

        public byte[] ToArray()
        {
            var bytes = new byte[Length];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = this[i];
            return bytes;
        }

        public static ByteArray operator+(ByteArray left, ByteArray right)
        {
            var result = new ByteArray();
            result._parts.AddRange(left._parts);
            result._parts.AddRange(right._parts);
            return result;
        }

        public static ByteArray operator +(byte[] left, ByteArray right)
        {
            var result = new ByteArray();
            result._parts.Add(left);
            result._parts.AddRange(right._parts);
            return result;
        }

        public static ByteArray operator +(ByteArray left, byte[] right)
        {
            var result = new ByteArray();
            result._parts.AddRange(left._parts);
            result._parts.Add(right);
            return result;
        }

        public static ByteArray operator^(ByteArray left, byte right)
        {
            var bytes = left.ToArray();
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)(left[i] ^ right);
            return new ByteArray(bytes);
        }
    }
}