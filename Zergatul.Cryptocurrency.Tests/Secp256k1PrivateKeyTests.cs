using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Cryptocurrency.Tests
{
    [TestClass]
    public class Secp256k1PrivateKeyTests
    {
        [TestMethod]
        public void Secp256k1_ToWIFTest()
        {
            var key = Secp256k1PrivateKey.FromHex("E48378200D4DE5265F957EE560657F44AE51F0FCE27A54D2E9718831F2C2FFA8");
            Assert.IsTrue(key.ToWIF(0x80) == "5KYverbS5ynp8VBhCpjJiyZZFXePBKYLe7JngrUoaZbTymK7ErF");
        }
    }
}