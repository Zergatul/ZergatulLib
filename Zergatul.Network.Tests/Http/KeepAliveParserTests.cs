using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Network.Http;

namespace Zergatul.Network.Tests.Http
{
    [TestClass]
    public class KeepAliveParserTests
    {
        [TestMethod]
        public void SimpleTest()
        {
            var pairs = KeepAliveParser.Parse("timeout=5, max=1000").ToArray();
            Assert.IsTrue(pairs.Length == 2);
            Assert.IsTrue(pairs[0].Key == "timeout");
            Assert.IsTrue(pairs[0].Value == "5");
            Assert.IsTrue(pairs[1].Key == "max");
            Assert.IsTrue(pairs[1].Value == "1000");
        }

        [TestMethod]
        public void QuotedTest()
        {
            var pairs = KeepAliveParser.Parse("timeout=\"5\"").ToArray();
            Assert.IsTrue(pairs.Length == 1);
            Assert.IsTrue(pairs[0].Key == "timeout");
            Assert.IsTrue(pairs[0].Value == "5");
        }

        [TestMethod]
        public void QuotedWithWhitespaceTest()
        {
            var pairs = KeepAliveParser.Parse("timeout=\"5\",extension=\"hello world!\"").ToArray();
            Assert.IsTrue(pairs.Length == 2);
            Assert.IsTrue(pairs[0].Key == "timeout");
            Assert.IsTrue(pairs[0].Value == "5");
            Assert.IsTrue(pairs[1].Key == "extension");
            Assert.IsTrue(pairs[1].Value == "hello world!");
        }
    }
}