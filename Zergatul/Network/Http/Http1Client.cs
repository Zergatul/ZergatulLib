using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Zergatul.Network.Tls;
using Zergatul.Security;
using Zergatul.Security.Tls;

namespace Zergatul.Network.Http
{
    public class Http1Client : IDisposable
    {
        private readonly Proxy.ProxyBase _proxy;
        private readonly NetworkProvider _provider;
        private readonly Dictionary<ConnectionGroup, List<ConnectionInfo>> _connections;
        private bool _disposed;

        public TlsStreamParameters TlsStreamParameters { get; set; }

        public Http1Client()
            : this(null)
        {

        }

        public Http1Client(Proxy.ProxyBase proxy)
            : this(proxy, new DefaultNetworkProvider())
        {

        }

        public Http1Client(Proxy.ProxyBase proxy, NetworkProvider provider)
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

            var response = new HttpResponse(this);
            response.ReadFrom(connection.Stream);
            connection.LastResponse = response;
            return response;
        }

        public Task<HttpResponse> GetResponseAsync(HttpRequest request) => GetResponseAsync(request, CancellationToken.None);

        public async Task<HttpResponse> GetResponseAsync(HttpRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ThrowIfDisposed();

            var connection = await GetConnectionAsync(request.Uri, cancellationToken).ConfigureAwait(false);
            await request.WriteToAsync(connection.Stream, cancellationToken).ConfigureAwait(false);
            await connection.Stream.FlushAsync(cancellationToken).ConfigureAwait(false);

            var response = new HttpResponse(this);
            await response.ReadFromAsync(connection.Stream, cancellationToken).ConfigureAwait(false);
            connection.LastResponse = response;
            return response;
        }

        #region Internal methods

        internal void DisposeResponse(HttpResponse response, bool keepAlive, int timeout)
        {
            List<ConnectionInfo> list = null;
            ConnectionInfo connection = null;
            lock (_connections)
                foreach (var @group in _connections)
                {
                    lock (group.Value)
                        foreach (var con in group.Value)
                            if (con.LastResponse == response)
                            {
                                list = group.Value;
                                connection = con;
                                break;
                            }

                    if (list != null)
                        break;
                }

            if (list == null || connection == null)
                throw new InvalidOperationException();

            if (keepAlive && response.RawBody.EndOfStream)
            {
                connection.InUse = false;
                connection.Timeout = timeout;
            }
            else
            {
                lock (list)
                    list.Remove(connection);
                connection.Stream.Close();
            }
        }

        #endregion

        #region Private methods

        private ConnectionInfo GetConnection(Uri uri)
        {
            if (uri.Scheme != "http" && uri.Scheme != "https")
                throw new InvalidOperationException("Invalid scheme");

            GetFreeConnection(uri, out List<ConnectionInfo> list, out ConnectionInfo connection);

            if (connection == null)
            {
                connection = CreateNewConnection(uri);
                lock (list)
                    list.Add(connection);
            }

            return connection;
        }

        private async Task<ConnectionInfo> GetConnectionAsync(Uri uri, CancellationToken cancellationToken)
        {
            if (uri.Scheme != "http" && uri.Scheme != "https")
                throw new InvalidOperationException("Invalid scheme");

            GetFreeConnection(uri, out List<ConnectionInfo> list, out ConnectionInfo connection);

            if (connection == null)
            {
                connection = await CreateNewConnectionAsync(uri);
                lock (list)
                    list.Add(connection);
            }

            return connection;
        }

        private void GetFreeConnection(Uri uri, out List<ConnectionInfo> list, out ConnectionInfo connection)
        {
            lock (_connections)
            {
                var @group = new ConnectionGroup
                {
                    Host = uri.Host,
                    Port = uri.Port,
                    IsSecure = uri.Scheme == "https"
                };
                _connections.TryGetValue(@group, out list);

                if (list == null)
                {
                    list = new List<ConnectionInfo>();
                    _connections.Add(group, list);
                }
            }

            lock (list)
            {
                DateTime now = DateTime.Now;
                for (int i = 0; i < list.Count; i++)
                {
                    connection = list[i];
                    if (connection.InUse)
                        continue;
                    if (connection.Timeout != 0)
                    {
                        // check if connection timed out
                        if ((now - connection.LastUse).TotalSeconds > connection.Timeout)
                        {
                            list.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                    connection.LastUse = now;
                    connection.InUse = true;
                    return;
                }
            }

            connection = null;
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
                        LastUse = DateTime.Now,
                        InUse = true
                    };

                case "https":
                    var tls = SecurityProvider.GetTlsStreamInstance(stream);
                    if (TlsStreamParameters != null)
                        tls.Parameters = TlsStreamParameters;
                    tls.Parameters.Host = uri.Host;
                    tls.AuthenticateAsClient();
                    return new ConnectionInfo
                    {
                        Stream = tls,
                        LastUse = DateTime.Now,
                        InUse = true
                    };

                default:
                    throw new InvalidOperationException();
            }
        }

        private async Task<ConnectionInfo> CreateNewConnectionAsync(Uri uri)
        {
            var stream = await _provider.GetTcpStreamAsync(uri.Host, uri.Port, _proxy);
            switch (uri.Scheme)
            {
                case "http":
                    var buffered = new IO.BufferedStream(stream);
                    return new ConnectionInfo
                    {
                        Stream = stream,
                        LastUse = DateTime.Now,
                        InUse = true
                    };

                case "https":
                    var tls = SecurityProvider.GetTlsStreamInstance(stream);
                    tls.Parameters.Host = uri.Host;
                    await tls.AuthenticateAsClientAsync();
                    return new ConnectionInfo
                    {
                        Stream = tls,
                        LastUse = DateTime.Now,
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
            public HttpResponse LastResponse;
        }

        #endregion
    }
}