using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Zergatul.Cryptocurrency.Bitcoin;

namespace Zergatul.Cryptocurrency.Tests
{
    [TestClass]
    public class P2PKHAddressTests
    {
        [TestMethod]
        public void FromPublicKeyTest()
        {
            var dict = new Dictionary<string, string>
            {
                ["02744857631b8c45ca22aff2d41eefbc61130e3c2567ce781b02f1f6cf8c900d12"] = "1LgXkhnzfPStj43SrYhqoLXXu6PL4abtr9",
                ["021a2f0c8e3f780f4edb2f8f095ceeae22230ddae13344f8599f02fc7f7eed90c0"] = "1edHTfdqTyFfHNcsM6h1nsrhmXjqC47s8",
                ["02460139eaaddcab3028179366d7248a9b0671038da3741a53dd394be037774b96"] = "19PNRPVYMVgppc9vAajy5eU62EMfMzvotK",
                ["02b800e5e0d26eecb8db3e85077b7ad564887b55becb03cc2669816b9d33b7bc2b"] = "1FePXqvftqtUG6UhwQj41qoG354g6WFk8b",
                ["0278d681632bc7d66b6ab44e582a5cfa3e122957249b89c230ba271c026b57c57b"] = "17yDXooqDjNqBmxP1CDMPfz4CZSPqXy2JZ",
                ["0393f8d31dae3fbc6a1aa812547f580e92fe4aad793afef479d090b4d45cf12fd3"] = "143iBpP94bPcp1CpbGtss1WDcvHbmsstg2",
                ["03c536c2e30bb38fafc9d1eac7ccfcca7d9540e95d94fe825c1d009f28a6214bc7"] = "1HKScWZtoGSjyRdyBUWEzB5HMGGEBZB5CW",
                ["02a54b23ab834067aa219dfe39d9cd951ba19ac71a8a30a2ee13447563673e01d6"] = "1JAQpbiMZ6TfmDZqpuer4jxMg7E7QFqxen",
                ["03d19668443fa6225ed3c6d591aad1097fe40fbec83b20a3ee53a202236d3d60be"] = "16ykpGhg2fa4R8ePRX6N7xDJg5ywvyzwNw",
                ["029ebce13c544952c406fb47b5c16a66baaf33af1204681dd339cb22287bffd23f"] = "1L1zm3zhsqHLV5t9WTdRbUgr8S9vQeESYC",

                ["04e70a02f5af48a1989bf630d92523c9d14c45c75f7d1b998e962bff6ff9995fc5bdb44f1793b37495d80324acba7c8f537caaf8432b8d47987313060cc82d8a93"] = "13A1W4jLPP75pzvn2qJ5KyyqG3qPSpb9jM",
                ["0450863AD64A87AE8A2FE83C1AF1A8403CB53F53E486D8511DAD8A04887E5B23522CD470243453A299FA9E77237716103ABC11A1DF38855ED6F2EE187E9C582BA6"] = "16UwLL9Risc3QfPqBUvKofHmBQ7wMtjvM"
            };

            foreach (var kv in dict)
            {
                var addr = new P2PKHAddress();
                addr.FromPublicKey(BitHelper.HexToBytes(kv.Key));
                Assert.IsTrue(addr.Value == kv.Value);
            }
        }

        [TestMethod]
        public void FromPublicKeyHashTest()
        {
            var dict = new Dictionary<string, string>
            {
                ["0000000000000000000000000000000000000000"] = "1111111111111111111114oLvT2",
                ["0000000000000000000000000000000000000001"] = "11111111111111111111BZbvjr",
            };

            foreach (var kv in dict)
            {
                var addr = new P2PKHAddress();
                addr.FromPublicKeyHash(BitHelper.HexToBytes(kv.Key));
                Assert.IsTrue(addr.Value == kv.Value);
            }
        }

        [TestMethod]
        public void WIFImportTest()
        {
            var addr = new P2PKHAddress();
            addr.FromWIF("5HueCGU8rMjxEXxiPuD5BDku4MkFqeZyd4dZ1jvhTVqvbTLvyTJ");
            Assert.IsTrue(addr.Value == "1GAehh7TsJAHuUAeKZcXf5CnwuGuGgyX2S");
            Assert.IsTrue(addr.ToCompressed().Value == "1LoVGDgRs9hTfTNJNuXKSpywcbdvwRXpmK");

            addr.FromWIF("5JRks4Vf268r9cuCKiod2iFz1VcSpawX5m6T3PKSA1v7cRqfZZD");
            Assert.IsTrue(addr.Value == "1Aop6KxiZLccPPPjqmfZHYgnmCKuhiVq57");
            Assert.IsTrue(addr.ToCompressed().Value == "1JMsC6fCtYWkTjPPdDrYX3we2aBrewuEM3");
            Assert.IsTrue(addr.ToWIF() == "5JRks4Vf268r9cuCKiod2iFz1VcSpawX5m6T3PKSA1v7cRqfZZD");

            addr.FromWIF("KyvGbxRUoofdw3TNydWn2Z78dBHSy2odn1d3wXWN2o3SAtccFNJL");
            Assert.IsTrue(addr.Value == "1JMsC6fCtYWkTjPPdDrYX3we2aBrewuEM3");
            Assert.IsTrue(addr.ToUncompressed().Value == "1Aop6KxiZLccPPPjqmfZHYgnmCKuhiVq57");
            Assert.IsTrue(addr.ToWIF() == "KyvGbxRUoofdw3TNydWn2Z78dBHSy2odn1d3wXWN2o3SAtccFNJL");

            addr.FromWIF("Kx6EWgKRJ2GZuXbrDquQPAE8MWZLLdsT4YYgQs4hdF7rRL4jGLHx");
            Assert.IsTrue(addr.Value == "13pRRXkGVC9mhUSiw6xkYkMi1EX91VvsBE");
            Assert.IsTrue(addr.ToWIF() == "Kx6EWgKRJ2GZuXbrDquQPAE8MWZLLdsT4YYgQs4hdF7rRL4jGLHx");

            addr.FromWIF("5J1jV2CspMgKnS4N7zJJz8Xcej3Lngcu89WP53jXW4CXEGF9M3A");
            Assert.IsTrue(addr.Value == "1Nt6XLmq8k8noafGGFdfwue74uJTFu9vQC");
            Assert.IsTrue(addr.ToWIF() == "5J1jV2CspMgKnS4N7zJJz8Xcej3Lngcu89WP53jXW4CXEGF9M3A");
        }
    }
}