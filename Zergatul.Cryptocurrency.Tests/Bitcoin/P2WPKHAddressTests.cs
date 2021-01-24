using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Bitcoin;
using Zergatul.Security;
using Zergatul.Security.OpenSsl;

namespace Zergatul.Cryptocurrency.Tests.Bitcoin
{
    [TestClass]
    public class P2WPKHAddressTests
    {
        [TestMethod]
        public void FromPublicKeyTest()
        {
            var addr = new P2WPKHAddress();
            addr.FromPublicKey(BitHelper.HexToBytes("0279BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798"));
            Assert.IsTrue(addr.Value == "bc1qw508d6qejxtdg4y5r3zarvary0c5xw7kv8f3t4");
        }

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            SecurityProvider.UnregisterAll();
            SecurityProvider.Register(new OpenSslProvider());
        }
    }
}