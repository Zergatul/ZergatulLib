using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class X17Tests
    {
        private SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        private string Name => MessageDigests.X17;

        [TestMethod]
        public void VergeBlock3305606Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(6148, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("db43e7acb37a0100a42ee0f825cce7d73bfa591d80e70fd77705b345a8c48754").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("69a7045ebd491e519af9035e19567dcb132de271d021547da1996ad6d389baad").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1563824381, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1b0b82d5").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(2371097921, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest(header);
                    var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                    Assert.IsTrue(hash == "0000000000067f51987b8859da1020e4311de6172217d168f96eb7d4c81c8660");
                }
        }

        [TestMethod]
        public void VergeBlock3305604Test()
        {
            byte[] header =
                // version
                BitHelper.GetBytes(6148, ByteOrder.LittleEndian)
                // prev block
                .Concat(BitHelper.HexToBytes("d7968c98002cf4a6f3b1ead9289cadb44f8c104698ab6c61198abcc57ef32d09").Reverse())
                // merkle
                .Concat(BitHelper.HexToBytes("ada131a9b6534108932791987a32a9e1cc09bfa86a547ca17d5c32ca55eb0163").Reverse())
                // time
                .Concat(BitHelper.GetBytes(1563824379, ByteOrder.LittleEndian))
                // bits
                .Concat(BitHelper.HexToBytes("1b0d2053").Reverse())
                // nonce
                .Concat(BitHelper.GetBytes(3430379524, ByteOrder.LittleEndian))
                .ToArray();

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest(header);
                    var hash = BitHelper.BytesToHex(digest.Reverse().ToArray());
                    Assert.IsTrue(hash == "000000000001ae739496e24b0b233f816906b5fcde9eb52c26aecfd7de3924a6");
                }
        }
    }
}