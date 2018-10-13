using System;

namespace Zergatul.Math
{
    public class BigFloat
    {
        // representation:
        // 0.[mantissa] * 2 ^ [exponent]
        // mantissa - big endian
        // exponent - little endian

        private uint[] _mantissa;
        private int _mantissaLength;
        private int _mantissaSign;

        private uint[] _exponent;
        private int _exponentLength;
        private int _exponentSign;

        private int _mantissaBits;

        #region Public Properties

        public bool IsZero => _mantissaSign == 0;

        #endregion

        #region Constructors

        public BigFloat(double value, int mantissaBits)
        {
            if (mantissaBits <= 0)
                throw new ArgumentOutOfRangeException("mantissaBits should be >= 1");

            this._mantissaBits = mantissaBits;

            long bits = BitConverter.DoubleToInt64Bits(value);
            bool negative = bits < 0;
            int exponent = (int)((bits >> 52) & 0x7FFL);
            long mantissa = bits & 0xFFFFFFFFFFFFFL;

            if (exponent == 0)
                exponent++;
            else
                mantissa |= 1L << 52;

            exponent -= 1075;

            if (mantissa == 0)
            {
                this._mantissaSign = 0;
                this._mantissaLength = 0;
                this._exponentLength = 0;
            }
            else
            {
                while ((mantissa & 1) == 0)
                {
                    mantissa >>= 1;
                    exponent++;
                }

                this._mantissaSign = negative ? -1 : 1;
                if (mantissa > uint.MaxValue)
                {
                    throw new NotImplementedException();
                    //this._mantissa = new uint[] { (uint)(mantissa & 0xFFFFFFFF), (uint)(mantissa >> 32) };
                    //this._mantissaLength = 2;
                }
                else
                {
                    this._mantissa = new uint[] { (uint)mantissa };
                    this._mantissaLength = 1;

                    exponent += 32;

                    if (exponent == 0)
                    {
                        this._exponentLength = 0;
                        this._exponentSign = 0;
                    }
                    else
                    {
                        this._exponentLength = 1;
                        if (exponent > 0)
                        {
                            this._exponent = new uint[] { (uint)exponent };
                            this._exponentSign = 1;
                        }
                        else
                        {
                            this._exponent = new uint[] { (uint)(-exponent) };
                            this._exponentSign = -1;
                        }
                    }
                }

                Normalize();
            }
        }

        #endregion

        #region ToString

        public string ToExponentialString(int decimals, int radix = 10)
        {
            if (IsZero)
                throw new NotImplementedException();

            uint[] mantissa = new uint[_mantissaLength];
            return "";
        }

        //public string ToNormalString()
        //{
        //    return "";
        //}

        public override string ToString()
        {
            if (_mantissaLength == 0)
                return "0";

            uint[] buffer = new uint[_mantissaLength];
            Array.Copy(_mantissa, buffer, _mantissaLength);
            BigInteger mantissa = new BigInteger(buffer, ByteOrder.LittleEndian);
            BigInteger integerPart;
            BigInteger fractionalPart;

            int exponentInt;
            if (_exponentLength > 0)
            {
                if (_exponentLength > 1 || _exponent[0] > int.MaxValue)
                    throw new NotImplementedException();

                if (_exponentSign == 1)
                    exponentInt = (int)_exponent[0];
                else
                    exponentInt = -(int)_exponent[0];
            }
            else
                exponentInt = 0;

            int mantissaBitLength = mantissa.BitSize;
            integerPart = mantissa << (mantissaBitLength + exponentInt - 1);

            string sign = _mantissaSign == -1 ? "-" : "";

            return sign + integerPart.ToString();
        }

        #endregion

        #region Private methods

        private void Normalize()
        {
            if (IsZero)
                return;

            int shift = 0;
            uint high = _mantissa[0];
            while ((high & 0x80000000U) == 0)
            {
                shift++;
                high <<= 1;
            }

            if (shift > 0)
            {

            }
        }

        #endregion
    }
}