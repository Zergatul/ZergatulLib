using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math
{
    // https://github.com/Zergatul/zChat/blob/master/client/bigint.js
    public class BigInteger : IComparable<BigInteger>, IEquatable<BigInteger>
    {
        /// <summary>
        /// Little-endian order.
        /// </summary>
        internal uint[] _words;

        /// <summary>
        /// Valuable uint's in array. Some last elements can be zero.
        /// </summary>
        internal int _wordsLength;

        #region Public properties
        public bool IsZero => _wordsLength == 0;
        #endregion

        #region Public constants
        public static readonly BigInteger Zero = new BigInteger(new uint[0]);
        public static readonly BigInteger One = new BigInteger(new uint[1] { 1 });
        #endregion

        #region Contructors

        private BigInteger(uint[] words)
        {
            this._words = words;
            this._wordsLength = words.Length;
            TruncateLeadingZeros();
        }

        private BigInteger(uint[] words, int length)
        {
            this._words = words;
            this._wordsLength = length;
            TruncateLeadingZeros();
        }

        public BigInteger(long value)
        {
            if (value < 0)
                throw new ArgumentException("Value must be >= 0", nameof(value));

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
            }
            else
                CopyFrom(Zero);
        }

        public BigInteger(byte[] data, ByteOrder order)
        {
            if (order == ByteOrder.BigEndian)
            {
                int dataLen = data.Length;
                int first = 0;
                while (first < dataLen && data[first] == 0)
                    first++;
                _words = new uint[(dataLen - first + 3) >> 2];
                _wordsLength = _words.Length;
                for (int i = _words.Length - 1; i >= 0; i--)
                {
                    byte b0 = SafeGetByte(data, dataLen, dataLen - 1 - ((i << 2) | 3));
                    byte b1 = SafeGetByte(data, dataLen, dataLen - 1 - ((i << 2) | 2));
                    byte b2 = SafeGetByte(data, dataLen, dataLen - 1 - ((i << 2) | 1));
                    byte b3 = SafeGetByte(data, dataLen, dataLen - 1 - ((i << 2) | 0));
                    _words[i] = (uint)((b0 << 24) | (b1 << 16) | (b2 << 8) | b3);
                }
            }
            if (order == ByteOrder.LittleEndian)
            {
                int dataLen = data.Length;
                int last = dataLen - 1;
                while (last >= 0 && data[last] == 0)
                    last--;
                _words = new uint[(last + 4) >> 2];
                _wordsLength = _words.Length;
                for (int i = 0; i < _words.Length; i++)
                {
                    byte b0 = SafeGetByte(data, dataLen, ((i << 2) | 3));
                    byte b1 = SafeGetByte(data, dataLen, ((i << 2) | 2));
                    byte b2 = SafeGetByte(data, dataLen, ((i << 2) | 1));
                    byte b3 = SafeGetByte(data, dataLen, ((i << 2) | 0));
                    _words[i] = (uint)((b0 << 24) | (b1 << 16) | (b2 << 8) | b3);
                }
            }
        }

        public BigInteger(string value, int radix = 10)
        {
            if (radix < 2 || radix > 36)
                throw new ArgumentOutOfRangeException(nameof(radix));

            value = value.TrimStart('0');
            if (value.Length == 0)
            {
                CopyFrom(Zero);
                return;
            }

            if (radix == 2 || radix == 4 || radix == 16)
            {
                ParsePowerOfTwo(value, radix);
                return;
            }

            int bitsCount = (int)System.Math.Ceiling(value.Length * System.Math.Log(radix, 2));
            _words = new uint[(bitsCount + 31) / 32];

            int cursor = 0;
            int firstGroupLen = value.Length % _digitsPerUInt32[radix];
            if (firstGroupLen == 0)
                firstGroupLen = _digitsPerUInt32[radix];
            string group = value.Substring(cursor, firstGroupLen);
            cursor += firstGroupLen;
            _words[0] = Convert.ToUInt32(group, radix);

            uint superRadix = _radixUInt32[radix];
            _wordsLength = 1;
            while (cursor < value.Length)
            {
                group = value.Substring(cursor, _digitsPerUInt32[radix]);
                cursor += _digitsPerUInt32[radix];
                uint groupValue = Convert.ToUInt32(group, radix);
                DestructiveMulAdd(superRadix, groupValue);

                if (_wordsLength < _words.Length && _words[_wordsLength] != 0)
                    _wordsLength++;
            }
        }

        #endregion

        #region Public methods

        public BigInteger Add(BigInteger value)
        {
            if (this.IsZero)
                return value;
            if (value.IsZero)
                return this;

            var newwords = new uint[System.Math.Max(this._wordsLength, value._wordsLength) + 1];
            uint carry = 0;
            for (int i = 0; i < newwords.Length; i++)
            {
                long sum = carry;
                if (i < this._wordsLength)
                    sum += this._words[i];
                if (i < value._wordsLength)
                    sum += value._words[i];
                if (sum >= UInt32Overflow)
                {
                    newwords[i] = (uint)(sum - UInt32Overflow);
                    carry = 1;
                }
                else
                {
                    newwords[i] = (uint)sum;
                    carry = 0;
                }
            }

            return new BigInteger(newwords);
        }

        public BigInteger Multiply(BigInteger value)
        {
            if (this.IsZero || value.IsZero)
                return Zero;

            if (this._wordsLength < value._wordsLength)
                return value.MultiplyGeneral(this);
            else
                return this.MultiplyGeneral(value);
        }

        public BigInteger Div(BigInteger value)
        {
            return Division(value).Item1;
        }

        public BigInteger Mod(BigInteger value)
        {
            return Division(value).Item2;
        }

        public Tuple<BigInteger, BigInteger> Division(BigInteger value)
        {
            if (this.CompareTo(value) < 0)
                return new Tuple<BigInteger, BigInteger>(Zero, this);

            var x = new uint[this._wordsLength + 1];
            var y = new uint[value._wordsLength];

            Array.Copy(this._words, x, this._wordsLength);
            Array.Copy(value._words, y, value._wordsLength);

            // calculate normalizing shift
            int shift = 0;
            uint yHighWord = y[value._wordsLength - 1];
            while ((yHighWord & 0x80000000) == 0)
            {
                yHighWord <<= 1;
                shift++;
            }

            int xLen = BitShiftLeftNoResize(x, this._wordsLength + 1, shift);
            int yLen = BitShiftLeftNoResize(y, value._wordsLength, shift);

            var q = new uint[xLen - yLen + 1];
            yHighWord = y[yLen - 1];
            for (int i = xLen; i >= yLen; i--)
            {
                ulong xHighWords = i == xLen ? x[i - 1] : (((ulong)x[i] << 32) | x[i - 1]);
                ulong nextQuotWords = xHighWords / yHighWord;
                if (nextQuotWords >= 0x100000000)
                    throw new OverflowException("Internal overflow");
                uint nextQuotWord = (uint)nextQuotWords;

                if (nextQuotWords != 0)
                    if (!WordShiftSubstractLinearCombination(x, xLen, y, yLen, nextQuotWord, i - yLen))
                    {
                        nextQuotWord--;
                        WordShiftNegativeAdd(x, xLen, y, yLen, i - yLen);
                    }
                q[i - yLen] = nextQuotWord;

                while (x[xLen - 1] == 0)
                    xLen--;
            }

            // shift back normalization of x
            BitShiftRightNoResize(x, xLen, shift);

            return new Tuple<BigInteger, BigInteger>(new BigInteger(q), new BigInteger(x, xLen));
        }

        public string ToString(int radix)
        {
            if (IsZero)
                return "0";

            if (radix == 2 || radix == 4 || radix == 16)
            {
                var result = "";
                for (int i = _wordsLength - 1; i >= 0; i--)
                {
                    string part = Convert.ToString(_words[i], radix);
                    if (result != "")
                    {
                        if (radix == 2)
                            part = part.PadLeft(32, '0');
                        if (radix == 4)
                            part = part.PadLeft(16, '0');
                        if (radix == 16)
                            part = part.PadLeft(8, '0');
                    }
                    result = result + part;
                }
                return result;
            }
            else
            {
                int radixLength = _digitsPerUInt32[radix];
                uint radixUIn32 = _radixUInt32[radix];
                var remainder = this;
                var result = "";
                while (!remainder.IsZero)
                {
                    var division = remainder.DivideByUInt32(radixUIn32);
                    remainder = division.Item1;
                    string part = Convert.ToString(division.Item2, radix);
                    if (!remainder.IsZero)
                        part = part.PadLeft(radixLength, '0');

                    result = part + result;
                }

                return result;
            }
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

        #region Operators

        public static bool operator==(BigInteger left, BigInteger right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BigInteger left, BigInteger right)
        {
            return !left.Equals(right);
        }

        public static BigInteger operator+(BigInteger left, BigInteger right)
        {
            return left.Add(right);
        }

        public static BigInteger operator *(BigInteger left, BigInteger right)
        {
            return left.Multiply(right);
        }

        public static BigInteger operator /(BigInteger left, BigInteger right)
        {
            return left.Div(right);
        }

        public static BigInteger operator %(BigInteger left, BigInteger right)
        {
            return left.Mod(right);
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
        }

        #endregion

        #region Private math helper methods

        /// <summary>
        /// Parses string into BigInteger. Allowed radix: 2, 4, 16
        /// </summary>
        private void ParsePowerOfTwo(string value, int radix)
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

            _words = new uint[(value.Length * bitsPerDigit + 31) / 32];
            _wordsLength = _words.Length;
            int cursor = 0;
            int groupLen = 32 / bitsPerDigit;
            int firstGroupLen = value.Length % groupLen;
            if (firstGroupLen == 0)
                firstGroupLen = groupLen;

            string group = value.Substring(cursor, firstGroupLen);
            cursor += firstGroupLen;
            _words[_words.Length - 1] = Convert.ToUInt32(group, radix);

            for (int i = _words.Length - 2; i >= 0; i--)
            {
                group = value.Substring(cursor, groupLen);
                cursor += groupLen;
                _words[i] = Convert.ToUInt32(group, radix);
            }
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
        /// this = this * mult + add
        /// </summary>
        private void DestructiveMulAdd(uint mult, uint add)
        {
            ulong carry = 0;
            for (int i = 0; i < _wordsLength; i++)
            {
                ulong product = (ulong)_words[i] * mult + carry;
                _words[i] = (uint)(product & 0xFFFFFFFF);
                carry = product >> 32;
            }
            if (carry > 0)
                _words[_wordsLength] = (uint)carry;

            carry = add;
            for (int i = 0; i <= _wordsLength; i++)
            {
                ulong sum = _words[i] + carry;
                _words[i] = (uint)(sum & 0xFFFFFFFF);
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

        #endregion

        #region Private static members

        private const long UInt32Overflow = 1L << 32;
        private static readonly int[] _digitsPerUInt32 = new int[37];
        private static readonly uint[] _radixUInt32 = new uint[37];

        private const int KaratsubaBitsLength = 30;

        static BigInteger()
        {
            for (int i = 2; i <= 36; i++)
            {
                _digitsPerUInt32[i] = 1;
                _radixUInt32[i] = (uint)i;
                while (_radixUInt32[i] * i < uint.MaxValue)
                {
                    _radixUInt32[i] *= (uint)i;
                    _digitsPerUInt32[i]++;
                }
            }
        }

        private static byte SafeGetByte(byte[] data, int dataLen, int index)
        {
            return (0 <= index) && (index < dataLen) ? data[index] : (byte)0;
        }

        #endregion
    }
}
