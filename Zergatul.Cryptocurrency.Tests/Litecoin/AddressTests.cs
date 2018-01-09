using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptocurrency.Litecoin;

namespace Zergatul.Cryptocurrency.Tests.Litecoin
{
    [TestClass]
    public class AddressTests
    {
        [TestMethod]
        public void P2PKHTests()
        {
            var addr = new P2PKHAddress();

            addr.FromPublicKey(BitHelper.HexToBytes("0382291d5c141e58a630d40dc32da4f01bff4a084c1bd554c41d4da72a621c787f"));
            Assert.IsTrue(addr.Value == "LeY8TUXtU9Xqpsg2uDrML8a57taNVjqL8S");

            addr.FromPublicKeyHash(BitHelper.HexToBytes("3906f974082d45d37b21986ffcbf17b4d1fbdffc"));
            Assert.IsTrue(addr.Value == "LQRV8jpJ5foifqvtQFqDAQ4tDUT1WWkvJd");

            addr.FromPublicKeyHash(BitHelper.HexToBytes("15ab841e86143c7e13dfe37c4890effbadcbc2ff"));
            Assert.IsTrue(addr.Value == "LMCXvPUhDJ5ZpKSowV6tpCEvsWguMkPdgc");

            addr.FromPublicKeyHash(BitHelper.HexToBytes("cb3dd6a21568ef962362964d476551c06e6a595b"));
            Assert.IsTrue(addr.Value == "LdkbWmBwK5Qx3EYJFkVFBHMViqeJBDPYB1");
        }

        [TestMethod]
        public void P2SHTests()
        {
            var addr = new P2SHAddress();

            //addr.FromScript(BitHelper.HexToBytes("304402203f841cbb0ecca40ca0b5087229560e59ca8766c6704917f7b94bc1843ee671ee0220018e48934f9101fc8891f111560d4903dc859c9e92e1cf445827461e79ef1c8801"));
            //Assert.IsTrue(addr.Value == "399qzpAPm9QmJt9EZhXKNYbkXpjqSYHqNV");

            addr.FromScriptHash(BitHelper.HexToBytes("fa113f5b347466b88e38a099c75d8ecf06092f01"));
            Assert.IsTrue(addr.Value == "3QVFWsrghPqhzEaVmoiEhkBCyQtNM4Ycd3");

            addr.FromScriptHash(BitHelper.HexToBytes("07b29d23a42123cb029de76fc1919510bca5253e"));
            Assert.IsTrue(addr.Value == "32PidC8VzmDvRu2fohvV7MLjq2bZkixyeb");

            addr.FromScriptHash(BitHelper.HexToBytes("94d78d26a08350e2e3e014061dd780304334cb32"));
            Assert.IsTrue(addr.Value == "3FG29hvkNoh8aiyXowCB7U6HNVakZxMFRH");
        }
    }
}