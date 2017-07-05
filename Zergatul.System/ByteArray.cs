using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul
{
    public class ByteArray
    {
        internal List<byte[]> Parts;

        public int Length { get; private set; }

        public byte this[int index]
        {
            get
            {
                int partIndex = 0;
                while (partIndex < Parts.Count && index >= Parts[partIndex].Length)
                {
                    index -= Parts[partIndex].Length;
                    partIndex++;
                }
                if (partIndex >= Parts.Count)
                    throw new IndexOutOfRangeException();
                return Parts[partIndex][index];
            }
        }

        public ByteArray()
        {
            this.Parts = new List<byte[]>();
            this.Length = 0;
        }

        public ByteArray(byte value)
            : this(new byte[] { value })
        {
        }

        public ByteArray(ushort value)
            : this(new byte[] { (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF) })
        {
        }

        public ByteArray(ulong value)
            : this(new byte[]
            {
                (byte)((value >> 56) & 0xFF),
                (byte)((value >> 48) & 0xFF),
                (byte)((value >> 40) & 0xFF),
                (byte)((value >> 32) & 0xFF),
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF),
            })
        {
        }

        public ByteArray(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException();

            this.Parts = new List<byte[]> { data };
            this.Length = data.Length;
        }

        public ByteArray(IRandom random, int length)
        {
            if (random == null)
                throw new ArgumentNullException();

            var data = new byte[length];
            random.GetBytes(data);

            this.Parts = new List<byte[]> { data };
            this.Length = length;
        }

        public void AddTo(List<byte> list)
        {
            for (int i = 0; i < Parts.Count; i++)
                list.AddRange(Parts[i]);
        }

        public void ClearMemory()
        {
            for (int i = 0; i < Parts.Count; i++)
                for (int j = 0; j < Parts[i].Length; j++)
                    Parts[i][j] = 0;
        }

        public ByteArray SubArray(int start, int length)
        {
            if (start + length > this.Length)
                throw new ArgumentException();

            var bytes = new byte[length];
            for (int i = 0; i < length; i++)
                bytes[i] = this[start + i];
            return new ByteArray(bytes);
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
                Array.Copy(Parts[partIndex], 0, bytes, position, System.Math.Min(Parts[partIndex].Length, length - position));
                position += Parts[partIndex].Length;
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
            if (left.Length == 0)
                return right;
            if (right.Length == 0)
                return left;

            var result = new ByteArray();
            result.Parts.AddRange(left.Parts);
            result.Parts.AddRange(right.Parts);
            result.Length = left.Length + right.Length;
            return result;
        }

        public static ByteArray operator +(byte[] left, ByteArray right)
        {
            var result = new ByteArray();
            result.Parts.Add(left);
            result.Parts.AddRange(right.Parts);
            result.Length = left.Length + right.Length;
            return result;
        }

        public static ByteArray operator +(ByteArray left, byte[] right)
        {
            var result = new ByteArray();
            result.Parts.AddRange(left.Parts);
            result.Parts.Add(right);
            result.Length = left.Length + right.Length;
            return result;
        }

        public static ByteArray operator^(ByteArray left, byte right)
        {
            var bytes = left.ToArray();
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)(left[i] ^ right);
            return new ByteArray(bytes);
        }

        public static ByteArray operator ^(ByteArray left, ByteArray right)
        {
            if (left.Length != right.Length)
                throw new InvalidOperationException();

            var bytes = new byte[left.Length];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)(left[i] ^ right[i]);
            return new ByteArray(bytes);
        }
    }
}