using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Skein512x512Tests : NISTMDTestWithData
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.Skein512x512;
        protected override string Algorithm => "Skein512";
        protected override int Size => 512;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "bc5b4c50925519c290cc634277ae3d6257212395cba733bbad37a4af0fa06af41fca7903d06564fea7a2d3730dbdb80c1f85562dfcc070334ea4d1d9e72cba7a");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "8f5dd9ec798152668e35129496b029a960c9a9b88662f7f9482f110b31f9f93893ecfb25c009baad9e46737197d5630379816a886aa05526d3a70df272d96e75");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "94c2ae036dba8783d0b3f7d6cc111ff810702f5c77707999be7e1c9486ff238a7044de734293147359b4ac7e1d09cd247c351d69826b78dcddd951f0ef912713");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "658223cb3d69b5e76e3588ca63feffba0dc2ead38a95d0650564f2a39da8e83fbb42c9d6ad9e03fbfde8a25a880357d457dbd6f74cbcb5e728979577dbce5436");
                md.Reset();
            };
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

            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);
                var digest = md.Digest(header);
                md = provider.GetMessageDigest(MessageDigests.SHA256);
                digest = md.Digest(digest);
                var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                Assert.IsTrue(hash == "000000000000123851cd59ff8bf4a86db7f6c29703d35adc7dd11312562e92e9");
            }
        }

        [TestMethod]
        public void DigibyteBlock9064800Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(536872450, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("5a12bcce1a4524e9f60b7030907129f659e326c966ff3f13cd4c3cf531df0cd5").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("276433666bd9ccc2a382da6833cfeb003e305c5070236516381b73a786b7bc74").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1563047268, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1a09597c").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(1040147642, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);
                var digest = md.Digest(header);
                md = provider.GetMessageDigest(MessageDigests.SHA256);
                digest = md.Digest(digest);
                var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                Assert.IsTrue(hash == "000000000000056bfc9136f66d055067ef17d42311a75eaf2bcd98a9888f254e");
            }
        }

        [TestMethod]
        public void DigibyteBlock9064804Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(536872450, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("fd91bd09e8824e0c0e71006645f5027ef358957e084a6c8574b553a9e1018052").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("5613ef4d471f441fc2210fc891a36acf4584b31301c1db96483c9e631472e113").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1563047352, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1a06410d").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(2317312626, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);
                var digest = md.Digest(header);
                md = provider.GetMessageDigest(MessageDigests.SHA256);
                digest = md.Digest(digest);
                var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                Assert.IsTrue(hash == "000000000000055087704cb720275436515a95800e2d1824c6fb27e769df9346");
            }
        }
    }
}