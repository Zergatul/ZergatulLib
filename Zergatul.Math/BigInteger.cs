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
        /// Little-endian order, only single last item can be zero
        /// </summary>
        internal uint[] _bits;

        #region Public properties
        public bool IsZero => _bits.Length == 0 || (_bits.Length == 1 && _bits[0] == 0);
        #endregion

        #region Private properties
        private bool IsLastUIntZero => _bits.Length > 0 && _bits[_bits.Length - 1] == 0;
        private int BitsLength => IsLastUIntZero ? _bits.Length - 1 : _bits.Length;
        #endregion

        #region Public constants
        public static readonly BigInteger Zero = new BigInteger(new uint[0]);
        #endregion

        #region Contructors

        private BigInteger(uint[] bits)
        {
            this._bits = bits;
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
            int length = 1;
            while (cursor < value.Length)
            {
                group = value.Substring(cursor, _digitsPerUInt32[radix]);
                cursor += _digitsPerUInt32[radix];
                uint groupValue = Convert.ToUInt32(group, radix);
                DestructiveMulAdd(length, superRadix, groupValue);

                length++;
            }

            TruncateArrayIfInvalid();
        }

        #endregion

        #region Public methods

        public BigInteger Add(BigInteger value)
        {
            if (this.IsZero)
                return value;
            if (value.IsZero)
                return this;

            int thisBitLen = this.BitsLength;
            int valueBitLen = value.BitsLength;

            bool addOneElement;
            if (thisBitLen > valueBitLen)
                addOneElement = this._bits[thisBitLen - 1] == uint.MaxValue;
            else
                if (thisBitLen < valueBitLen)
                    addOneElement = value._bits[valueBitLen - 1] == uint.MaxValue;
                else
                    addOneElement = (long)this._bits[thisBitLen - 1] + value._bits[valueBitLen - 1] >= uint.MaxValue;

            var newbits = new uint[System.Math.Max(thisBitLen, valueBitLen) + (addOneElement ? 1 : 0)];
            uint remainder = 0;
            for (int i = 0; i < newbits.Length; i++)
            {
                long sum = (long)this._bits[i] + value._bits[i] + remainder;
                if (sum >= UInt32Overflow)
                {
                    newbits[i] = (uint)(sum - UInt32Overflow);
                    remainder = 1;
                }
                else
                {
                    newbits[i] = (uint)sum;
                    remainder = 0;
                }
            }

            return new BigInteger(newbits);
        }

        public string ToString(int radix)
        {
            if (IsZero)
                return "0";

            if (radix == 2 || radix == 4 || radix == 8 || radix == 16)
            {
                var result = "";
                for (int i = BitsLength - 1; i >= 0; i--)
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
            int length = BitsLength;
            for (int i = 0; i < length; i++)
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

            if (this.BitsLength != other.BitsLength)
                return false;

            int length = this.BitsLength;
            for (int i = 0; i < length; i++)
                if (this._bits[i] != other._bits[i])
                    return false;

            return true;
        }

        #endregion

        #region IComparable<BigInteger>

        public int CompareTo(BigInteger other)
        {
            throw new NotImplementedException();
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

        #endregion

        private void CopyFrom(BigInteger value)
        {
            this._bits = value._bits;
        }

        private Tuple<BigInteger, uint> DivideByUInt32(uint divisor)
        {
            var quotient = new uint[BitsLength];
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
        /// length - _bits length
        /// </summary>
        private void DestructiveMulAdd(int length, uint mult, uint add)
        {
            ulong carry = 0;
            for (int i = 0; i < length; i++)
            {
                ulong product = (ulong)_bits[i] * mult + carry;
                _bits[i] = (uint)(product & 0xFFFFFFFF);
                carry = product >> 32;
            }
            _bits[length] = (uint)carry;

            carry = add;
            for (int i = 0; i <= length; i++)
            {
                ulong sum = _bits[i] + carry;
                _bits[i] = (uint)(sum & 0xFFFFFFFF);
                carry = sum >> 32;

                if (carry == 0)
                    break;
            }
        }

        /// <summary>
        /// Truncates _bits array if last 2+ elements equal zero
        /// </summary>
        private void TruncateArrayIfInvalid()
        {
            if (_bits.Length >= 2 && _bits[_bits.Length - 1] == 0 && _bits[_bits.Length - 2] == 0)
            {
                int newLength = _bits.Length - 2;
                while (newLength > 0 && _bits[newLength - 1] == 0)
                    newLength--;
                uint[] newBits = new uint[newLength];
                Array.Copy(_bits, newBits, newLength);
                _bits = newBits;
            }
        }

        #region Private static members

        private const long UInt32Overflow = 1L << 32;
        private static readonly int[] _digitsPerUInt32 = new int[37];
        private static readonly uint[] _radixUInt32 = new uint[37];

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
