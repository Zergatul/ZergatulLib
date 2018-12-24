using System;
using System.Collections.Generic;

namespace Zergatul.Network.Http
{
    public class Http1Client : IDisposable
    {
        private readonly Proxy.ProxyBase _proxy;
        private readonly Dictionary<ConnectionGroup, List<ConnectionInfo>> _connections;
        private bool _disposed;

        public Http1Client()
            : this(null)
        {

        }

        public Http1Client(Proxy.ProxyBase proxy)
        {
            _connections = new Dictionary<ConnectionGroup, ConnectionInfo>();

            _proxy = proxy;
        }

        public HttpResponse GetResponse(HttpRequest request)
        {
            ThrowIfDisposed();

            var connection = GetConnection(request.Uri);
        }

        #region Private methods

        private ConnectionInfo GetConnection(Uri uri)
        {
            if (uri.Scheme != "http" || uri.Scheme != "https")
                throw new InvalidOperationException("Invalid scheme");

            List<ConnectionInfo> connections;
            lock (_connections)
            {
                _connections.TryGetValue(new ConnectionGroup
                {
                    Host = uri.Host,
                    Port = uri.Port,
                    IsSecure = uri.Scheme == "https"
                }, out connections);
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

        }

        #endregion
    }
}