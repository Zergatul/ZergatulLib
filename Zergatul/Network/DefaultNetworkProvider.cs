﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Zergatul.Network.Proxy;

namespace Zergatul.Network
{
    public class DefaultNetworkProvider : NetworkProvider
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

        public override async Task<Stream> GetTcpStreamAsync(string host, int port, ProxyBase proxy)
        {
            if (proxy != null)
                throw new NotImplementedException();

            var addresses = await DnsResolveAsync(host);
            if (addresses.Length == 0)
                throw new InvalidOperationException();

            var address = addresses[0];
            var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await SocketHelper.ConnectAsync(socket, new IPEndPoint(address, port));

            return new NetworkStream(socket, true);
        }

        public override IPAddress[] DnsResolve(string host) => Dns.GetHostAddresses(host);

        public override Task<IPAddress[]> DnsResolveAsync(string host) => Dns.GetHostAddressesAsync(host);
    }
}