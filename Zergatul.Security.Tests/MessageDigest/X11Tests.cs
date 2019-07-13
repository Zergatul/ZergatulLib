using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class X11Tests
    {
        private SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        private string Name => MessageDigests.X11;

        [TestMethod]
        public void DashBlock1102892Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(0x20000000, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("00000000000000076b9eeb7dde49d00745963f5b9b1100a84d4036c703191845").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("bf7024f61103efad9232e788252d848e7366aac447708febba4db00fda81d41f").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1563017785, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("192ad777").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(4130843361, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest(header);
                    var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                    Assert.IsTrue(hash == "00000000000000036627f0f3baf43714ea32a4d4908d5f5274cf8ad6476e4ac1");
                }
        }

        [TestMethod]
        public void DashBlock1102912Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(0x20000000, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("000000000000001960a492ae714baf13b99d719505953d2349717d8477d6f26f").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("4828a16ec2741fa9276d446ff489cd413b91f50188cb9284f8f7ef2fd9e7dc0e").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1563019986, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("191a47be").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(4054666580, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest(header);
                    var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                    Assert.IsTrue(hash == "0000000000000008668d6df87a146e4b0bf5bf0477c9d443700b6d83ac6c12e3");
                }
        }

        [TestMethod]
        public void DashBlock1102916Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(0x20000000, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("00000000000000029b3ed7931a0d3f22b2a998902a52a66a477f26d122b33d1a").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("c5dca700dbc38825771b5b0078a56210d20abc9fd6c6510d50f7e3adc016338f").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1563020914, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("191ba675").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(1614415576, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest(header);
                    var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                    Assert.IsTrue(hash == "0000000000000009c424d99ae2430e44748e5d4e97263a270a12805ecd3cee4f");
                }
        }
    }
}