using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.IO.Compression;

namespace Zergatul.Tests
{
    [TestClass]
    public class DeflateStreamTests
    {
        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void EmptyStreamTest()
        {
            BinTest("", "");
        }

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public async Task EmptyStreamTestAsync()
        {
            await BinTestAsync("", "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public void ReservedBlockTypeTest()
        {
            BinTest("1 11", "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public async Task ReservedBlockTypeAsyncTest()
        {
            await BinTestAsync("1 11", "");
        }

        [TestMethod]
        public void UncompressedBlockEmptyTest()
        {
            BinTest("1 00 00000   0000000000000000 1111111111111111", "");
        }

        [TestMethod]
        public async Task UncompressedBlockEmptyAsyncTest()
        {
            await BinTestAsync("1 00 00000   0000000000000000 1111111111111111", "");
        }

        [TestMethod]
        public void UncompressedBlock3BytesTest()
        {
            BinTest("1 00 00000   1100000000000000 0011111111111111   10100000 00101000 11000100", "05 14 23");
        }

        [TestMethod]
        public async Task UncompressedBlock3BytesAsyncTest()
        {
            await BinTestAsync("1 00 00000   1100000000000000 0011111111111111   10100000 00101000 11000100", "05 14 23");
        }

        [TestMethod]
        public void Uncompressed2BlocksTest()
        {
            BinTest(
                "0 00 00000   0100000000000000 1011111111111111   10100000 00101000   1 00 00000   1000000000000000 0111111111111111   11000100",
                "05 14 23");
        }

        [TestMethod]
        public async Task Uncompressed2BlocksAsyncTest()
        {
            await BinTestAsync(
                "0 00 00000   0100000000000000 1011111111111111   10100000 00101000   1 00 00000   1000000000000000 0111111111111111   11000100",
                "05 14 23");
        }

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void UncompressedBlockPartialLengthTest()
        {
            BinTest("1 00 00000 0000000000", "");
        }

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public async Task UncompressedBlockPartialLengthAsyncTest()
        {
            await BinTestAsync("1 00 00000 0000000000", "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public void UncompressedBlockMismatchLengthTest()
        {
            BinTest("1 00 00000 0010000000010000 1111100100110101", "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public async Task UncompressedBlockMismatchLengthAsyncTest()
        {
            await BinTestAsync("1 00 00000 0010000000010000 1111100100110101", "");
        }

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void UncompressedBlockEndOfStreamTest()
        {
            BinTest("1 00 11111 0110000000000000 1001111111111111 10101010 01110111", "");
        }

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public async Task UncompressedBlockEndOfStreamAsyncTest()
        {
            await BinTestAsync("1 00 11111 0110000000000000 1001111111111111 10101010 01110111", "");
        }

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void UncompressedBlockNoFinalTest()
        {
            BinTest("0 00 00000   0000000000000000 1111111111111111", "");
        }

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public async Task UncompressedBlockNoFinalAsyncTest()
        {
            await BinTestAsync("0 00 00000   0000000000000000 1111111111111111", "");
        }

        [TestMethod]
        public void FixedBlockTest()
        {
            BinTest("0 10 110010000 110100001 111111111 0000000  1 00 0100000000000000 1011111111111111 11010101 10110011", "90 A1 FF AB CD");
        }

        [TestMethod]
        public async Task FixedBlockAsyncTest()
        {
            await BinTestAsync("0 10 110010000 110100001 111111111 0000000  1 00 0100000000000000 1011111111111111 11010101 10110011", "90 A1 FF AB CD");
        }

        [TestMethod]
        public void EmptyFixedBlockTest()
        {
            BinTest("1 10 0000000", "");
        }

        [TestMethod]
        public async Task EmptyFixedBlockAsyncTest()
        {
            await BinTestAsync("1 10 0000000", "");
        }

        [TestMethod]
        public void FixedBlockLiteralsTest()
        {
            BinTest("1 10 00110000 10110000 10111111 110010000 111000000 111111111 0000000", "00 80 8F 90 C0 FF");
        }

        [TestMethod]
        public async Task FixedBlockLiteralsAsyncTest()
        {
            await BinTestAsync("1 10 00110000 10110000 10111111 110010000 111000000 111111111 0000000", "00 80 8F 90 C0 FF");
        }

        [TestMethod]
        public void FixedBlockNoOverlapTest()
        {
            BinTest("1 10 00110000 00110001 00110010 0000001 00010 0000000", "00 01 02 00 01 02");
        }

        [TestMethod]
        public async Task FixedBlockNoOverlapAsyncTest()
        {
            await BinTestAsync("1 10 00110000 00110001 00110010 0000001 00010 0000000", "00 01 02 00 01 02");
        }

        [TestMethod]
        public void FixedBlockOverlap1Test()
        {
            BinTest("1 10 00110001 0000010 00000 0000000", "01 01 01 01 01");
        }

        [TestMethod]
        public async Task FixedBlockOverlap1AsyncTest()
        {
            await BinTestAsync("1 10 00110001 0000010 00000 0000000", "01 01 01 01 01");
        }

        [TestMethod]
        public void FixedBlockOverlap2Test()
        {
            BinTest("1 10 10111110 10111111 0000011 00001 0000000", "8E 8F 8E 8F 8E 8F 8E");
        }

        [TestMethod]
        public async Task FixedBlockOverlap2AsyncTest()
        {
            await BinTestAsync("1 10 10111110 10111111 0000011 00001 0000000", "8E 8F 8E 8F 8E 8F 8E");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public void FixedBlockInvalidLengthCode286Test()
        {
            BinTest("1 10 11000110", "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public async Task FixedBlockInvalidLengthCode286AsyncTest()
        {
            await BinTestAsync("1 10 11000110", "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public void FixedBlockInvalidLengthCode287Test()
        {
            BinTest("1 10 11000111", "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public async Task FixedBlockInvalidLengthCode287AsyncTest()
        {
            await BinTestAsync("1 10 11000111", "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public void FixedBlockInvalidDistanceCode30Test()
        {
            BinTest("1 10 00110000 0000001 11110", "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public async Task FixedBlockInvalidDistanceCode30AsyncTest()
        {
            await BinTestAsync("1 10 00110000 0000001 11110", "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public void FixedBlockInvalidDistanceCode31Test()
        {
            BinTest("1 10 00110000 0000001 11111", "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public async Task FixedBlockInvalidDistanceCode31AsyncTest()
        {
            await BinTestAsync("1 10 00110000 0000001 11111", "");
        }

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void FixedBlockEndOnHuffmanSymbolTest()
        {
            BinTest("1 10 00000", "");
        }

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public async Task FixedBlockEndOnHuffmanSymbolAsyncTest()
        {
            await BinTestAsync("1 10 00000", "");
        }

        [TestMethod]
        public void DynamicBlockEmptyTest()
        {
            // numCodeLen=19
            //   codeLenCodeLen = 0:0, 1:1, 2:0, ..., 15:0, 16:0, 17:0, 18:1
            // numLitLen=257, numDist=2
            //   litLenCodeLen = 0:1, 1:0, ..., 255:0, 256:1
            //   distCodeLen = 0:1, 1:1
            string blockHeader = "1 01";
            string codeCounts = "00000 10000 1111";
            string codeLenCodeLens = "000 000 100 000 000 000 000 000 000 000 000 000 000 000 000 000 000 100 000";
            string codeLens = "0 11111111 10101011 0 0 0";
            string data = "1";
            BinTest(blockHeader + codeCounts + codeLenCodeLens + codeLens + data, "");
        }

        [TestMethod]
        public async Task DynamicBlockEmptyAsyncTest()
        {
            string blockHeader = "1 01";
            string codeCounts = "00000 10000 1111";
            string codeLenCodeLens = "000 000 100 000 000 000 000 000 000 000 000 000 000 000 000 000 000 100 000";
            string codeLens = "0 11111111 10101011 0 0 0";
            string data = "1";
            await BinTestAsync(blockHeader + codeCounts + codeLenCodeLens + codeLens + data, "");
        }

        [TestMethod]
        public void DynamicBlockNoDistanceCodeTest()
        {
            // numCodeLen=18
            //   codeLenCodeLen = 0:2, 1:2, 2:0, ..., 15:0, 16:0, 17:0, 18:1
            // numLitLen=257, numDist=1
            //   litLenCodeLen = 0:0, ..., 254:0, 255:1, 256:1
            //   distCodeLen = 0:0
            string blockHeader = "1 01";
            string codeCounts = "00000 00000 0111";
            string codeLenCodeLens = "000 000 100 010 000 000 000 000 000 000 000 000 000 000 000 000 000 010";
            string codeLens = "01111111 00101011 11 11 10";
            string data = "1";
            BinTest(blockHeader + codeCounts + codeLenCodeLens + codeLens + data, "");
        }

        [TestMethod]
        public async Task DynamicBlockNoDistanceCodeAsyncTest()
        {
            string blockHeader = "1 01";
            string codeCounts = "00000 00000 0111";
            string codeLenCodeLens = "000 000 100 010 000 000 000 000 000 000 000 000 000 000 000 000 000 010";
            string codeLens = "01111111 00101011 11 11 10";
            string data = "1";
            await BinTestAsync(blockHeader + codeCounts + codeLenCodeLens + codeLens + data, "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public void DynamicBlockRepeatOnStartTest()
        {
            // numLitLen=257, numDist=1, numCodeLen=18
            // codeLenCodeLen = 0:0, 1:1, 2:0, ..., 15:0, 16:1, 17:0, 18:0
            // Literal/length/distance code lengths: #16+00
            string blockHeader = "1 01";
            string codeCounts = "00000 00000 0111";
            string codeLenCodeLens = "100 000 000 000 000 000 000 000 000 000 000 000 000 000 000 000 000 100";
            string codeLens = "1";
            BinTest(blockHeader + codeCounts + codeLenCodeLens + codeLens, "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public async Task DynamicBlockRepeatOnStartAsyncTest()
        {
            string blockHeader = "1 01";
            string codeCounts = "00000 00000 0111";
            string codeLenCodeLens = "100 000 000 000 000 000 000 000 000 000 000 000 000 000 000 000 000 100";
            string codeLens = "1";
            await BinTestAsync(blockHeader + codeCounts + codeLenCodeLens + codeLens, "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public void DynamicBlockTooManyCodeLengthsTest()
        {
            // numLitLen=257, numDist=1, numCodeLen=18
            // codeLenCodeLen = 0:0, 1:1, 2:0, ..., 15:0, 16:0, 17:0, 18:1
            // Literal/length/distance code lengths: 1 1 #18+1111111 #18+1101100
            string blockHeader = "1 01";
            string codeCounts = "00000 00000 0111";
            string codeLenCodeLens = "000 000 100 000 000 000 000 000 000 000 000 000 000 000 000 000 000 100";
            string codeLens = "0 0 11111111 10011011";
            BinTest(blockHeader + codeCounts + codeLenCodeLens + codeLens, "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public async Task DynamicBlockTooManyCodeLengthsAsyncTest()
        {
            string blockHeader = "1 01";
            string codeCounts = "00000 00000 0111";
            string codeLenCodeLens = "000 000 100 000 000 000 000 000 000 000 000 000 000 000 000 000 000 100";
            string codeLens = "0 0 11111111 10011011";
            await BinTestAsync(blockHeader + codeCounts + codeLenCodeLens + codeLens, "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public void DynamicBlockOverfullCode0Test()
        {
            // numLitLen=257, numDist=1, numCodeLen=4
            // codeLenCodeLen = 0:1, 1:1, 2:1, 3:0
            string blockHeader = "1 01";
            string codeCounts = "00000 00000 0000";
            string codeLenCodeLens = "100 100 100 000";
            string padding = "0000000000000000000";
            BinTest(blockHeader + codeCounts + codeLenCodeLens + padding, "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public async Task DynamicBlockOverfullCode0AsyncTest()
        {
            string blockHeader = "1 01";
            string codeCounts = "00000 00000 0000";
            string codeLenCodeLens = "100 100 100 000";
            string padding = "0000000000000000000";
            await BinTestAsync(blockHeader + codeCounts + codeLenCodeLens + padding, "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public void DynamicBlockOverfullCodeTest()
        {
            // numLitLen=257, numDist=1, numCodeLen=4
            // codeLenCodeLen = 0:1, 1:1, 2:1, 3:1
            string blockHeader = "1 01";
            string codeCounts = "00000 00000 0000";
            string codeLenCodeLens = "100 100 100 100";
            string padding = "0000000000000000000";
            BinTest(blockHeader + codeCounts + codeLenCodeLens + padding, "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public async Task DynamicBlockOverfullCodeAsyncTest()
        {
            string blockHeader = "1 01";
            string codeCounts = "00000 00000 0000";
            string codeLenCodeLens = "100 100 100 100";
            string padding = "0000000000000000000";
            await BinTestAsync(blockHeader + codeCounts + codeLenCodeLens + padding, "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public void DynamicBlockEmptyCodeTest()
        {
            // numLitLen=257, numDist=1, numCodeLen=4
            // codeLenCodeLen = 0:0, 1:0, 2:0, 3:0
            string blockHeader = "1 01";
            string codeCounts = "00000 00000 0000";
            string codeLenCodeLens = "000 000 000 000";
            string padding = "0000000000000000000";
            BinTest(blockHeader + codeCounts + codeLenCodeLens + padding, "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public async Task DynamicBlockEmptyCodeAsyncTest()
        {
            string blockHeader = "1 01";
            string codeCounts = "00000 00000 0000";
            string codeLenCodeLens = "000 000 000 000";
            string padding = "0000000000000000000";
            await BinTestAsync(blockHeader + codeCounts + codeLenCodeLens + padding, "");
        }

        [TestMethod]
        public void DynamicBlockOneDistanceCodeTest()
        {
            // numLitLen=258, numDist=1, numCodeLen=18
            // codeLenCodeLen = 0:2, 1:2, 2:2, ..., 15:0, 16:0, 17:0, 18:2
            // Literal/length/distance code lengths: 0 2 #18+1111111 #18+1101001 1 2 1
            // Data: 01 #257 #0 #256
            string blockHeader = "1 01";
            string codeCounts = "10000 00000 0111";
            string codeLenCodeLens = "000 000 010 010 000 000 000 000 000 000 000 000 000 000 000 010 000 010";
            string codeLens = "00 10 111111111 111001011 01 10 01";
            string data = "10 11 0 0";
            BinTest(blockHeader + codeCounts + codeLenCodeLens + codeLens + data, "01 01 01 01");
        }

        [TestMethod]
        public async Task DynamicBlockOneDistanceCodeAsyncTest()
        {
            string blockHeader = "1 01";
            string codeCounts = "10000 00000 0111";
            string codeLenCodeLens = "000 000 010 010 000 000 000 000 000 000 000 000 000 000 000 010 000 010";
            string codeLens = "00 10 111111111 111001011 01 10 01";
            string data = "10 11 0 0";
            await BinTestAsync(blockHeader + codeCounts + codeLenCodeLens + codeLens + data, "01 01 01 01");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public void DynamicBlockDistanceCodeInvalidTest()
        {
            // numLitLen=258, numDist=1, numCodeLen=18
            // codeLenCodeLen = 0:2, 1:2, 2:2, ..., 15:0, 16:0, 17:0, 18:2
            // Literal/length/distance code lengths: 0 2 #18+1111111 #18+1101001 1 2 1
            // Data: 01 #257 #31 #256
            string blockHeader = "1 01";
            string codeCounts = "10000 00000 0111";
            string codeLenCodeLens = "000 000 010 010 000 000 000 000 000 000 000 000 000 000 000 010 000 010";
            string codeLens = "00 10 111111111 111001011 01 10 01";
            string data = "10 11 1 0";
            BinTest(blockHeader + codeCounts + codeLenCodeLens + codeLens + data, "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public async Task DynamicBlockDistanceCodeInvalidAsyncTest()
        {
            string blockHeader = "1 01";
            string codeCounts = "10000 00000 0111";
            string codeLenCodeLens = "000 000 010 010 000 000 000 000 000 000 000 000 000 000 000 010 000 010";
            string codeLens = "00 10 111111111 111001011 01 10 01";
            string data = "10 11 1 0";
            await BinTestAsync(blockHeader + codeCounts + codeLenCodeLens + codeLens + data, "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public void DynamicBlockUseOfNullDistanceCodeTest()
        {
            // numLitLen=258, numDist=1, numCodeLen=18
            // codeLenCodeLen = 0:2, 1:2, 2:2, ..., 15:0, 16:0, 17:0, 18:2
            // Literal/length/distance code lengths: 2 #18+1111111 #18+1101010 1 2 0
            // Data: 00 #257
            string blockHeader = "1 01";
            string codeCounts = "10000 00000 0111";
            string codeLenCodeLens = "000 000 010 010 000 000 000 000 000 000 000 000 000 000 000 010 000 010";
            string codeLens = "10 111111111 110101011 01 10 00";
            string data = "10 11";
            string padding = "0000000000000000";
            BinTest(blockHeader + codeCounts + codeLenCodeLens + codeLens + data + padding, "");
        }

        [TestMethod]
        [ExpectedException(typeof(DeflateDataFormatException))]
        public async Task DynamicBlockUseOfNullDistanceCodeAsyncTest()
        {
            string blockHeader = "1 01";
            string codeCounts = "10000 00000 0111";
            string codeLenCodeLens = "000 000 010 010 000 000 000 000 000 000 000 000 000 000 000 010 000 010";
            string codeLens = "10 111111111 110101011 01 10 00";
            string data = "10 11";
            string padding = "0000000000000000";
            await BinTestAsync(blockHeader + codeCounts + codeLenCodeLens + codeLens + data + padding, "");
        }

        private static void BinTest(string binInput, string hexOutput)
        {
            binInput = binInput.Replace(" ", "");
            if (binInput.Length % 8 != 0)
                binInput = binInput + new string('0', 8 - binInput.Length % 8);

            var ms = new MemoryStream();
            for (int i = 0; i < binInput.Length / 8; i++)
            {
                string bits = new string(binInput.Substring(i * 8, 8).Reverse().ToArray());
                ms.WriteByte(Convert.ToByte(bits, 2));
            }

            ms.Position = 0;

            var output = new List<byte>();
            var buffer = new byte[20];

            var ds = new DeflateStream(ms, CompressionMode.Decompress);
            while (true)
            {
                int read = ds.Read(buffer, 0, buffer.Length);
                output.AddRange(buffer.Take(read));
                if (read == 0)
                    break;
            }

            Assert.IsTrue(BitHelper.BytesToHex(output.ToArray()) == hexOutput.Replace(" ", "").ToLower());
            Assert.IsTrue(ds.Position == output.Count);
        }

        private static async Task BinTestAsync(string binInput, string hexOutput)
        {
            binInput = binInput.Replace(" ", "");
            if (binInput.Length % 8 != 0)
                binInput = binInput + new string('0', 8 - binInput.Length % 8);

            List<byte> data = new List<byte>();
            for (int i = 0; i < binInput.Length / 8; i++)
            {
                string bits = new string(binInput.Substring(i * 8, 8).Reverse().ToArray());
                data.Add(Convert.ToByte(bits, 2));
            }

            var ms = new AsyncMemoryStream(data.ToArray(), 10);

            var output = new List<byte>();
            var buffer = new byte[20];

            var ds = new DeflateStream(ms, CompressionMode.Decompress);
            while (true)
            {
                int read = await ds.ReadAsync(buffer, 0, buffer.Length);
                output.AddRange(buffer.Take(read));
                if (read == 0)
                    break;
            }

            Assert.IsTrue(BitHelper.BytesToHex(output.ToArray()) == hexOutput.Replace(" ", "").ToLower());
            Assert.IsTrue(ds.Position == output.Count);
        }
    }
}