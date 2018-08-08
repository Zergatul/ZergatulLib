using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Zergatul.Network.Tls;

namespace Zergatul.Network.Http
{
    public class DefaultKeepAliveConnectionProvider : KeepAliveConnectionProvider
    {
        public static KeepAliveConnectionProvider Instance { get; private set; } = new DefaultKeepAliveConnectionProvider();

        private List<DefaultHttpConnection> _connections = new List<DefaultHttpConnection>();

        private DefaultKeepAliveConnectionProvider()
        {

        }

        public override HttpConnection GetConnection(Uri uri)
        {
            string host = uri.Host.ToLower();

            lock (_connections)
                for (int i = 0; i < _connections.Count; i++)
                {
                    // remove timeout connection
                    if (_connections[i].Timeout > 0)
                        if (_connections[i].Timer.ElapsedMilliseconds >= _connections[i].Timeout * 1000)
                        {
                            for (int j = i; j < _connections.Count - 1; j++)
                                _connections[j] = _connections[j + 1];
                            _connections.RemoveAt(_connections.Count - 1);
                            i--;
                            continue;
                        }

                    if (_connections[i].Host != host)
                        continue;
                    if (!_connections[i].InUse)
                    {
                        _connections[i].InUse = true;
                        return _connections[i];
                    }
                }

            var client = new TcpClient();
            client.Connect(uri.Host, uri.Port);
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

            var connection = new DefaultHttpConnection(stream, _connections)
            {
                Host = host
            };
            lock (_connections)
            {
                _connections.Add(connection);
                return connection;
            }
        }

        private class DefaultHttpConnection : HttpConnection
        {
            public override Stream Stream => _stream;

            public string Host;
            public volatile bool InUse;

            private Stream _stream;
            private List<DefaultHttpConnection> _connections;

            public DefaultHttpConnection(Stream stream, List<DefaultHttpConnection> connections)
            {
                this.InUse = true;

                this._stream = stream;
                this._connections = connections;
            }

            public override void Close()
            {
                lock (_connections)
                    InUse = false;
            }

            public override void CloseUnderlyingStream()
            {
                _stream.Dispose();
                lock (_connections)
                    _connections.Remove(this);
            }
        }
    }
}