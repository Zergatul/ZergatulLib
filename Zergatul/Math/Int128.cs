using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.Math
{
    public struct Int128
    {
        internal uint _w1;
        internal uint _w2;
        internal uint _w3;
        internal uint _w4;

        #region Constructors

        public Int128(uint w1, uint w2, uint w3, uint w4)
        {
            _w1 = w1;
            _w2 = w2;
            _w3 = w3;
            _w4 = w4;
        }

        public Int128(int value)
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
                _w1 = uint.MaxValue;
                _w2 = uint.MaxValue;
                _w3 = uint.MaxValue;
                _w4 = (uint)value;
            }
        }

        public Int128(long value)
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
                _w1 = uint.MaxValue;
                _w2 = uint.MaxValue;
                _w3 = (uint)(value >> 32);
                _w4 = (uint)(value & 0xFFFFFFFF);
            }
        }

        public Int128(ulong value)
        {
            _w1 = 0;
            _w2 = 0;
            _w3 = (uint)(value >> 32);
            _w4 = (uint)(value & 0xFFFFFFFF);
        }

        #endregion

        #region Math operators

        public static Int128 operator -(Int128 value)
        {
            uint w1 = ~value._w1;
            uint w2 = ~value._w2;
            uint w3 = ~value._w3;
            uint w4 = ~value._w4;

            ulong carry = 1;

            carry = w4 + carry;
            w4 = (uint)carry;
            carry >>= 32;

            carry = w3 + carry;
            w3 = (uint)carry;
            carry >>= 32;

            carry = w2 + carry;
            w2 = (uint)carry;
            carry >>= 32;

            carry = w1 + carry;
            w1 = (uint)carry;

            return new Int128(w1, w2, w3, w4);
        }

        //public static Int128 operator +(Int128 val1, Int128 val2)
        //{
        //    ulong carry = 0;

        //    carry = w4 + carry;
        //    w4 = (uint)carry;
        //    carry >>= 32;

        //    carry = w3 + carry;
        //    w3 = (uint)carry;
        //    carry >>= 32;

        //    carry = w2 + carry;
        //    w2 = (uint)carry;
        //    carry >>= 32;

        //    carry = w1 + carry;
        //    w1 = (uint)carry;
        //}

        #endregion

        #region Compare operators

        public static bool operator >(Int128 left, int right)
        {
            if (right >= 0)
            {
                return
                    (left._w1 & 0x80000000) == 0 &&
                    left._w1 > 0 ||
                    left._w2 > 0 ||
                    left._w3 > 0 ||
                    left._w4 > (uint)right;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static bool operator <(Int128 left, int right)
        {
            if (right >= 0)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Object overrides

        public override string ToString()
        {
            var sb = new StringBuilder();

            Int128 value = this;
            bool isNegative = false;
            if (value.IsNegative)
            {
                isNegative = true;
                value = -value;
            }

            while (!value.IsZero)
            {
                int rem;
                (value, rem) = DivMod(value, 1000000000);
                sb.Insert(0, rem.ToString().PadLeft(9, '0'));
            }

            if (sb.Length == 0)
                return "0";

            int zeros = 0;
            while (sb[zeros] == '0')
                zeros++;

            sb.Remove(0, zeros);

            if (isNegative)
                sb.Insert(0, '-');

            return sb.ToString();
        }

        #endregion

        #region Public instance methods

        public bool IsZero => _w1 == 0 && _w2 == 0 && _w3 == 0 && _w4 == 0;
        public bool IsNegative => (_w1 & 0x80000000) != 0;

        #endregion

        #region Static

        public static (Int128, int) DivMod(Int128 divider, int divisor)
        {
            bool isDividerNegative;
            if ((divider._w1 & 0x80000000) != 0)
            {
                isDividerNegative = true;
                divider = -divider;
            }
            else
            {
                isDividerNegative = false;
            }

            bool isDivisorNegative = divisor < 0;
            uint divisorUInt = divisor < 0 ? (uint)-divisor : (uint)divisor;

            ulong carry;
            carry = divider._w1 / divisorUInt;
            uint w1 = (uint)carry;
            carry = (ulong)(divider._w1 - divisor * w1);

            carry = ((carry << 32) | divider._w2) / divisorUInt;
            uint w2 = (uint)carry;
            carry = ((carry << 32) | divider._w2) - divisorUInt * w2;

            carry = ((carry << 32) | divider._w3) / divisorUInt;
            uint w3 = (uint)carry;
            carry = ((carry << 32) | divider._w3) - divisorUInt * w3;

            carry = ((carry << 32) | divider._w4) / divisorUInt;
            uint w4 = (uint)carry;
            carry = ((carry << 32) | divider._w4) - divisorUInt * w4;

            Int128 div = new Int128(w1, w2, w3, w4);
            int mod = (int)carry;

            if (isDividerNegative ^ isDivisorNegative)
                div = -div;
            if (isDividerNegative)
                mod = -mod;

            return (div, mod);
        }

        #endregion
    }
}