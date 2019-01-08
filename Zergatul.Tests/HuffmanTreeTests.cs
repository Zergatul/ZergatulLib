using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Zergatul.IO;
using Zergatul.IO.Compression;

namespace Zergatul.Tests
{
    [TestClass]
    public class HuffmanTreeTests
    {
        [TestMethod]
        public void SimpleTest()
        {
            int symbol;
            var tree = new HuffmanTree(new[] { 3, 3, 3, 3, 3, 2, 4, 4 });

            /*
                Symbol Length   Code
                ------ ------   ----
                A       3        010
                B       3        011
                C       3        100
                D       3        101
                E       3        110
                F       2         00
                G       4       1110
                H       4       1111
            */

            {
                var ms = new MemoryStream(new byte[] { 0 });
                var reader = new BitReader(ms);

                symbol = tree.ReadNextSymbol(reader);
                Assert.IsTrue(symbol == 5);

                symbol = tree.ReadNextSymbol(reader);
                Assert.IsTrue(symbol == 5);

                symbol = tree.ReadNextSymbol(reader);
                Assert.IsTrue(symbol == 5);

                symbol = tree.ReadNextSymbol(reader);
                Assert.IsTrue(symbol == 5);
            }

            {
                var ms = new MemoryStream(new byte[] { Convert.ToByte("0111" + "1111", 2) });
                var reader = new BitReader(ms);

                symbol = tree.ReadNextSymbol(reader);
                Assert.IsTrue(symbol == 7);

                symbol = tree.ReadNextSymbol(reader);
                Assert.IsTrue(symbol == 6);
            }

            {
                var ms = new MemoryStream(new byte[] { Convert.ToByte("101" + "001" + "00", 2) });
                var reader = new BitReader(ms);

                symbol = tree.ReadNextSymbol(reader);
                Assert.IsTrue(symbol == 5);

                symbol = tree.ReadNextSymbol(reader);
                Assert.IsTrue(symbol == 2);

                symbol = tree.ReadNextSymbol(reader);
                Assert.IsTrue(symbol == 3);
            }
        }
    }
}