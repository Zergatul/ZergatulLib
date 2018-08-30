using System;

namespace Zergatul.Network.Http
{
    public abstract class KeepAliveConnectionProvider
    {
        public abstract HttpConnection GetConnection(Uri uri, Proxy.ProxyBase proxy);
    }
}