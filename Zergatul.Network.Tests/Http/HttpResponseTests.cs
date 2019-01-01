using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Network.Http;
using Zergatul.Tests;

namespace Zergatul.Network.Tests.Http
{
    [TestClass]
    public class HttpResponseTests
    {
        [TestMethod]
        public void ChunkedEncoding1Test()
        {
            var fs = new FileStream("Http/Chunked1.txt", FileMode.Open);
            var response = new HttpResponse();
            response.ReadFrom(fs);

            Assert.IsTrue(response.Version == "HTTP/1.1");
            Assert.IsTrue(response.Status == HttpStatusCode.BadRequest);
            Assert.IsTrue(response.ReasonPhase == "Bad Request");

            Assert.IsTrue(response[HttpResponseHeaders.Server] == "nginx");
            Assert.IsTrue(response[HttpResponseHeaders.Date] == "Wed, 08 Aug 2018 15:04:01 GMT");
            Assert.IsTrue(response[HttpResponseHeaders.ContentType] == "text/html; charset=utf-8");
            Assert.IsTrue(response[HttpResponseHeaders.TransferEncoding] == "chunked");
            Assert.IsTrue(response[HttpResponseHeaders.Connection] == "close");

            string html = new StreamReader(response.Body, Encoding.UTF8).ReadToEnd();
            Assert.IsTrue(html ==
                "<html>" + Constants.TelnetEndOfLine +
                "<head><title>400 Bad Request</title></head>" + Constants.TelnetEndOfLine +
                "<body bgcolor=\"white\">" + Constants.TelnetEndOfLine +
                "<center><h1>400 Bad Request</h1></center>" + Constants.TelnetEndOfLine +
                "<hr><center>nginx</center>" + Constants.TelnetEndOfLine +
                "</body>" + Constants.TelnetEndOfLine +
                "</html>" + Constants.TelnetEndOfLine);

            fs.Close();  // close stream first to ensure dispose doesn't read from stream anymore
            response.Dispose();
        }

        [TestMethod]
        public async Task ChunkedEncodingAsync1Test()
        {
            var ms = new AsyncMemoryStream(10);
            using (var fs = new FileStream("Http/Chunked1.txt", FileMode.Open))
                await fs.CopyToAsync(ms);

            ms.Position = 0;

            var response = new HttpResponse();
            await response.ReadFromAsync(ms, CancellationToken.None);

            Assert.IsTrue(response.Version == "HTTP/1.1");
            Assert.IsTrue(response.Status == HttpStatusCode.BadRequest);
            Assert.IsTrue(response.ReasonPhase == "Bad Request");

            Assert.IsTrue(response[HttpResponseHeaders.Server] == "nginx");
            Assert.IsTrue(response[HttpResponseHeaders.Date] == "Wed, 08 Aug 2018 15:04:01 GMT");
            Assert.IsTrue(response[HttpResponseHeaders.ContentType] == "text/html; charset=utf-8");
            Assert.IsTrue(response[HttpResponseHeaders.TransferEncoding] == "chunked");
            Assert.IsTrue(response[HttpResponseHeaders.Connection] == "close");

            var sr = new StreamReader(response.Body, Encoding.UTF8);
            string html = await sr.ReadToEndAsync();
            Assert.IsTrue(html ==
                "<html>" + Constants.TelnetEndOfLine +
                "<head><title>400 Bad Request</title></head>" + Constants.TelnetEndOfLine +
                "<body bgcolor=\"white\">" + Constants.TelnetEndOfLine +
                "<center><h1>400 Bad Request</h1></center>" + Constants.TelnetEndOfLine +
                "<hr><center>nginx</center>" + Constants.TelnetEndOfLine +
                "</body>" + Constants.TelnetEndOfLine +
                "</html>" + Constants.TelnetEndOfLine);

            ms.Close();
            response.Dispose();
        }

        const string EmptyReasonPhraseResponse =
            "HTTP/1.1 404\r\n" +
            "Server: nginx/1.8.0\r\n" +
            "Date: Tue, 01 Jan 2019 15:50:24 GMT\r\n" +
            "Content-Type: application/json\r\n" +
            "Transfer-Encoding: chunked\r\n" +
            "Connection: keep-alive\r\n" +
            "Expires: Thu, 19 Nov 1981 08:52:00 GMT\r\n" +
            "Cache-Control: no-store, no-cache, must-revalidate, post-check=0, pre-check=0\r\n" +
            "Pragma: no-cache\r\n" +
            "\r\n" +
            "6\r\n" +
            "errors\r\n" +
            "0\r\n" +
            "\r\n";

        [TestMethod]
        public void EmptyReasonPhraseTest()
        {
            var ms = new MemoryStream(Encoding.ASCII.GetBytes(EmptyReasonPhraseResponse));
            var response = new HttpResponse();
            response.ReadFrom(ms);

            Assert.IsTrue(response.Version == "HTTP/1.1");
            Assert.IsTrue(response.Status == HttpStatusCode.NotFound);
            Assert.IsTrue(response.ReasonPhase == null);

            string data = new StreamReader(response.Body, Encoding.UTF8).ReadToEnd();
            Assert.IsTrue(data == "errors");
        }

        [TestMethod]
        public async Task EmptyReasonPhraseAsyncTest()
        {
            var ms = new AsyncMemoryStream(Encoding.ASCII.GetBytes(EmptyReasonPhraseResponse), 10);
            var response = new HttpResponse();
            await response.ReadFromAsync(ms, CancellationToken.None);

            Assert.IsTrue(response.Version == "HTTP/1.1");
            Assert.IsTrue(response.Status == HttpStatusCode.NotFound);
            Assert.IsTrue(response.ReasonPhase == null);

            var sr = new StreamReader(response.Body, Encoding.UTF8);
            string data = await sr.ReadToEndAsync();
            Assert.IsTrue(data == "errors");
        }
    }
}