﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Security;

namespace Zergatul.Math
{
    public class BinaryPolynomial : IEquatable<BinaryPolynomial>
    {
        public static readonly BinaryPolynomial Zero = FromPowers();
        public static readonly BinaryPolynomial One = FromPowers(0);

        private static string _superscript = "⁰¹²³⁴⁵⁶⁷⁸⁹";

        // little-endian
        private ulong[] _words;
        public int Degree { get; private set; }
        public int StrictWordLength => Degree / 64 + 1;

        private BinaryPolynomial()
        {

        }

        public BinaryPolynomial(byte[] words, ByteOrder order)
        {
            this._words = new ulong[(words.Length + 7) / 8];

            int byteLen = words.Length;
            if (order == ByteOrder.BigEndian)
                for (int i = 0; i < this._words.Length; i++)
                {
                    byte b1 = (byteLen - 1 - (8 * i + 0) >= 0) ? words[byteLen - 1 - (8 * i + 0)] : (byte)0;
                    byte b2 = (byteLen - 1 - (8 * i + 1) >= 0) ? words[byteLen - 1 - (8 * i + 1)] : (byte)0;
                    byte b3 = (byteLen - 1 - (8 * i + 2) >= 0) ? words[byteLen - 1 - (8 * i + 2)] : (byte)0;
                    byte b4 = (byteLen - 1 - (8 * i + 3) >= 0) ? words[byteLen - 1 - (8 * i + 3)] : (byte)0;
                    byte b5 = (byteLen - 1 - (8 * i + 4) >= 0) ? words[byteLen - 1 - (8 * i + 4)] : (byte)0;
                    byte b6 = (byteLen - 1 - (8 * i + 5) >= 0) ? words[byteLen - 1 - (8 * i + 5)] : (byte)0;
                    byte b7 = (byteLen - 1 - (8 * i + 6) >= 0) ? words[byteLen - 1 - (8 * i + 6)] : (byte)0;
                    byte b8 = (byteLen - 1 - (8 * i + 7) >= 0) ? words[byteLen - 1 - (8 * i + 7)] : (byte)0;
                    this._words[i] =
                        ((ulong)b8 << 56) |
                        ((ulong)b7 << 48) |
                        ((ulong)b6 << 40) |
                        ((ulong)b5 << 32) |
                        ((ulong)b4 << 24) |
                        ((ulong)b3 << 16) |
                        ((ulong)b2 << 08) |
                        ((ulong)b1 << 00);
                }
            if (order == ByteOrder.LittleEndian)
                for (int i = 0; i < this._words.Length; i++)
                {
                    byte b1 = (8 * i + 0) < byteLen ? words[8 * i + 0] : (byte)0;
                    byte b2 = (8 * i + 1) < byteLen ? words[8 * i + 1] : (byte)0;
                    byte b3 = (8 * i + 2) < byteLen ? words[8 * i + 2] : (byte)0;
                    byte b4 = (8 * i + 3) < byteLen ? words[8 * i + 3] : (byte)0;
                    byte b5 = (8 * i + 4) < byteLen ? words[8 * i + 4] : (byte)0;
                    byte b6 = (8 * i + 5) < byteLen ? words[8 * i + 5] : (byte)0;
                    byte b7 = (8 * i + 6) < byteLen ? words[8 * i + 6] : (byte)0;
                    byte b8 = (8 * i + 7) < byteLen ? words[8 * i + 7] : (byte)0;
                    this._words[i] =
                        ((ulong)b8 << 56) |
                        ((ulong)b7 << 48) |
                        ((ulong)b6 << 40) |
                        ((ulong)b5 << 32) |
                        ((ulong)b4 << 24) |
                        ((ulong)b3 << 16) |
                        ((ulong)b2 << 08) |
                        ((ulong)b1 << 00);
                }

            CalculateRealDegree(words.Length * 8 - 1);
        }

        public BinaryPolynomial(uint[] words, ByteOrder order)
        {
            this._words = new ulong[(words.Length + 1) / 2];

            if (order == ByteOrder.BigEndian)
                Array.Reverse(words);

            for (int i = 0; i < this._words.Length; i++)
            {
                ulong low = words[2 * i];
                ulong high = 2 * i + 1 < words.Length ? words[2 * i + 1] : 0;
                this._words[i] = (high << 32) | low;
            }

            CalculateRealDegree(words.Length * 32 - 1);
        }

        public BinaryPolynomial(BigInteger value)
            : this(value.ToBytes(ByteOrder.BigEndian), ByteOrder.BigEndian)
        {
        }

        public static BinaryPolynomial Random(int degree, SecureRandom random)
        {
            byte[] buffer = new byte[8];

            ulong[] words = new ulong[degree / 64 + 1];
            for (int i = words.Length - 1; i >= 0; i--)
            {
                random.GetNextBytes(buffer);
                words[i] = BitHelper.ToUInt64(buffer, 0, ByteOrder.BigEndian);
            }

            if (degree % 64 != 63)
                words[words.Length - 1] &= (1UL << (degree % 64)) - 1;

            return new BinaryPolynomial { _words = words, Degree = degree };
        }

        public bool IsZero
        {
            get
            {
                for (int i = 0; i < _words.Length; i++)
                    if (_words[i] != 0)
                        return false;
                return true;
            }
        }

        public bool IsBitSet(int bit)
        {
            if (bit > Degree)
                return false;
            return BitHelper.CheckBit(_words[bit / 64], bit % 64);
        }

        public byte[] ToBytes(ByteOrder order)
        {
            int bit = _words.Length * 64;
            while (!IsBitSet(bit))
                bit--;
            return ToBytes(order, (bit + 7) / 8);
        }

        public byte[] ToBytes(ByteOrder order, int length)
        {
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                int index = i / 8;
                ulong value = index < _words.Length ? _words[index] : 0;
                result[i] = (byte)(value >> ((i % 8) * 8));
            }

            if (order == ByteOrder.BigEndian)
                Array.Reverse(result);

            return result;
        }

        public BigInteger ToBigInteger()
        {
            return new BigInteger(ToBytes(ByteOrder.BigEndian), ByteOrder.BigEndian);
        }

        #region System.Object

        public override bool Equals(object obj)
        {
            if (obj is BinaryPolynomial)
                return Equals((BinaryPolynomial)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _words.GetHashCode();
        }

        public override string ToString()
        {
            if (Degree == -1)
                return "0";

            StringBuilder sb = new StringBuilder();
            bool prepend = false;
            for (int power = Degree; power >= 0; power--)
                if (IsBitSet(power))
                {
                    if (prepend)
                        sb.Append('+');
                    else
                        prepend = true;

                    sb.Append(power > 0 ? 'x' : '1');

                    if (power > 1)
                    {
                        string powerStr = power.ToString();
                        for (int i = 0; i < powerStr.Length; i++)
                            sb.Append(_superscript[(int)powerStr[i] - (int)'0']);
                    }
                }
            return sb.ToString();
        }

        #endregion

        #region IEquatable<BinaryPolynomial>

        public bool Equals(BinaryPolynomial other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (this.Degree != other.Degree)
                return false;
            for (int i = this.StrictWordLength - 1; i >= 0; i--)
                if (this._words[i] != other._words[i])
                    return false;
            return true;
        }

        #endregion

        #region Private instance methods

        private void CalculateRealDegree(int startFrom)
        {
            Degree = startFrom;
            while (Degree >= 0 && !IsBitSet(Degree))
                Degree--;
        }

        #endregion

        #region Operators

        public static bool operator ==(BinaryPolynomial p1, BinaryPolynomial p2)
        {
            if (ReferenceEquals(p1, null))
            {
                if (ReferenceEquals(p2, null))
                    return true;
                else
                    return p2.Equals(p1);
            }
            else
                return p1.Equals(p2);
        }

        public static bool operator !=(BinaryPolynomial p1, BinaryPolynomial p2)
        {
            return !(p1 == p2);
        }

        public static BinaryPolynomial operator +(BinaryPolynomial p1, BinaryPolynomial p2)
        {
            return Add(p1, p2);
        }

        public static BinaryPolynomial operator *(BinaryPolynomial p1, BinaryPolynomial p2)
        {
            return Multiplication(p1, p2);
        }

        public static BinaryPolynomial operator %(BinaryPolynomial p1, BinaryPolynomial p2)
        {
            return ModularReduction(p1, p2);
        }

        #endregion

        public static BinaryPolynomial FromPowers(params int[] powers)
        {
            int max = -1;
            for (int i = 0; i < powers.Length; i++)
                if (powers[i] > max)
                    max = powers[i];

            var polinomial = new BinaryPolynomial();
            polinomial.Degree = max;
            polinomial._words = new ulong[max / 64 + 1];
            for (int i = 0; i < powers.Length; i++)
            {
                int power = powers[i];
                polinomial._words[power / 64] = BitHelper.SetBit(polinomial._words[power / 64], power % 64);
            }
            return polinomial;
        }

        private static BinaryPolynomial FromWords(ulong[] words)
        {
            var polynomial = new BinaryPolynomial();
            polynomial._words = words;
            polynomial.CalculateRealDegree(words.Length * 64 - 1);
            return polynomial;
        }

        public static BinaryPolynomial Add(BinaryPolynomial p1, BinaryPolynomial p2)
        {
            if (p1.Degree < p2.Degree)
            {
                var buf = p1;
                p1 = p2;
                p2 = buf;
            }

            ulong[] words = new ulong[p1.StrictWordLength];
            Array.Copy(p1._words, words, words.Length);

            for (int i = p2.StrictWordLength - 1; i >= 0; i--)
                words[i] ^= p2._words[i];

            var polynomial = new BinaryPolynomial();
            polynomial._words = words;
            polynomial.CalculateRealDegree(p1.Degree);
            return polynomial;
        }

        public static BinaryPolynomial Multiplication(BinaryPolynomial p1, BinaryPolynomial p2)
        {
            if (p1.Degree == -1)
                return p1;
            if (p2.Degree == -1)
                return p2;
            if (p1.Degree == 0)
                return p2;
            if (p2.Degree == 0)
                return p1;

            if (p1.Degree < p2.Degree)
            {
                var buf = p1;
                p1 = p2;
                p2 = buf;
            }

            ulong[] words = new ulong[(p1.Degree + p2.Degree) / 64 + 1];
            int p1WordsLength = p1.StrictWordLength;
            for (int i = 0; i <= p2.Degree; i++)
                if (p2.IsBitSet(i))
                {
                    int wordIndex = i / 64;
                    int bitIndex = i % 64;
                    for (int j = 0; j < p1WordsLength; j++)
                        words[wordIndex + j] ^= p1._words[j] << bitIndex;
                    if (bitIndex > 0)
                        for (int j = 0; j < p1WordsLength; j++)
                            if (wordIndex + j + 1 < words.Length)
                                words[wordIndex + j + 1] ^= p1._words[j] >> (64 - bitIndex);
                }

            var polynomial = new BinaryPolynomial();
            polynomial._words = words;
            polynomial.Degree = p1.Degree + p2.Degree;
            return polynomial;
        }

        public static BinaryPolynomial Square(BinaryPolynomial p)
        {
            if (p.Degree <= 0)
                return p;

            ulong[] words = new ulong[p.Degree / 32 + 1];
            for (int i = p.Degree; i >= 0; i--)
                if (p.IsBitSet(i))
                {
                    int wordIndex = i / 32;
                    int bitIndex = 2 * (i % 64);
                    words[wordIndex] = BitHelper.SetBit(words[wordIndex], bitIndex);
                }

            var polynomial = new BinaryPolynomial();
            polynomial._words = words;
            polynomial.Degree = p.Degree * 2;
            return polynomial;
        }

        public static BinaryPolynomial ModularReduction(BinaryPolynomial p, BinaryPolynomial m)
        {
            int mWordsLength = m.StrictWordLength;
            ulong[] words = new ulong[p.StrictWordLength];
            Array.Copy(p._words, words, words.Length);

            for (int i = p.Degree; i >= m.Degree; i--)
            {
                int wordIndex = (i - m.Degree) / 64;
                int bitIndex = (i - m.Degree) % 64;
                if (BitHelper.CheckBit(words[i / 64], i % 64))
                    for (int j = 0; j < mWordsLength; j++)
                    {
                        words[wordIndex + j] ^= m._words[j] << bitIndex;
                        if (bitIndex > 0)
                            if (wordIndex + j + 1 < words.Length)
                                words[wordIndex + j + 1] ^= m._words[j] >> (64 - bitIndex);
                    }
            }

            var polynomial = new BinaryPolynomial();
            polynomial._words = words;
            polynomial.CalculateRealDegree(m.Degree - 1);
            return polynomial;
        }

        /// <summary>
        /// Assumes p1 &lt; m and p2 &lt; m
        /// </summary>
        public static BinaryPolynomial ModularMultiplication(BinaryPolynomial p1, BinaryPolynomial p2, BinaryPolynomial m)
        {
            if (p1.Degree > m.Degree)
                p1 = ModularReduction(p1, m);
            if (p2.Degree > m.Degree)
                p2 = ModularReduction(p2, m);

            ulong[] words = new ulong[m._words.Length];
            ulong[] p2Words = new ulong[m._words.Length];
            Array.Copy(p2._words, p2Words, p2._words.Length);

            if (p1.IsBitSet(0))
                Array.Copy(p2Words, words, p2Words.Length);

            for (int i = 1; i < m.Degree; i++)
            {
                ShiftLeftOneBit(p2Words);
                if (BitHelper.CheckBit(p2Words[m.Degree / 64], m.Degree % 64))
                    Xor(p2Words, m._words);
                if (p1.IsBitSet(i))
                    Xor(words, p2Words);
            }

            var polynomial = new BinaryPolynomial();
            polynomial._words = words;
            polynomial.CalculateRealDegree(m.Degree - 1);
            return polynomial;
        }

        public static BinaryPolynomial ModularInverse(BinaryPolynomial p, BinaryPolynomial m)
        {
            if (p.Degree >= m.Degree)
                p = ModularReduction(p, m);

            ulong[] b = new ulong[m.StrictWordLength];
            ulong[] c = new ulong[m.StrictWordLength];
            ulong[] u = new ulong[m.StrictWordLength];
            ulong[] v = new ulong[m.StrictWordLength];

            // b = 1
            b[0] = 1;
            // c = 0
            // u = p
            Array.Copy(p._words, u, System.Math.Min(u.Length, p._words.Length));
            int uDegree = p.Degree;
            // v = m
            Array.Copy(m._words, v, v.Length);
            int vDegree = m.Degree;

            while (true)
            {
                // while x divides u
                while (uDegree >= 0 && (u[0] & 1) == 0)
                {
                    // u = u / x
                    ShiftRightOneBit(u);
                    uDegree--;

                    // it x divides b
                    if ((b[0] & 1) == 0)
                        // b = b / x
                        ShiftRightOneBit(b);
                    else
                    {
                        // b = (b + m) / x
                        Xor(b, m._words);
                        ShiftRightOneBit(b);
                    }
                }

                bool highWordsZero = true;
                for (int i = 1; i < u.Length; i++)
                    if (u[i] != 0)
                    {
                        highWordsZero = false;
                        break;
                    }

                if (highWordsZero && u[0] == 1)
                {
                    var polynomial = new BinaryPolynomial();
                    polynomial._words = b;
                    polynomial.CalculateRealDegree(m.Degree - 1);
                    return polynomial;
                }

                if (highWordsZero && u[0] == 0)
                    return null;

                if (uDegree < vDegree)
                {
                    int buf1 = uDegree;
                    uDegree = vDegree;
                    vDegree = buf1;

                    var buf2 = u;
                    u = v;
                    v = buf2;

                    buf2 = b;
                    b = c;
                    c = buf2;
                }

                // u = u + v
                Xor(u, v);
                while (uDegree >= 0 && !BitHelper.CheckBit(u[uDegree / 64], uDegree % 64))
                    uDegree--;
                // b = b + c
                Xor(b, c);
            }
        }

        public static BinaryPolynomial ModularDivision(BinaryPolynomial p1, BinaryPolynomial p2, BinaryPolynomial m)
        {
            return ModularMultiplication(p1, ModularInverse(p2, m), m);
        }

        #region Private static methods

        private static void ShiftLeftOneBit(ulong[] words)
        {
            ulong carry = 0;
            for (int i = 0; i < words.Length; i++)
            {
                ulong temp = words[i] >> 63;
                words[i] = (words[i] << 1) | carry;
                carry = temp;
            }
        }

        private static void ShiftRightOneBit(ulong[] words)
        {
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i] >> 1;
                if (i + 1 < words.Length)
                    words[i] |= words[i + 1] << 63;
            }
        }

        /// <summary>
        /// w1 = w1 ^ w2
        /// </summary>
        private static void Xor(ulong[] w1, ulong[] w2)
        {
            for (int i = 0; i < w1.Length; i++)
                w1[i] ^= w2[i];
        }

        #endregion
    }
}