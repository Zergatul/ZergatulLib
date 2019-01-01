using System;
using System.Collections.Generic;
using System.IO;
using Zergatul.Network.Tls;

namespace Zergatul.Network.Http
{
    public class Http1Client : IDisposable
    {
        private readonly Proxy.ProxyBase _proxy;
        private readonly Provider _provider;
        private readonly Dictionary<ConnectionGroup, List<ConnectionInfo>> _connections;
        private bool _disposed;

        public Http1Client()
            : this(null)
        {

        }

        public Http1Client(Proxy.ProxyBase proxy)
            : this(proxy, new DefaultProvider())
        {

        }

        public Http1Client(Proxy.ProxyBase proxy, Provider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _connections = new Dictionary<ConnectionGroup, List<ConnectionInfo>>();
            _proxy = proxy;
            _provider = provider;
        }

        public HttpResponse GetResponse(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ThrowIfDisposed();

            var connection = GetConnection(request.Uri);
            request.WriteTo(connection.Stream);
            connection.Stream.Flush();

            var response = new HttpResponse();
            response.ReadFrom(connection.Stream);
            return response;
        }

        #region Private methods

        private ConnectionInfo GetConnection(Uri uri)
        {
            if (uri.Scheme != "http" && uri.Scheme != "https")
                throw new InvalidOperationException("Invalid scheme");

            List<ConnectionInfo> connectionsList;
            lock (_connections)
            {
                var @group = new ConnectionGroup
                {
                    Host = uri.Host,
                    Port = uri.Port,
                    IsSecure = uri.Scheme == "https"
                };
                _connections.TryGetValue(@group, out connectionsList);

                if (connectionsList == null)
                {
                    connectionsList = new List<ConnectionInfo>();
                    _connections.Add(group, connectionsList);
                }
            }

            lock (connectionsList)
            {
                DateTime now = DateTime.Now;
                for (int i = 0; i < connectionsList.Count; i++)
                {
                    var connection = connectionsList[i];
                    if (connection.InUse)
                        continue;
                    if (connection.Timeout != 0)
                    {
                        // check if connection timed out
                        if ((now - connection.LastUse).TotalSeconds > connection.Timeout)
                        {
                            connectionsList.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                    connection.InUse = true;
                    return connection;
                }
            }

            var newConnection = CreateNewConnection(uri);
            lock (connectionsList)
                connectionsList.Add(newConnection);
            return newConnection;
        }

        private ConnectionInfo CreateNewConnection(Uri uri)
        {
            var stream = _provider.GetTcpStream(uri.Host, uri.Port, _proxy);
            switch (uri.Scheme)
            {
                case "http":
                    var buffered = new IO.BufferedStream(stream);
                    return new ConnectionInfo
                    {
                        Stream = stream,
                        InUse = true
                    };

                case "https":
                    var tls = new TlsStream(stream);
                    tls.AuthenticateAsClient(uri.Host);
                    return new ConnectionInfo
                    {
                        Stream = tls,
                        InUse = true
                    };

                default:
                    throw new InvalidOperationException();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Http1Client));
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                
            }

            _disposed = true;
        }

        #endregion

        #region Nested classes

        private struct ConnectionGroup
        {
            public string Host;
            public int Port;
            public bool IsSecure;

            public override bool Equals(object obj)
            {
                if (obj is ConnectionGroup other)
                {
                    return
                        Host == other.Host &&
                        Port == other.Port &&
                        IsSecure == other.IsSecure;
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return BitHelper.RotateLeft(Host.GetHashCode(), 13) ^ BitHelper.RotateLeft(Port, 23) ^ IsSecure.GetHashCode();
            }
        }

        private class ConnectionInfo
        {
            public bool InUse;
            public int Timeout;
            public DateTime LastUse;
            public Stream Stream;
        }

        #endregion
    }
}