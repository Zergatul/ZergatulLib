using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.OpenSsl;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class BLAKE2sTests
    {
        private static SecurityProvider[] _providers = new SecurityProvider[]
        {
            new ZergatulProvider(),
            new OpenSslProvider()
        };

        private static SecurityProvider[] _providersWithParamSupport = new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in _providers)
                using (var md = provider.GetMessageDigest(MessageDigests.BLAKE2s))
                {
                    var digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "508c5e8c327c14e2e1a72ba34eeb452f37458b209ed63a294d999b4c86675982");
                    md.Reset();

                    digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "69217a3079908094e11121d042354a7c1f55b6482ca1a51e1b250dfd1ed0eef9");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "606beeec743ccbeff6cbcdf5d5302aa855c256c29b88c8ed331ea1a6bf3c8812");
                    md.Reset();
                }
        }

        [TestMethod]
        public void DifferentHashSizeTest()
        {
            foreach (var provider in _providersWithParamSupport)
                using (var md = provider.GetMessageDigest(MessageDigests.BLAKE2s))
                {
                    md.Init(new BLAKE2Parameters
                    {
                        DigestLength = 16
                    });

                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "64550d6ffe2c0a01a14aba1eade0200c");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "96fd07258925748a0d2fb1c8a1167a73");
                    md.Reset();
                }
        }

        [TestMethod]
        public void VectorsTest()
        {
            var text = File.ReadAllText("MessageDigest/BLAKE2s.txt");
            var r = new Regex("in:\\s+(?<data>[0-9a-f]*)\\s+hash:\\s+(?<hash>[0-9a-f]+)", RegexOptions.Multiline);

            foreach (var provider in _providers)
                using (var md = provider.GetMessageDigest(MessageDigests.BLAKE2s))
                {
                    var matches = r.Matches(text);
                    Assert.IsTrue(matches.Count == 99);
                    foreach (Match m in matches)
                    {
                        var digest = md.Digest(BitHelper.HexToBytes(m.Groups["data"].Value));
                        Assert.IsTrue(BitHelper.BytesToHex(digest) == m.Groups["hash"].Value);
                        md.Reset();
                    }
                }
        }

        [TestMethod]
        public void KeyVectorsTest()
        {
            var text = File.ReadAllText("MessageDigest/BLAKE2sKey.txt");
            var r = new Regex("in:\\s+(?<data>[0-9a-f]*)\\s+key:\\s+(?<key>[0-9a-f]+)\\s+hash:\\s+(?<hash>[0-9a-f]+)", RegexOptions.Multiline);

            foreach (var provider in _providersWithParamSupport)
                using (var md = provider.GetMessageDigest(MessageDigests.BLAKE2s))
                {
                    var matches = r.Matches(text);
                    Assert.IsTrue(matches.Count == 256);
                    foreach (Match m in matches)
                    {
                        md.Init(new BLAKE2Parameters
                        {
                            Key = BitHelper.HexToBytes(m.Groups["key"].Value)
                        });
                        var digest = md.Digest(BitHelper.HexToBytes(m.Groups["data"].Value));
                        Assert.IsTrue(BitHelper.BytesToHex(digest) == m.Groups["hash"].Value);
                    }
                }
        }

        [TestMethod]
        public void VergeBlock2373791Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(8196, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("ccab8766651abb8ecd690f1008827157ad3e36977b247ac2a4e765a2a37ab1cf").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("9379635ff782f2850db554b235e3eadd896f299c848a30fb02f7f4a3d32cd4c2").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1532615300, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1a024a8a").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(835507996, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in _providers)
                using (var md = provider.GetMessageDigest(MessageDigests.BLAKE2s))
                {
                    var digest = md.Digest(header);
                    var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                    Assert.IsTrue(hash == "00000000000000702dc521d00f507e737b338a483251d3151ed3d64556ac18e3");
                }
        }
    }
}