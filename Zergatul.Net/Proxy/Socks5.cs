using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Proxy
{
    // RFCs
    // SOCKS Protocol Version 5 https://tools.ietf.org/html/rfc1928
    // Username/Password Authentication for SOCKS V5 https://tools.ietf.org/html/rfc1929

    public class Socks5 : ProxyBase
    {
        private const byte Version = 5;

        private static class AuthenticationMethod
        {
            public const byte NoAuthentication = 0;
            public const byte GSSAPI = 1;
            public const byte UsernamePassword = 2;
            public const byte NoAcceptableMethods = 0xFF;
        }

        private static class Command
        {
            public const byte Connect = 1;
            public const byte Bind = 2;
            public const byte UdpAssociate = 3;
        }

        private static class AddressType
        {
            public const byte IPv4 = 1;
            public const byte DomainName = 3;
            public const byte IPv6 = 4;
        }

        public static class Reply
        {
            public const byte Succeeded = 0;
            public const byte ServerFailure = 1;
            public const byte ConnectionNotAllowedByRuleset = 2;
            public const byte NetworkUnreachable = 3;
            public const byte HostUnreachable = 4;
            public const byte ConnectionRefused = 5;
            public const byte TTLExpired = 6;
            public const byte CommandNotSupported = 7;
            public const byte AddressTypeNotSupported = 8;
        }

        public override bool AllowConnectionsByDomainName { get { return true; } }

        public Socks5(IPAddress address, int port)
            : base(address, port)
        {
        }

        public Socks5(string hostname, int port)
            : base(hostname, port)
        {
        }

        public override TcpClient CreateConnection(IPAddress address, int port, TcpClient tcp)
        {
            tcp = ConnectToServer(tcp);

            Greeting(tcp.GetStream());
            Connect(tcp.GetStream(), address, port);
            return tcp;
        }

        public override TcpClient CreateConnection(string hostname, int port, TcpClient tcp = null)
        {
            tcp = ConnectToServer(tcp);

            Greeting(tcp.GetStream());
            Connect(tcp.GetStream(), hostname, port);
            return tcp;
        }

        private void Greeting(NetworkStream stream)
        {
            byte[] packet = new byte[3];
            packet[0] = Version;
            packet[1] = 1;
            packet[2] = AuthenticationMethod.NoAuthentication;
            stream.Write(packet, 0, packet.Length);

            byte[] response = new byte[2];
            stream.Read(response, 0, response.Length);
            if (response[0] != Version)
                throw new Socks5Exception("Server response: invalid socks version");
            if (response[1] == AuthenticationMethod.NoAcceptableMethods)
                throw new Socks5Exception("Server response: client didn't offer acceptable authentication methods");
        }

        private void Connect(NetworkStream stream, IPAddress address, int port)
        {
            byte[] packet = new byte[10];
            packet[0] = Version;
            packet[1] = Command.Connect;
            packet[2] = 0;
            packet[3] = AddressType.IPv4;
            Array.Copy(address.GetAddressBytes(), 0, packet, 4, 4);
            packet[8] = (byte)(port / 256);
            packet[9] = (byte)(port % 256);
            stream.Write(packet, 0, packet.Length);

            ReadConnectResponse(stream);
        }

        private void Connect(NetworkStream stream, string domainName, int port)
        {
            byte[] packet = new byte[5 + domainName.Length + 2];
            packet[0] = Version;
            packet[1] = Command.Connect;
            packet[2] = 0;
            packet[3] = AddressType.DomainName;
            packet[4] = (byte)domainName.Length;
            var domainNameBytes = Encoding.ASCII.GetBytes(domainName);
            Array.Copy(domainNameBytes, 0, packet, 5, domainName.Length);
            packet[packet.Length - 2] = (byte)(port / 256);
            packet[packet.Length - 1] = (byte)(port % 256);
            stream.Write(packet, 0, packet.Length);

            ReadConnectResponse(stream);
        }

        private void ReadConnectResponse(NetworkStream stream)
        {
            byte[] response = new byte[256];
            int totalRead = 0;
            while (totalRead < 4)
            {
                int bytesRead = stream.Read(response, totalRead, 4 - totalRead);
                totalRead += bytesRead;
            }
            if (response[0] != Version)
                throw new Socks5Exception("Server response: invalid socks version");
            if (response[1] != Reply.Succeeded)
                throw new Socks5Exception("Server response: reply failed");
            if (response[2] != 0)
                throw new Socks5Exception("Server response: invalid reserved field");
            switch (response[3])
            {
                case AddressType.IPv4:
                    while (totalRead < 8)
                    {
                        int bytesRead = stream.Read(response, totalRead, 8 - totalRead);
                        totalRead += bytesRead;
                    }
                    break;
                case AddressType.DomainName:
                    while (totalRead < 5)
                    {
                        int bytesRead = stream.Read(response, totalRead, 5 - totalRead);
                        totalRead += bytesRead;
                    }
                    while (totalRead < 5 + response[4])
                    {
                        int bytesRead = stream.Read(response, totalRead, 5 + response[4] - totalRead);
                        totalRead += bytesRead;
                    }
                    break;
                case AddressType.IPv6:
                    throw new NotImplementedException();
                default:
                    throw new Socks5Exception("Server response: invalid address type");
            }
            int responseLength = totalRead + 2;
            while (totalRead < responseLength)
            {
                int bytesRead = stream.Read(response, totalRead, responseLength - totalRead);
                totalRead += bytesRead;
            }
        }

        public override TcpListener CreateListener(int port)
        {
            throw new NotImplementedException();
        }
    }
}