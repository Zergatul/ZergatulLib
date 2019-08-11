using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.OpenSsl;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class BLAKE2bTests
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
                using (var md = provider.GetMessageDigest(MessageDigests.BLAKE2b))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "786a02f742015903c6c6fd852552d272912f4740e15847618a86e217f71f5419d25e1031afee585313896444934eb04b903a685b1448b755d56f701afe9be2ce");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "ba80a53f981c4d0d6a2797b69f12f6e94c212f14685ac4b74b12bb6fdbffa2d17d87c5392aab792dc252d5de4533cc9518d38aa8dbf1925ab92386edd4009923");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "a8add4bdddfd93e4877d2746e62817b116364a1fa7bc148d95090bc7333b3673f82401cf7aa2e4cb1ecd90296e3f14cb5413f8ed77be73045b13914cdcd6a918");
                    md.Reset();
                }
        }

        [TestMethod]
        public void VectorsTest()
        {
            var text = File.ReadAllText("MessageDigest/TestCases/BLAKE2/BLAKE2b.txt");
            var r = new Regex("in:\\s+(?<data>[0-9a-f]*)\\s+hash:\\s+(?<hash>[0-9a-f]+)", RegexOptions.Multiline);

            foreach (var provider in _providers)
                using (var md = provider.GetMessageDigest(MessageDigests.BLAKE2b))
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
            var text = File.ReadAllText("MessageDigest/TestCases/BLAKE2/BLAKE2bKey.txt");
            var r = new Regex("in:\\s+(?<data>[0-9a-f]*)\\s+key:\\s+(?<key>[0-9a-f]+)\\s+hash:\\s+(?<hash>[0-9a-f]+)", RegexOptions.Multiline);

            foreach (var provider in _providersWithParamSupport)
                using (var md = provider.GetMessageDigest(MessageDigests.BLAKE2b))
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
    }
}