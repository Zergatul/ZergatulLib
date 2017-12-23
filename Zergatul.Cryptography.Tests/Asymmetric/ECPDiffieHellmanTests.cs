using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Math.EllipticCurves.PrimeField;
using Zergatul.Cryptography.Asymmetric;

namespace Zergatul.Cryptography.Tests.Asymmetric
{
    [TestClass]
    public class ECPDiffieHellmanTests
    {
        [TestMethod]
        public void TestCurves()
        {
            TestCurve(EllipticCurve.secp112r1);
            TestCurve(EllipticCurve.secp112r2);
            TestCurve(EllipticCurve.secp128r1);
            TestCurve(EllipticCurve.secp128r2);
            TestCurve(EllipticCurve.secp160k1);
            TestCurve(EllipticCurve.secp160r1);
            TestCurve(EllipticCurve.secp160r2);
            TestCurve(EllipticCurve.secp192k1);
            TestCurve(EllipticCurve.secp192r1);
            TestCurve(EllipticCurve.secp224k1);
            TestCurve(EllipticCurve.secp224r1);
            TestCurve(EllipticCurve.secp256k1);
            TestCurve(EllipticCurve.secp256r1);
            TestCurve(EllipticCurve.secp384r1);
            TestCurve(EllipticCurve.secp521r1);
        }

        private static void TestCurve(EllipticCurve curve)
        {
            var random = new DefaultSecureRandom();

            var ecdh1 = new ECPDiffieHellman();
            ecdh1.Random = random;
            ecdh1.Parameters = new ECPParameters(curve);
            ecdh1.GenerateKeyPair(0);

            var ecdh2 = new ECPDiffieHellman();
            ecdh2.Random = random;
            ecdh2.Parameters = new ECPParameters(curve);
            ecdh2.GenerateKeyPair(0);

            byte[] secret1 = ecdh1.CalculateSharedSecret(ecdh2.PublicKey);
            byte[] secret2 = ecdh2.CalculateSharedSecret(ecdh1.PublicKey);

            Assert.IsTrue(ByteArray.Equals(secret1, secret2));
        }
    }
}