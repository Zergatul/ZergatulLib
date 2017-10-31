using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Bitcoin;

namespace Zergatul.Cryptocurrency.Tests
{
    [TestClass]
    public class TransactionTests
    {
        [TestMethod]
        public void Parse1()
        {
            var tx = Transaction.FromHex("01000000013edff2a0a837f864695159d17c92cc588148845c877f0a98651779c05d65e7ce000000006a4730440220165fff9ba94070672ae4d86dda2d66c484f171ab97f6af5ff919c563c65ca17d022035cedddc003b96465db8d24a62e41ee4cfd983083532b6b7d028f44bea7cf1490121029513dea9bae3a0c6beb25bfe07242025ce46567793e7bbbe91537502d6c2d505ffffffff0210037d01000000001976a914a2971b8954610ab1cbf82d577bea9f927b7cca2688ac3063bb0b000000001976a91438c6174002d44124c2d7ef37c0dbf1fcd844c48b88ac00000000");
        }
    }
}