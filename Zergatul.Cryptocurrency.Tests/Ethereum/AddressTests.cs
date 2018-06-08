using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptocurrency.Ethereum;

namespace Zergatul.Cryptocurrency.Tests.Ethereum
{
    [TestClass]
    public class AddressTests
    {
        [TestMethod]
        public void FromPrivateKey()
        {
            var map = new Dictionary<string, string>
            {
                ["208065a247edbe5df4d86fbdc0171303f23a76961be9f6013850dd2bdc759bbb"] =
                    "0x0bed7abd61247635c1973eb38474a2516ed1d884",
                ["b205a1e03ddf50247d8483435cd91f9c732bad281ad420061ab4310c33166276"] =
                    "0xafdefc1937ae294c3bd55386a8b9775539d81653",
                ["981679905857953c9a21e1807aab1b897a395ea0c5c96b32794ccb999a3cd781"] =
                    "0x5fe3062b24033113fbf52b2b75882890d7d8ca54",
                ["9442b4b82c8011530f3a363cc87a4ea91efd53552faab2e63fd352db9367bb24"] =
                    "0x083c41ea13af6c2d5aaddf6e73142eb9a7b00183",
                ["0000000000000000000000000000000000000000000000000000000000000001"] =
                    "0x7e5f4552091a69125d5dfcb7b8c2659029395bdf",
                ["0000000000000000000000000000000000000000000000000000000000000002"] =
                    "0x2b5ad5c4795c026514f8317c7a215e218dccd6cf",
            };

            foreach (var kv in map)
            {
                var addr = new Address();
                addr.FromPrivateKey(kv.Key);
                Assert.IsTrue(addr.Value == kv.Value);
            }
        }
    }
}