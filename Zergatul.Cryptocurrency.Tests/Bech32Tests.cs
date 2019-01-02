using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Cryptocurrency.Tests
{
    [TestClass]
    public class Bech32Tests
    {
        [TestMethod]
        public void EncodeTest()
        {
            string text = Bech32Encoding.Encode("bc", 0, BitHelper.HexToBytes("751e76e8199196d454941c45d1b3a323f1433bd6"));
            Assert.IsTrue(text == "bc1qw508d6qejxtdg4y5r3zarvary0c5xw7kv8f3t4");
        }

        [TestMethod]
        public void DecodeTest()
        {
            Assert.IsTrue(Bech32Encoding.TryDecode("bc", 0, "bc1qw508d6qejxtdg4y5r3zarvary0c5xw7kv8f3t4", out byte[] data));
            Assert.IsTrue(BitHelper.BytesToHex(data) == "751e76e8199196d454941c45d1b3a323f1433bd6");
        }
    }
}