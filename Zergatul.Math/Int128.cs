using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math
{
    public struct Int128
    {
        internal int _high1;
        internal uint _high2;
        internal uint _low1;
        internal uint _low2;

        public int WordsLength
        {
            get
            {
                if (_high1 != 0)
                    return 4;
                if (_high2 != 0)
                    return 3;
                if (_low1 != 0)
                    return 2;
                if (_low2 != 0)
                    return 1;
                return 0;
            }
        }

        public Int128(int high1, uint high2, uint low1, uint low2)
        {
            this._high1 = high1;
            this._high2 = high2;
            this._low1 = low1;
            this._low2 = low2;
        }

        public Int128(long value)
        {
            if (value >= 0)
            {
                this._high1 = 0;
                this._high2 = 0;
                this._low1 = (uint)(value >> 32);
                this._low2 = (uint)(value & 0xFFFFFFFF);
            }
            else
            {
                this._high1 = -1;
                this._high2 = uint.MaxValue;
                this._low1 = (uint)(value >> 32);
                this._low2 = (uint)(value & 0xFFFFFFFF);
            }
        }

        public Int128(ulong value)
        {
            this._high1 = 0;
            this._high2 = 0;
            this._low1 = (uint)(value >> 32);
            this._low2 = (uint)(value & 0xFFFFFFFF);
        }

        public static Int128 operator +(Int128 left, Int128 right)
        {
            long carry = (long)left._low2 + right._low2;
            uint low2 = (uint)(carry & 0xFFFFFFFF);
            carry = carry >> 32;

            carry = carry + left._low1 + right._low1;
            uint low1 = (uint)(carry & 0xFFFFFFFF);
            carry = carry >> 32;

            carry = carry + left._high2 + right._high2;
            uint high2 = (uint)(carry & 0xFFFFFFFF);
            carry = carry >> 32;

            int high1 = (int)carry + left._high1 + right._high1;

            return new Int128(high1, high2, low1, low2);
        }

        public static Int128 operator -(Int128 left, Int128 right)
        {
            long carry = (long)left._low2 - right._low2;
            uint low2 = (uint)(carry & 0xFFFFFFFF);
            carry = carry >> 32;

            carry = carry + left._low1 - right._low1;
            uint low1 = (uint)(carry & 0xFFFFFFFF);
            carry = carry >> 32;

            carry = carry + left._high2 - right._high2;
            uint high2 = (uint)(carry & 0xFFFFFFFF);
            carry = carry >> 32;

            carry = carry + left._high1 - right._high1;
            int high1 = (int)(carry & 0xFFFFFFFF);

            return new Int128(high1, high2, low1, low2);
        }

        public static Int128 operator *(Int128 left, Int128 right)
        {
            long carry = (long)left._low2 * right._low2;
            uint low2 = (uint)(carry & 0xFFFFFFFF);
            carry = carry >> 32;

            carry = carry + (long)left._low2 * right._low1 + (long)left._low1 * right._low2;
            uint low1 = (uint)(carry & 0xFFFFFFFF);
            carry = carry >> 32;

            carry = carry + (long)left._low2 * right._high2 + (long)left._low1 * right._low1 + (long)left._high2 * right._low2;
            uint high2 = (uint)(carry & 0xFFFFFFFF);
            carry = carry >> 32;

            carry = carry + (long)left._low2 * right._high1 + (long)left._low1 * right._high2 + (long)left._high2 * right._low1 + (long)left._high1 * right._low2;
            int high1 = (int)(carry & 0xFFFFFFFF);

            return new Int128(high1, high2, low1, low2);
        }

        public static Int128 operator /(Int128 left, uint right)
        {
            long carry1 = left._high1 / right;
            int high1 = (int)carry1;
            ulong carry = (ulong)(left._high1 - right * high1);

            carry = ((carry << 32) | left._high2) / right;
            uint high2 = (uint)carry;
            carry = ((carry << 32) | left._high2) - right * high2;

            carry = ((carry << 32) | left._low1) / right;
            uint low1 = (uint)carry;
            carry = ((carry << 32) | left._low1) - right * low1;

            carry = ((carry << 32) | left._low2) / right;
            uint low2 = (uint)carry;

            return new Int128(high1, high2, low1, low2);
        }

        public static bool operator <(Int128 left, int right)
        {
            if (right != 0)
                throw new NotImplementedException();

            return left._high1 < 0;
        }

        public static bool operator >(Int128 left, int right)
        {
            if (right != 0)
                throw new NotImplementedException();

            return left._high1 > 0;
        }
    }
}