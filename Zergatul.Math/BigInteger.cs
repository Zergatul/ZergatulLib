using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math
{
    // https://github.com/Zergatul/zChat/blob/master/client/bigint.js
    public class BigInteger : IComparable<BigInteger>, IEquatable<BigInteger>, IEquatable<int>
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

        public static BigInteger Sum(BigInteger summand1, BigInteger summand2)
        {
            if (summand1.IsZero)
                return summand2;
            if (summand2.IsZero)
                return summand1;

            var words = new uint[System.Math.Max(summand1._wordsLength, summand2._wordsLength) + 1];
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

            return new BigInteger(words);
        }

        public static BigInteger Difference(BigInteger minuend, BigInteger subtrahend)
        {
            throw new NotImplementedException();
        }

        public static BigInteger Product(BigInteger factor1, BigInteger factor2)
        {
            if (factor1.IsZero || factor2.IsZero)
                return Zero;

            if (factor1._wordsLength < factor2._wordsLength)
                return factor2.MultiplyGeneral(factor1);
            else
                return factor1.MultiplyGeneral(factor2);
        }

        public static void Division(BigInteger divident, BigInteger divisor, out BigInteger quotient, out BigInteger remainder)
        {
            if (divident.CompareTo(divisor) < 0)
            {
                quotient = Zero;
                remainder = divident;
                return;
            }

            BigInteger _quotient = null;
            BigInteger _remainder = null;
            DivisionGeneral(divident, divisor, ref _quotient, ref _remainder, true, true);
            quotient = _quotient;
            remainder = _remainder;
        }

        public static BigInteger Modulo(BigInteger divident, BigInteger divisor)
        {
            if (divident.CompareTo(divisor) < 0)
                return divident;

            BigInteger quotient = null;
            BigInteger remainder = null;
            DivisionGeneral(divident, divisor, ref quotient, ref remainder, false, true);

            return remainder;
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

            if (modulus.IsBitSet(0))
            {
                // use Montgomery exponentiation
                var rWords = new uint[modulus._wordsLength + 1];
                rWords[modulus._wordsLength] = 1;
                var r = new BigInteger(rWords);

                var x = modulus; // ???

                long mPrime = -ExtendedEuclideanInt64(0x100000000, modulus._words[0]).y;
                if (mPrime < 0)
                    mPrime += 0x100000000;

                var xPrime = @base * r % modulus;
                var a = r % modulus;

                for (int i = exponent.GetBitsLength() - 1; i >= 0; i--)
                {
                    a = MontgomeryMultiplication(a, a, modulus, (uint)mPrime);
                    if (exponent.IsBitSet(i))
                        a = MontgomeryMultiplication(a, xPrime, modulus, (uint)mPrime);
                }
                // final step montgomery * 1 ????

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

        #region IEquatable<int>

        public bool Equals(int other)
        {
            if (other == 0)
                return IsZero;
            if (other < 0)
                return false;
            return _wordsLength == 1 && _words[0] == other;
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

        public static BigInteger operator+(BigInteger left, BigInteger right)
        {
            return Sum(left, right);
        }

        public static BigInteger operator -(BigInteger left, BigInteger right)
        {
            return Difference(left, right);
        }

        public static BigInteger operator *(BigInteger left, BigInteger right)
        {
            return Product(left, right);
        }

        public static BigInteger operator /(BigInteger left, BigInteger right)
        {
            return null;
        }

        public static BigInteger operator %(BigInteger left, BigInteger right)
        {
            return Modulo(left, right);
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

        private bool IsBitSet(int position)
        {
            int wordIndex = position / 32;
            if (wordIndex >= _wordsLength)
                return false;
            return (_words[wordIndex] & (1 << (position & 0x1f))) != 0;
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

        private static void DivisionGeneral(BigInteger divident, BigInteger divisor, ref BigInteger quotient, ref BigInteger remainder, bool calcQuotient, bool calcRemainder)
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

                uint nextQuotWord = (uint)((((ulong)xHighWord1 << 32) | xHighWord2) / yHighWord1);

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

                while (x[xLen - 1] == 0)
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
        private static ExtEuclideanResult ExtendedEuclideanInt64(long a, long b)
        {
            if (a < b)
                throw new ArgumentException("a should be greater than b.");
            if (b == 0)
                return new ExtEuclideanResult
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
            return new ExtEuclideanResult
            {
                d = a,
                x = x2,
                y = y2
            };
        }

        private struct ExtEuclideanResult
        {
            public long x, y, d;
        }

        /// <summary>
        /// m - odd
        /// mInv = -(1 / m) mod 0x100000000
        /// </summary>
        private static BigInteger MontgomeryMultiplication(BigInteger x, BigInteger y, BigInteger m, uint mInv)
        {
            var result = new uint[m._wordsLength + 2];
            for (int i = 0; i < m._wordsLength; i++)
            {
                uint u = (uint)((((result[0] + (ulong)x._words[i] * y._words[0]) & 0xFFFFFFFF) * mInv) & 0xFFFFFFFF);

                ulong carry = 0;
                ulong sum;
                for (int j = 0; j < m._wordsLength; j++)
                {
                    sum = checked(carry + result[j] + (ulong)x._words[i] * m._words[j]);
                    carry = sum >> 32;
                    result[j] = (uint)(sum & 0xFFFFFFFF);
                }
                sum = result[m._wordsLength] + carry;
                result[m._wordsLength] = (uint)(sum & 0xFFFFFFFF);
                result[m._wordsLength + 1] += (uint)(sum >> 32);

                carry = 0;
                for (int j = 0; j < m._wordsLength; j++)
                {
                    sum = checked(carry + result[j] + (ulong)u * m._words[j]);
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
                for (int i = m._wordsLength - 1; i >= 0; i++)
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

        private static uint SafeGetUInt32(uint[] data, int dataLen, int index)
        {
            return (0 <= index) && (index < dataLen) ? data[index] : 0;
        }

        #endregion
    }
}
