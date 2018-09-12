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
        public void TestEmpty()
        {
            TestFile("empty");
        }

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
        public void TestQuickFoxRepeated()
        {
            TestFile("quickfox_repeated");
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

        [TestMethod]
        public void TestAsyoulik()
        {
            TestFile("asyoulik");
        }

        [TestMethod]
        public void TestBackward65536()
        {
            TestFile("backward65536");
        }

        [TestMethod]
        public void TestCompressedFile()
        {
            TestFile("compressed_file");
        }

        [TestMethod]
        public void TestCompressedRepeated()
        {
            TestFile("compressed_repeated");
        }

        [TestMethod]
        public void TestLcet10()
        {
            TestFile("lcet10");
        }

        [TestMethod]
        public void TestMapsdatazrh()
        {
            TestFile("mapsdatazrh");
        }

        [TestMethod]
        public void TestMonkey()
        {
            TestFile("monkey");
        }

        [TestMethod]
        public void TestPlrabn12()
        {
            TestFile("plrabn12");
        }

        [TestMethod]
        public void TestRandomOrg10k()
        {
            TestFile("random_org_10k.bin");
        }

        [TestMethod]
        public void TestUkkonooa()
        {
            TestFile("ukkonooa");
        }

        [TestMethod]
        public void TestXyzzy()
        {
            TestFile("xyzzy");
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

            byte[] rawBytes = File.ReadAllBytes(rawFile);
            byte[] decomprs = decompressed.ToArray();
            int len = System.Math.Min(rawBytes.Length, decomprs.Length);
            for (int i = 0; i < len; i++)
            {
                if (rawBytes[i] != decomprs[i])
                    Assert.Fail("Data mismatch on index " + i);
            }
            if (rawBytes.Length != decomprs.Length)
                Assert.Fail("Data length mismatch");
        }
    }
}