using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Ripple;
using Zergatul.Cryptography.Asymmetric;

namespace Zergatul.Cryptocurrency.Tests.Ripple
{
    [TestClass]
    public class AccountTests
    {
        [TestMethod]
        public void Ripple_Addr_TestFromPublicKey()
        {
            var addr = Account.FromPublicKey(new EdDSAPublicKey(BitHelper.HexToBytes("9434799226374926EDA3B54B1B461B4ABF7237962EAE18528FEA67595397FA32")));
            Assert.IsTrue(addr.Value == "rDTXLQ7ZKZVKz33zJbHjgVShjsBnqMBhmN");
        }

        [TestMethod]
        public void Ripple_Addr_TestSpecialAddresses()
        {
            Assert.IsTrue(Account.ACCOUNT_ZERO.Value == "rrrrrrrrrrrrrrrrrrrrrhoLvTp");
            Assert.IsTrue(Account.ACCOUNT_ONE.Value == "rrrrrrrrrrrrrrrrrrrrBZbvji");
            Assert.IsTrue(Account.Genesis.Value == "rHb9CJAWyB4rj91VRWn96DkukG4bwdtyTh");
            Assert.IsTrue(Account.NameReservation.Value == "rrrrrrrrrrrrrrrrrNAMEtxvNvQ");
            Assert.IsTrue(Account.NaN.Value == "rrrrrrrrrrrrrrrrrrrn5RM1rHd");
        }

        [TestMethod]
        public void Ripple_Addr_FromSecretKey()
        {
            Assert.IsTrue(Account.FromSecretKey("snoPBrXtMeMyMHUVTgbuqAfg1SUTb").Value == "rHb9CJAWyB4rj91VRWn96DkukG4bwdtyTh");
            Assert.IsTrue(Account.FromSecretKey("shHM53KPZ87Gwdqarm1bAmPeXg8Tn").Value == "rhcfR9Cg98qCxHpCcPBmMonbDBXo84wyTn");
        }

        [TestMethod]
        public void Ripple_Addr_FromPassphrase()
        {
            Assert.IsTrue(Account.FromPassphrase("masterpassphrase").Value == "rHb9CJAWyB4rj91VRWn96DkukG4bwdtyTh");
        }
    }
}