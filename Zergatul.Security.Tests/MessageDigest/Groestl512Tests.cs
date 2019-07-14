using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Groestl512Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.Groestl512;
        protected override string Algorithm => "Groestl";
        protected override int Size => 512;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "6d3ad29d279110eef3adbd66de2a0345a77baede1557f5d099fce0c03d6dc2ba8e6d4a6633dfbd66053c20faa87d1a11f39a7fbe4a6c2f009801370308fc4ad8");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "badc1f70ccd69e0cf3760c3f93884289da84ec13c70b3d12a53a7a8a4a513f99715d46288f55e1dbf926e6d084a0538e4eebfc91cf2b21452921ccde9131718d");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "518a55cc274fc887d8dcbd0bb24000395f6d3be62445d84cc9e85d419161a968268e490f7537e475e57d8c009b0957caa05882bc8c20ce22d50caa2106d0dcfd");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "70e1c68c60df3b655339d67dc291cc3f1dde4ef343f11b23fdd44957693815a75a8339c682fc28322513fd1f283c18e53cff2b264e06bf83a2f0ac8c1f6fbff6");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopqopqrpqrsqrstrstustuvtuvwuvwxvwxywxyzxyzayzabzabcabcdbcdecdefdefg"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "9f0867f941b5f3f2520e7b60b6e615eca82b61e2c5dd810f562450466f6a80fd72e6391f829dea656c4f84cdd7615e2098a99336d330b7226299e4139d3def75");
                md.Reset();
            };
        }

        [TestMethod]
        public void VergeBlock2374458Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(4100, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("ac0b0628787eaf4dc8703305aafd4d7c7bf1242f1f638572656f9536b07d8405").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("4c9bcaa3a955eef15086c2bfeb02a8987e942d71c2e459655a58ce2130f38de2").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1532638372, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1a21d227").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(428628904, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);
                var digest = md.Digest(header);
                md = provider.GetMessageDigest(MessageDigests.SHA256);
                digest = md.Digest(digest);
                var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                Assert.IsTrue(hash == "00000000000008bd05f110e25745edfe9f640776d65d29642b1c56745f543a72");
            }
        }

        [TestMethod]
        public void DigibyteBlock7054312Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(536871938, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("b4e9feaddced86e4d5c2bec2afec4da03cc3c7cdaeede690fec89ee033fbee98").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("73cedf213a8932e8f8175ef935baa9242cd75ffad68e3a580546897dfa43bc27").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1533049525, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1a1b2a4e").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(793270297, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);
                var digest = md.Digest(header);
                md = provider.GetMessageDigest(MessageDigests.SHA256);
                digest = md.Digest(digest);
                var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                Assert.IsTrue(hash == "0000000000000f6276a4ab87dce37338f0d4b0fe1d50cad5b0571f6c7feee2c7");
            }
        }

        [TestMethod]
        public void DigibyteBlock9064817Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(536871938, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("1c5b964a2632753813d6587cdc1ab51c13be4dbedd5b0445629efdfa739c3fe8").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("5d9561f72cd9dd403f013b59266d6d8429de12d90982bc697b4faab93e1d17b9").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1563047631, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1a03bd64").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(916867120, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);
                var digest = md.Digest(header);
                md = provider.GetMessageDigest(MessageDigests.SHA256);
                digest = md.Digest(digest);
                var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                Assert.IsTrue(hash == "00000000000002a635fe60cbf902a104a2c730a95eca4e12b9c66ca4b4e17d35");
            }
        }
    }
}