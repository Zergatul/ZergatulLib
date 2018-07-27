using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Groestl512Tests
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
                var md = provider.GetMessageDigest(MessageDigests.Groestl512);

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

            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.Groestl512);
                var digest = md.Digest(header);
                md = provider.GetMessageDigest(MessageDigests.SHA256);
                digest = md.Digest(digest);
                var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                Assert.IsTrue(hash == "00000000000008bd05f110e25745edfe9f640776d65d29642b1c56745f543a72");
            }
        }
    }
}