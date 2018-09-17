using System;

namespace Zergatul.Network.Http
{
    public abstract class Http1KeepAliveConnectionProvider
    {
        public abstract Http1Connection GetConnection(Uri uri, Proxy.ProxyBase proxy);
    }
}