using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Proxy;
using Zergatul.Network.Tls;

namespace Zergatul.Network.Http
{
    public class DefaultHttpConnectionProvider : HttpConnectionProvider
    {
        public static DefaultHttpConnectionProvider Instance { get; private set; } = new DefaultHttpConnectionProvider();

        private List<DefaultHttpConnection> _http1Connections = new List<DefaultHttpConnection>();

        private DefaultHttpConnectionProvider()
        {

        }

        public override Http1Connection GetHttp1Connection(Uri uri, ProxyBase proxy)
        {
            string host = uri.Host.ToLower();

            lock (_http1Connections)
                for (int i = 0; i < _http1Connections.Count; i++)
                {
                    // remove timed out connection
                    if (_http1Connections[i].Timeout > 0)
                        if (_http1Connections[i].Timer.ElapsedMilliseconds >= _http1Connections[i].Timeout * 1000)
                        {
                            _http1Connections[i].Stream.Dispose();
                            for (int j = i; j < _http1Connections.Count - 1; j++)
                                _http1Connections[j] = _http1Connections[j + 1];
                            _http1Connections.RemoveAt(_http1Connections.Count - 1);
                            i--;
                            continue;
                        }

                    if (_http1Connections[i].Host != host)
                        continue;
                    if (proxy != null && !proxy.Equals(_http1Connections[i].Proxy))
                        continue;
                    if (!_http1Connections[i].InUse)
                    {
                        _http1Connections[i].InUse = true;
                        return _http1Connections[i];
                    }
                }

            var client = TcpConnector.GetTcpClient(uri.Host, uri.Port, proxy);
            Stream stream;
            switch (uri.Scheme)
            {
                case "http":
                    stream = client.GetStream();
                    break;
                case "https":
                    var tls = new TlsStream(client.GetStream());
                    tls.AuthenticateAsClient(uri.Host);
                    stream = tls;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var connection = new DefaultHttpConnection(stream, host, proxy, _http1Connections);
            lock (_http1Connections)
            {
                _http1Connections.Add(connection);
                return connection;
            }
        }

        #region Nested classes

        private class DefaultHttpConnection : Http1Connection
        {
            public string Host;
            public ProxyBase Proxy;
            public volatile bool InUse;

            private List<DefaultHttpConnection> _connections;

            public DefaultHttpConnection(Stream stream, string host, Proxy.ProxyBase proxy, List<DefaultHttpConnection> connections)
            {
                this.Stream = stream;
                this.Host = host;
                this.Proxy = proxy;
                this.InUse = true;

                this._connections = connections;
            }

            public override void WriteHeader(byte[] data)
            {
                Stream.Write(data, 0, data.Length);
            }

            public override void WriteBody(byte[] data)
            {
                Stream.Write(data, 0, data.Length);
            }

            public override void WriteBody(Stream stream)
            {
                throw new NotImplementedException();
            }

            public override void Close()
            {
                lock (_connections)
                    InUse = false;
            }

            public override void CloseUnderlyingStream()
            {
                Stream.Dispose();
                lock (_connections)
                    _connections.Remove(this);
            }
        }

        #endregion
    }
}