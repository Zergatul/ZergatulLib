using System;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace Zergatul.Math
{
    public struct UInt128
    {
        internal uint _w1;
        internal uint _w2;
        internal uint _w3;
        internal uint _w4;

        #region Constructors

        public UInt128(uint w1, uint w2, uint w3, uint w4)
        {
            _w1 = w1;
            _w2 = w2;
            _w3 = w3;
            _w4 = w4;
        }

        public UInt128(int value)
        {
            if (value >= 0)
            {
                _w1 = 0;
                _w2 = 0;
                _w3 = 0;
                _w4 = (uint)value;
            }
            else
            {
                throw new OverflowException();
            }
        }

        public UInt128(long value)
        {
            if (value >= 0)
            {
                _w1 = 0;
                _w2 = 0;
                _w3 = (uint)(value >> 32);
                _w4 = (uint)(value & 0xFFFFFFFF);
            }
            else
            {
                throw new OverflowException();
            }
        }

        public UInt128(ulong value)
        {
            _w1 = 0;
            _w2 = 0;
            _w3 = (uint)(value >> 32);
            _w4 = (uint)(value & 0xFFFFFFFF);
        }

        #endregion

        #region Math operators

        public static UInt128 operator +(UInt128 left, UInt128 right)
        {
            ulong carry = (ulong)left._w4 + right._w4;
            uint w4 = (uint)(carry & 0xFFFFFFFF);
            carry >>= 32;

            carry = carry + left._w3 + right._w3;
            uint w3 = (uint)(carry & 0xFFFFFFFF);
            carry >>= 32;

            carry = carry + left._w2 + right._w2;
            uint w2 = (uint)(carry & 0xFFFFFFFF);
            carry >>= 32;

            uint w1 = (uint)carry + left._w1 + right._w1;

            return new UInt128(w1, w2, w3, w4);
        }

        public static UInt128 operator -(UInt128 left, UInt128 right)
        {
            long carry = (long)left._w4 - right._w4;
            uint w4 = (uint)(carry & 0xFFFFFFFF);
            carry >>= 32;

            carry = carry + left._w3 - right._w3;
            uint w3 = (uint)(carry & 0xFFFFFFFF);
            carry >>= 32;

            carry = carry + left._w2 - right._w2;
            uint w2 = (uint)(carry & 0xFFFFFFFF);
            carry >>= 32;

            carry = carry + left._w1 - right._w1;
            uint w1 = (uint)(carry & 0xFFFFFFFF);

            return new UInt128(w1, w2, w3, w4);
        }

        public static UInt128 operator *(UInt128 left, int right)
        {
            if (right >= 0)
            {
                return left * (uint)right;
            }
            else
            {
                throw new OverflowException();
            }
        }

        public static UInt128 operator *(UInt128 left, uint right)
        {
            ulong carry = (ulong)left._w4 * right;
            uint w4 = (uint)(carry & 0xFFFFFFFF);
            carry >>= 32;

            carry += (ulong)left._w3 * right;
            uint w3 = (uint)(carry & 0xFFFFFFFF);

            carry += (ulong)left._w2* right;
            uint w2 = (uint)(carry & 0xFFFFFFFF);

            carry += (ulong)left._w1 * right;
            uint w1 = (uint)(carry & 0xFFFFFFFF);

            return new UInt128(w1, w2, w3, w4);
        }

        public static UInt128 operator *(UInt128 left, long right)
        {
            if (right >= 0)
            {
                return left * (ulong)right;
            }
            else
            {
                throw new OverflowException();
            }
        }

        public static UInt128 operator *(UInt128 left, ulong right)
        {
            ulong rw1 = right >> 32;
            ulong rw2 = right & 0xFFFFFFFF;

            ulong sum, carry;

            sum = left._w4 * rw2;
            uint w4 = (uint)sum;
            carry = sum >> 32;

            sum = carry + left._w4 * rw1;
            carry = sum >> 32;
            sum &= 0xFFFFFFFF;
            sum += left._w3 * rw2;
            carry += sum >> 32;
            uint w3 = (uint)sum;

            sum = carry + left._w3 * rw1;
            carry = sum >> 32;
            sum &= 0xFFFFFFFF;
            sum += left._w2 * rw2;
            carry += sum >> 32;
            uint w2 = (uint)sum;

            sum = carry + left._w2 * rw1 + left._w1 * rw2;
            uint w1 = (uint)sum;

            return new UInt128(w1, w2, w3, w4);
        }

        public static UInt128 operator *(UInt128 left, UInt128 right)
        {
            ulong sum, carry;

            sum = (ulong)left._w4 * right._w4;
            uint w4 = (uint)sum;
            carry = sum >> 32;

            sum = carry + (ulong)left._w4 * right._w3;
            carry = sum >> 32;
            sum &= 0xFFFFFFFF;
            sum += (ulong)left._w3 * right._w4;
            carry += sum >> 32;
            uint w3 = (uint)sum;

            sum = carry + (ulong)left._w4 * right._w2;
            carry = sum >> 32;
            sum &= 0xFFFFFFFF;
            sum += (ulong)left._w3 * right._w3;
            carry += sum >> 32;
            sum &= 0xFFFFFFFF;
            sum += (ulong)left._w2 * right._w4;
            carry += sum >> 32;
            uint w2 = (uint)sum;

            sum = carry + (ulong)left._w4 * right._w1 + (ulong)left._w3 * right._w2 + (ulong)left._w2 * right._w3 + (ulong)left._w1 * right._w4;
            uint w1 = (uint)sum;

            return new UInt128(w1, w2, w3, w4);
        }

        public static UInt128 operator /(UInt128 left, uint right)
        {
            long carry1 = left._w1 / right;
            uint w1 = (uint)carry1;
            ulong carry = (ulong)(left._w1 - right * w1);

            carry = ((carry << 32) | left._w2) / right;
            uint w2 = (uint)carry;
            carry = ((carry << 32) | left._w2) - right * w2;

            carry = ((carry << 32) | left._w3) / right;
            uint w3 = (uint)carry;
            carry = ((carry << 32) | left._w3) - right * w3;

            carry = ((carry << 32) | left._w4) / right;
            uint w4 = (uint)carry;

            return new UInt128(w1, w2, w3, w4);
        }

        public static ulong operator %(UInt128 left, ulong right)
        {
            return DivMod(left, right).Item2;
        }

        #endregion

        #region Conversion operators

        public static implicit operator UInt128(int value) => new UInt128(value);
        public static implicit operator UInt128(ulong value) => new UInt128(value);

        public static explicit operator ulong(UInt128 value)
        {
            if (value._w1 != 0 || value._w2 != 0)
                throw new OverflowException();
            return ((ulong)value._w3 << 32) | value._w4;
        }

        #endregion

        #region Compare operators

        public static bool operator <(UInt128 left, int right)
        {
            if (right < 0)
            {
                return false;
            }
            else
            {
                return left._w1 == 0 && left._w2 == 0 && left._w3 == 0 && left._w4 < (uint)right;
            }
        }

        public static bool operator >(UInt128 left, int right)
        {
            if (right >= 0)
            {
                return left._w1 != 0 || left._w2 != 0 || left._w3 != 0 || left._w4 > (uint)right;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region Static

        public static (UInt128, uint) DivMod(UInt128 divider, uint divisor)
        {
            ulong carry;
            carry = divider._w1 / divisor;
            uint w1 = (uint)carry;
            carry = (ulong)(divider._w1 - divisor * w1);

            carry = ((carry << 32) | divider._w2) / divisor;
            uint w2 = (uint)carry;
            carry = ((carry << 32) | divider._w2) - divisor * w2;

            carry = ((carry << 32) | divider._w3) / divisor;
            uint w3 = (uint)carry;
            carry = ((carry << 32) | divider._w3) - divisor * w3;

            carry = ((carry << 32) | divider._w4) / divisor;
            uint w4 = (uint)carry;
            carry = ((carry << 32) | divider._w4) - divisor * w4;

            return (new UInt128(w1, w2, w3, w4), (uint)carry);
        }

        public static (UInt128, ulong) DivMod(UInt128 divider, ulong divisor)
        {
            if (divisor <= 0xFFFFFFFFUL)
                return DivMod(divider, (uint)divisor);

            int shift = 0;
            while ((divisor & 0x8000000000000000UL) == 0)
            {
                shift++;
                divisor <<= 1;
            }

            ulong m1 = divisor >> 32;
            ulong m2 = divisor & 0xFFFFFFFF;

            ulong w1, w2, w3, w4, w5;
            if (shift == 0)
            {
                w1 = 0;
                w2 = divider._w1;
                w3 = divider._w2;
                w4 = divider._w3;
                w5 = divider._w4;
            }
            else
            {
                int rshift = 32 - shift;
                w1 = divider._w1 >> rshift;
                w2 = (divider._w1 << shift) | (divider._w2 >> rshift);
                w3 = (divider._w2 << shift) | (divider._w3 >> rshift);
                w4 = (divider._w3 << shift) | (divider._w4 >> rshift);
                w5 = divider._w4 << shift;
            }

            ulong carry;

            /*****/
            ulong q1 = ((w1 << 32) | w2) / m1;
            w3 -= m2 * q1;
            carry = ((~w3 >> 32) + 1) & 0xFFFFFFFF;
            w3 = w3 & 0xFFFFFFFF;

            w2 = w2 - m1 * q1 - carry;
            carry = ((~w2 >> 32) + 1) & 0xFFFFFFFF;
            w2 = w2 & 0xFFFFFFFF;

            w1 -= carry;
            // overhit, this loop should cycle 0..1 times and rarely 2 times
            while ((w1 & 0x80000000) != 0)
            {
                q1--;

                w3 += m2;
                carry = w3 >> 32;
                w3 = w3 & 0xFFFFFFFF;

                w2 += m1 + carry;
                carry = w2 >> 32;
                w2 = w2 & 0xFFFFFFFF;

                w1 += carry;
            }
            Debug.Assert(w1 == 0);
            /*****/
            ulong q2 = ((w2 << 32) | w3) / m1;
            w4 -= m2 * q2;
            carry = ((~w4 >> 32) + 1) & 0xFFFFFFFF;
            w4 = w4 & 0xFFFFFFFF;

            w3 = w3 - m1 * q2 - carry;
            carry = ((~w3 >> 32) + 1) & 0xFFFFFFFF;
            w3 = w3 & 0xFFFFFFFF;

            w2 -= carry;
            // overhit, this loop should cycle 0..1 times and rarely 2 times
            while ((w2 & 0x80000000) != 0)
            {
                q2--;

                w4 += m2;
                carry = w4 >> 32;
                w4 = w4 & 0xFFFFFFFF;

                w3 += m1 + carry;
                carry = w3 >> 32;
                w3 = w3 & 0xFFFFFFFF;

                w2 += carry;
            }
            Debug.Assert(w2 == 0);
            /*****/
            ulong q3 = ((w3 << 32) | w4) / m1;
            w5 -= m2 * q3;
            carry = ((~w5 >> 32) + 1) & 0xFFFFFFFF;
            w5 = w5 & 0xFFFFFFFF;

            w4 = w4 - m1 * q3 - carry;
            carry = ((~w4 >> 32) + 1) & 0xFFFFFFFF;
            w4 = w4 & 0xFFFFFFFF;

            w3 -= carry;
            // overhit, this loop should cycle 0..1 times and rarely 2 times
            while ((w3 & 0x80000000) != 0)
            {
                q3--;

                w5 += m2;
                carry = w5 >> 32;
                w5 = w5 & 0xFFFFFFFF;

                w4 += m1 + carry;
                carry = w4 >> 32;
                w4 = w4 & 0xFFFFFFFF;

                w3 += carry;
            }
            Debug.Assert(w3 == 0);

            if (shift > 0)
            {
                w5 = (w4 << (32 - shift)) | (w5 >> shift);
                w4 = w4 >> shift;
            }

            return (new UInt128(0, (uint)q1, (uint)q2, (uint)q3), (w4 << 32) | w5);
        }

        public static ulong ModPow(ulong value, ulong exponent, ulong module)
        {
            value %= module;
            exponent %= module;
            if (module < 4294967296UL)
            {
                ulong result = 1;
                while (exponent > 0)
                {
                    if ((exponent & 1) == 1)
                    {
                        result = result * value % module;
                        exponent--;
                    }
                    else
                    {
                        value = value * value % module;
                        exponent >>= 1;
                    }
                }
                return result;
            }
            else
            {
                UInt128 result = 1;
                UInt128 value128 = value;
                while (exponent > 0)
                {
                    if ((exponent & 1) == 1)
                    {
                        result = result * value128 % module;
                        exponent--;
                    }
                    else
                    {
                        value128 = value128 * value128 % module;
                        exponent >>= 1;
                    }
                }
                return (ulong)result;
            }
        }

        #endregion

        #region ToString & Parse

        public override string ToString()
        {
            var sb = new StringBuilder();
            UInt128 value = this;
            while (value > 0)
            {
                uint r;
                (value, r) = DivMod(value, 1000000000);
                sb.Insert(0, r.ToString().PadLeft(9, '0'));
            }

            if (sb.Length == 0)
                return "0";

            int zeros = 0;
            while (sb[zeros] == '0')
                zeros++;

            sb.Remove(0, zeros);

            return sb.ToString();
        }

        #endregion

        #region Private methods

        #endregion
    }
}