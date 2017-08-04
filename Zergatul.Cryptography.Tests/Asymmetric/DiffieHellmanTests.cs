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
        public void Test1()
        {
            var random1 = new TestRandom(new byte[] { 0x03, 0x7C }); // 892
            var random2 = new TestRandom(new byte[] { 0x02, 0xF3 }); // 755

            var dh1 = new DiffieHellman();
            dh1.Parameters = new DiffieHellmanParameters(new BigInteger(2), new BigInteger(997));
            dh1.GenerateKeys(random1);

            var dh2 = new DiffieHellman();
            dh2.Parameters = new DiffieHellmanParameters(new BigInteger(2), new BigInteger(997));
            dh2.GenerateKeys(random2);

            // 2 ^ 892 mod 997
            Assert.IsTrue(dh1.PublicKey == 9);

            // 2 ^ 755 mod 997
            Assert.IsTrue(dh2.PublicKey == 658);

            // 658 ^ 892 mod 997
            dh1.KeyExchange.CalculateSharedSecret(dh2.PublicKey);
            Assert.IsTrue(dh1.KeyExchange.SharedSecret == 249);

            // 9 ^ 755 mod 997
            dh2.KeyExchange.CalculateSharedSecret(dh1.PublicKey);
            Assert.IsTrue(dh2.KeyExchange.SharedSecret == 249);
        }

        [TestMethod]
        public void Test2()
        {
            var random = new DefaultSecureRandom();

            var dh1 = new DiffieHellman();
            dh1.Parameters = DiffieHellmanParameters.Group14;
            dh1.GenerateKeys(random);

            var dh2 = new DiffieHellman();
            dh2.Parameters = DiffieHellmanParameters.Group14;
            dh2.GenerateKeys(random);

            dh1.KeyExchange.CalculateSharedSecret(dh2.PublicKey);
            dh2.KeyExchange.CalculateSharedSecret(dh1.PublicKey);

            Assert.IsTrue(dh1.KeyExchange.SharedSecret == dh2.KeyExchange.SharedSecret);
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