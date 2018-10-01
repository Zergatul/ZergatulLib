using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Network.Http;

namespace Zergatul.Network.Tests.Http
{
    [TestClass]
    public class HttpResponseTests
    {
        [TestMethod]
        public void ChunkedEncoding1Test()
        {
            var fs = new FileStream("Http/Chunked1.txt", FileMode.Open);
            var connection = new TestHttpConnection(fs);
            var response = new HttpResponse(connection);

            Assert.IsTrue(response.Version == "HTTP/1.1");
            Assert.IsTrue(response.Status == HttpStatusCode.BadRequest);
            Assert.IsTrue(response.ReasonPhase == "Bad Request");

            Assert.IsTrue(response[HttpResponseHeaders.Server] == "nginx");
            Assert.IsTrue(response[HttpResponseHeaders.Date] == "Wed, 08 Aug 2018 15:04:01 GMT");
            Assert.IsTrue(response[HttpResponseHeaders.ContentType] == "text/html; charset=utf-8");
            Assert.IsTrue(response[HttpResponseHeaders.TransferEncoding] == "chunked");
            Assert.IsTrue(response[HttpResponseHeaders.Connection] == "close");

            string html = new StreamReader(response.Stream, Encoding.UTF8).ReadToEnd();
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
        public void ChunkedEncoding2Test()
        {
            var fs = new FileStream("Http/Chunked2.txt", FileMode.Open);
            var connection = new TestHttpConnection(fs);
            var response = new HttpResponse(connection);

            Assert.IsTrue(response.Version == "HTTP/1.1");
            Assert.IsTrue(response.Status == HttpStatusCode.OK);
            Assert.IsTrue(response.ReasonPhase == "OK");

            Assert.IsTrue(response[HttpResponseHeaders.Server] == "nginx");
            Assert.IsTrue(response[HttpResponseHeaders.Date] == "Wed, 08 Aug 2018 15:50:58 GMT");
            Assert.IsTrue(response[HttpResponseHeaders.ContentType] == "text/html;charset=UTF-8");
            Assert.IsTrue(response[HttpResponseHeaders.TransferEncoding] == "chunked");
            Assert.IsTrue(response[HttpResponseHeaders.Connection] == "close");
            Assert.IsTrue(response["X-Cached"] == "HIT");

            string html = new StreamReader(response.Stream, Encoding.UTF8).ReadToEnd();
            Assert.IsTrue(html.Length > 0);

            fs.Close();  // close stream first to ensure dispose doesn't read from stream anymore
            response.Dispose();
        }

        private class TestHttpConnection : Http1Connection
        {
            private Stream _stream;

            public TestHttpConnection(Stream stream)
            {
                this._stream = stream;
            }

            public override void WriteBody(byte[] data)
            {
                throw new NotImplementedException();
            }

            public override void WriteBody(Stream stream)
            {
                throw new NotImplementedException();
            }

            public override void WriteHeader(byte[] data)
            {
                throw new NotImplementedException();
            }

            public override void Close()
            {
                
            }

            public override void CloseUnderlyingStream()
            {
                
            }
        }
    }
}