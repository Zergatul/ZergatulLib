using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class X15Tests
    {
        private SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        private string Name => MessageDigests.X15;

        [TestMethod]
        public void EverGreenCoinBlock50004Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(6, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("061b6499c36a6efde360802d93f39cb38e6e5fbae071af6114ddd95552e3d089").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("b7978f2bca1971f5d05aa63272f3aa3c14c4b2a94cd35f3dbd4a9da77a2af46f").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1452706176, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1c151800").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(3124180397, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest(header);
                    var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                    Assert.IsTrue(hash == "000000000004d1d4032e811c0fc67b01c014492f5f19153b14a0e0d464754561");
                }
        }

        [TestMethod]
        public void EverGreenCoinBlock431000Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(6, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("8e1fa2620e219bbd4dac1d8333bdbca191d9b942424523766b7d67aa6fc7cfdb").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("bb21f62b7243f8ef846ee0a41d06cb178f58157d0344e8f63b93c476d5568e52").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1464291478, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1c0ef91e").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(577220885, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest(header);
                    var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                    Assert.IsTrue(hash == "0000000000e9c020b6abbfb10d116f6bdf5cd11dd1457bdaa900beb58c1f3f2e");
                }
        }

        [TestMethod]
        public void KobocoinBlock21274Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(6, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("00000000016e8e2cfa2be304c1c8cfdd36f0d55f83bd43b44fc7866edf9af243").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("e23467df680befcb3da2c848e41b307f876d246b38b462b593d224ab681db016").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1424130744, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1c01d98e").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(568150, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest(header);
                    var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                    Assert.IsTrue(hash == "0000000001ae31e13d288ca5cafaa8b996cb521d5308591391bddd85cfca56f3");
                }
        }
    }
}