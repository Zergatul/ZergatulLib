using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Skein512Tests
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
                var md = provider.GetMessageDigest(MessageDigests.Skein512_512);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "bc5b4c50925519c290cc634277ae3d6257212395cba733bbad37a4af0fa06af41fca7903d06564fea7a2d3730dbdb80c1f85562dfcc070334ea4d1d9e72cba7a");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "94c2ae036dba8783d0b3f7d6cc111ff810702f5c77707999be7e1c9486ff238a7044de734293147359b4ac7e1d09cd247c351d69826b78dcddd951f0ef912713");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "658223cb3d69b5e76e3588ca63feffba0dc2ead38a95d0650564f2a39da8e83fbb42c9d6ad9e03fbfde8a25a880357d457dbd6f74cbcb5e728979577dbce5436");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "8f5dd9ec798152668e35129496b029a960c9a9b88662f7f9482f110b31f9f93893ecfb25c009baad9e46737197d5630379816a886aa05526d3a70df272d96e75");
                md.Reset();
            };
        }

        [TestMethod]
        public void NistTest()
        {
            List<byte> list = new List<byte>();
            foreach (var line in File.ReadAllLines("MessageDigest/NISTData.txt"))
                list.AddRange(BitHelper.HexToBytes(line));
            byte[] data = list.ToArray();

            string[] digests = File.ReadAllLines("MessageDigest/Skein512.txt");

            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.Skein512_512);

                int index = 0;
                for (int i = 0; i < 2048; i++)
                {
                    if (i % 8 == 0)
                    {
                        var digest = md.Digest(data, index, i / 8);
                        Assert.IsTrue(BitHelper.BytesToHex(digest) == digests[i]);
                        md.Reset();
                    }
                    index += (i + 7) / 8;
                }
            }
        }

        [TestMethod]
        public void LongTest()
        {
            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.Skein512_512);

                byte[] data = Encoding.ASCII.GetBytes("abcdefghbcdefghicdefghijdefghijkefghijklfghijklmghijklmnhijklmno");

                for (int i = 0; i < 16777216; i++)
                    md.Update(data);
                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "988e111f2963f48c616b4971e0e7ed4bf04ae7d4b5a632b3ab3d9eaff0ab4db2dc430abccc3b6de33b1b826baf6d9329fabd6110cfa6d0d8ac2a35610fad1827");
            }
        }

        [TestMethod]
        public void DigibyteBlock7054279Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(536872450, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("530b046e734f13660763ab68c373821cba031c2d14d991555ab3a9c17a06668f").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("427e6a43de1cf6a214abcfee613c3b0cfb77948a5449b90a790bfe29d35b3f67").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1533049087, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1a2d5671").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(3073187952, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.Skein512_512);
                var digest = md.Digest(header);
                md = provider.GetMessageDigest(MessageDigests.SHA256);
                digest = md.Digest(digest);
                var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                Assert.IsTrue(hash == "000000000000123851cd59ff8bf4a86db7f6c29703d35adc7dd11312562e92e9");
            }
        }
    }
}