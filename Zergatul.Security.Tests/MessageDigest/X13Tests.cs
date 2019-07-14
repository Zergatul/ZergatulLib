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
    public class X13Tests
    {
        private SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        private string Name => MessageDigests.X13;

        [TestMethod]
        public void DeepOnionBlock1346541Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(6, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("000000000002bb8407ee4a401cb7441293af505ac6b210420bc2e3d8f4c61dcc").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("106e2e6169cabecee441bdf7f32e4154fa62f6bda18a45158eb7a27045c2555b").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1563124511, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1b0355db").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(1123491313, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest(header);
                    var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                    Assert.IsTrue(hash == "000000000000e512369df06fe667acc2894e7f1d6a87363c69d949434bc5931a");
                }
        }

        [TestMethod]
        public void DeepOnionBlock1346561Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(6, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("0000000000003fd45bdeff8977edc21fab4cf2bbea3d3af7f2c1d02dc763818e").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("958b6a20d8f4dbcb6b2338616f5172d44f6542c9bf392adf610ba0fa7c459d24").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1563125512, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1b035fb7").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(1582646581, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest(header);
                    var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                    Assert.IsTrue(hash == "000000000001eab9cd0668c3d8d2c4f32c993228f57bb2dc69a53420f5a4ec7d");
                }
        }

        [TestMethod]
        public void DeepOnionBlock1346632Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(6, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("0000000000039761e30b4d20d1677c8d5dfea56f66fff6cd1d31aee017d8674d").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("883c2b9b9de4dc8d0dfccda755603ca7aaa075a4a9df08c6d20c2df253fa77e5").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1563128725, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1b03ba60").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(710434234, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest(header);
                    var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                    Assert.IsTrue(hash == "00000000000023319fac1d5e32ce09ebbac1974dccbb83372e91ecfd811efa83");
                }
        }
    }
}