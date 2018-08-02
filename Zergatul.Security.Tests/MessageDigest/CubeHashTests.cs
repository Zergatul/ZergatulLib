using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class CubeHashTests
    {
        private static Provider[] _providers = new Provider[]
        {
            new DefaultProvider()
        };

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.CubeHash);
                byte[] digest;

                md.Init(new CubeHashParameters
                {
                    InitializationRounds = 80,
                    RoundsPerBlock = 8,
                    BytesPerBlock = 1,
                    FinalizationRounds = 80,
                    HashSizeBits = 512
                });
                digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "90bc3f2948f7374065a811f1e47a208a53b1a2f3be1c0072759ed49c9c6c7f28f26eb30d5b0658c563077d599da23f97df0c2c0ac6cce734ffe87b2e76ff7294");

                md.Init(new CubeHashParameters
                {
                    InitializationRounds = 10,
                    RoundsPerBlock = 1,
                    BytesPerBlock = 1,
                    FinalizationRounds = 10,
                    HashSizeBits = 512
                });
                digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "3f917707df9acd9b94244681b3812880e267d204f1fdf795d398799b584fa8f1f4a0b2dbd52fd1c4b6c5e020dc7a96192397dd1bce9b6d16484049f85bb71f2f");

                md.Init(new CubeHashParameters
                {
                    InitializationRounds = 80,
                    RoundsPerBlock = 8,
                    BytesPerBlock = 1,
                    FinalizationRounds = 80,
                    HashSizeBits = 256
                });
                digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "38d1e8a22d7baac6fd5262d83de89cacf784a02caa866335299987722aeabc59");

                md.Init(new CubeHashParameters
                {
                    InitializationRounds = 10,
                    RoundsPerBlock = 1,
                    BytesPerBlock = 1,
                    FinalizationRounds = 10,
                    HashSizeBits = 256
                });
                digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "80f72e07d04ddadb44a78823e0af2ea9f72ef3bf366fd773aa1fa33fc030e5cb");
            };
        }
    }
}