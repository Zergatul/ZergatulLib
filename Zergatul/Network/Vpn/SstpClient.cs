using System;
using System.IO;
using System.Text;

namespace Zergatul.Network.Vpn
{
    public class SstpClient : IDisposable
    {
        private Provider _provider;
        private Stream _stream;
        private Guid _correlationId;

        public SstpClient()
            : this(new DefaultProvider())
        {

        }

        public SstpClient(Provider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _provider = provider;
        }

        public void Connect(string hostname, int port)
        {
            _stream = _provider.GetTcpStream(hostname, port, null);

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
                _stream?.Close();
            }
        }

        #endregion
    }
}