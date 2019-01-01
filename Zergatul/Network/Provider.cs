using System.IO;
using System.Net;

namespace Zergatul.Network
{
    public abstract class Provider
    {
        public abstract Stream GetTcpStream(string host, int port, Proxy.ProxyBase proxy);
        public abstract IPAddress[] DnsResolve(string host);
    }
}