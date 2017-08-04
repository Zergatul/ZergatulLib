using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Math;

namespace Zergatul.Cryptography.Tests.Asymmetric
{
    [TestClass]
    public class ECDiffieHellmanTests
    {
        [TestMethod]
        public void TestPrimaryField1()
        {
            TestCurve(Math.EllipticCurves.PrimeField.EllipticCurve.secp112r1);
        }

        [TestMethod]
        public void TestPrimaryField2()
        {
            TestCurve(Math.EllipticCurves.PrimeField.EllipticCurve.secp256k1);
        }

        [TestMethod]
        public void TestBinaryField1()
        {
            TestCurve(Math.EllipticCurves.BinaryField.EllipticCurve.sect163k1);
        }

        [TestMethod]
        public void TestBinaryField2()
        {
            TestCurve(Math.EllipticCurves.BinaryField.EllipticCurve.sect163r1);
        }

        [TestMethod]
        public void TestBinaryField3()
        {
            TestCurve(Math.EllipticCurves.BinaryField.EllipticCurve.sect571k1);
        }

        [TestMethod]
        public void TestBinaryField4()
        {
            TestCurve(Math.EllipticCurves.BinaryField.EllipticCurve.sect571r1);
        }

        [TestMethod]
        public void TestBinaryField5()
        {
            var curve = Math.EllipticCurves.BinaryField.EllipticCurve.sect163k1;

            var ecdh1 = new ECDiffieHellman();
            var ecdh2 = new ECDiffieHellman();
            ecdh1.Parameters = ecdh2.Parameters = curve;

            ecdh1.PrivateKey = new ECPrivateKey
            {
                BinaryPolynomial = BinaryPolynomial.FromPowers(161, 159, 155, 154, 153, 151, 149, 147, 145, 143, 142, 141, 140, 138, 130, 129, 128, 127, 126, 122, 121, 118, 117, 116, 113, 112, 110, 109, 108, 106, 105, 104, 101, 100, 98, 97, 94, 91, 90, 89, 88, 87, 86, 85, 84, 82, 77, 75, 74, 73, 70, 62, 61, 60, 59, 53, 51, 49, 48, 47, 46, 45, 42, 41, 35, 34, 32, 31, 29, 28, 27, 26, 19, 18, 17, 15, 14, 10, 9, 7, 6, 4, 2, 1)
            };
            ecdh1.PublicKey = new ECPointGeneric
            {
                BFECPoint = ecdh1.PrivateKey.BinaryPolynomial * curve.g
            };

            ecdh2.PrivateKey = new ECPrivateKey
            {
                BinaryPolynomial = BinaryPolynomial.FromPowers(161, 158, 157, 154, 151, 150, 148, 145, 142, 140, 139, 138, 136, 135, 134, 132, 131, 129, 128, 126, 125, 124, 123, 122, 121, 113, 111, 110, 109, 106, 104, 102, 100, 96, 95, 93, 92, 91, 89, 87, 86, 84, 83, 82, 79, 78, 77, 76, 74, 72, 71, 70, 68, 66, 65, 64, 63, 60, 59, 58, 57, 56, 54, 51, 50, 48, 44, 40, 36, 35, 34, 32, 31, 30, 29, 27, 26, 25, 24, 20, 18, 16, 15, 11, 10, 9, 6, 5, 1, 0)
            };
            ecdh2.PublicKey = new ECPointGeneric
            {
                BFECPoint = ecdh2.PrivateKey.BinaryPolynomial * curve.g
            };

            ecdh1.KeyExchange.CalculateSharedSecret(ecdh2.PublicKey);
            ecdh2.KeyExchange.CalculateSharedSecret(ecdh1.PublicKey);

            Assert.IsTrue(ecdh1.KeyExchange.SharedSecret.BFECPoint == ecdh2.KeyExchange.SharedSecret.BFECPoint);
        }

        private static void TestCurve(Math.EllipticCurves.IEllipticCurve curve)
        {
            var random = new DefaultSecureRandom();

            var ecdh1 = new ECDiffieHellman();
            ecdh1.Parameters = curve;
            ecdh1.GenerateKeys(random);

            var ecdh2 = new ECDiffieHellman();
            ecdh2.Parameters = curve;
            ecdh2.GenerateKeys(random);

            ecdh1.KeyExchange.CalculateSharedSecret(ecdh2.PublicKey);
            ecdh2.KeyExchange.CalculateSharedSecret(ecdh1.PublicKey);

            Assert.IsTrue(ecdh1.KeyExchange.SharedSecret.PFECPoint == ecdh2.KeyExchange.SharedSecret.PFECPoint);
            Assert.IsTrue(ecdh1.KeyExchange.SharedSecret.BFECPoint == ecdh2.KeyExchange.SharedSecret.BFECPoint);
        }
    }
}