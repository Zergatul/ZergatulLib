using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math
{
    public class BigReal
    {
        private uint[] _mantissa;
        private int _mantissaLength;
        private int _mantissaSign;

        private uint[] _exponent;
        private int _exponentLength;
        private int _exponentSign;

        private int _mantissaBits;

        #region Constructors

        public BigReal(double value, int mantissaBits)
        {
            if (mantissaBits <= 0)
                throw new ArgumentOutOfRangeException("mantissaBits should be >= 1");

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
                _mantissaLength = 0;
                _exponentLength = 0;
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
                    _mantissa = new uint[] { (uint)(mantissa & 0xFFFFFFFF), (uint)(mantissa >> 32) };
                    _mantissaLength = 2;
                }
                else
                {
                    _mantissa = new uint[] { (uint)mantissa };
                    _mantissaLength = 1;
                }

                if (exponent == 0)
                {
                    _exponentLength = 0;
                }
                else
                {
                    if (exponent < 0)
                    {
                        _exponent = new uint[] { (uint)(-exponent) };
                        _exponentLength = 1;
                        _exponentSign = -1;
                    }
                    else
                    {
                        _exponent = new uint[] { (uint)(exponent) };
                        _exponentLength = 1;
                        _exponentSign = 1;
                    }
                }

                TruncateMantissaBits();
            }

            _mantissaBits = mantissaBits;

            TruncateMantissaBits();
        }

        #endregion

        #region ToString

        public string ToExpString()
        {
            //if ()
            return "";
        }

        #endregion

        #region Private methods

        private void TruncateMantissaBits()
        {

        }

        #endregion
    }
}