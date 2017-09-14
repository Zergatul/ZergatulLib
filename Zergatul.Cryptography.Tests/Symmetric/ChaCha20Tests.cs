using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Symmetric;
using System.Reflection;
using System.Linq;

namespace Zergatul.Cryptography.Tests.BlockCipher
{
    [TestClass]
    public class ChaCha20Tests
    {
        [TestMethod]
        public void QuarterRoundTest()
        {
            var method = typeof(ChaCha20).GetMethod(
                "QuarterRound",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                Enumerable.Repeat(typeof(uint).MakeByRefType(), 4).ToArray(),
                null);
            object[] args = new object[4] { 0x11111111U, 0x01020304U, 0x9b8d6f43U, 0x01234567U };
            method.Invoke(null, args);

            Assert.IsTrue((uint)args[0] == 0xea2a92f4U);
            Assert.IsTrue((uint)args[1] == 0xcb1cf8ceU);
            Assert.IsTrue((uint)args[2] == 0x4581472eU);
            Assert.IsTrue((uint)args[3] == 0x5881c4bbU);
        }

        [TestMethod]
        public void QuarterRoundOnStateTest()
        {
            var state = new uint[16]
            {
                0x879531e0, 0xc5ecf37d, 0x516461b1, 0xc9a62f8a,
                0x44c20ef3, 0x3390af7f, 0xd9fc690b, 0x2a5f714c,
                0x53372767, 0xb00a5631, 0x974c541a, 0x359e9963,
                0x5c971061, 0x3d631689, 0x2098d9d6, 0x91dbd320,
            };

            var method = typeof(ChaCha20).GetMethod(
                "QuarterRound",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[] { typeof(uint).MakeArrayType(), typeof(int), typeof(int), typeof(int), typeof(int) },
                null);

            object[] args = new object[5] { state, 2, 7, 8, 13 };
            method.Invoke(null, args);

            Assert.IsTrue(state.SequenceEqual(new uint[16]
            {
                0x879531e0, 0xc5ecf37d, 0xbdb886dc, 0xc9a62f8a,
                0x44c20ef3, 0x3390af7f, 0xd9fc690b, 0xcfacafd2,
                0xe46bea80, 0xb00a5631, 0x974c541a, 0x359e9963,
                0x5c971061, 0xccc07c79, 0x2098d9d6, 0x91dbd320,
            }));
        }

        [TestMethod]
        public void Vector1()
        {
            byte[] key = BitHelper.HexToBytes("00:01:02:03:04:05:06:07:08:09:0a:0b:0c:0d:0e:0f:10:11:12:13:14:15:16:17:18:19:1a:1b:1c:1d:1e:1f".Replace(":", ""));
            byte[] nonce = BitHelper.HexToBytes("00:00:00:09:00:00:00:4a:00:00:00:00".Replace(":", ""));

            var chacha20 = new ChaCha20();
            var keyStream = chacha20.InitKeyStream(key, nonce, 1);

            byte[] block = new byte[64];
            keyStream.Read(block, 0, 64);

            Assert.IsTrue(block.SequenceEqual(new byte[]
            {
                0x10, 0xf1, 0xe7, 0xe4, 0xd1, 0x3b, 0x59, 0x15, 0x50, 0x0f, 0xdd, 0x1f, 0xa3, 0x20, 0x71, 0xc4,
                0xc7, 0xd1, 0xf4, 0xc7, 0x33, 0xc0, 0x68, 0x03, 0x04, 0x22, 0xaa, 0x9a, 0xc3, 0xd4, 0x6c, 0x4e,
                0xd2, 0x82, 0x64, 0x46, 0x07, 0x9f, 0xaa, 0x09, 0x14, 0xc2, 0xd7, 0x05, 0xd9, 0x8b, 0x02, 0xa2,
                0xb5, 0x12, 0x9c, 0xd1, 0xde, 0x16, 0x4e, 0xb9, 0xcb, 0xd0, 0x83, 0xe8, 0xa2, 0x50, 0x3c, 0x4e,
            }));
        }
    }
}