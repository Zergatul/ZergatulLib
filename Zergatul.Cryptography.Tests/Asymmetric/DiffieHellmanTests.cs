using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Math;
using System.Collections.Generic;
using Zergatul.Cryptography.Asymmetric;

namespace Zergatul.Cryptography.Tests.Asymmetric
{
    [TestClass]
    public class DiffieHellmanTests
    {
        [TestMethod]
        public void DiffieHellman_Test1()
        {
            var random1 = new TestRandom(new byte[] { 0x03, 0x7C }); // 892
            var random2 = new TestRandom(new byte[] { 0x02, 0xF3 }); // 755

            var dh1 = new DiffieHellman();
            dh1.Random = random1;
            dh1.Parameters = new DiffieHellmanParameters(new BigInteger(2), new BigInteger(997));
            dh1.GenerateKeyPair(0);

            var dh2 = new DiffieHellman();
            dh2.Random = random2;
            dh2.Parameters = new DiffieHellmanParameters(new BigInteger(2), new BigInteger(997));
            dh2.GenerateKeyPair(0);

            // 2 ^ 892 mod 997
            Assert.IsTrue(dh1.PublicKey.Value == 9);

            // 2 ^ 755 mod 997
            Assert.IsTrue(dh2.PublicKey.Value == 658);

            // 658 ^ 892 mod 997
            Assert.IsTrue(new BigInteger(dh1.CalculateSharedSecret(dh2.PublicKey), ByteOrder.BigEndian) == 249);

            // 9 ^ 755 mod 997
            Assert.IsTrue(new BigInteger(dh2.CalculateSharedSecret(dh1.PublicKey), ByteOrder.BigEndian) == 249);
        }

        [TestMethod]
        public void DiffieHellman_Test2()
        {
            var random = new DefaultSecureRandom();

            var dh1 = new DiffieHellman();
            dh1.Random = random;
            dh1.Parameters = DiffieHellmanParameters.Group14;
            dh1.GenerateKeyPair(0);

            var dh2 = new DiffieHellman();
            dh2.Random = random;
            dh2.Parameters = DiffieHellmanParameters.Group14;
            dh2.GenerateKeyPair(0);

            byte[] secret1 = dh1.CalculateSharedSecret(dh2.PublicKey);
            byte[] secret2 = dh2.CalculateSharedSecret(dh1.PublicKey);

            Assert.IsTrue(ByteArray.Equals(secret1, secret1));
        }

        private class TestRandom : AbstractRandom, ISecureRandom
        {
            private IEnumerator<byte> _enumerator;

            public TestRandom(IEnumerable<byte> data)
            {
                this._enumerator = data.GetEnumerator();
                this._enumerator.MoveNext();
            }

            public override void GetBytes(byte[] data, int offset, int count)
            {
                for (int i = offset; i < offset + count; i++)
                {
                    data[i] = _enumerator.Current;
                    _enumerator.MoveNext();
                }
            }
        }
    }
}