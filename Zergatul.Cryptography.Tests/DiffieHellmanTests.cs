using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Math;
using System.Collections.Generic;

namespace Zergatul.Cryptography.Tests
{
    [TestClass]
    public class DiffieHellmanTests
    {
        [TestMethod]
        public void Test1()
        {
            var random = new TestRandom(new byte[] { 0x03, 0x7C });
            var dh = new DiffieHellman(
                new BigInteger(2),
                new BigInteger(997),
                random);
            dh.Ya = new BigInteger(67);
            dh.CalculateForBSide();
            var serverSharedSecret = BigInteger.ModularExponentiation(dh.Yb, new BigInteger(516), dh.p);
            Assert.IsTrue(serverSharedSecret == dh.ZZ);
        }

        private class TestRandom : ISecureRandom
        {
            private IEnumerator<byte> _enumerator;

            public TestRandom(IEnumerable<byte> data)
            {
                this._enumerator = data.GetEnumerator();
                this._enumerator.MoveNext();
            }

            public void GetBytes(byte[] data)
            {
                throw new NotImplementedException();
            }

            public void GetBytes(byte[] data, int offset, int count)
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
