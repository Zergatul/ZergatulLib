using System;
using System.Net;
using System.Net.Sockets;

namespace Zergatul.Network
{
    public static class TcpClientHelper
    {
        public static void ConnectWithTimeout(this TcpClient client, string hostname, int port, int timeout)
        {
            if (timeout <= 0)
            {
                client.Connect(hostname, port);
            }
            else
            {
                var result = client.BeginConnect(hostname, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeout));
                if (!success)
                    throw new TimeoutException();
                client.EndConnect(result);
            }
        }

        public static void ConnectWithTimeout(this TcpClient client, IPAddress address, int port, int timeout)
        {
            if (timeout <= 0)
            {
                client.Connect(address, port);
            }
            else
            {
                var result = client.BeginConnect(address, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeout));
                if (!success)
                    throw new TimeoutException();
                client.EndConnect(result);
            }
        }
    }
}