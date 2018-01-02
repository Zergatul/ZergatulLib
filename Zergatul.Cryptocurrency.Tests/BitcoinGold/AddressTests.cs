using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Zergatul.Cryptocurrency.Tests.BitcoinGold
{
    [TestClass]
    public class AddressTests
    {
        [TestMethod]
        public void ConvertTests()
        {
            var dict = new Dictionary<string, string>
            {
                ["1BQLNJtMDKmMZ4PyqVFfRuBNvoGhjigBKF"] = "GUFFnSDJCBNedXhGmRumrfXGqy4Ym8J3hC",
                ["1HQQNnJgLDt8FzXQavJBYpm2fag9khAPg4"] = "GaFKnuddK5VRLTphWrxHyb6vakTzonEmZQ",
                ["1111111111111111111114oLvT2"] = "GHqvR8KwyrcJ5UJHvwf7RmLtvAnr5uTHdV",
                ["11111111111111111111BZbvjr"] = "GHqvR8KwyrcJ5UJHvwf7RmLtvAnr8syHin"
            };

            foreach (var kv in dict)
            {
                var btc = new Cryptocurrency.Bitcoin.P2PKHAddress(kv.Key);
                Assert.IsTrue(new Cryptocurrency.BitcoinGold.P2PKHAddress(btc).Value == kv.Value);

                var bcg = new Cryptocurrency.BitcoinGold.P2PKHAddress(kv.Value);
                Assert.IsTrue(new Cryptocurrency.Bitcoin.P2PKHAddress(bcg).Value == kv.Key);
            }

            dict = new Dictionary<string, string>
            {
                ["34rHZwgXDnkKvrBAU2fJAhjySTTEFroekd"] = "AJw9Hu3i1366eegiuaf2txe8mY6D4RVNE4",
                ["3KdKstrSuBisEUfpx9mf9LD4kr4Ky1fx5z"] = "AZiBbrDdgS4dxHBPPhmPsb7E5vhJme4Qka",
            };

            foreach (var kv in dict)
            {
                var btc = new Cryptocurrency.Bitcoin.P2SHAddress(kv.Key);
                Assert.IsTrue(new Cryptocurrency.BitcoinGold.P2SHAddress(btc).Value == kv.Value);

                var bcg = new Cryptocurrency.BitcoinGold.P2SHAddress(kv.Value);
                Assert.IsTrue(new Cryptocurrency.Bitcoin.P2SHAddress(bcg).Value == kv.Key);
            }
        }
    }
}