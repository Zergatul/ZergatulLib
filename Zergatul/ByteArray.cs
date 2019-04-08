using System;

namespace Zergatul
{
    public static class ByteArray
    {
        public static byte[] Concat(byte[] array1, byte[] array2)
        {
            byte[] result = new byte[array1.Length + array2.Length];
            Buffer.BlockCopy(array1, 0, result, 0, array1.Length);
            Buffer.BlockCopy(array2, 0, result, array1.Length, array2.Length);
            return result;
        }

        public static byte[] Concat(byte[] array1, byte[] array2, byte[] array3)
        {
            byte[] result = new byte[array1.Length + array2.Length + array3.Length];
            Buffer.BlockCopy(array1, 0, result, 0, array1.Length);
            Buffer.BlockCopy(array2, 0, result, array1.Length, array2.Length);
            Buffer.BlockCopy(array3, 0, result, array1.Length + array2.Length, array3.Length);
            return result;
        }

        public static byte[] SubArray(byte[] array, int startIndex, int length)
        {
            byte[] result = new byte[length];
            Buffer.BlockCopy(array, startIndex, result, 0, length);
            return result;
        }

        public static bool Equals(byte[] array1, byte[] array2)
        {
            if (array1 == null && array2 == null)
                return true;
            if (array1 == null || array2 == null)
                return false;
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

        public static int IndexOf(byte[] array1, int offset, int length, byte[] array2)
        {
            if (array1 == null || array2 == null)
                throw new ArgumentNullException();
            if (array1.Length - offset < array2.Length)
                return -1;
            for (int i = offset; i < offset + length - array2.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < array2.Length; j++)
                    if (array1[i + j] != array2[j])
                    {
                        match = false;
                        break;
                    }
                if (match)
                    return i;
            }
            return -1;
        }

        public static bool Contains(byte[] array1, byte[] array2)
        {
            for (int i = 0; i <= array1.Length - array1.Length; i++)
            {
                bool contains = true;
                for (int j = 0; j < array2.Length; j++)
                    if (array1[i + j] != array2[j])
                    {
                        contains = false;
                        break;
                    }
                if (contains)
                    return true;
            }
            return false;
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
    }
}