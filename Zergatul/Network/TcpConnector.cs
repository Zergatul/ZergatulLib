using System.Net.Sockets;
using Zergatul.Network.Dns;

namespace Zergatul.Network
{
    internal static class TcpConnector
    {
        public static TcpClient GetTcpClient(string host, int port)
        {
            var client = new TcpClient();
            if (DnsProvider.Instance != null)
            {
                var entry = DnsProvider.Instance.Resolve(host);
                client.Connect(entry.AddressList[0], port);
            }
            else
                client.Connect(host, port);

            return client;
        }
    }
}