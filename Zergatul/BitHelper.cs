﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul
{
    public static class BitHelper
    {
        public static byte[] GetBytes(long value, ByteOrder order)
        {
            if (order == ByteOrder.BigEndian)
                return new byte[]
                {
                    (byte)((value >> 56) & 0xFF),
                    (byte)((value >> 48) & 0xFF),
                    (byte)((value >> 40) & 0xFF),
                    (byte)((value >> 32) & 0xFF),
                    (byte)((value >> 24) & 0xFF),
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 08) & 0xFF),
                    (byte)((value >> 00) & 0xFF)
                };

            if (order == ByteOrder.LittleEndian)
                return new byte[]
                {
                    (byte)((value >> 00) & 0xFF),
                    (byte)((value >> 08) & 0xFF),
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 24) & 0xFF),
                    (byte)((value >> 32) & 0xFF),
                    (byte)((value >> 40) & 0xFF),
                    (byte)((value >> 48) & 0xFF),
                    (byte)((value >> 56) & 0xFF)
                };

            throw new NotImplementedException();
        }

        public static byte[] GetBytes(ulong value, ByteOrder order)
        {
            if (order == ByteOrder.BigEndian)
                return new byte[]
                {
                    (byte)(value >> 56),
                    (byte)(value >> 48),
                    (byte)(value >> 40),
                    (byte)(value >> 32),
                    (byte)(value >> 24),
                    (byte)(value >> 16),
                    (byte)(value >> 08),
                    (byte)(value >> 00)
                };

            if (order == ByteOrder.LittleEndian)
                return new byte[]
                {
                    (byte)(value >> 00),
                    (byte)(value >> 08),
                    (byte)(value >> 16),
                    (byte)(value >> 24),
                    (byte)(value >> 32),
                    (byte)(value >> 40),
                    (byte)(value >> 48),
                    (byte)(value >> 56)
                };

            throw new NotImplementedException();
        }

        public static void GetBytes(ulong value, ByteOrder order, byte[] array, int index)
        {
            if (order == ByteOrder.BigEndian)
            {
                array[index    ] = (byte)(value >> 56);
                array[index + 1] = (byte)(value >> 48);
                array[index + 2] = (byte)(value >> 40);
                array[index + 3] = (byte)(value >> 32);
                array[index + 4] = (byte)(value >> 24);
                array[index + 5] = (byte)(value >> 16);
                array[index + 6] = (byte)(value >> 08);
                array[index + 7] = (byte)(value);
                return;
            }

            if (order == ByteOrder.LittleEndian)
            {
                array[index    ] = (byte)(value);
                array[index + 1] = (byte)(value >> 08);
                array[index + 2] = (byte)(value >> 16);
                array[index + 3] = (byte)(value >> 24);
                array[index + 4] = (byte)(value >> 32);
                array[index + 5] = (byte)(value >> 40);
                array[index + 6] = (byte)(value >> 48);
                array[index + 7] = (byte)(value >> 56);
                return;
            }

            throw new NotImplementedException();
        }

        public static byte[] GetBytes(uint value, ByteOrder order)
        {
            if (order == ByteOrder.BigEndian)
                return new byte[]
                {
                    (byte)((value >> 24) & 0xFF),
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 08) & 0xFF),
                    (byte)((value >> 00) & 0xFF)
                };

            if (order == ByteOrder.LittleEndian)
                return new byte[]
                {
                    (byte)((value >> 00) & 0xFF),
                    (byte)((value >> 08) & 0xFF),
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 24) & 0xFF)
                };

            throw new NotImplementedException();
        }

        public static void GetBytes(uint value, ByteOrder order, byte[] array, int index)
        {
            if (order == ByteOrder.BigEndian)
            {
                array[index    ] = (byte)(value >> 24);
                array[index + 1] = (byte)(value >> 16);
                array[index + 2] = (byte)(value >> 08);
                array[index + 3] = (byte)(value);
                return;
            }

            if (order == ByteOrder.LittleEndian)
            {
                array[index    ] = (byte)(value);
                array[index + 1] = (byte)(value >> 08);
                array[index + 2] = (byte)(value >> 16);
                array[index + 3] = (byte)(value >> 24);
                return;
            }

            throw new NotImplementedException();
        }

        public static void GetBytes(ushort value, ByteOrder order, byte[] array, int index)
        {
            if (order == ByteOrder.BigEndian)
            {
                array[index    ] = (byte)(value >> 08);
                array[index + 1] = (byte)(value);;
                return;
            }

            if (order == ByteOrder.LittleEndian)
            {
                array[index    ] = (byte)(value);
                array[index + 1] = (byte)(value >> 08);
                return;
            }

            throw new NotImplementedException();
        }

        public static ushort ToUInt16(byte[] data, int index, ByteOrder order)
        {
            if (order == ByteOrder.BigEndian)
                return (ushort)(
                    (data[index + 0] << 08) |
                    (data[index + 1] << 00));

            if (order == ByteOrder.LittleEndian)
                return (ushort)(
                    (data[index + 0] << 00) |
                    (data[index + 1] << 08));

            throw new NotImplementedException();
        }

        public static uint ToUInt32(byte[] data, int index, ByteOrder order)
        {
            if (order == ByteOrder.BigEndian)
                return (uint)(
                    (data[index + 0] << 24) |
                    (data[index + 1] << 16) |
                    (data[index + 2] << 08) |
                    (data[index + 3] << 00));

            if (order == ByteOrder.LittleEndian)
                return (uint)(
                    (data[index + 0] << 00) |
                    (data[index + 1] << 08) |
                    (data[index + 2] << 16) |
                    (data[index + 3] << 24));

            throw new NotImplementedException();
        }

        public static uint ToUInt32(byte[] data, ByteOrder order) => ToUInt32(data, 0, order);

        public static ulong ToUInt64(byte[] data, int index, ByteOrder order)
        {
            if (order == ByteOrder.BigEndian)
                return (ulong)(
                    ((ulong)data[index + 0] << 56) |
                    ((ulong)data[index + 1] << 48) |
                    ((ulong)data[index + 2] << 40) |
                    ((ulong)data[index + 3] << 32) |
                    ((ulong)data[index + 4] << 24) |
                    ((ulong)data[index + 5] << 16) |
                    ((ulong)data[index + 6] << 08) |
                    ((ulong)data[index + 7] << 00));

            throw new NotImplementedException();
        }

        public static ushort RotateRight(ushort value, int bits)
        {
            return (ushort)((value >> bits) | (value << (16 - bits)));
        }

        public static uint RotateRight(uint value, int bits)
        {
            return (value >> bits) | (value << (32 - bits));
        }

        public static ulong RotateRight(ulong value, int bits)
        {
            return (value >> bits) | (value << (64 - bits));
        }

        public static ushort RotateLeft(ushort value, int bits)
        {
            return (ushort)((value << bits) | (value >> (16 - bits)));
        }

        public static uint RotateLeft(uint value, int bits)
        {
            return (value << bits) | (value >> (32 - bits));
        }

        public static ulong RotateLeft(ulong value, int bits)
        {
            return (value << bits) | (value >> (64 - bits));
        }

        public static byte[] HexToBytes(string value)
        {
            byte[] result = new byte[value.Length / 2];
            for (int i = 0; i < value.Length / 2; i++)
                result[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
            return result;
        }

        public static string BytesToHex(byte[] data)
        {
            var sb = new StringBuilder(data.Length * 2);
            for (int i = 0; i < data.Length; i++)
                sb.Append(Convert.ToString(data[i], 16).PadLeft(2, '0'));
            return sb.ToString();
        }

        public static ulong SetBit(ulong value, int bit)
        {
            return value | (1UL << bit);
        }

        public static bool CheckBit(ulong value, int bit)
        {
            return (value & (1UL << bit)) != 0;
        }

        public static string ToBin(ulong value)
        {
            return Convert.ToString((uint)(value >> 32), 2).PadLeft(32, '0') + Convert.ToString((uint)(value & 0xFFFFFFFF), 2).PadLeft(32, '0');
        }

        /// <summary>
        /// a = a ^ (b <<< bits)
        /// </summary>
        public static void RotateLeft128AndXor(byte[] a, int aindex, byte[] b, int bindex, int bits)
        {
            int byteShift = bits / 8;
            int bitShift = bits % 8;
            if (bitShift != 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    a[aindex + i] ^= (byte)(b[bindex + (i + byteShift) % 16] << bitShift);
                    a[aindex + i] ^= (byte)(b[bindex + (i + byteShift + 1) % 16] >> (8 - bitShift));
                }
            }
            else
                throw new NotImplementedException();
        }

        /// <summary>
        /// a = a ^ (b >>> bits)
        /// </summary>
        public static void RotateRight128AndXor(byte[] a, int aindex, byte[] b, int bindex, int bits)
        {
            int byteShift = bits / 8;
            int bitShift = bits % 8;
            if (bitShift != 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    a[aindex + i] ^= (byte)(b[bindex + (32 + i - byteShift) % 16] >> bitShift);
                    a[aindex + i] ^= (byte)(b[bindex + (32 + i - byteShift - 1) % 16] << (8 - bitShift));
                }
            }
            else
                throw new NotImplementedException();
        }
    }
}