using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
                data = sr.ReadToEnd();

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
                data = await sr.ReadToEndAsync();

            Assert.IsTrue(data == "Wikipedia in\r\n\r\nchunks.");
        }
    }
}