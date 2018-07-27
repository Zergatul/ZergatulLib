using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class SHA512Tests
    {
        private static Provider[] _providers = new Provider[]
        {
            new DefaultProvider(),
            new OpenSslProvider()
        };

        [TestMethod]
        public void Test()
        {
            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.SHA512);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "07e547d9586f6a73f73fbac0435ed76951218fb7d0c8d788a309d785436bbb642e93a252a954f23912547d1e8a3b5ed6e1bfd7097821233fa0538f3db854fee6");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("Test vector from febooti.com"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "09fb898bc97319a243a63f6971747f8e102481fb8d5346c55cb44855adc2e0e98f304e552b0db1d4eeba8a5c8779f6a3010f0e1a2beb5b9547a13b6edca11e8a");
                md.Reset();
            };
        }
    }
}