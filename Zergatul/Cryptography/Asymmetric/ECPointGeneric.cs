using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math.EllipticCurves;

namespace Zergatul.Cryptography.Asymmetric
{
    public class ECPointGeneric
    {
        public Math.EllipticCurves.BinaryField.ECPoint BFECPoint;
        public Math.EllipticCurves.PrimeField.ECPoint PFECPoint;

        public static ECPointGeneric Parse(byte[] data, IEllipticCurve curve)
        {
            if (data == null)
                throw new ArgumentNullException();
            if (data.Length == 0)
                throw new ArgumentException(nameof(data));

            if (data[0] != 2 && data[0] != 3 && data[0] != 4)
                throw new Exception("Invalid EC point format");

            byte[] x;
            byte[] y = null;
            if (data[0] == 3) // compressed
            {
                x = new byte[data.Length - 1];
                Array.Copy(data, 1, x, 0, x.Length);
            }
            else
            {
                if (data.Length % 2 == 0)
                    throw new InvalidOperationException("Invalid EC point format");
                x = new byte[data.Length / 2];
                y = new byte[data.Length / 2];
                Array.Copy(data, 1, x, 0, x.Length);
                Array.Copy(data, 1 + x.Length, y, 0, y.Length);
            }

            if (curve is Math.EllipticCurves.PrimeField.EllipticCurve)
            {
                var point = new Math.EllipticCurves.PrimeField.ECPoint
                {
                    Curve = (Math.EllipticCurves.PrimeField.EllipticCurve)curve,
                    x = new Math.BigInteger(x, ByteOrder.BigEndian)
                };
                if (y == null)
                    point.CalculateY((data[0] & 1) == 1);
                else
                {
                    point.y = new Math.BigInteger(y, ByteOrder.BigEndian);
                    if (!point.Validate())
                        throw new InvalidOperationException("Point is not on the curve");
                }
                return new ECPointGeneric { PFECPoint = point };
            }
            if (curve is Math.EllipticCurves.BinaryField.EllipticCurve)
            {
                var point = new Math.EllipticCurves.BinaryField.ECPoint
                {
                    Curve = (Math.EllipticCurves.BinaryField.EllipticCurve)curve,
                    x = new Math.BinaryPolynomial(x, ByteOrder.BigEndian)
                };
                if (y == null)
                    //point.CalculateY(true);
                    throw new NotImplementedException();
                else
                {
                    point.y = new Math.BinaryPolynomial(y, ByteOrder.BigEndian);
                    if (!point.Validate())
                        throw new InvalidOperationException("Point is not on the curve");
                }
                return new ECPointGeneric { BFECPoint = point };
            }

            throw new InvalidOperationException();
        }

        public byte[] XToBytes()
        {
            if (PFECPoint != null)
            {
                int length = (PFECPoint.Curve.p.BitSize + 7) / 8;
                return PFECPoint.x.ToBytes(ByteOrder.BigEndian, length);
            }
            if (BFECPoint != null)
            {
                int length = (BFECPoint.Curve.f.Degree + 7) / 8;
                return BFECPoint.x.ToBytes(ByteOrder.BigEndian, length);
            }

            throw new InvalidOperationException();
        }

        public byte[] ToBytes()
        {
            if (PFECPoint != null)
            {
                int length = (PFECPoint.Curve.p.BitSize + 7) / 8;
                byte[] x = PFECPoint.x.ToBytes(ByteOrder.BigEndian, length);
                byte[] y = PFECPoint.y.ToBytes(ByteOrder.BigEndian, length);

                byte[] result = new byte[1 + 2 * length];
                result[0] = 4;
                Array.Copy(x, 0, result, 1, length);
                Array.Copy(y, 0, result, 1 + length, length);

                return result;
            }
            if (BFECPoint != null)
            {
                int length = (BFECPoint.Curve.f.Degree + 7) / 8;
                byte[] x = BFECPoint.x.ToBytes(ByteOrder.BigEndian, length);
                byte[] y = BFECPoint.y.ToBytes(ByteOrder.BigEndian, length);

                byte[] result = new byte[1 + 2 * length];
                result[0] = 4;
                Array.Copy(x, 0, result, 1, length);
                Array.Copy(y, 0, result, 1 + length, length);

                return result;
            }

            throw new InvalidOperationException();
        }
    }
}