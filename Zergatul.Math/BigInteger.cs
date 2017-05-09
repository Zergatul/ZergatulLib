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
        internal uint[] _bits;

        /// <summary>
        /// Valuable uint's in array. Some last elements can be zero.
        /// </summary>
        internal int _bitsLength;

        #region Public properties
        public bool IsZero => _bitsLength == 0;
        #endregion

        #region Public constants
        public static readonly BigInteger Zero = new BigInteger(new uint[0]);
        public static readonly BigInteger One = new BigInteger(new uint[1] { 1 });
        #endregion

        #region Contructors

        private BigInteger(uint[] bits)
        {
            this._bits = bits;
            this._bitsLength = bits.Length;
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
                    _bits = new uint[2] { (uint)(value & 0xFFFFFFFF), (uint)(value >> 32) };
                    _bitsLength = 2;
                }
                else
                {
                    _bits = new uint[1] { (uint)value };
                    _bitsLength = 1;
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
                _bits = new uint[(dataLen - first + 3) >> 2];
                _bitsLength = _bits.Length;
                for (int i = _bits.Length - 1; i >= 0; i--)
                {
                    byte b0 = SafeGetByte(data, dataLen, dataLen - 1 - ((i << 2) | 3));
                    byte b1 = SafeGetByte(data, dataLen, dataLen - 1 - ((i << 2) | 2));
                    byte b2 = SafeGetByte(data, dataLen, dataLen - 1 - ((i << 2) | 1));
                    byte b3 = SafeGetByte(data, dataLen, dataLen - 1 - ((i << 2) | 0));
                    _bits[i] = (uint)((b0 << 24) | (b1 << 16) | (b2 << 8) | b3);
                }
            }
            if (order == ByteOrder.LittleEndian)
            {
                int dataLen = data.Length;
                int last = dataLen - 1;
                while (last >= 0 && data[last] == 0)
                    last--;
                _bits = new uint[(last + 4) >> 2];
                _bitsLength = _bits.Length;
                for (int i = 0; i < _bits.Length; i++)
                {
                    byte b0 = SafeGetByte(data, dataLen, ((i << 2) | 3));
                    byte b1 = SafeGetByte(data, dataLen, ((i << 2) | 2));
                    byte b2 = SafeGetByte(data, dataLen, ((i << 2) | 1));
                    byte b3 = SafeGetByte(data, dataLen, ((i << 2) | 0));
                    _bits[i] = (uint)((b0 << 24) | (b1 << 16) | (b2 << 8) | b3);
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
            _bits = new uint[(bitsCount + 31) / 32];

            int cursor = 0;
            int firstGroupLen = value.Length % _digitsPerUInt32[radix];
            if (firstGroupLen == 0)
                firstGroupLen = _digitsPerUInt32[radix];
            string group = value.Substring(cursor, firstGroupLen);
            cursor += firstGroupLen;
            _bits[0] = Convert.ToUInt32(group, radix);

            uint superRadix = _radixUInt32[radix];
            _bitsLength = 1;
            while (cursor < value.Length)
            {
                group = value.Substring(cursor, _digitsPerUInt32[radix]);
                cursor += _digitsPerUInt32[radix];
                uint groupValue = Convert.ToUInt32(group, radix);
                DestructiveMulAdd(superRadix, groupValue);

                if (_bitsLength < _bits.Length && _bits[_bitsLength] != 0)
                    _bitsLength++;
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

            var newbits = new uint[System.Math.Max(this._bitsLength, value._bitsLength) + 1];
            uint carry = 0;
            for (int i = 0; i < newbits.Length; i++)
            {
                long sum = carry;
                if (i < this._bitsLength)
                    sum += this._bits[i];
                if (i < value._bitsLength)
                    sum += value._bits[i];
                if (sum >= UInt32Overflow)
                {
                    newbits[i] = (uint)(sum - UInt32Overflow);
                    carry = 1;
                }
                else
                {
                    newbits[i] = (uint)sum;
                    carry = 0;
                }
            }

            return new BigInteger(newbits);
        }

        public BigInteger Multiply(BigInteger value)
        {
            if (this.IsZero || value.IsZero)
                return Zero;

            if (this._bitsLength < value._bitsLength)
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

            var x = new uint[this._bitsLength + 1];
            var y = new uint[value._bitsLength];

            Array.Copy(this._bits, x, this._bitsLength);
            Array.Copy(value._bits, y, value._bitsLength);

            // calculate normalizing shift
            int shift = 0;
            uint yLead = y[value._bitsLength];
            while ((yLead & 1) == 0)
            {
                yLead <<= 1;
                shift++;
            }

            ShiftLeftNoResize(x, this._bitsLength + 1, shift);
            ShiftLeftNoResize(y, value._bitsLength, shift);

            var q = new uint[this._bitsLength - value._bitsLength + 1];

            return null;
        }

        public string ToString(int radix)
        {
            if (IsZero)
                return "0";

            if (radix == 2 || radix == 4 || radix == 16)
            {
                var result = "";
                for (int i = _bitsLength - 1; i >= 0; i--)
                {
                    string part = Convert.ToString(_bits[i], radix);
                    if (result != "")
                        part = part.PadLeft(32 * 2 / radix, '0');
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
            for (int i = 0; i < this._bitsLength; i++)
            {
                int shift = i * 59 % 32;
                result = result ^ (int)((_bits[i] << shift) | (_bits[i] >> (32 - shift)));
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
            if (this._bits == other._bits)
                return true;

            if (this._bitsLength != other._bitsLength)
                return false;

            for (int i = 0; i < this._bitsLength; i++)
                if (this._bits[i] != other._bits[i])
                    return false;

            return true;
        }

        #endregion

        #region IComparable<BigInteger>

        public int CompareTo(BigInteger other)
        {
            if (this._bits == other._bits)
                return 0;

            int compare = this._bitsLength.CompareTo(other._bitsLength);
            if (compare != 0)
                return compare;

            for (int i = this._bitsLength - 1; i >= 0; i--)
            {
                compare = this._bits[i].CompareTo(other._bits[i]);
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
            this._bits = value._bits;
            this._bitsLength = value._bitsLength;
        }

        private void TruncateLeadingZeros()
        {
            while (_bitsLength > 0 && _bits[_bitsLength - 1] == 0)
                _bitsLength--;
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

            _bits = new uint[(value.Length * bitsPerDigit + 31) / 32];
            _bitsLength = _bits.Length;
            int cursor = 0;
            int groupLen = 32 / bitsPerDigit;
            int firstGroupLen = value.Length % groupLen;
            if (firstGroupLen == 0)
                firstGroupLen = groupLen;

            string group = value.Substring(cursor, firstGroupLen);
            cursor += firstGroupLen;
            _bits[_bits.Length - 1] = Convert.ToUInt32(group, radix);

            for (int i = _bits.Length - 2; i >= 0; i--)
            {
                group = value.Substring(cursor, groupLen);
                cursor += groupLen;
                _bits[i] = Convert.ToUInt32(group, radix);
            }
        }

        private Tuple<BigInteger, uint> DivideByUInt32(uint divisor)
        {
            var quotient = new uint[_bitsLength];
            long remainder = 0;
            for (int i = quotient.Length - 1; i >= 0; i--)
            {
                remainder = (remainder << 32) | _bits[i];
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
            for (int i = 0; i < _bitsLength; i++)
            {
                ulong product = (ulong)_bits[i] * mult + carry;
                _bits[i] = (uint)(product & 0xFFFFFFFF);
                carry = product >> 32;
            }
            if (carry > 0)
                _bits[_bitsLength] = (uint)carry;

            carry = add;
            for (int i = 0; i <= _bitsLength; i++)
            {
                ulong sum = _bits[i] + carry;
                _bits[i] = (uint)(sum & 0xFFFFFFFF);
                carry = sum >> 32;

                if (carry == 0)
                    break;
            }
        }

        /// <summary>
        /// Assumes this._bitsLength >= value._bitsLength, both values != 0
        /// </summary>
        private BigInteger MultiplyGeneral(BigInteger value)
        {
            uint[] result = new uint[this._bitsLength + value._bitsLength];
            if (value._bitsLength < KaratsubaBitsLength)
            {
                // O(n^2) algo
                for (int i1 = 0; i1 < this._bitsLength; i1++)
                    for (int i2 = 0; i2 < value._bitsLength; i2++)
                    {
                        int index = i1 + i2;
                        ulong carry = (ulong)this._bits[i1] * value._bits[i2];
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
                int half = value._bitsLength / 2;
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

                Array.Copy(z0._bits, result, z0._bitsLength);
                DestructiveShiftSum(result, z1, half);
                DestructiveShiftSum(result, z2, 2 * half);
            }

            return new BigInteger(result);
        }

        /// <summary>
        /// Split bits into 2 parts. Length beginning from lower bits.
        /// </summary>
        private Tuple<BigInteger, BigInteger> Split(int length)
        {
            uint[] low = new uint[length];
            uint[] high = new uint[_bitsLength - length];
            Array.Copy(_bits, 0, low, 0, length);
            Array.Copy(_bits, length, high, 0, _bitsLength - length);

            return new Tuple<BigInteger, BigInteger>(new BigInteger(low), new BigInteger(high));
        }

        /// <summary>
        /// Assumes result >= 0
        /// this = this - value
        /// </summary>
        private void DestructiveSubstract(BigInteger value)
        {
            long carry = 0;
            for (int i = 0; i < this._bitsLength; i++)
            {
                long sub = this._bits[i];
                sub += carry;
                if (i < value._bitsLength)
                    sub -= value._bits[i];
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
                this._bits[i] = (uint)sub;
            }

            TruncateLeadingZeros();
        }

        /// <summary>
        /// self += value &lt;&lt; shift
        /// Assumes no overflow
        /// </summary>
        private static void DestructiveShiftSum(uint[] bits, BigInteger value, int shift)
        {
            long carry = 0;
            for (int i = 0; i < value._bitsLength; i++)
            {
                long sum = (long)bits[shift + i] + value._bits[i] + carry;
                if (sum >= UInt32Overflow)
                {
                    bits[shift + i] = (uint)(sum & 0xFFFFFFFF);
                    carry = 1;
                }
                else
                {
                    bits[shift + i] = (uint)(sum);
                    carry = 0;
                }
            }
            for (int i = value._bitsLength + shift; ; i++)
            {
                if (carry == 0)
                    break;
                long sum = (long)bits[i] + carry;
                if (sum >= UInt32Overflow)
                {
                    bits[i] = (uint)(sum & 0xFFFFFFFF);
                    carry = 1;
                }
                else
                {
                    bits[i] = (uint)(sum);
                    carry = 0;
                }
            }
        }

        /// <summary>
        /// Assumes operation will not change array size
        /// </summary>
        private static void ShiftLeftNoResize(uint[] bits, int length, int shift)
        {
            if (shift == 0)
                return;
            for (int i = length - 1; i >= 0; i--)
            {
                if (i > 0)
                    bits[i] = (bits[i] << shift) | (bits[i - 1] >> (32 - shift));
                else
                    bits[i] = (bits[i] << shift);
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
