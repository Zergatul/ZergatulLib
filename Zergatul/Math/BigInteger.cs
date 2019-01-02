﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math
{
#if !UseOpenSSL

    public class BigInteger : IComparable<BigInteger>, IComparable<long>, IComparable<int>, IEquatable<BigInteger>, IEquatable<int>
    {
        private static IRandom _random = new DefaultRandom();

        /// <summary>
        /// Little-endian order.
        /// </summary>
        internal uint[] _words;

        /// <summary>
        /// Valuable uint's in array. Some last elements can be zero.
        /// </summary>
        internal int _wordsLength;

        internal int _sign;

        #region Public properties

        public bool IsZero => _wordsLength == 0;

        public bool IsOdd => !IsZero && (_words[0] & 1) == 1;

        public bool IsEven => IsZero || (_words[0] & 1) == 0;

        public int BitSize => _wordsLength == 0 ? 0 : 32 * (_wordsLength - 1) + 1 + (int)System.Math.Log(_words[_wordsLength - 1], 2);

        #endregion

        #region Public constants

        public static readonly BigInteger Zero = new BigInteger(new uint[0]);
        public static readonly BigInteger One = new BigInteger(1);

        #endregion

        #region Contructors

        private BigInteger(uint[] words)
        {
            this._words = words;
            this._wordsLength = words.Length;
            this._sign = 1;
            TruncateLeadingZeros();
        }

        private BigInteger(uint[] words, int length)
        {
            this._words = words;
            this._wordsLength = length;
            this._sign = 1;
            TruncateLeadingZeros();
        }

        public BigInteger(int value)
        {
            if (value == 0)
            {
                CopyFrom(Zero);
                return;
            }

            if (value < 0)
            {
                if (value == int.MinValue)
                    _words = new uint[1] { 0x80000000 };
                else
                    _words = new uint[1] { (uint)(-value) };
                _wordsLength = 1;
                _sign = -1;
            }
            else
            {
                _words = new uint[1] { (uint)(value) };
                _wordsLength = 1;
                _sign = 1;
            }
        }

        public BigInteger(long value)
        {
            if (value == 0)
            {
                CopyFrom(Zero);
                return;
            }

            if (value > 0)
            {
                if (value > uint.MaxValue)
                {
                    _words = new uint[2] { (uint)(value & 0xFFFFFFFF), (uint)(value >> 32) };
                    _wordsLength = 2;
                }
                else
                {
                    _words = new uint[1] { (uint)value };
                    _wordsLength = 1;
                }
                _sign = 1;
                return;
            }
            else
            {
                if (value == long.MinValue)
                {
                    _words = new uint[2] { 0, 0x80000000 };
                    _wordsLength = 2;
                }
                else
                {
                    value = -value;
                    if (value > uint.MaxValue)
                    {
                        _words = new uint[2] { (uint)(value & 0xFFFFFFFF), (uint)(value >> 32) };
                        _wordsLength = 2;
                    }
                    else
                    {
                        _words = new uint[1] { (uint)value };
                        _wordsLength = 1;
                    }
                }
                _sign = -1;
                return;
            }
        }

        public BigInteger(byte[] data, int offset, int length, ByteOrder order)
        {
            _sign = 1;
            if (order == ByteOrder.BigEndian)
            {
                int first = 0;
                while (first < length && data[offset + first] == 0)
                    first++;
                _words = new uint[(length - first + 3) >> 2];
                _wordsLength = _words.Length;
                for (int i = _words.Length - 1; i >= 0; i--)
                {
                    byte b0 = SafeGetByte(data, offset + length, offset + length - 1 - ((i << 2) | 3));
                    byte b1 = SafeGetByte(data, offset + length, offset + length - 1 - ((i << 2) | 2));
                    byte b2 = SafeGetByte(data, offset + length, offset + length - 1 - ((i << 2) | 1));
                    byte b3 = SafeGetByte(data, offset + length, offset + length - 1 - ((i << 2) | 0));
                    _words[i] = (uint)((b0 << 24) | (b1 << 16) | (b2 << 8) | b3);
                }
            }
            if (order == ByteOrder.LittleEndian)
            {
                int last = length - 1;
                while (last >= 0 && data[offset + last] == 0)
                    last--;
                _words = new uint[(last + 4) >> 2];
                _wordsLength = _words.Length;
                for (int i = 0; i < _words.Length; i++)
                {
                    byte b0 = SafeGetByte(data, offset + length, offset + ((i << 2) | 3));
                    byte b1 = SafeGetByte(data, offset + length, offset + ((i << 2) | 2));
                    byte b2 = SafeGetByte(data, offset + length, offset + ((i << 2) | 1));
                    byte b3 = SafeGetByte(data, offset + length, offset + ((i << 2) | 0));
                    _words[i] = (uint)((b0 << 24) | (b1 << 16) | (b2 << 8) | b3);
                }
            }
        }

        public BigInteger(byte[] data, ByteOrder order)
            : this(data, 0, data.Length, order)
        {
            
        }

        public BigInteger(uint[] words, ByteOrder order)
        {
            this._sign = 1;
            this._words = new uint[words.Length];
            Array.Copy(words, this._words, words.Length);

            if (order == ByteOrder.BigEndian)
                Array.Reverse(this._words);

            this._wordsLength = words.Length;
            TruncateLeadingZeros();
        }

        public static BigInteger Parse(string value, int radix = 10)
        {
            return Parse(value, radix, DefaultSymbols);
        }

        public static BigInteger Parse(string value, int radix, char[] symbols)
        {
            int sign = 1;
            if (value.StartsWith("-"))
            {
                sign = -1;
                value = value.Substring(1);
            }

            value = value.TrimStart(symbols[0]);
            if (value.Length == 0)
                return Zero;

            BigInteger result;
            if (radix == 2 || radix == 4 || radix == 16)
            {
                result = ParsePowerOfTwo(value, radix);
                result._sign = sign;
                return result;
            }

            int bitsCount = (int)System.Math.Ceiling(value.Length * System.Math.Log(radix, 2));
            uint[] words = new uint[(bitsCount + 31) / 32];

            int cursor = 0;
            int firstGroupLen = value.Length % DigitsPerUInt32[radix];
            if (firstGroupLen == 0)
                firstGroupLen = DigitsPerUInt32[radix];
            string group = value.Substring(cursor, firstGroupLen);
            cursor += firstGroupLen;
            words[0] = StringToUInt32(group, radix, symbols); //Convert.ToUInt32(group, radix);

            uint superRadix = RadixUInt32[radix];
            int wordsLength = 1;
            while (cursor < value.Length)
            {
                group = value.Substring(cursor, DigitsPerUInt32[radix]);
                cursor += DigitsPerUInt32[radix];
                uint groupValue = StringToUInt32(group, radix, symbols); //Convert.ToUInt32(group, radix);
                DestructiveMulAdd(words, wordsLength, superRadix, groupValue);

                if (wordsLength < words.Length && words[wordsLength] != 0)
                    wordsLength++;
            }

            result = new BigInteger(words, wordsLength);
            result._sign = sign;
            return result;
        }

        /// <summary>
        /// Generates random number in range [0..value-1]
        /// </summary>
        public static BigInteger Random(BigInteger value, IRandom random)
        {
            if (value.IsZero)
                throw new InvalidOperationException();

            var bytes = value.ToBytes(ByteOrder.BigEndian);
            var result = new byte[bytes.Length];
            bool finished = false;
            while (!finished)
            {
                bool highBytesEqual = true;
                for (int i = 0; i < result.Length; i++)
                {
                    random.GetBytes(result, i, 1);
                    if (highBytesEqual && result[i] > bytes[i])
                        break;
                    if (highBytesEqual && result[i] < bytes[i])
                        highBytesEqual = false;
                    if (i == result.Length - 1)
                        finished = true;
                }
            }

            return new BigInteger(result, ByteOrder.BigEndian);
        }

        /// <summary>
        /// Generates random number in range [from..to-1]
        /// </summary>
        public static BigInteger Random(BigInteger from, BigInteger to, IRandom random)
        {
            if (from >= to)
                throw new InvalidOperationException();

            return Random(to - from, random) + from;
        }

        #endregion

        #region Public methods

        public bool IsBitSet(int position)
        {
            int wordIndex = position / 32;
            if (wordIndex >= _wordsLength)
                return false;
            return (_words[wordIndex] & (1 << (position & 0x1f))) != 0;
        }

        public static BigInteger Sum(BigInteger summand1, BigInteger summand2)
        {
            if (summand1.IsZero)
                return summand2;
            if (summand2.IsZero)
                return summand1;

            if (summand1._sign == summand2._sign)
            {
                BigInteger result = new BigInteger(UnsignedSum(summand1, summand2));
                result._sign = summand1._sign;
                return result;
            }
            else
            {
                int compare = UnsignedCompare(summand1, summand2);
                if (compare == 0)
                    return Zero;
                if (compare > 0)
                {
                    BigInteger result = new BigInteger(UnsignedDifference(summand1, summand2));
                    result._sign = summand1._sign;
                    return result;
                }
                else
                {
                    BigInteger result = new BigInteger(UnsignedDifference(summand2, summand1));
                    result._sign = summand2._sign;
                    return result;
                }
            }
        }

        public static BigInteger Sum(BigInteger summand1, int summand2)
        {
            uint[] words = new uint[summand1._wordsLength + 1];
            Array.Copy(summand1._words, 0, words, 0, summand1._wordsLength);

            long carry = summand2;
            for (int i = 0; i < summand1._wordsLength; i++)
            {
                long sum = carry + words[i];
                words[i] = (uint)(sum & 0xFFFFFFFF);
                carry = sum >> 32;

                if (carry == 0)
                    break;
            }
            if (carry != 0)
                words[summand1._wordsLength] = (uint)carry;

            return new BigInteger(words);
        }

        public static BigInteger Difference(BigInteger minuend, BigInteger subtrahend)
        {
            if (minuend.IsZero)
                return subtrahend.AdditiveInverse();
            if (subtrahend.IsZero)
                return minuend;

            if (minuend._sign == subtrahend._sign)
            {
                int compare = UnsignedCompare(minuend, subtrahend);
                if (compare == 0)
                    return Zero;
                if (compare > 0)
                {
                    BigInteger result = new BigInteger(UnsignedDifference(minuend, subtrahend));
                    result._sign = minuend._sign;
                    return result;
                }
                else
                {
                    BigInteger result = new BigInteger(UnsignedDifference(subtrahend, minuend));
                    result._sign = -subtrahend._sign;
                    return result;
                }
            }
            else
            {
                BigInteger result = new BigInteger(UnsignedSum(minuend, subtrahend));
                result._sign = minuend._sign;
                return result;
            }
        }

        public static BigInteger Difference(BigInteger minuend, int subtrahend)
        {
            if (subtrahend == 0)
                return minuend;

            var words = new uint[minuend._wordsLength];

            long diff = minuend._words[0] - subtrahend;
            long carry;
            if (diff < 0)
            {
                carry = -1;
                diff += 0x100000000;
            }
            else
                carry = 0;
            words[0] = (uint)diff;
            for (int i = 1; i < words.Length; i++)
            {
                if (carry == 0)
                {
                    words[i] = minuend._words[i];
                    continue;
                }
                diff = minuend._words[i] + carry;
                if (diff < 0)
                {
                    carry = -1;
                    diff += 0x100000000;
                }
                else
                    carry = 0;
                words[i] = (uint)diff;
            }
            if (carry == -1)
                throw new InvalidOperationException();

            return new BigInteger(words);
        }

        public BigInteger AdditiveInverse()
        {
            if (IsZero)
                return Zero;

            BigInteger result = new BigInteger(_words, _wordsLength);
            result._sign = -_sign;
            return result;
        }

        public static BigInteger ShiftRight(BigInteger value, int bits)
        {
            if (bits < 0)
                throw new ArgumentException("bits should be >= 0");
            if (bits == 0)
                return value;

            int deltaWords = bits / 32;
            int deltaBits = bits % 32;
            uint[] words = new uint[value._wordsLength - deltaWords];
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = value._words[i + deltaWords] >> deltaBits;
                if (deltaBits != 0 && i + 1 + deltaWords < value._wordsLength)
                    words[i] |= value._words[i + 1 + deltaWords] << (32 - deltaBits);
            }
            return new BigInteger(words);
        }

        public static BigInteger ShiftLeft(BigInteger value, int bits)
        {
            if (bits < 0)
                throw new ArgumentException("bits should be >= 0");
            if (bits == 0)
                return value;

            int deltaWords = bits / 32;
            int deltaBits = bits % 32;
            uint[] words = new uint[value._wordsLength + deltaWords + (deltaBits > 0 ? 1 : 0)];
            for (int i = deltaWords; i < words.Length; i++)
            {
                if (i - deltaWords < value._wordsLength)
                    words[i] = value._words[i - deltaWords] << deltaBits;
                if (deltaBits != 0 && i - deltaWords - 1 >= 0)
                    words[i] |= value._words[i - deltaWords - 1] >> (32 - deltaBits);
            }
            return new BigInteger(words);
        }

        public static BigInteger Product(BigInteger factor1, BigInteger factor2)
        {
            if (factor1.IsZero || factor2.IsZero)
                return Zero;

            BigInteger result;
            if (factor1._wordsLength < factor2._wordsLength)
                result = factor2.MultiplyGeneral(factor1);
            else
                result = factor1.MultiplyGeneral(factor2);

            result._sign = factor1._sign * factor2._sign;

            return result;
        }

        public static BigInteger Product(BigInteger factor1, int factor2)
        {
            if (factor1.IsZero || factor2 == 0)
                return Zero;

            uint[] words = new uint[factor1._wordsLength + 1];
            uint factor2UInt32 = factor2 < 0 ? (uint)(-factor2) : (uint)factor2;
            ulong carry = 0;
            for (int i = 0; i < factor1._wordsLength; i++)
            {
                ulong product = (ulong)factor1._words[i] * factor2UInt32 + carry;
                words[i] = (uint)(product & 0xFFFFFFFF);
                carry = product >> 32;
            }
            if (carry > 0)
                words[factor1._wordsLength] = (uint)carry;

            var result = new BigInteger(words);
            result._sign = factor1._sign * factor2 < 0 ? -1 : 1;
            return result;
        }

        public static void Division(BigInteger divident, BigInteger divisor, out BigInteger quotient, out BigInteger remainder)
        {
            if (divident.CompareTo(divisor) < 0)
            {
                quotient = Zero;
                remainder = divident;
                return;
            }

            if (divident._sign > 0 && divisor._sign > 0)
            {
                BigInteger _quotient = null;
                BigInteger _remainder = null;
                UnsignedDivisionGeneral(divident, divisor, ref _quotient, ref _remainder, true, true);

                quotient = _quotient;
                quotient._sign = 1;

                remainder = _remainder;
                remainder._sign = 1;
            }
            else
                throw new NotImplementedException();
        }

        public static void Division(BigInteger divident, int divisor, out BigInteger quotient, out int remainder)
        {
            var division = divident.DivideByUInt32((uint)divisor);
            quotient = division.Item1;
            remainder = (int)division.Item2;
        }

        public static BigInteger Modulo(BigInteger dividend, BigInteger divisor)
        {
            if (dividend.CompareTo(divisor) < 0)
                return dividend;

            if (divisor._sign > 0)
            {
                BigInteger quotient = null;
                BigInteger remainder = null;
                UnsignedDivisionGeneral(dividend, divisor, ref quotient, ref remainder, false, true);
                if (!remainder.IsZero)
                    remainder._sign = dividend._sign;
                if (remainder._sign == -1)
                    remainder += divisor;
                return remainder;
            }
            else
                throw new NotImplementedException();
        }

        public static BigInteger ModularInverse(BigInteger value, BigInteger modulus)
        {
            if (value.IsZero)
                return null;

            if (modulus._sign != 1)
                throw new ArithmeticException("Modulus must be positive number");
            if (modulus == 1)
                return Zero;

            value = value % modulus;
            if (value == 1)
                return One;

            // Extended Euclidean
            var euclidean = ExtendedEuclidean(modulus, value);
            if (euclidean.d != 1)
                throw new NotImplementedException();
            if (euclidean.y < 0)
                euclidean.y += modulus;
            return euclidean.y;
        }

        public static BigInteger ModularDivision(BigInteger dividend, BigInteger divisor, BigInteger modulus)
        {
            if (divisor.IsZero)
                return null;
            if (dividend < 0)
                dividend += modulus;
            if (divisor < 0)
                divisor += modulus;
            return dividend * ModularInverse(divisor, modulus) % modulus;
        }

        public static BigInteger ModularExponentiation(BigInteger @base, BigInteger exponent, BigInteger modulus)
        {
            if (modulus.IsZero)
                throw new ArgumentException("Modulus cannot be zero.", nameof(modulus));
            if (modulus == 1)
                return Zero;
            if (@base >= modulus)
                @base = Modulo(@base, modulus);
            if (@base.IsZero)
                return Zero;
            if (exponent.IsZero)
                return One;

            if (modulus.IsOdd)
            {
                // use Montgomery exponentiation
                var rWords = new uint[modulus._wordsLength + 1];
                rWords[modulus._wordsLength] = 1;
                var r = new BigInteger(rWords);

                long mPrime = -ExtendedEuclideanInt64(0x100000000, modulus._words[0]).y;
                if (mPrime < 0)
                    mPrime += 0x100000000;
                uint mPrimeUInt32 = (uint)mPrime;

                // base * r = WordShift(base, modulus._wordsLength)
                var xPrime = WordShift(@base, modulus._wordsLength) % modulus;
                var a = r % modulus;

                for (int i = exponent.GetBitsLength() - 1; i >= 0; i--)
                {
                    a = MontgomeryMultiplication(a, a, modulus, mPrimeUInt32);
                    if (exponent.IsBitSet(i))
                        a = MontgomeryMultiplication(a, xPrime, modulus, mPrimeUInt32);
                }

                var one = new uint[modulus._wordsLength];
                one[0] = 1;
                a = MontgomeryMultiplication(a, new BigInteger(one, 1), modulus, mPrimeUInt32);

                return a;
            }
            else
            {
                // general method
                var result = One;
                for (int i = exponent.GetBitsLength() - 1; i >= 0; i--)
                {
                    result = result * result % modulus;
                    if (exponent.IsBitSet(i))
                        result = result * @base % modulus;
                }
                return result;
            }
        }

        /// <summary>
        /// <para>Calculates square root mod <param name="modulus">modulus</param></para>
        /// <para><param name="modulus">modulus</param> should be prime</para>
        /// </summary>
        public static BigInteger ModularSquareRoot(BigInteger value, BigInteger modulus)
        {
            // check if a ^ ((m - 1) / 2) mod m == 1
            // in this case there is no square root
            if (LegendreSymbol(value, modulus) != 1)
                return null;

            if (modulus._words[0] % 4 == 3)
            {
                // r = a ^ ((m + 1) / 4) mod m
                return ModularExponentiation(value, (modulus + 1) >> 2, modulus);
            }

            if (modulus._words[0] % 8 == 5)
            {
                var valuex2 = value << 1;
                // v = (2a) ^ ((m - 5) / 8) mod m
                var v = ModularExponentiation(valuex2, (modulus - 5) >> 3, modulus);
                // i = 2av^2 mod m
                var i = valuex2 * v * v % modulus;
                // r = av(i-1) mod m
                return value * v * (i - 1) % modulus;
            }

            // Shanks method
            if (modulus._words[0] % 8 == 1)
            {
                int e = 1;
                while (!modulus.IsBitSet(e))
                    e++;
                BigInteger s = modulus >> e;

                BigInteger n;
                do
                {
                    n = Random(modulus, _random);
                }
                while (LegendreSymbol(n, modulus) != -1);

                BigInteger x = ModularExponentiation(value, (s + 1) >> 1, modulus);
                BigInteger b = ModularExponentiation(value, s, modulus);
                BigInteger g = ModularExponentiation(n, s, modulus);
                int r = e;

                while (true)
                {
                    int m;
                    BigInteger b2pow = b;
                    for (m = 1; m < r; m++)
                    {
                        // b2pow = b ^ (2^m) mod modulus
                        b2pow = b2pow * b2pow % modulus;
                        if (b2pow == 1)
                            break;
                    }
                    if (m == r)
                        throw new ArithmeticException("Unexpected state of Shanks algorithm");

                    x = x * ModularExponentiation(g, One << (r - m - 1), modulus) % modulus;
                    b = b * ModularExponentiation(g, One << (r - m), modulus) % modulus;
                    g = ModularExponentiation(g, One << (r - m), modulus);
                    r = m;

                    if (b == 1)
                        return x;
                }
            }

            throw new NotImplementedException();
        }

        public string ToString(int radix)
        {
            return ToString(radix, DefaultSymbols);
        }

        public string ToString(int radix, char[] symbols)
        {
            if (IsZero)
                return symbols[0].ToString();

            if (radix == 2 || radix == 4 || radix == 16)
            {
                int radixLength;
                switch (radix)
                {
                    case 2: radixLength = 32; break;
                    case 4: radixLength = 16; break;
                    case 16: radixLength = 8; break;
                    default:
                        throw new InvalidOperationException();
                }
                char[] result = new char[radixLength * _wordsLength];
                char[] buffer = new char[radixLength];
                int index = 0;

                for (int i = 0; i < _wordsLength; i++)
                {
                    switch (radix)
                    {
                        case 2: UIntToString2(_words[i], symbols, buffer); break;
                        case 4: UIntToString4(_words[i], symbols, buffer); break;
                        case 16: UIntToString16(_words[i], symbols, buffer); break;
                        default:
                            throw new InvalidOperationException();
                    }
                    Array.Copy(buffer, 0, result, index, radixLength);
                    index += radixLength;
                }

                Array.Reverse(result);
                index = 0;
                while (result[index] == symbols[0])
                    index++;

                return (_sign < 0 ? "-" : "") + new string(result, index, result.Length - index);
            }
            else
            {
                int radixLength = DigitsPerUInt32[radix];
                uint radixUIn32 = RadixUInt32[radix];
                var remainder = this;
                char[] result = new char[(radixLength + 1) * (_wordsLength + 1)];
                char[] buffer = new char[radixLength];
                int index = 0;

                while (!remainder.IsZero)
                {
                    var division = remainder.DivideByUInt32(radixUIn32);
                    remainder = division.Item1;
                    UIntToString(division.Item2, (uint)radix, symbols, buffer);
                    Array.Copy(buffer, 0, result, index, radixLength);
                    index += radixLength;
                }

                Array.Reverse(result);
                index = 0;
                while (result[index] == symbols[0] || result[index] == '\0')
                    index++;
                return (_sign < 0 ? "-" : "") + new string(result, index, result.Length - index);
            }
        }

        public byte[] ToBytes(ByteOrder order)
        {
            if (IsZero)
                return new byte[0];

            uint high = _words[_wordsLength - 1];
            int highWordBytes = 1;
            if (high >= (1U << 8))
                highWordBytes = 2;
            if (high >= (1U << 16))
                highWordBytes = 3;
            if (high >= (1U << 24))
                highWordBytes = 4;

            byte[] result = new byte[(_wordsLength - 1) * 4 + highWordBytes];
            for (int i = 0; i < result.Length; i++)
            {
                if (order == ByteOrder.BigEndian)
                {
                    int i2 = result.Length - 1 - i;
                    result[i] = ByteFromUInt32(_words[i2 / 4], i2 % 4);
                }
                if (order == ByteOrder.LittleEndian)
                {
                    result[i] = ByteFromUInt32(_words[i / 4], i % 4);
                }
            }

            return result;
        }

        /// <summary>
        /// length - pads zeros if resulting array is shorter
        /// </summary>
        public byte[] ToBytes(ByteOrder order, int length)
        {
            byte[] result = new byte[length];
            for (int i = 0; i < length; i++)
            {
                if (order == ByteOrder.BigEndian)
                {
                    int i2 = length - 1 - i;
                    result[i] = ByteFromUInt32(SafeGetUInt32(_words, _wordsLength, i2 / 4), i2 % 4);
                }
                if (order == ByteOrder.LittleEndian)
                {
                    result[i] = ByteFromUInt32(SafeGetUInt32(_words, _wordsLength, i / 4), i % 4);
                }
            }

            return result;
        }

        #endregion

        #region System.Object overrides

        public override bool Equals(object obj)
        {
            if (obj is BigInteger)
                return this.Equals(obj as BigInteger);
            else
                return false;
        }

        public override int GetHashCode()
        {
            int result = 2057369101;
            for (int i = 0; i < this._wordsLength; i++)
            {
                int shift = i * 59 % 32;
                result = result ^ (int)((_words[i] << shift) | (_words[i] >> (32 - shift)));
            }
            return result;
        }

        public override string ToString()
        {
            return ToString(10);
        }

        #endregion

        #region IEquatable<BigInteger>

        public bool Equals(BigInteger other)
        {
            if (this._words == other._words)
                return true;

            if (this._wordsLength != other._wordsLength)
                return false;

            for (int i = 0; i < this._wordsLength; i++)
                if (this._words[i] != other._words[i])
                    return false;

            return true;
        }

        #endregion

        #region IEquatable<int>

        public bool Equals(int other)
        {
            if (other == 0)
                return IsZero;
            if (other < 0)
                return _wordsLength == 1 && _words[0] == -other && _sign == -1;
            else
                return _wordsLength == 1 && _words[0] == other && _sign == 1;
        }

        #endregion

        #region IComparable<BigInteger>

        public int CompareTo(BigInteger other)
        {
            if (this._words == other._words)
                return 0;

            int compare = this._wordsLength.CompareTo(other._wordsLength);
            if (compare != 0)
                return compare;

            for (int i = this._wordsLength - 1; i >= 0; i--)
            {
                compare = this._words[i].CompareTo(other._words[i]);
                if (compare != 0)
                    return compare;
            }

            return 0;
        }

        #endregion

        #region IComparable<long>

        public int CompareTo(long other)
        {
            if (IsZero)
                return (0L).CompareTo(other);

            if (_sign < 0 && other >= 0)
                return -1;
            if (_sign > 0 && other <= 0)
                return 1;

            if (_wordsLength > 2)
                return other > 0 ? 1 : -1;

            if (_wordsLength == 1)
                return (_sign * (long)_words[0]).CompareTo(other);

            int compare;
            if (other == long.MinValue)
            {
                compare = _words[1].CompareTo(uint.MaxValue);
                if (compare != 0)
                    return -compare;
                return -_words[0].CompareTo(uint.MaxValue);
            }
            else
            {
                int sign = other > 0 ? 1 : -1;
                if (sign < 0)
                    other = -other;
                compare = ((long)_words[1]).CompareTo(other >> 32);
                if (compare != 0)
                    return sign * compare;
                return sign * ((long)_words[0]).CompareTo(other & 0xFFFFFFFF);
            }
        }

        #endregion

        #region IComparable<int>

        public int CompareTo(int other)
        {
            if (this.IsZero)
                return (0).CompareTo(other);

            if (_sign < 0 && other >= 0)
                return -1;
            if (_sign > 0 && other <= 0)
                return 1;

            if (this._wordsLength > 1)
                return other > 0 ? 1 : -1;
            if (other < 0)
                return -_words[0].CompareTo((uint)(-other));
            else
                return _words[0].CompareTo((uint)other);
        }

        #endregion

        #region Operators

        #region Comparison

        public static bool operator==(BigInteger left, BigInteger right)
        {
            bool leftnull = ReferenceEquals(left, null);
            bool rightnull = ReferenceEquals(right, null);
            if (leftnull && rightnull)
                return true;
            if (leftnull ^ rightnull)
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(BigInteger left, BigInteger right)
        {
            return !(left == right);
        }

        public static bool operator ==(BigInteger left, int right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BigInteger left, int right)
        {
            return !left.Equals(right);
        }

        public static bool operator >=(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(BigInteger left, int right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(BigInteger left, int right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(BigInteger left, long right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(BigInteger left, long right)
        {
            return left.CompareTo(right) < 0;
        }

        #endregion

        #region Bit

        public static BigInteger operator >>(BigInteger left, int right)
        {
            return ShiftRight(left, right);
        }

        public static BigInteger operator <<(BigInteger left, int right)
        {
            return ShiftLeft(left, right);
        }

        #endregion

        #region Arithmetic

        public static BigInteger operator +(BigInteger left, BigInteger right)
        {
            return Sum(left, right);
        }

        public static BigInteger operator +(BigInteger left, int right)
        {
            return Sum(left, right);
        }

        public static BigInteger operator -(BigInteger left, BigInteger right)
        {
            return Difference(left, right);
        }

        public static BigInteger operator -(BigInteger left, int right)
        {
            return Difference(left, right);
        }

        public static BigInteger operator *(BigInteger left, BigInteger right)
        {
            return Product(left, right);
        }

        public static BigInteger operator *(BigInteger left, int right)
        {
            return Product(left, right);
        }

        public static BigInteger operator *(int left, BigInteger right)
        {
            return Product(right, left);
        }

        public static BigInteger operator /(BigInteger left, BigInteger right)
        {
            BigInteger quotient, remainder;
            Division(left, right, out quotient, out remainder);
            return quotient;
        }

        public static BigInteger operator /(BigInteger left, int right)
        {
            BigInteger quotient;
            int remainder;
            Division(left, right, out quotient, out remainder);
            return quotient;
        }

        public static BigInteger operator %(BigInteger left, BigInteger right)
        {
            return Modulo(left, right);
        }

        #endregion

        #region Conversion

        public static explicit operator int(BigInteger value)
        {
            if (value < int.MinValue || value > int.MaxValue)
                throw new ArgumentOutOfRangeException();

            if (value.IsZero)
                return 0;
            return (int)(value._words[0] * value._sign);
        }

        public static explicit operator long(BigInteger value)
        {
            if (value < long.MinValue || value > long.MaxValue)
                throw new ArgumentOutOfRangeException();

            switch (value._wordsLength)
            {
                case 0:
                    return 0;
                case 1:
                    return checked(value._words[0] * value._sign);
                case 2:
                    if (value._sign == -1 && value._words[0] == uint.MaxValue && value._words[1] == uint.MaxValue)
                        return long.MinValue;
                    return checked(value._sign * (((long)value._words[1] << 32) | value._words[0]));
                default:
                    throw new InvalidOperationException();
            }
        }

        #endregion

        #endregion

        #region Internal helper methods

        internal static int BitSizeOfArray(byte[] data, ByteOrder order)
        {
            if (order == ByteOrder.BigEndian)
            {
                int index = 0;
                while (index < data.Length && data[index] == 0)
                    index++;
                if (index == data.Length)
                    return 0;
                int result = 8 * (data.Length - 1 - index);
                int high = data[index];
                int power2 = 1;
                while (high >= power2)
                {
                    result++;
                    power2 <<= 1;
                }
                return result;
            }
            throw new NotImplementedException();
        }

        #endregion

        #region Private instance methods

        private void CopyFrom(BigInteger value)
        {
            this._words = value._words;
            this._wordsLength = value._wordsLength;
        }

        private void TruncateLeadingZeros()
        {
            while (_wordsLength > 0 && _words[_wordsLength - 1] == 0)
                _wordsLength--;
            if (_wordsLength == 0)
                _sign = 0;
        }

        private int GetBitsLength()
        {
            if (IsZero)
                return 0;

            int result = _wordsLength * 32;
            int shift = 31;
            while ((_words[_wordsLength - 1] & (1 << shift)) == 0)
            {
                shift--;
                result--;
            }

            return result;
        }

        #endregion

        #region Private math helper methods

        private static uint[] UnsignedSum(BigInteger summand1, BigInteger summand2)
        {
            uint[] words = new uint[System.Math.Max(summand1._wordsLength, summand2._wordsLength) + 1];
            long carry = 0;
            for (int i = 0; i < words.Length; i++)
            {
                long sum = carry;
                if (i < summand1._wordsLength)
                    sum += summand1._words[i];
                if (i < summand2._wordsLength)
                    sum += summand2._words[i];

                words[i] = (uint)(sum & 0xFFFFFFFF);
                carry = sum >> 32;
            }
            return words;
        }

        /// <summary>
        /// Assumes Abs(minuend) >= Abs(subtrahend)
        /// </summary>
        private static uint[] UnsignedDifference(BigInteger minuend, BigInteger subtrahend)
        {
            uint[] words = new uint[minuend._wordsLength];
            long carry = 0;
            for (int i = 0; i < words.Length; i++)
            {
                long diff = minuend._words[i] + carry;
                if (i < subtrahend._wordsLength)
                    diff -= subtrahend._words[i];

                if (diff < 0)
                {
                    diff += 0x100000000;
                    carry = -1;
                }
                else
                    carry = 0;

                words[i] = (uint)(diff & 0xFFFFFFFF);
            }
            return words;
        }

        private static int UnsignedCompare(BigInteger value1, BigInteger value2)
        {
            if (value1._words == value2._words)
                return 0;

            int compare = value1._wordsLength.CompareTo(value2._wordsLength);
            if (compare != 0)
                return compare;

            for (int i = value1._wordsLength - 1; i >= 0; i--)
            {
                compare = value1._words[i].CompareTo(value2._words[i]);
                if (compare != 0)
                    return compare;
            }

            return 0;
        }

        /// <summary>
        /// Parses string into BigInteger. Allowed radix: 2, 4, 16
        /// </summary>
        private static BigInteger ParsePowerOfTwo(string value, int radix)
        {
            int bitsPerDigit;
            switch (radix)
            {
                case 2:
                    bitsPerDigit = 1;
                    break;
                case 4:
                    bitsPerDigit = 2;
                    break;
                case 16:
                    bitsPerDigit = 4;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            uint[] words = new uint[(value.Length * bitsPerDigit + 31) / 32];
            int cursor = 0;
            int groupLen = 32 / bitsPerDigit;
            int firstGroupLen = value.Length % groupLen;
            if (firstGroupLen == 0)
                firstGroupLen = groupLen;

            string group = value.Substring(cursor, firstGroupLen);
            cursor += firstGroupLen;
            words[words.Length - 1] = Convert.ToUInt32(group, radix);

            for (int i = words.Length - 2; i >= 0; i--)
            {
                group = value.Substring(cursor, groupLen);
                cursor += groupLen;
                words[i] = Convert.ToUInt32(group, radix);
            }

            return new BigInteger(words);
        }

        private Tuple<BigInteger, uint> DivideByUInt32(uint divisor)
        {
            var quotient = new uint[_wordsLength];
            long remainder = 0;
            for (int i = quotient.Length - 1; i >= 0; i--)
            {
                remainder = (remainder << 32) | _words[i];
                quotient[i] = (uint)(remainder / divisor);
                remainder = remainder % divisor;
            }
            return new Tuple<BigInteger, uint>(new BigInteger(quotient), (uint)remainder);
        }

        /// <summary>
        /// words = words * mult + add
        /// </summary>
        private static void DestructiveMulAdd(uint[] words, int wordsLength, uint mult, uint add)
        {
            ulong carry = 0;
            for (int i = 0; i < wordsLength; i++)
            {
                ulong product = (ulong)words[i] * mult + carry;
                words[i] = (uint)(product & 0xFFFFFFFF);
                carry = product >> 32;
            }
            if (carry > 0)
                words[wordsLength] = (uint)carry;

            carry = add;
            for (int i = 0; i <= wordsLength; i++)
            {
                ulong sum = words[i] + carry;
                words[i] = (uint)(sum & 0xFFFFFFFF);
                carry = sum >> 32;

                if (carry == 0)
                    break;
            }
        }

        /// <summary>
        /// Assumes this._wordsLength >= value._wordsLength, both values != 0
        /// </summary>
        private BigInteger MultiplyGeneral(BigInteger value)
        {
            uint[] result = new uint[this._wordsLength + value._wordsLength];
            if (value._wordsLength < KaratsubaBitsLength)
            {
                // O(n^2) algo
                for (int i1 = 0; i1 < this._wordsLength; i1++)
                    for (int i2 = 0; i2 < value._wordsLength; i2++)
                    {
                        int index = i1 + i2;
                        ulong carry = (ulong)this._words[i1] * value._words[i2];
                        do
                        {
                            carry += result[index];
                            result[index] = (uint)(carry & 0xFFFFFFFF);
                            carry = carry >> 32;
                            index++;
                        }
                        while (carry > 0);
                    }
            }
            else
            {
                // O(n^1.585) algo, http://en.wikipedia.org/wiki/Karatsuba_algorithm
                int half = value._wordsLength / 2;
                var t1 = this.Split(half);
                var t2 = value.Split(half);
                var low1 = t1.Item1;
                var low2 = t2.Item1;
                var high1 = t1.Item2;
                var high2 = t2.Item2;

                var z0 = low1.MultiplyGeneral(low2);
                var z1 = (low1 + high1).MultiplyGeneral(low2 + high2);
                var z2 = high1.MultiplyGeneral(high2);
                z1.DestructiveSubstract(z0);
                z1.DestructiveSubstract(z2);

                Array.Copy(z0._words, result, z0._wordsLength);
                WordShiftSum(result, z1, half);
                WordShiftSum(result, z2, 2 * half);
            }

            return new BigInteger(result);
        }

        /// <summary>
        /// Split bits into 2 parts. Length beginning from lower bits.
        /// </summary>
        private Tuple<BigInteger, BigInteger> Split(int length)
        {
            uint[] low = new uint[length];
            uint[] high = new uint[_wordsLength - length];
            Array.Copy(_words, 0, low, 0, length);
            Array.Copy(_words, length, high, 0, _wordsLength - length);

            return new Tuple<BigInteger, BigInteger>(new BigInteger(low), new BigInteger(high));
        }

        /// <summary>
        /// Assumes result >= 0
        /// this = this - value
        /// </summary>
        private void DestructiveSubstract(BigInteger value)
        {
            long carry = 0;
            for (int i = 0; i < this._wordsLength; i++)
            {
                long sub = this._words[i];
                sub += carry;
                if (i < value._wordsLength)
                    sub -= value._words[i];
                else
                    if (carry == 0)
                        break;
                if (sub < 0)
                {
                    carry = -1;
                    sub += UInt32Overflow;
                }
                else
                    carry = 0;
                this._words[i] = (uint)sub;
            }

            TruncateLeadingZeros();
        }

        /// <summary>
        /// self += value &lt;&lt; shift
        /// Assumes no overflow
        /// </summary>
        private static void WordShiftSum(uint[] words, BigInteger value, int shift)
        {
            long carry = 0;
            for (int i = 0; i < value._wordsLength; i++)
            {
                long sum = (long)words[shift + i] + value._words[i] + carry;
                if (sum >= UInt32Overflow)
                {
                    words[shift + i] = (uint)(sum & 0xFFFFFFFF);
                    carry = 1;
                }
                else
                {
                    words[shift + i] = (uint)(sum);
                    carry = 0;
                }
            }
            for (int i = value._wordsLength + shift; ; i++)
            {
                if (carry == 0)
                    break;
                long sum = (long)words[i] + carry;
                if (sum >= UInt32Overflow)
                {
                    words[i] = (uint)(sum & 0xFFFFFFFF);
                    carry = 1;
                }
                else
                {
                    words[i] = (uint)(sum);
                    carry = 0;
                }
            }
        }

        private static void UnsignedDivisionGeneral(BigInteger divident, BigInteger divisor, ref BigInteger quotient, ref BigInteger remainder, bool calcQuotient, bool calcRemainder)
        {
            var x = new uint[divident._wordsLength + 1];
            var y = new uint[divisor._wordsLength];

            Array.Copy(divident._words, x, divident._wordsLength);
            Array.Copy(divisor._words, y, divisor._wordsLength);

            // calculate normalizing shift
            int shift = 0;
            uint yHighWord1 = y[divisor._wordsLength - 1];
            while ((yHighWord1 & 0x80000000) == 0)
            {
                yHighWord1 <<= 1;
                shift++;
            }

            int xLen = BitShiftLeftNoResize(x, divident._wordsLength + 1, shift);
            int yLen = BitShiftLeftNoResize(y, divisor._wordsLength, shift);

            uint[] q = null;
            if (calcQuotient)
                q = new uint[xLen - yLen + 1];

            yHighWord1 = y[yLen - 1];
            uint yHighWord2 = SafeGetUInt32(y, yLen, yLen - 2);
            for (int i = xLen; i >= yLen; i--)
            {
                uint xHighWord1 = SafeGetUInt32(x, xLen, i);
                uint xHighWord2 = x[i - 1];
                uint xHighWord3 = SafeGetUInt32(x, xLen, i - 2);

                ulong nextQuotWordUInt64 = (((ulong)xHighWord1 << 32) | xHighWord2) / yHighWord1;
                uint nextQuotWord = nextQuotWordUInt64 == 0x100000000 ? 0xFFFFFFFF : (uint)nextQuotWordUInt64;

                ulong carry = (ulong)yHighWord2 * nextQuotWord;
                uint qyHighWord3 = (uint)(carry & 0xFFFFFFFF);
                carry = carry >> 32;

                carry = carry + (ulong)yHighWord1 * nextQuotWord;
                uint qyHighWord2 = (uint)(carry & 0xFFFFFFFF);
                uint qyHighWord1 = (uint)(carry >> 32);

                if (qyHighWord1 > xHighWord1)
                    nextQuotWord--;
                else
                    if (qyHighWord1 == xHighWord1 && qyHighWord2 > xHighWord2)
                        nextQuotWord--;
                    else
                        if (qyHighWord2 == xHighWord2 && qyHighWord3 > xHighWord3)
                            nextQuotWord--;

                if (nextQuotWord != 0)
                    if (!WordShiftSubstractLinearCombination(x, xLen, y, yLen, nextQuotWord, i - yLen))
                    {
                        nextQuotWord--;
                        WordShiftNegativeAdd(x, xLen, y, yLen, i - yLen);
                    }

                if (calcQuotient)
                    q[i - yLen] = nextQuotWord;

                while (xLen > 0 && x[xLen - 1] == 0)
                    xLen--;
            }

            // shift back normalization of x
            BitShiftRightNoResize(x, xLen, shift);

            if (calcQuotient)
                quotient = new BigInteger(q);
            if (calcRemainder)
                remainder = new BigInteger(x, xLen);
        }

        /// <summary>
        /// Assumes operation will not change array size
        /// Return resulted length
        /// </summary>
        private static int BitShiftLeftNoResize(uint[] words, int length, int shift)
        {
            if (shift != 0)
                for (int i = length - 1; i >= 0; i--)
                {
                    if (i > 0)
                        words[i] = (words[i] << shift) | (words[i - 1] >> (32 - shift));
                    else
                        words[i] = (words[i] << shift);
                }

            while (length > 0 && words[length - 1] == 0)
                length--;
            return length;
        }

        private static void BitShiftRightNoResize(uint[] words, int length, int shift)
        {
            if (shift != 0)
                for (int i = 0; i < length; i++)
                {
                    if (i + 1 < length)
                        words[i] = (words[i] >> shift) | (words[i + 1] << (32 - shift));
                    else
                        words[i] = (words[i] >> shift);
                }
        }

        /// <summary>
        /// x = x - (y * q) &lt;&lt; shift
        /// Return true if result >= 0
        /// </summary>
        private static bool WordShiftSubstractLinearCombination(uint[] x, int xLen, uint[] y, int yLen, uint q, int shift)
        {
            ulong multCarry = 0;
            long diffCarry = 0;
            for (int i = 0; i < yLen; i++)
            {
                ulong mult = (ulong)y[i] * q + multCarry;
                multCarry = mult >> 32;
                long diff = (long)x[i + shift] - (uint)(mult & 0xFFFFFFFF) + diffCarry;
                x[i + shift] = (uint)(diff & 0xFFFFFFFF);
                diffCarry = diff >> 32;
            }

            int index = yLen;
            while ((multCarry > 0 || diffCarry != 0) && index + shift < xLen)
            {
                long diff = (long)x[index + shift] - (uint)multCarry + diffCarry;
                multCarry = 0;
                x[index + shift] = (uint)(diff & 0xFFFFFFFF);
                diffCarry = diff >> 32;

                index++;
            }
            return diffCarry >= 0;
        }

        /// <summary>
        /// x &lt; 0, as result of WordShiftSubstractLinearCombination method
        /// x = x + (y &lt;&lt; shift)
        /// </summary>
        private static void WordShiftNegativeAdd(uint[] x, int xLen, uint[] y, int yLen, int shift)
        {
            long carry = 0;
            for (int i = 0; i < yLen; i++)
            {
                carry = carry + x[i + shift] + y[i];
                x[i + shift] = (uint)(carry & 0xFFFFFFFF);
                carry = carry >> 32;
            }

            int index = yLen;
            while (carry != 0 && shift + index < xLen)
            {
                carry = carry + x[index + shift];
                x[index + shift] = (uint)(carry & 0xFFFFFFFF);
                carry = carry >> 32;

                index++;
            }
        }

        /// <summary>
        /// value1 >= value2
        /// Returns: d = gcd(a, b); ax+by=d
        /// </summary>
        internal static ExtEuclideanResult<long> ExtendedEuclideanInt64(long a, long b)
        {
            if (a < b)
                throw new ArgumentException("a should be greater than b.");
            if (b == 0)
                return new ExtEuclideanResult<long>
                {
                    d = a,
                    x = 1,
                    y = 0
                };

            long x1 = 0, x2 = 1, y1 = 1, y2 = 0, x, y;
            while (b > 0)
            {
                long q = a / b;
                long r = a % b;
                x = x2 - q * x1;
                y = y2 - q * y1;
                a = b;
                b = r;
                x2 = x1;
                x1 = x;
                y2 = y1;
                y1 = y;
            }
            return new ExtEuclideanResult<long>
            {
                d = a,
                x = x2,
                y = y2
            };
        }

        /// <summary>
        /// value1 >= value2
        /// Returns: d = gcd(a, b); ax+by=d
        /// </summary>
        internal static ExtEuclideanResult<BigInteger> ExtendedEuclidean(BigInteger a, BigInteger b)
        {
            if (a < b)
                throw new ArgumentException("a should be greater than b.");
            if (b.IsZero)
                return new ExtEuclideanResult<BigInteger>
                {
                    d = a,
                    x = One,
                    y = Zero
                };

            BigInteger x1 = Zero, x2 = One, y1 = One, y2 = Zero, x, y;
            while (b > 0)
            {
                BigInteger q, r;
                Division(a, b, out q, out r);
                x = x2 - q * x1;
                y = y2 - q * y1;
                a = b;
                b = r;
                x2 = x1;
                x1 = x;
                y2 = y1;
                y1 = y;
            }
            return new ExtEuclideanResult<BigInteger>
            {
                d = a,
                x = x2,
                y = y2
            };
        }

        internal struct ExtEuclideanResult<T>
        {
            public T x, y, d;
        }

        /// <summary>
        /// m - odd
        /// mInv = - m^(-1) mod 0x100000000
        /// </summary>
        internal static BigInteger MontgomeryMultiplication(BigInteger x, BigInteger y, BigInteger m, uint mInv)
        {
            if (x >= m || y >= m)
                throw new InvalidOperationException();
            checked
            {
                var result = new uint[m._wordsLength + 2];
                for (int i = 0; i < m._wordsLength; i++)
                {
                    uint u = (uint)((((result[0] + (ulong)x._words[i] * y._words[0]) & 0xFFFFFFFF) * mInv) & 0xFFFFFFFF);

                    ulong carry = 0;
                    ulong sum;
                    for (int j = 0; j < m._wordsLength; j++)
                    {
                        sum = carry + result[j] + (ulong)x._words[i] * y._words[j];
                        carry = sum >> 32;
                        result[j] = (uint)(sum & 0xFFFFFFFF);
                    }
                    sum = result[m._wordsLength] + carry;
                    result[m._wordsLength] = (uint)(sum & 0xFFFFFFFF);
                    result[m._wordsLength + 1] += (uint)(sum >> 32);

                    carry = 0;
                    for (int j = 0; j < m._wordsLength; j++)
                    {
                        sum = carry + result[j] + (ulong)u * m._words[j];
                        carry = sum >> 32;
                        result[j] = (uint)(sum & 0xFFFFFFFF);
                    }
                    sum = result[m._wordsLength] + carry;
                    result[m._wordsLength] = (uint)(sum & 0xFFFFFFFF);
                    result[m._wordsLength + 1] += (uint)(sum >> 32);

                    if (result[0] != 0)
                        throw new Exception("Error occured");
                    for (int j = 0; j <= m._wordsLength; j++)
                        result[j] = result[j + 1];
                    result[m._wordsLength + 1] = 0;
                }

                // check if result > m
                bool greater = false;
                if (result[m._wordsLength] > 0)
                    greater = true;
                else
                    for (int i = m._wordsLength - 1; i >= 0; i--)
                        if (result[i] > m._words[i])
                        {
                            greater = true;
                            break;
                        }
                        else
                            if (result[i] < m._words[i])
                            break;

                if (greater)
                {
                    long carry = 0;
                    for (int i = 0; i < m._wordsLength; i++)
                    {
                        long diff = carry + result[i] - m._words[i];
                        result[i] = (uint)(diff & 0xFFFFFFFF);
                        carry = diff >> 32;
                    }
                }

                return new BigInteger(result, m._wordsLength);
            }
        }

        internal static BigInteger WordShift(BigInteger value, int shift)
        {
            if (value.IsZero)
                return Zero;

            var words = new uint[value._wordsLength + shift];
            Array.Copy(value._words, 0, words, shift, value._wordsLength);

            return new BigInteger(words, words.Length);
        }

        private static int LegendreSymbol(BigInteger a, BigInteger p)
        {
            var minusOne = p - 1;
            var result = ModularExponentiation(a, minusOne >> 1, p);
            if (result.IsZero)
                return 0;
            if (result == minusOne)
                return -1;
            if (result == 1)
                return 1;

            throw new ArithmeticException("Unexpected result");
        }

        #endregion

        #region Private static members

        private const long UInt32Overflow = 1L << 32;
        private const int MaxRadixSupported = 64;
        private static readonly int[] DigitsPerUInt32 = new int[MaxRadixSupported];
        private static readonly uint[] RadixUInt32 = new uint[MaxRadixSupported];
        private static char[] DefaultSymbols = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();

        private const int KaratsubaBitsLength = 30;

        static BigInteger()
        {
            for (int i = 2; i < MaxRadixSupported; i++)
            {
                DigitsPerUInt32[i] = 1;
                RadixUInt32[i] = (uint)i;
                while (RadixUInt32[i] * i < uint.MaxValue)
                {
                    RadixUInt32[i] *= (uint)i;
                    DigitsPerUInt32[i]++;
                }
            }
        }

        static void UIntToString(uint value, uint radix, char[] symbols, char[] result)
        {
            int index = 0;
            while (value > 0)
            {
                uint rem = value % radix;
                result[index++] = symbols[rem];
                value /= radix;
            }
            while (index < result.Length)
                result[index++] = symbols[0];
        }

        static void UIntToString2(uint value, char[] symbols, char[] result)
        {
            for (int i = 0; i < 32; i++)
            {
                result[i] = symbols[value & 0x01];
                value >>= 1;
            }
        }

        static void UIntToString4(uint value, char[] symbols, char[] result)
        {
            for (int i = 0; i < 16; i++)
            {
                result[i] = symbols[value & 0x03];
                value >>= 2;
            }
        }

        static void UIntToString16(uint value, char[] symbols, char[] result)
        {
            for (int i = 0; i < 8; i++)
            {
                result[i] = symbols[value & 0x0F];
                value >>= 4;
            }
        }

        static uint StringToUInt32(string value, int radix, char[] symbols)
        {
            uint result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                int index = -1;
                for (int j = 0; j < symbols.Length; j++)
                    if (symbols[j] == value[i])
                    {
                        index = j;
                        break;
                    }
                if (index == -1)
                    throw new InvalidOperationException();
                result = result * (uint)radix + (uint)index;
            }
            return result;
        }

        private static byte SafeGetByte(byte[] data, int dataLen, int index)
        {
            return (0 <= index) && (index < dataLen) ? data[index] : (byte)0;
        }

        private static uint SafeGetUInt32(uint[] data, int dataLen, int index)
        {
            return (0 <= index) && (index < dataLen) ? data[index] : 0;
        }

        private static byte ByteFromUInt32(uint value, int index)
        {
            if (index == 0)
                return (byte)(value & 0xFF);
            if (index == 1)
                return (byte)((value >> 8) & 0xFF);
            if (index == 2)
                return (byte)((value >> 16) & 0xFF);
            if (index == 3)
                return (byte)((value >> 24) & 0xFF);
            throw new InvalidOperationException();
        }

        #endregion
    }

#else

    public class BigInteger : IComparable<BigInteger>, IComparable<int>, IEquatable<BigInteger>, IEquatable<int>
    {
        internal IntPtr BIGNUM;
        private bool _disposable;

        internal static readonly IntPtr BN_CTX = OpenSSL.BN_CTX_new();

        #region Public properties

        public bool IsZero => OpenSSL.BN_is_zero(BIGNUM) == 1;

        public bool IsOdd => OpenSSL.BN_is_odd(BIGNUM) == 1;

        public bool IsEven => OpenSSL.BN_is_odd(BIGNUM) == 0;

        public int BitSize => OpenSSL.BN_num_bits(BIGNUM);

        #endregion

        #region Public constants

        public static readonly BigInteger Zero = new BigInteger(OpenSSL.BN_new());
        public static readonly BigInteger One = new BigInteger(OpenSSL.BN_value_one(), false);

        #endregion

        #region Contructors

        private BigInteger(IntPtr bignum)
            : this(bignum, true)
        {
            
        }

        private BigInteger(IntPtr bignum, bool disposable)
        {
            if (bignum == IntPtr.Zero)
                throw new InvalidOperationException();

            this.BIGNUM = bignum;
            this._disposable = disposable;
        }

        public BigInteger(int value)
        {
            this.BIGNUM = OpenSSL.BN_new();
            if (value >= 0)
                OpenSSL.BN_set_word(this.BIGNUM, (ulong)value);
            else
            {
                OpenSSL.BN_set_word(this.BIGNUM, (ulong)(-(long)value));
                OpenSSL.BN_set_negative(this.BIGNUM, 1);
            }
        }

        public BigInteger(long value)
        {
            this.BIGNUM = OpenSSL.BN_new();
            if (value >= 0)
                OpenSSL.BN_set_word(this.BIGNUM, (ulong)value);
            else
            {
                if (value != long.MinValue)
                    OpenSSL.BN_set_word(this.BIGNUM, (ulong)(-value));
                else
                    OpenSSL.BN_set_word(this.BIGNUM, 0x8000000000000000);
                OpenSSL.BN_set_negative(this.BIGNUM, 1);
            }
        }

        public BigInteger(byte[] data, int offset, int length, ByteOrder order)
        {
            byte[] buffer = new byte[length];
            Array.Copy(data, offset, buffer, 0, length);

            if (order == ByteOrder.LittleEndian)
                Array.Reverse(buffer);

            this.BIGNUM = OpenSSL.BN_bin2bn(buffer, length, IntPtr.Zero);
            if (this.BIGNUM == IntPtr.Zero)
                throw new InvalidOperationException();
        }

        public BigInteger(byte[] data, ByteOrder order)
            : this(data, 0, data.Length, order)
        {

        }

        public BigInteger(uint[] words, ByteOrder order)
            : this(BitHelper.ToByteArray(words, order), order)
        {

        }

        public static BigInteger Parse(string value, int radix = 10)
        {
            return Parse(value, radix, DefaultSymbols);
        }

        public static BigInteger Parse(string value, int radix, char[] symbols)
        {
            if (radix == 10 && symbols == DefaultSymbols)
            {
                IntPtr bignum = IntPtr.Zero;
                OpenSSL.BN_dec2bn(ref bignum, value);
                return new BigInteger(bignum);
            }
            if (radix == 16 && symbols == DefaultSymbols)
            {
                IntPtr bignum = IntPtr.Zero;
                OpenSSL.BN_hex2bn(ref bignum, value);
                return new BigInteger(bignum);
            }

            bool negative = value[0] == '-';
            if (negative)
                value = value.Substring(1);

            int[] indexes = new int[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                int index = -1;
                for (int j = 0; j < radix; j++)
                    if (symbols[j] == value[i])
                    {
                        index = j;
                        break;
                    }

                if (index == -1)
                    throw new InvalidOperationException();

                indexes[i] = index;
            }

            IntPtr res = OpenSSL.BN_new();
            try
            {
                ulong rad = checked((ulong)radix);
                for (int i = 0; i < indexes.Length; i++)
                {
                    OpenSSL.BN_mul_word(res, rad);
                    OpenSSL.BN_add_word(res, checked((ulong)indexes[i]));
                }
            }
            catch
            {
                OpenSSL.BN_free(res);
                throw;
            }

            if (negative)
                OpenSSL.BN_set_negative(res, 1);

            return new BigInteger(res);
        }

        /// <summary>
        /// Generates random number in range [0..value-1]
        /// </summary>
        public static BigInteger Random(BigInteger value, IRandom random)
        {
            if (value.IsZero)
                throw new InvalidOperationException();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates random number in range [from..to-1]
        /// </summary>
        public static BigInteger Random(BigInteger from, BigInteger to, IRandom random)
        {
            if (from >= to)
                throw new InvalidOperationException();

            return Random(to - from, random) + from;
        }

        ~BigInteger()
        {
            if (this.BIGNUM != IntPtr.Zero)
            {
                if (_disposable)
                    OpenSSL.BN_free(this.BIGNUM);
                this.BIGNUM = IntPtr.Zero;
            }
        }

        #endregion

        #region Public methods

        public bool IsBitSet(int position)
        {
            return OpenSSL.BN_is_bit_set(this.BIGNUM, position) == 1;
        }

        public static BigInteger Sum(BigInteger summand1, BigInteger summand2)
        {
            BigInteger result = new BigInteger(OpenSSL.BN_new());
            OpenSSL.BN_add(result.BIGNUM, summand1.BIGNUM, summand2.BIGNUM);
            return result;
        }

        public static BigInteger Sum(BigInteger summand1, int summand2)
        {
            if (summand2 == 0)
                return summand1;
            if (summand2 > 0)
            {
                BigInteger result = new BigInteger(OpenSSL.BN_dup(summand1.BIGNUM));
                OpenSSL.BN_add_word(result.BIGNUM, (ulong)summand2);
                return result;
            }
            else
            {
                BigInteger result = new BigInteger(OpenSSL.BN_dup(summand1.BIGNUM));
                OpenSSL.BN_sub_word(result.BIGNUM, (ulong)(-summand2));
                return result;
            }
        }

        public static BigInteger Difference(BigInteger minuend, BigInteger subtrahend)
        {
            BigInteger result = new BigInteger(OpenSSL.BN_new());
            OpenSSL.BN_sub(result.BIGNUM, minuend.BIGNUM, subtrahend.BIGNUM);
            return result;
        }

        public static BigInteger Difference(BigInteger minuend, int subtrahend)
        {
            if (subtrahend == 0)
                return minuend;

            if (subtrahend > 0)
            {
                BigInteger result = new BigInteger(OpenSSL.BN_dup(minuend.BIGNUM));
                OpenSSL.BN_sub_word(result.BIGNUM, (ulong)subtrahend);
                return result;
            }
            else
            {
                BigInteger result = new BigInteger(OpenSSL.BN_dup(minuend.BIGNUM));
                OpenSSL.BN_add_word(result.BIGNUM, (ulong)(-subtrahend));
                return result;
            }
        }

        public BigInteger AdditiveInverse()
        {
            if (IsZero)
                return Zero;

            BigInteger result = new BigInteger(OpenSSL.BN_dup(this.BIGNUM));
            if (OpenSSL.BN_is_negative(result.BIGNUM) == 1)
                OpenSSL.BN_set_negative(result.BIGNUM, 0);
            else
                OpenSSL.BN_set_negative(result.BIGNUM, 1);
            return result;
        }

        public static BigInteger ShiftRight(BigInteger value, int bits)
        {
            BigInteger result = new BigInteger(OpenSSL.BN_new());
            OpenSSL.BN_rshift(result.BIGNUM, value.BIGNUM, bits);
            return result;
        }

        public static BigInteger ShiftLeft(BigInteger value, int bits)
        {
            BigInteger result = new BigInteger(OpenSSL.BN_new());
            OpenSSL.BN_lshift(result.BIGNUM, value.BIGNUM, bits);
            return result;
        }

        public static BigInteger Product(BigInteger factor1, BigInteger factor2)
        {
            BigInteger result = new BigInteger(OpenSSL.BN_new());
            OpenSSL.BN_mul(result.BIGNUM, factor1.BIGNUM, factor2.BIGNUM, BN_CTX);
            return result;
        }

        public static BigInteger Product(BigInteger factor1, int factor2)
        {
            if (factor1.IsZero || factor2 == 0)
                return Zero;

            if (factor2 > 0)
            {
                BigInteger result = new BigInteger(OpenSSL.BN_dup(factor1.BIGNUM));
                OpenSSL.BN_mul_word(result.BIGNUM, (ulong)factor2);
                return result;
            }
            else
            {
                BigInteger result = new BigInteger(OpenSSL.BN_dup(factor1.BIGNUM));
                OpenSSL.BN_mul_word(result.BIGNUM, (ulong)(-factor2));
                if (OpenSSL.BN_is_negative(result.BIGNUM) == 1)
                    OpenSSL.BN_set_negative(result.BIGNUM, 0);
                else
                    OpenSSL.BN_set_negative(result.BIGNUM, 1);
                return result;
            }
        }

        public static void Division(BigInteger dividend, BigInteger divisor, out BigInteger quotient, out BigInteger remainder)
        {
            quotient = new BigInteger(OpenSSL.BN_new());
            remainder = new BigInteger(OpenSSL.BN_new());
            OpenSSL.BN_div(quotient.BIGNUM, remainder.BIGNUM, dividend.BIGNUM, divisor.BIGNUM, BN_CTX);
        }

        public static void Division(BigInteger dividend, int divisor, out BigInteger quotient, out int remainder)
        {
            BigInteger divisorBigInt = new BigInteger(divisor);
            quotient = new BigInteger(OpenSSL.BN_new());
            IntPtr remainderBigNum = OpenSSL.BN_new();
            OpenSSL.BN_div(quotient.BIGNUM, remainderBigNum, dividend.BIGNUM, divisorBigInt.BIGNUM, BN_CTX);
            remainder = checked((int)OpenSSL.BN_get_word(remainderBigNum));
        }

        public static BigInteger Modulo(BigInteger dividend, BigInteger divisor)
        {
            BigInteger result = new BigInteger(OpenSSL.BN_new());
            OpenSSL.BN_div(IntPtr.Zero, result.BIGNUM, dividend.BIGNUM, divisor.BIGNUM, BN_CTX);
            return result;
        }

        public static BigInteger ModularInverse(BigInteger value, BigInteger modulus)
        {
            return new BigInteger(OpenSSL.BN_mod_inverse(IntPtr.Zero, value.BIGNUM, modulus.BIGNUM, BN_CTX));
        }

        public static BigInteger ModularDivision(BigInteger dividend, BigInteger divisor, BigInteger modulus)
        {
            if (dividend < 0)
                dividend += modulus;
            if (divisor < 0)
                divisor += modulus;
            return dividend * ModularInverse(divisor, modulus) % modulus;
        }

        public static BigInteger ModularExponentiation(BigInteger @base, BigInteger exponent, BigInteger modulus)
        {
            if (modulus.IsZero)
                throw new ArgumentException("Modulus cannot be zero.", nameof(modulus));
            if (modulus == 1)
                return Zero;
            if (@base >= modulus)
                @base = Modulo(@base, modulus);
            if (@base.IsZero)
                return Zero;
            if (exponent.IsZero)
                return One;

            BigInteger result = new BigInteger(OpenSSL.BN_new());
            if (OpenSSL.BN_mod_exp(result.BIGNUM, @base.BIGNUM, exponent.BIGNUM, modulus.BIGNUM, BN_CTX) != 1)
                throw new InvalidOperationException();

            return result;
        }

        /// <summary>
        /// <para>Calculates square root mod <param name="modulus">modulus</param></para>
        /// <para><param name="modulus">modulus</param> should be prime</para>
        /// </summary>
        public static BigInteger ModularSquareRoot(BigInteger value, BigInteger modulus)
        {
            IntPtr bignum = OpenSSL.BN_mod_sqrt(IntPtr.Zero, value.BIGNUM, modulus.BIGNUM, BN_CTX);
            if (bignum == IntPtr.Zero)
                return null;

            return new BigInteger(bignum);
        }

        public string ToString(int radix)
        {
            return ToString(radix, DefaultSymbols);
        }

        public string ToString(int radix, char[] symbols)
        {
            if (IsZero)
                return symbols[0].ToString();

            if (radix == 10 && symbols == DefaultSymbols)
                return OpenSSL.BN_bn2dec(this.BIGNUM);

            if (radix == 16 && symbols == DefaultSymbols)
                return OpenSSL.BN_bn2hex(this.BIGNUM).TrimStart('0');

            List<char> list = new List<char>();
            bool negative = OpenSSL.BN_is_negative(this.BIGNUM) == 1;
            IntPtr rad = OpenSSL.BN_new();
            IntPtr num = OpenSSL.BN_dup(this.BIGNUM);
            IntPtr div = OpenSSL.BN_new();
            IntPtr rem = OpenSSL.BN_new();
            try
            {
                OpenSSL.BN_set_word(rad, checked((ulong)radix));
                while (OpenSSL.BN_is_zero(num) == 0)
                {
                    OpenSSL.BN_div(div, rem, num, rad, BN_CTX);
                    int word = checked((int)OpenSSL.BN_get_word(rem));
                    list.Add(symbols[word]);

                    OpenSSL.BN_swap(div, num);
                }
            }
            finally
            {
                OpenSSL.BN_free(rad);
                OpenSSL.BN_free(num);
                OpenSSL.BN_free(div);
                OpenSSL.BN_free(rem);
            }

            if (negative)
                list.Add('-');
            list.Reverse();

            var sb = new StringBuilder(list.Count);
            for (int i = 0; i < list.Count; i++)
                sb.Append(list[i]);

            return sb.ToString();
        }

        public byte[] ToBytes(ByteOrder order)
        {
            int length = (this.BitSize + 7) / 8;
            byte[] result = new byte[length];
            OpenSSL.BN_bn2bin(this.BIGNUM, result);

            if (order == ByteOrder.LittleEndian)
                Array.Reverse(result);

            return result;
        }

        /// <summary>
        /// length - pads zeros if resulting array is shorter
        /// </summary>
        public byte[] ToBytes(ByteOrder order, int length)
        {
            int fullLength = (this.BitSize + 7) / 8;
            byte[] bytes = new byte[fullLength];
            OpenSSL.BN_bn2bin(this.BIGNUM, bytes);

            byte[] result = new byte[length];
            if (fullLength > length)
                Array.Copy(bytes, fullLength - length, result, 0, length);
            else
                Array.Copy(bytes, 0, result, length - fullLength, fullLength);

            if (order == ByteOrder.LittleEndian)
                Array.Reverse(result);

            return result;
        }

        #endregion

        #region System.Object overrides

        public override bool Equals(object obj)
        {
            if (obj is BigInteger)
                return this.Equals(obj as BigInteger);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (int)BIGNUM;
        }

        public override string ToString()
        {
            return ToString(10);
        }

        #endregion

        #region IEquatable<BigInteger>

        public bool Equals(BigInteger other)
        {
            return OpenSSL.BN_cmp(this.BIGNUM, other.BIGNUM) == 0;
        }

        #endregion

        #region IEquatable<int>

        public bool Equals(int other)
        {
            return Equals(new BigInteger(other));
        }

        #endregion

        #region IComparable<BigInteger>

        public int CompareTo(BigInteger other)
        {
            return OpenSSL.BN_cmp(this.BIGNUM, other.BIGNUM);
        }

        #endregion

        #region IComparable<int>

        public int CompareTo(int other)
        {
            return CompareTo(new BigInteger(other));
        }

        #endregion

        #region Operators

        #region Comparison

        public static bool operator ==(BigInteger left, BigInteger right)
        {
            bool leftnull = ReferenceEquals(left, null);
            bool rightnull = ReferenceEquals(right, null);
            if (leftnull && rightnull)
                return true;
            if (leftnull ^ rightnull)
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(BigInteger left, BigInteger right)
        {
            return !(left == right);
        }

        public static bool operator ==(BigInteger left, int right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BigInteger left, int right)
        {
            return !left.Equals(right);
        }

        public static bool operator >=(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(BigInteger left, int right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(BigInteger left, int right)
        {
            return left.CompareTo(right) < 0;
        }

        #endregion

        #region Bit

        public static BigInteger operator >>(BigInteger left, int right)
        {
            return ShiftRight(left, right);
        }

        public static BigInteger operator <<(BigInteger left, int right)
        {
            return ShiftLeft(left, right);
        }

        #endregion

        #region Arithmetic

        public static BigInteger operator +(BigInteger left, BigInteger right)
        {
            return Sum(left, right);
        }

        public static BigInteger operator +(BigInteger left, int right)
        {
            return Sum(left, right);
        }

        public static BigInteger operator -(BigInteger left, BigInteger right)
        {
            return Difference(left, right);
        }

        public static BigInteger operator -(BigInteger left, int right)
        {
            return Difference(left, right);
        }

        public static BigInteger operator *(BigInteger left, BigInteger right)
        {
            return Product(left, right);
        }

        public static BigInteger operator *(BigInteger left, int right)
        {
            return Product(left, right);
        }

        public static BigInteger operator *(int left, BigInteger right)
        {
            return Product(right, left);
        }

        public static BigInteger operator /(BigInteger left, BigInteger right)
        {
            BigInteger result = new BigInteger(OpenSSL.BN_new());
            OpenSSL.BN_div(result.BIGNUM, IntPtr.Zero, left.BIGNUM, right.BIGNUM, BN_CTX);
            return result;
        }

        public static BigInteger operator /(BigInteger left, int right)
        {
            return left / new BigInteger(right);
        }

        public static BigInteger operator %(BigInteger left, BigInteger right)
        {
            return Modulo(left, right);
        }

        #endregion

        #region Conversion

        public static explicit operator int(BigInteger value)
        {
            if (value < int.MinValue || value > int.MaxValue)
                throw new ArgumentOutOfRangeException();

            if (value.IsZero)
                return 0;

            ulong result = OpenSSL.BN_get_word(value.BIGNUM);
            if (OpenSSL.BN_is_negative(value.BIGNUM) == 1)
                return (int)(-((long)result));
            else
                return (int)result;
        }

        #endregion

        #endregion

        #region Private static members

        private static char[] DefaultSymbols = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();

        #endregion
    }

#endif
}