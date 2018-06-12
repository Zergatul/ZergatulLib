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
    public class RdlTests
    {
        [TestMethod]
        public void EncodeTest()
        {
            byte[] encoded;

            // "dog"
            encoded = RdlEncoding.Encode(new RdlItem
            {
                String = Encoding.ASCII.GetBytes("dog")
            });
            Assert.IsTrue(BitHelper.BytesToHex(encoded) == "83646f67");

            // [ "cat", "dog" ]
            encoded = RdlEncoding.Encode(new RdlItem
            {
                Items = new[]
                {
                    new RdlItem
                    {
                        String = Encoding.ASCII.GetBytes("cat")
                    },
                    new RdlItem
                    {
                        String = Encoding.ASCII.GetBytes("dog")
                    }
                }
                
            });
            Assert.IsTrue(BitHelper.BytesToHex(encoded) == "c88363617483646f67");

            // ""
            encoded = RdlEncoding.Encode(new RdlItem
            {
                String = new byte[0]
            });
            Assert.IsTrue(BitHelper.BytesToHex(encoded) == "80");

            // []
            encoded = RdlEncoding.Encode(new RdlItem
            {
                Items = new RdlItem[0]
            });
            Assert.IsTrue(BitHelper.BytesToHex(encoded) == "c0");

            // "00..00"
            encoded = RdlEncoding.Encode(new RdlItem
            {
                String = new byte[1024]
            });
            Assert.IsTrue(BitHelper.BytesToHex(encoded).StartsWith("b90400"));

            // [ [], [[]], [ [], [[]] ] ]
            encoded = RdlEncoding.Encode(new RdlItem
            {
                Items = new[]
                {
                    new RdlItem
                    {
                        Items = new RdlItem[0]
                    },
                    new RdlItem
                    {
                        Items = new[]
                        {
                            new RdlItem
                            {
                                Items = new RdlItem[0]
                            }
                        }
                    },
                    new RdlItem
                    {
                        Items = new[]
                        {
                            new RdlItem
                            {
                                Items = new RdlItem[0]
                            },
                            new RdlItem
                            {
                                Items = new[]
                                {
                                    new RdlItem
                                    {
                                        Items = new RdlItem[0]
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
            RdlItem item;

            // "dog"
            item = RdlEncoding.Decode(BitHelper.HexToBytes("83646f67"));
            Assert.IsTrue(item.String != null);
            Assert.IsTrue(Encoding.ASCII.GetString(item.String) == "dog");

            // [ "cat", "dog" ]
            item = RdlEncoding.Decode(BitHelper.HexToBytes("c88363617483646f67"));
            Assert.IsTrue(item.Items?.Length == 2);
            Assert.IsTrue(Encoding.ASCII.GetString(item.Items[0].String) == "dog");
            Assert.IsTrue(Encoding.ASCII.GetString(item.Items[1].String) == "cat");

            // ""
            item = RdlEncoding.Decode(BitHelper.HexToBytes("80"));
            Assert.IsTrue(item.String?.Length == 0);

            // []
            item = RdlEncoding.Decode(BitHelper.HexToBytes("c0"));
            Assert.IsTrue(item.Items?.Length == 0);

            // "00..00"
            /*encoded = RdlEncoding.Encode(new RdlItem
            {
                String = new byte[1024]
            });
            Assert.IsTrue(BitHelper.BytesToHex(encoded).StartsWith("b90400"));*/
        }
    }
}