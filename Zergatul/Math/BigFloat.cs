using System;

namespace Zergatul.Math
{
    public struct BigFloat
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

            _mantissaBits = mantissaBits;

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
                _mantissaSign = 0;
                _mantissaLength = 0;
                _mantissa = null;
                _exponentSign = 0;
                _exponentLength = 0;
                _exponent = null;
            }
            else
            {
                while ((mantissa & 1) == 0)
                {
                    mantissa >>= 1;
                    exponent++;
                }

                _mantissaSign = negative ? -1 : 1;
                if (mantissa > uint.MaxValue)
                {
                    throw new NotImplementedException();
                    //this._mantissa = new uint[] { (uint)(mantissa & 0xFFFFFFFF), (uint)(mantissa >> 32) };
                    //this._mantissaLength = 2;
                }
                else
                {
                    _mantissa = new uint[] { (uint)mantissa };
                    _mantissaLength = 1;
                    _mantissaSign = 1;

                    if (exponent == 0)
                    {
                        _exponentLength = 0;
                        _exponentSign = 0;
                        _exponent = null;
                    }
                    else
                    {
                        _exponentLength = 1;
                        if (exponent > 0)
                        {
                            _exponent = new uint[] { (uint)exponent };
                            _exponentSign = 1;
                        }
                        else
                        {
                            _exponent = new uint[] { (uint)(-exponent) };
                            _exponentSign = -1;
                        }
                    }
                }
            }
        }

        #endregion

        #region Converters

        public static explicit operator double(BigFloat value)
        {
            if (value.IsZero)
                return 0;

            if (value._exponentLength > 1)
                throw new OverflowException();

            long exponent = 0;
            if (value._exponentLength == 1)
            {
                exponent = value._exponent[0];
            }

            long mantissa = (long)value._mantissa[0] << 20;
            //if (value._mantissaLength > 1)
            //    mantissa


            return 0;
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