using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Zergatul.Network
{
    public abstract class NetworkProvider
    {
        public abstract Stream GetTcpStream(string host, int port, Proxy.ProxyBase proxy);
        public abstract Task<Stream> GetTcpStreamAsync(string host, int port, Proxy.ProxyBase proxy);
        public abstract IPAddress[] DnsResolve(string host);
        public abstract Task<IPAddress[]> DnsResolveAsync(string host);
    }
}