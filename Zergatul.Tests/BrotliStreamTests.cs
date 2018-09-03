using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.IO.Compression;

namespace Zergatul.Tests
{
    [TestClass]
    public class BrotliStreamTests
    {
        [TestMethod]
        public void Test10x10y()
        {
            TestFile("10x10y");
        }

        [TestMethod]
        public void Test64x()
        {
            TestFile("64x");
        }

        [TestMethod]
        public void TestQuickFox()
        {
            TestFile("quickfox");
        }

        [TestMethod]
        public void TestZeros()
        {
            TestFile("zeros");
        }

        [TestMethod]
        public void TestAlice29()
        {
            TestFile("alice29");
        }

        private static void TestFile(string filename)
        {
            List<byte> decompressed = new List<byte>();
            byte[] buffer = new byte[1024];
            using (var fs = new FileStream($"BrotliTestCases/{filename}.compressed", FileMode.Open))
            using (var bs = new BrotliStream(fs, System.IO.Compression.CompressionMode.Decompress))
            {
                int read;
                while ((read = bs.Read(buffer, 0, buffer.Length)) != 0)
                    decompressed.AddRange(buffer.Take(read));
            }

            string rawFile = File.Exists($"BrotliTestCases/{filename}.txt") ?
                $"BrotliTestCases/{filename}.txt" :
                $"BrotliTestCases/{filename}";
            Assert.IsTrue(ByteArray.Equals(decompressed.ToArray(), File.ReadAllBytes(rawFile)));
        }
    }
}