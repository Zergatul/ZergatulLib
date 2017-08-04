using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math.EllipticCurves.PrimeField
{
    /// <summary>
    /// y² ≡ x³ + ax + b (mod p)
    /// </summary>
    public class EllipticCurve : IEllipticCurve
    {
        public BigInteger a;
        public BigInteger b;
        public BigInteger p;
        public ECPoint g;
        public BigInteger n;
        public int h;

        public EllipticCurve()
        {

        }

        public EllipticCurve(BigInteger a, BigInteger b, BigInteger p, ECPoint g, BigInteger n, int h)
        {
            this.a = a;
            this.b = b;
            this.p = p;
            this.n = n;
            this.h = h;

            this.g = g;
            this.g.Curve = this;
        }

        #region IEllipticCurve

        public int BitSize => n.BitSize;

        #endregion

        #region Curves

        public static EllipticCurve secp112r1 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0xDB7C, 0x2ABF62E3, 0x5E668076, 0xBEAD208B }, ByteOrder.BigEndian),
            a: new BigInteger(new uint[] { 0xDB7C, 0x2ABF62E3, 0x5E668076, 0xBEAD2088 }, ByteOrder.BigEndian),
            b: new BigInteger(new uint[] { 0x659E, 0xF8BA0439, 0x16EEDE89, 0x11702B22 }, ByteOrder.BigEndian),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x0948, 0x7239995A, 0x5EE76B55, 0xF9C2F098 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0xA89C, 0xE5AF8724, 0xC0A23E0E, 0x0FF77500 }, ByteOrder.BigEndian)
            },
            n: new BigInteger(new uint[] { 0xDB7C, 0x2ABF62E3, 0x5E7628DF, 0xAC6561C5 }, ByteOrder.BigEndian),
            h: 1);

        public static EllipticCurve secp112r2 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0xDB7C, 0x2ABF62E3, 0x5E668076, 0xBEAD208B }, ByteOrder.BigEndian),
            a: new BigInteger(new uint[] { 0x6127, 0xC24C05F3, 0x8A0AAAF6, 0x5C0EF02C }, ByteOrder.BigEndian),
            b: new BigInteger(new uint[] { 0x51DE, 0xF1815DB5, 0xED74FCC3, 0x4C85D709 }, ByteOrder.BigEndian),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x4BA3, 0x0AB5E892, 0xB4E1649D, 0xD0928643 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0xADCD, 0x46F5882E, 0x3747DEF3, 0x6E956E97 }, ByteOrder.BigEndian)
            },
            n: new BigInteger(new uint[] { 0x36DF, 0x0AAFD8B8, 0xD7597CA1, 0x0520D04B }, ByteOrder.BigEndian),
            h: 4);

        public static EllipticCurve secp128r1 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0xFFFFFFFD, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF }, ByteOrder.BigEndian),
            a: new BigInteger(new uint[] { 0xFFFFFFFD, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFC }, ByteOrder.BigEndian),
            b: new BigInteger(new uint[] { 0xE87579C1, 0x1079F43D, 0xD824993C, 0x2CEE5ED3 }, ByteOrder.BigEndian),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x161FF752, 0x8B899B2D, 0x0C28607C, 0xA52C5B86 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0xCF5AC839, 0x5BAFEB13, 0xC02DA292, 0xDDED7A83 }, ByteOrder.BigEndian)
            },
            n: new BigInteger(new uint[] { 0xFFFFFFFE, 0x00000000, 0x75A30D1B, 0x9038A115 }, ByteOrder.BigEndian),
            h: 1);

        public static EllipticCurve secp128r2 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0xFFFFFFFD, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF }, ByteOrder.BigEndian),
            a: new BigInteger(new uint[] { 0xD6031998, 0xD1B3BBFE, 0xBF59CC9B, 0xBFF9AEE1 }, ByteOrder.BigEndian),
            b: new BigInteger(new uint[] { 0x5EEEFCA3, 0x80D02919, 0xDC2C6558, 0xBB6D8A5D }, ByteOrder.BigEndian),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x7B6AA5D8, 0x5E572983, 0xE6FB32A7, 0xCDEBC140 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x27B6916A, 0x894D3AEE, 0x7106FE80, 0x5FC34B44 }, ByteOrder.BigEndian)
            },
            n: new BigInteger(new uint[] { 0x3FFFFFFF, 0x7FFFFFFF, 0xBE002472, 0x0613B5A3 }, ByteOrder.BigEndian),
            h: 4);

        public static EllipticCurve secp160k1 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE, 0xFFFFAC73 }, ByteOrder.BigEndian),
            a: BigInteger.Zero,
            b: new BigInteger(7),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x3B4C382C, 0xE37AA192, 0xA4019E76, 0x3036F4F5, 0xDD4D7EBB }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x938CF935, 0x318FDCED, 0x6BC28286, 0x531733C3, 0xF03C4FEE }, ByteOrder.BigEndian)
            },
            n: new BigInteger(new uint[] { 0x01, 0x00000000, 0x00000000, 0x0001B8FA, 0x16DFAB9A, 0xCA16B6B3 }, ByteOrder.BigEndian),
            h: 1);

        public static EllipticCurve secp160r1 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0x7FFFFFFF }, ByteOrder.BigEndian),
            a: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0x7FFFFFFC }, ByteOrder.BigEndian),
            b: new BigInteger(new uint[] { 0x1C97BEFC, 0x54BD7A8B, 0x65ACF89F, 0x81D4D4AD, 0xC565FA45 }, ByteOrder.BigEndian),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x4A96B568, 0x8EF57328, 0x46646989, 0x68C38BB9, 0x13CBFC82 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x23A62855, 0x3168947D, 0x59DCC912, 0x04235137, 0x7AC5FB32 }, ByteOrder.BigEndian)
            },
            n: new BigInteger(new uint[] { 0x01, 0x00000000, 0x00000000, 0x0001F4C8, 0xF927AED3, 0xCA752257 }, ByteOrder.BigEndian),
            h: 1);

        public static EllipticCurve secp160r2 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE, 0xFFFFAC73 }, ByteOrder.BigEndian),
            a: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE, 0xFFFFAC70 }, ByteOrder.BigEndian),
            b: new BigInteger(new uint[] { 0xB4E134D3, 0xFB59EB8B, 0xAB572749, 0x04664D5A, 0xF50388BA }, ByteOrder.BigEndian),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x52DCB034, 0x293A117E, 0x1F4FF11B, 0x30F7199D, 0x3144CE6D }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0xFEAFFEF2, 0xE331F296, 0xE071FA0D, 0xF9982CFE, 0xA7D43F2E }, ByteOrder.BigEndian)
            },
            n: new BigInteger(new uint[] { 0x01, 0x00000000, 0x00000000, 0x0000351E, 0xE786A818, 0xF3A1A16B }, ByteOrder.BigEndian),
            h: 1);

        public static EllipticCurve secp192k1 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE, 0xFFFFEE37 }, ByteOrder.BigEndian),
            a: BigInteger.Zero,
            b: new BigInteger(3),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0xDB4FF10E, 0xC057E9AE, 0x26B07D02, 0x80B7F434, 0x1DA5D1B1, 0xEAE06C7D }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x9B2F2F6D, 0x9C5628A7, 0x844163D0, 0x15BE8634, 0x4082AA88, 0xD95E2F9D }, ByteOrder.BigEndian)
            },
            n: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE, 0x26F2FC17, 0x0F69466A, 0x74DEFD8D }, ByteOrder.BigEndian),
            h: 1);

        public static EllipticCurve secp192r1 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE, 0xFFFFFFFF, 0xFFFFFFFF }, ByteOrder.BigEndian),
            a: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE, 0xFFFFFFFF, 0xFFFFFFFC }, ByteOrder.BigEndian),
            b: new BigInteger(new uint[] { 0x64210519, 0xE59C80E7, 0x0FA7E9AB, 0x72243049, 0xFEB8DEEC, 0xC146B9B1 }, ByteOrder.BigEndian),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x188DA80E, 0xB03090F6, 0x7CBF20EB, 0x43A18800, 0xF4FF0AFD, 0x82FF1012 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x07192B95, 0xFFC8DA78, 0x631011ED, 0x6B24CDD5, 0x73F977A1, 0x1E794811 }, ByteOrder.BigEndian)
            },
            n : new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0x99DEF836, 0x146BC9B1, 0xB4D22831 }, ByteOrder.BigEndian),
            h: 1);

        public static EllipticCurve secp224k1 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE, 0xFFFFE56D }, ByteOrder.BigEndian),
            a: BigInteger.Zero,
            b: new BigInteger(5),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0xA1455B33, 0x4DF099DF, 0x30FC28A1, 0x69A467E9, 0xE47075A9, 0x0F7E650E, 0xB6B7A45C }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x7E089FED, 0x7FBA3442, 0x82CAFBD6, 0xF7E319F7, 0xC0B0BD59, 0xE2CA4BDB, 0x556D61A5 }, ByteOrder.BigEndian)
            },
            n: new BigInteger(new uint[] { 0x01, 0x00000000, 0x00000000, 0x00000000, 0x0001DCE8, 0xD2EC6184, 0xCAF0A971, 0x769FB1F7 }, ByteOrder.BigEndian),
            h: 1);

        public static EllipticCurve secp224r1 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0x00000000, 0x00000000, 0x00000001 }, ByteOrder.BigEndian),
            a: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE }, ByteOrder.BigEndian),
            b: new BigInteger(new uint[] { 0xB4050A85, 0x0C04B3AB, 0xF5413256, 0x5044B0B7, 0xD7BFD8BA, 0x270B3943, 0x2355FFB4 }, ByteOrder.BigEndian),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0xB70E0CBD, 0x6BB4BF7F, 0x321390B9, 0x4A03C1D3, 0x56C21122, 0x343280D6, 0x115C1D21 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0xBD376388, 0xB5F723FB, 0x4C22DFE6, 0xCD4375A0, 0x5A074764, 0x44D58199, 0x85007E34 }, ByteOrder.BigEndian)
            },
            n: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFF16A2, 0xE0B8F03E, 0x13DD2945, 0x5C5C2A3D }, ByteOrder.BigEndian),
            h: 1);

        public static EllipticCurve secp256k1 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE, 0xFFFFFC2F }, ByteOrder.BigEndian),
            a: BigInteger.Zero,
            b: new BigInteger(7),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x79BE667E, 0xF9DCBBAC, 0x55A06295, 0xCE870B07, 0x029BFCDB, 0x2DCE28D9, 0x59F2815B, 0x16F81798 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x483ADA77, 0x26A3C465, 0x5DA4FBFC, 0x0E1108A8, 0xFD17B448, 0xA6855419, 0x9C47D08F, 0xFB10D4B8 }, ByteOrder.BigEndian)
            },
            n: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE, 0xBAAEDCE6, 0xAF48A03B, 0xBFD25E8C, 0xD0364141 }, ByteOrder.BigEndian),
            h: 1);

        public static EllipticCurve secp256r1 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0xFFFFFFFF, 0x00000001, 0x00000000, 0x00000000, 0x00000000, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF }, ByteOrder.BigEndian),
            a: new BigInteger(new uint[] { 0xFFFFFFFF, 0x00000001, 0x00000000, 0x00000000, 0x00000000, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFC }, ByteOrder.BigEndian),
            b: new BigInteger(new uint[] { 0x5AC635D8, 0xAA3A93E7, 0xB3EBBD55, 0x769886BC, 0x651D06B0, 0xCC53B0F6, 0x3BCE3C3E, 0x27D2604B }, ByteOrder.BigEndian),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x6B17D1F2, 0xE12C4247, 0xF8BCE6E5, 0x63A440F2, 0x77037D81, 0x2DEB33A0, 0xF4A13945, 0xD898C296 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x4FE342E2, 0xFE1A7F9B, 0x8EE7EB4A, 0x7C0F9E16, 0x2BCE3357, 0x6B315ECE, 0xCBB64068, 0x37BF51F5 }, ByteOrder.BigEndian)
            },
            n: new BigInteger(new uint[] { 0xFFFFFFFF, 0x00000000, 0xFFFFFFFF, 0xFFFFFFFF, 0xBCE6FAAD, 0xA7179E84, 0xF3B9CAC2, 0xFC632551 }, ByteOrder.BigEndian),
            h: 1);

        public static EllipticCurve secp384r1 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE, 0xFFFFFFFF, 0x00000000, 0x00000000, 0xFFFFFFFF }, ByteOrder.BigEndian),
            a: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE, 0xFFFFFFFF, 0x00000000, 0x00000000, 0xFFFFFFFC }, ByteOrder.BigEndian),
            b: new BigInteger(new uint[] { 0xB3312FA7, 0xE23EE7E4, 0x988E056B, 0xE3F82D19, 0x181D9C6E, 0xFE814112, 0x0314088F, 0x5013875A, 0xC656398D, 0x8A2ED19D, 0x2A85C8ED, 0xD3EC2AEF }, ByteOrder.BigEndian),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0xAA87CA22, 0xBE8B0537, 0x8EB1C71E, 0xF320AD74, 0x6E1D3B62, 0x8BA79B98, 0x59F741E0, 0x82542A38, 0x5502F25D, 0xBF55296C, 0x3A545E38, 0x72760AB7 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x3617DE4A, 0x96262C6F, 0x5D9E98BF, 0x9292DC29, 0xF8F41DBD, 0x289A147C, 0xE9DA3113, 0xB5F0B8C0, 0x0A60B1CE, 0x1D7E819D, 0x7A431D7C, 0x90EA0E5F }, ByteOrder.BigEndian)
            },
            n: new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xC7634D81, 0xF4372DDF, 0x581A0DB2, 0x48B0A77A, 0xECEC196A, 0xCCC52973 }, ByteOrder.BigEndian),
            h: 1);

        public static EllipticCurve secp521r1 = new EllipticCurve(
            p: new BigInteger(new uint[] { 0x01FF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF }, ByteOrder.BigEndian),
            a: new BigInteger(new uint[] { 0x01FF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFC }, ByteOrder.BigEndian),
            b: new BigInteger(new uint[] { 0x0051, 0x953EB961, 0x8E1C9A1F, 0x929A21A0, 0xB68540EE, 0xA2DA725B, 0x99B315F3, 0xB8B48991, 0x8EF109E1, 0x56193951, 0xEC7E937B, 0x1652C0BD, 0x3BB1BF07, 0x3573DF88, 0x3D2C34F1, 0xEF451FD4, 0x6B503F00 }, ByteOrder.BigEndian),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x00C6, 0x858E06B7, 0x0404E9CD, 0x9E3ECB66, 0x2395B442, 0x9C648139, 0x053FB521, 0xF828AF60, 0x6B4D3DBA, 0xA14B5E77, 0xEFE75928, 0xFE1DC127, 0xA2FFA8DE, 0x3348B3C1, 0x856A429B, 0xF97E7E31, 0xC2E5BD66 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x0118, 0x39296A78, 0x9A3BC004, 0x5C8A5FB4, 0x2C7D1BD9, 0x98F54449, 0x579B4468, 0x17AFBD17, 0x273E662C, 0x97EE7299, 0x5EF42640, 0xC550B901, 0x3FAD0761, 0x353C7086, 0xA272C240, 0x88BE9476, 0x9FD16650 }, ByteOrder.BigEndian)
            },
            n: new BigInteger(new uint[] { 0x01FF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFA, 0x51868783, 0xBF2F966B, 0x7FCC0148, 0xF709A5D0, 0x3BB5C9B8, 0x899C47AE, 0xBB6FB71E, 0x91386409 }, ByteOrder.BigEndian),
            h: 1);

        #endregion
    }
}