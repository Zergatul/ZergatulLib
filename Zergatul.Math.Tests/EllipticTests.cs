using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Zergatul.Math.Tests
{
    [TestClass]
    public class EllipticTests
    {
        [TestMethod]
        public void Double_1()
        {
            var curve = new EllipticCurve
            {
                a = BigInteger.One,
                b = BigInteger.One,
                p = new BigInteger(23)
            };
            var point = new ECPoint
            {
                x = new BigInteger(3),
                y = new BigInteger(10),
                Curve = curve
            };
            var result = ECPoint.Double(point);
            Assert.IsTrue(result.x == 7);
            Assert.IsTrue(result.y == 12);
        }

        [TestMethod]
        public void Mult_1()
        {
            var curve = new EllipticCurve
            {
                a = BigInteger.One,
                b = BigInteger.One,
                p = new BigInteger(23)
            };
            var point = new ECPoint
            {
                x = new BigInteger(3),
                y = new BigInteger(10),
                Curve = curve
            };
            Dictionary<int, Tuple<int, int>> results = new Dictionary<int, Tuple<int, int>>
            {
                [ 1] = new Tuple<int, int>( 3, 10),
                [ 2] = new Tuple<int, int>( 7, 12),
                [ 3] = new Tuple<int, int>(19,  5),
                [ 4] = new Tuple<int, int>(17,  3),
                [ 5] = new Tuple<int, int>( 9, 16),
                [ 6] = new Tuple<int, int>(12,  4),
                [ 7] = new Tuple<int, int>(11,  3),
                [ 8] = new Tuple<int, int>(13, 16),
                [ 9] = new Tuple<int, int>( 0,  1),
                [10] = new Tuple<int, int>( 6,  4),
                [11] = new Tuple<int, int>(18, 20),
                [12] = new Tuple<int, int>( 5,  4),
                [13] = new Tuple<int, int>( 1,  7),
                [14] = new Tuple<int, int>( 4,  0),
                [15] = new Tuple<int, int>( 1, 16),
                [16] = new Tuple<int, int>( 5, 19),
                [17] = new Tuple<int, int>(18,  3),
                [18] = new Tuple<int, int>( 6, 19),
                [19] = new Tuple<int, int>( 0, 22),
                [20] = new Tuple<int, int>(13,  7),
                [21] = new Tuple<int, int>(11, 20),
                [22] = new Tuple<int, int>(12, 19),
                [23] = new Tuple<int, int>( 9,  7),
                [24] = new Tuple<int, int>(17, 20),
                [25] = new Tuple<int, int>(19, 18),
                [26] = new Tuple<int, int>( 7, 11),
                [27] = new Tuple<int, int>( 3, 13),
            };
            foreach (int key in results.Keys)
            {
                var pm = ECPoint.Multiplication(point, new BigInteger(key));
                Assert.IsTrue(pm.x == results[key].Item1);
                Assert.IsTrue(pm.y == results[key].Item2);
            }
        }

        [TestMethod]
        public void Mult_2()
        {
            int sum = 100;
            ECPoint result = ECPoint.Multiplication(EllipticCurve.secp192r1.g, new BigInteger(sum));
            for (int i = 1; i < sum / 2; i++)
            {
                var p1 = ECPoint.Multiplication(EllipticCurve.secp192r1.g, new BigInteger(i));
                var p2 = ECPoint.Multiplication(EllipticCurve.secp192r1.g, new BigInteger(sum - i));

                Assert.IsTrue(ECPoint.Sum(p1, p2) == result);
            }
        }

        [TestMethod]
        public void Validate_Curves()
        {
            Assert.IsTrue(EllipticCurve.secp112r1.g.Validate());
            Assert.IsTrue(EllipticCurve.secp112r2.g.Validate());

            Assert.IsTrue(EllipticCurve.secp128r1.g.Validate());
            Assert.IsTrue(EllipticCurve.secp128r2.g.Validate());

            Assert.IsTrue(EllipticCurve.secp160k1.g.Validate());
            Assert.IsTrue(EllipticCurve.secp160r1.g.Validate());
            Assert.IsTrue(EllipticCurve.secp160r2.g.Validate());

            Assert.IsTrue(EllipticCurve.secp192k1.g.Validate());
            Assert.IsTrue(EllipticCurve.secp192r1.g.Validate());

            Assert.IsTrue(EllipticCurve.secp224k1.g.Validate());
            Assert.IsTrue(EllipticCurve.secp224r1.g.Validate());

            Assert.IsTrue(EllipticCurve.secp384r1.g.Validate());

            Assert.IsTrue(EllipticCurve.secp521r1.g.Validate());
        }
    }
}
