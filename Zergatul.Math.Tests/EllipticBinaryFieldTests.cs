using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Math.EllipticCurves.BinaryField;
using System.Collections.Generic;

namespace Zergatul.Math.Tests
{
    [TestClass]
    public class EllipticBinaryFieldTests
    {
        [TestMethod]
        public void GF4Tests()
        {
            var f = BinaryPolynomial.FromPowers(4, 1, 0);

            var gp = new Dictionary<int, BinaryPolynomial>();
            gp[-1] = BinaryPolynomial.Zero;
            gp[0] = BinaryPolynomial.One;
            gp[1] = BinaryPolynomial.FromPowers(1);
            for (int i = 2; i < 17; i++)
                gp[i] = BinaryPolynomial.ModularMultiplication(gp[i - 1], gp[1], f);

            var curve = new EllipticCurve(gp[4], BinaryPolynomial.One, f, new ECPoint { x = gp[12], y = gp[12] }, 0, null, 0);

            var group = new Tuple<int, int>[]
            {
                null,
                new Tuple<int, int>(12, 12),
                new Tuple<int, int>(5, 11),
                new Tuple<int, int>(6, 8),
                new Tuple<int, int>(0, 6),
                new Tuple<int, int>(9, 10),
                new Tuple<int, int>(10, 8),
                new Tuple<int, int>(3, 8),
                new Tuple<int, int>(-1, 0),
                new Tuple<int, int>(3, 13),
                new Tuple<int, int>(10, 1),
                new Tuple<int, int>(9, 13),
                new Tuple<int, int>(0, 13),
                new Tuple<int, int>(6, 14),
                new Tuple<int, int>(5, 3),
                new Tuple<int, int>(12, -1)
            };

            var p = curve.g;

            for (int i = 2; i < 16; i++)
            {
                p = p + curve.g;

                Assert.IsTrue(p.x == gp[group[i].Item1]);
                Assert.IsTrue(p.y == gp[group[i].Item2]);
            }

            for (int i = 1; i < 16; i++)
            {
                p = new BinaryPolynomial(new BigInteger(i)) * curve.g;

                Assert.IsTrue(p.x == gp[group[i].Item1]);
                Assert.IsTrue(p.y == gp[group[i].Item2]);
            }
        }

        [TestMethod]
        public void Validate_Curves()
        {
            Assert.IsTrue(EllipticCurve.sect163k1.g.Validate());
            Assert.IsTrue(EllipticCurve.sect163r1.g.Validate());
            Assert.IsTrue(EllipticCurve.sect163r2.g.Validate());

            Assert.IsTrue(EllipticCurve.sect193r1.g.Validate());
            Assert.IsTrue(EllipticCurve.sect193r2.g.Validate());

            Assert.IsTrue(EllipticCurve.sect233k1.g.Validate());
            Assert.IsTrue(EllipticCurve.sect233r1.g.Validate());

            Assert.IsTrue(EllipticCurve.sect239k1.g.Validate());

            Assert.IsTrue(EllipticCurve.sect283k1.g.Validate());
            Assert.IsTrue(EllipticCurve.sect283r1.g.Validate());

            Assert.IsTrue(EllipticCurve.sect409k1.g.Validate());
            Assert.IsTrue(EllipticCurve.sect409r1.g.Validate());

            Assert.IsTrue(EllipticCurve.sect571k1.g.Validate());
            Assert.IsTrue(EllipticCurve.sect571r1.g.Validate());
        }
    }
}