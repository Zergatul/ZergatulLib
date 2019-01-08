using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Zergatul.Tests
{
    [TestClass]
    public class BufferedStreamTests
    {
        [TestMethod]
        public void WriteLessThanBufferSizeTest()
        {
            var mock = new Mock<Stream>(MockBehavior.Strict);
            var bs = new IO.BufferedStream(mock.Object, 10);
            bs.Write(new byte[9], 0, 9);
        }

        [TestMethod]
        public async Task WriteLessThanBufferSizeAsyncTest()
        {
            var mock = new Mock<Stream>(MockBehavior.Strict);
            var bs = new IO.BufferedStream(mock.Object, 10);
            await bs.WriteAsync(new byte[9], 0, 9);
        }

        [TestMethod]
        public void WriteBufferSizeTest()
        {
            var mock = new Mock<Stream>(MockBehavior.Strict);
            mock.Setup(s => s.Write(It.IsAny<byte[]>(), 0, 5)).Callback<byte[], int, int>((buffer, offset, count) =>
            {
                Assert.IsTrue(buffer.Length == 5);
                Assert.IsTrue(buffer[0] == 1);
                Assert.IsTrue(buffer[1] == 2);
                Assert.IsTrue(buffer[2] == 3);
                Assert.IsTrue(buffer[3] == 4);
                Assert.IsTrue(buffer[4] == 5);
            });
            mock.Setup(s => s.Flush());

            var bs = new IO.BufferedStream(mock.Object, 5);
            bs.Write(new byte[5] { 1, 2, 3, 4, 5 }, 0, 5);

            mock.Verify(s => s.Write(It.IsAny<byte[]>(), 0, 5), Times.Once());
            mock.Verify(s => s.Flush(), Times.Once());
        }

        [TestMethod]
        public async Task WriteBufferSizeAsyncTest()
        {
            var mock = new Mock<Stream>(MockBehavior.Strict);
            mock.Setup(s => s.WriteAsync(It.IsAny<byte[]>(), 0, 5, CancellationToken.None)).Returns<byte[], int, int, CancellationToken>((buffer, offset, count, token) =>
            {
                return Task.Run(() =>
                {
                    Assert.IsTrue(buffer.Length == 5);
                    Assert.IsTrue(buffer[0] == 1);
                    Assert.IsTrue(buffer[1] == 2);
                    Assert.IsTrue(buffer[2] == 3);
                    Assert.IsTrue(buffer[3] == 4);
                    Assert.IsTrue(buffer[4] == 5);
                });
            });
            mock.Setup(s => s.FlushAsync(CancellationToken.None)).Returns(Task.FromResult(0));

            var bs = new IO.BufferedStream(mock.Object, 5);
            await bs.WriteAsync(new byte[5] { 1, 2, 3, 4, 5 }, 0, 5);

            mock.Verify(s => s.WriteAsync(It.IsAny<byte[]>(), 0, 5, CancellationToken.None), Times.Once());
            mock.Verify(s => s.FlushAsync(CancellationToken.None), Times.Once());
        }

        [TestMethod]
        public void WriteBigBufferTest()
        {
            var ms = new MemoryStream();
            var bs = new IO.BufferedStream(ms, 10);
            byte[] buffer = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();
            bs.Write(buffer, 0, buffer.Length);

            Assert.IsTrue(ms.Length == 250);
            for (int i = 0; i < 250; i++)
                Assert.IsTrue(ms.GetBuffer()[i] == i);

            bs.Flush();

            Assert.IsTrue(ms.Length == 256);
            for (int i = 250; i < 256; i++)
                Assert.IsTrue(ms.GetBuffer()[i] == i);
        }

        [TestMethod]
        public async Task WriteBigBufferAsyncTest()
        {
            var ms = new AsyncMemoryStream(10);
            var bs = new IO.BufferedStream(ms, 10);
            byte[] buffer = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();
            await bs.WriteAsync(buffer, 0, buffer.Length);

            Assert.IsTrue(ms.Length == 250);
            for (int i = 0; i < 250; i++)
                Assert.IsTrue(ms.GetBuffer()[i] == i);

            await bs.FlushAsync();

            Assert.IsTrue(ms.Length == 256);
            for (int i = 250; i < 256; i++)
                Assert.IsTrue(ms.GetBuffer()[i] == i);
        }
    }
}