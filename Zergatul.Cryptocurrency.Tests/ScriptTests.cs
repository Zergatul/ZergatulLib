using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Cryptocurrency.Tests
{
    [TestClass]
    public class ScriptTests
    {
        [TestMethod]
        public void Script_P2PKHTest()
        {
            var script = Script.FromHex("76A91489ABCDEFABBAABBAABBAABBAABBAABBAABBAABBA88AC");
            Assert.IsTrue(script.IsPayToPublicKeyHashRedeem);
        }
    }
}