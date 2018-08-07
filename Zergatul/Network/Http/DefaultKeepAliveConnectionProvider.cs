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

        private List<KeepAliveConnection> _connections = new List<KeepAliveConnection>();

        private DefaultKeepAliveConnectionProvider()
        {

        }

        public override HttpConnection GetConnection(Uri uri)
        {
            string host = uri.Host.ToLower();

            lock (_connections)
                for (int i = 0; i < _connections.Count; i++)
                {
                    if (_connections[i].Host != host)
                        continue;
                    if (!_connections[i].InUse)
                        return new DefaultHttpConnection(_connections[i], _connections);
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

            var connection = new KeepAliveConnection
            {
                Host = host,
                Stream = stream
            };
            lock (_connections)
            {
                _connections.Add(connection);
                return new DefaultHttpConnection(connection, _connections);
            }
        }

        private class DefaultHttpConnection : HttpConnection
        {
            public override Stream Stream => _stream;

            private Stream _stream;
            private KeepAliveConnection _connection;
            private List<KeepAliveConnection> _connections;

            public DefaultHttpConnection(KeepAliveConnection connection, List<KeepAliveConnection> connections)
            {
                connection.InUse = true;

                this._stream = connection.Stream;
                this._connection = connection;
                this._connections = connections;
            }

            public override void Close()
            {
                lock (_connections)
                    _connection.InUse = false;
            }

            public override void CloseUnderlyingStream()
            {
                _stream.Dispose();
                lock (_connections)
                    _connections.Remove(_connection);
            }
        }

        private class KeepAliveConnection
        {
            public string Host;
            public Stream Stream;
            public volatile bool InUse;
        }
    }
}