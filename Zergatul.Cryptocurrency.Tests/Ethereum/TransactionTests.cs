using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Ethereum;

namespace Zergatul.Cryptocurrency.Tests.Ethereum
{
    [TestClass]
    public class TransactionTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            // https://github.com/ethereum/wiki/wiki/%5BEnglish%5D-RLP
            // https://ropsten.etherscan.io/getRawTx?tx=0x8b69a0ca303305a92d8d028704d65e4942b7ccc9a99917c8c9e940c9d57a9662
            // https://medium.com/@codetractio/inside-an-ethereum-transaction-fa94ffca912f
            var addr = new Address();
            addr.FromPrivateKey("c0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0de");
            Assert.IsTrue(addr.Value == "0x53ae893e4b22d707943299a8d0c844df0e3d5557");
        }
    }
}