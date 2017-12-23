using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul
{
    public struct ByteArray
    {
        public byte[] Array;

        public int Length => Array.Length;

        public byte this[int index] => Array[index];

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

            this.Array = data;
        }

        public ByteArray(IRandom random, int length)
        {
            if (random == null)
                throw new ArgumentNullException();

            Array = new byte[length];
            random.GetBytes(Array);
        }

        public void ClearMemory()
        {
            System.Array.Clear(Array, 0, Length);
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

            byte[] result = new byte[length];
            System.Array.Copy(Array, result, length);

            return new ByteArray(result);
        }

        #region Static Helper Methods

        public static byte[] Concat(byte[] array1, byte[] array2)
        {
            byte[] result = new byte[array1.Length + array2.Length];
            System.Array.Copy(array1, 0, result, 0, array1.Length);
            System.Array.Copy(array2, 0, result, array1.Length, array2.Length);
            return result;
        }

        public static byte[] Concat(byte[] array1, byte[] array2, byte[] array3)
        {
            byte[] result = new byte[array1.Length + array2.Length + array3.Length];
            System.Array.Copy(array1, 0, result, 0, array1.Length);
            System.Array.Copy(array2, 0, result, array1.Length, array2.Length);
            System.Array.Copy(array3, 0, result, array1.Length + array2.Length, array3.Length);
            return result;
        }

        public static byte[] SubArray(byte[] array, int startIndex, int length)
        {
            byte[] result = new byte[length];
            System.Array.Copy(array, startIndex, result, 0, length);
            return result;
        }

        public static bool Equals(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                return false;
            for (int i = 0; i < array1.Length; i++)
                if (array1[i] != array2[i])
                    return false;
            return true;
        }

        public static bool IsSubArray(byte[] array1, byte[] array2, int index)
        {
            if (array1.Length - array2.Length < index)
                return false;
            for (int i = 0; i < array2.Length; i++)
                if (array1[index + i] != array2[i])
                    return false;
            return true;
        }

        public static bool IsZero(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] != 0)
                    return false;
            return true;
        }

        /// <summary>
        /// b1 = b1 ^ b2
        /// </summary>
        public static void Xor(byte[] array1, byte[] array2)
        {
            for (int i = 0; i < array1.Length; i++)
                array1[i] ^= array2[i];
        }

        #endregion

        #region Operators

        public static ByteArray operator+(ByteArray left, ByteArray right)
        {
            return new ByteArray(Concat(left.Array, right.Array));
        }

        public static ByteArray operator +(byte[] left, ByteArray right)
        {
            return new ByteArray(Concat(left, right.Array));
        }

        public static ByteArray operator +(ByteArray left, byte[] right)
        {
            return new ByteArray(Concat(left.Array, right));
        }

        public static ByteArray operator^(ByteArray left, byte right)
        {
            byte[] result = new byte[left.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = (byte)(left[i] ^ right);
            return new ByteArray(result);
        }

        public static ByteArray operator ^(ByteArray left, ByteArray right)
        {
            var result = new byte[left.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = (byte)(left[i] ^ right[i]);
            return new ByteArray(result);
        }

        #endregion
    }
}