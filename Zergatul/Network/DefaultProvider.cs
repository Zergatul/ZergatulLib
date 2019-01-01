using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Zergatul.Network.Proxy;

namespace Zergatul.Network
{
    public class DefaultProvider : Provider
    {
        public override Stream GetTcpStream(string host, int port, ProxyBase proxy)
        {
            if (proxy != null)
                throw new NotImplementedException();

            var addresses = DnsResolve(host);
            if (addresses.Length == 0)
                throw new InvalidOperationException();

            var address = addresses[0];
            var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(address, port);

            return new NetworkStream(socket, true);
        }

        public override IPAddress[] DnsResolve(string host)
        {
            return System.Net.Dns.GetHostAddresses(host);
        }
    }
}