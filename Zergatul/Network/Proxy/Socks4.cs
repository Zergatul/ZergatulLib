using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Proxy
{
    // http://ftp.icm.edu.pl/packages/socks/socks4/SOCKS4.protocol

    public class Socks4 : ProxyBase
    {
        private const byte Version = 4;

        private static class Command
        {
            public const byte Connect = 0x01;
            public const byte Bind = 0x02;
        }

        private static class Reply
        {
            public const byte RequestGranted = 0x5a;
            public const byte RequestRejectedOrFailed = 0x5b;
            public const byte ClientNotIdent = 0x5c;
            public const byte ClientIdentUserIDFailed = 0x5d;
        }

        public override bool AllowConnectionsByDomainName { get { return false; } }
        public override bool AllowListener { get { return true; } }

        public Socks4(IPAddress address, int port)
            : base(address, port)
        {
        }

        public Socks4(string hostname, int port)
            : base(hostname, port)
        {
        }

        public override TcpClient CreateConnection(IPAddress address, int port, TcpClient tcp)
        {
            tcp = ConnectToServer(tcp);

            SendCommand(tcp.GetStream(), Command.Connect, address.GetAddressBytes(), port);

            return tcp;
        }

        public override TcpClient CreateConnection(string hostname, int port, TcpClient tcp = null)
        {
            if (ResolveDnsLocally)
            {
                var addresses = Dns.GetHostAddresses(hostname);
                return CreateConnection(addresses[0], port, tcp);
            }

            throw new NotSupportedByProtocolException("Proxy connection by host name is not supported by SOCKS4");
        }

        public override TcpListener CreateListener(int port)
        {
            throw new NotImplementedException();
        }

        void SendCommand(NetworkStream stream, byte command, byte[] host, int port)
        {
            byte[] request = new byte[9];
            request[0] = Version;
            request[1] = command;
            request[2] = (byte)(port / 256);
            request[3] = (byte)(port % 256);
            host.CopyTo(request, 4);
            request[8] = 0;
            stream.Write(request, 0, request.Length);

            byte[] response = new byte[8];
            stream.Read(response, 0, response.Length);
            // check null byte
            if (response[0] != 0)
                throw new Socks4Exception("Server response: invalid null byte");
            switch (response[1])
            {
                case Reply.RequestGranted:
                    return;
                case Reply.RequestRejectedOrFailed:
                    throw new Socks4Exception("Server response: request rejected or failed");
                case Reply.ClientNotIdent:
                    throw new Socks4Exception("Server response: request failed because client is not running identd (or not reachable from the server)");
                case Reply.ClientIdentUserIDFailed:
                    throw new Socks4Exception("request failed because client's identd could not confirm the user ID string in the request");
                default:
                    throw new Socks4Exception("Server response: unknown reply");
            }
        }
    }
}