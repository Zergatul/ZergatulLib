using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Zergatul.IO.Compression;
using Zergatul.Network.Http;
using Zergatul.Tests;

namespace Zergatul.Network.Tests.Http
{
    [TestClass]
    public class HttpResponseStreamTests
    {
        [TestMethod]
        public void ChunkedTest()
        {
            string s =
                "4\r\n" +
                "Wiki\r\n" +
                "5\r\n" +
                "pedia\r\n" +
                "E\r\n" +
                " in\r\n" +
                "\r\n" +
                "chunks.\r\n" +
                "0\r\n" +
                "\r\n";

            string data;

            using (var ms = new MemoryStream(Encoding.ASCII.GetBytes(s)))
            using (var stream = new HttpResponseStream(ms, true, -1))
            using (var sr = new StreamReader(stream))
            {
                data = sr.ReadToEnd();
                Assert.IsTrue(stream.EndOfStream);
            }

            Assert.IsTrue(data == "Wikipedia in\r\n\r\nchunks.");
        }

        [TestMethod]
        public async Task ChunkedAsyncTest()
        {
            string s =
                "4\r\n" +
                "Wiki\r\n" +
                "5\r\n" +
                "pedia\r\n" +
                "E\r\n" +
                " in\r\n" +
                "\r\n" +
                "chunks.\r\n" +
                "0\r\n" +
                "\r\n";

            string data;

            using (var ms = new AsyncMemoryStream(Encoding.ASCII.GetBytes(s), 10))
            using (var stream = new HttpResponseStream(ms, true, -1))
            using (var sr = new StreamReader(stream))
            {
                data = await sr.ReadToEndAsync();
                Assert.IsTrue(stream.EndOfStream);
            }

            Assert.IsTrue(data == "Wikipedia in\r\n\r\nchunks.");
        }

        [TestMethod]
        public void ChunkedGzipTest()
        {
            byte[] gzipData = BitHelper.HexToBytes("1f8b08000000000004000bcecf4d5528492d2e5148492c490400ba76704e0e000000");

            byte[] chunked = ByteArray.Concat(
                Encoding.ASCII.GetBytes(Convert.ToString(gzipData.Length, 16) + "\r\n"),
                gzipData,
                Encoding.ASCII.GetBytes("\r\n0\r\n\r\n"));

            string data;

            using (var ms = new MemoryStream(chunked))
            using (var stream = new HttpResponseStream(ms, true, -1))
            using (var gzip = new GzipStream(stream, CompressionMode.Decompress))
            using (var sr = new StreamReader(gzip))
            {
                data = sr.ReadToEnd();
                Assert.IsTrue(stream.EndOfStream);
            }

            Assert.IsTrue(data == "Some test data");
        }

        [TestMethod]
        public async Task ChunkedGzipAsyncTest()
        {
            byte[] gzipData = BitHelper.HexToBytes("1f8b08000000000004000bcecf4d5528492d2e5148492c490400ba76704e0e000000");

            byte[] chunked = ByteArray.Concat(
                Encoding.ASCII.GetBytes(Convert.ToString(gzipData.Length, 16) + "\r\n"),
                gzipData,
                Encoding.ASCII.GetBytes("\r\n0\r\n\r\n"));

            string data;

            using (var ams = new AsyncMemoryStream(chunked, 10))
            using (var stream = new HttpResponseStream(ams, true, -1))
            using (var gzip = new GzipStream(stream, CompressionMode.Decompress))
            using (var sr = new StreamReader(gzip))
            {
                data = await sr.ReadToEndAsync();
                Assert.IsTrue(stream.EndOfStream);
            }

            Assert.IsTrue(data == "Some test data");
        }
    }
}