using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Proxy
{
    public abstract class ProxyBase
    {
        public virtual abstract bool AllowConnectionsByDomainName { get; }

        public virtual abstract TcpClient CreateConnection(IPAddress address, int port, TcpClient tcp);

        public virtual abstract TcpClient CreateConnection(string hostname, int port, TcpClient tcp);

        public virtual TcpClient CreateConnection(IPAddress address, int port)
        {
            return CreateConnection(address, port, null);
        }

        public virtual TcpClient CreateConnection(string hostname, int port)
        {
            return CreateConnection(hostname, port, null);
        }

        public virtual abstract TcpListener CreateListener(int port);
    }
}