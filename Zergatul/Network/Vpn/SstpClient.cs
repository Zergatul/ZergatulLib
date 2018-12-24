using System;
using System.Net.Sockets;
using System.Text;

namespace Zergatul.Network.Vpn
{
    public class SstpClient : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private Guid _correlationId;

        public SstpClient()
        {

        }

        public void Connect(string hostname, int port)
        {
            _client = TcpConnector.GetTcpClient(hostname, port, null);
            _stream = _client.GetStream();

            _correlationId = Guid.NewGuid();

            string request =
                "SSTP_DUPLEX_POST /sra_{BA195980-CD49-458b-9E23-C84EE0ADCD75}/ HTTP/1.1" + Constants.TelnetEndOfLine +
                "Host: " + hostname + Constants.TelnetEndOfLine +
                "Content-Length: 18446744073709551615" + Constants.TelnetEndOfLine +
                "SSTPCORRELATIONID: " + _correlationId + Constants.TelnetEndOfLine +
                Constants.TelnetEndOfLine;
            byte[] buffer = Encoding.ASCII.GetBytes(request);
            _stream.Write(buffer, 0, buffer.Length);

            //new Http.HttpResponse();
        }

        public void Connect(string hostname) => Connect(hostname, 443);

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client?.Dispose();
            }
        }

        #endregion
    }
}