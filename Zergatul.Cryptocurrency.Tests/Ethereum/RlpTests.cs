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
    public class RlpTests
    {
        [TestMethod]
        public void EncodeTest()
        {
            byte[] encoded;

            // "dog"
            encoded = Rlp.Encode(new RlpItem
            {
                String = Encoding.ASCII.GetBytes("dog")
            });
            Assert.IsTrue(BitHelper.BytesToHex(encoded) == "83646f67");

            // [ "cat", "dog" ]
            encoded = Rlp.Encode(new RlpItem
            {
                Items = new[]
                {
                    new RlpItem
                    {
                        String = Encoding.ASCII.GetBytes("cat")
                    },
                    new RlpItem
                    {
                        String = Encoding.ASCII.GetBytes("dog")
                    }
                }
                
            });
            Assert.IsTrue(BitHelper.BytesToHex(encoded) == "c88363617483646f67");

            // ""
            encoded = Rlp.Encode(new RlpItem
            {
                String = new byte[0]
            });
            Assert.IsTrue(BitHelper.BytesToHex(encoded) == "80");

            // []
            encoded = Rlp.Encode(new RlpItem
            {
                Items = new RlpItem[0]
            });
            Assert.IsTrue(BitHelper.BytesToHex(encoded) == "c0");

            // "00..00"
            encoded = Rlp.Encode(new RlpItem
            {
                String = new byte[1024]
            });
            Assert.IsTrue(BitHelper.BytesToHex(encoded).StartsWith("b90400"));

            // [ [], [[]], [ [], [[]] ] ]
            encoded = Rlp.Encode(new RlpItem
            {
                Items = new[]
                {
                    new RlpItem
                    {
                        Items = new RlpItem[0]
                    },
                    new RlpItem
                    {
                        Items = new[]
                        {
                            new RlpItem
                            {
                                Items = new RlpItem[0]
                            }
                        }
                    },
                    new RlpItem
                    {
                        Items = new[]
                        {
                            new RlpItem
                            {
                                Items = new RlpItem[0]
                            },
                            new RlpItem
                            {
                                Items = new[]
                                {
                                    new RlpItem
                                    {
                                        Items = new RlpItem[0]
                                    }
                                }
                            },
                        }
                    }
                }
            });
            Assert.IsTrue(BitHelper.BytesToHex(encoded) == "c7c0c1c0c3c0c1c0");
        }

        [TestMethod]
        public void DecodeTest()
        {
            RlpItem item;

            // "dog"
            item = Rlp.Decode(BitHelper.HexToBytes("83646f67"));
            Assert.IsTrue(item.String != null);
            Assert.IsTrue(Encoding.ASCII.GetString(item.String) == "dog");

            // [ "cat", "dog" ]
            item = Rlp.Decode(BitHelper.HexToBytes("c88363617483646f67"));
            Assert.IsTrue(item.Items?.Length == 2);
            Assert.IsTrue(Encoding.ASCII.GetString(item.Items[0].String) == "cat");
            Assert.IsTrue(Encoding.ASCII.GetString(item.Items[1].String) == "dog");

            // ""
            item = Rlp.Decode(BitHelper.HexToBytes("80"));
            Assert.IsTrue(item.String?.Length == 0);

            // []
            item = Rlp.Decode(BitHelper.HexToBytes("c0"));
            Assert.IsTrue(item.Items?.Length == 0);

            // "00..00"
            item = Rlp.Decode(ByteArray.Concat(BitHelper.HexToBytes("b90400"), new byte[1024]));
            Assert.IsTrue(item.String?.Length == 1024);
            Assert.IsTrue(ByteArray.IsZero(item.String));

            // [ [], [[]], [ [], [[]] ] ]
            item = Rlp.Decode(BitHelper.HexToBytes("c7c0c1c0c3c0c1c0"));
            Assert.IsTrue(item.Items[0].Items.Length == 0);
            Assert.IsTrue(item.Items[1].Items[0].Items.Length == 0);
            Assert.IsTrue(item.Items[2].Items[1].Items[0].Items.Length == 0);
        }
    }
}