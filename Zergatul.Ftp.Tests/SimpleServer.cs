using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zergatul.Network;

namespace Zergatul.Ftp.Tests
{
    class SimpleServer
    {
        TcpListener _listener;
        Thread _thread;

        public Func<string, string> ReplyFunction;
        public string Greeting;

        public SimpleServer(AddressFamily af, int port)
        {
            if (af == AddressFamily.InterNetwork)
            {
                _listener = new TcpListener(IPAddress.Any, port);
            }
            if (af == AddressFamily.InterNetworkV6)
            {
                _listener = new TcpListener(IPAddress.IPv6Any, port);
                _listener.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0);
            }
        }

        public void Start()
        {
            _listener.Start();
            _thread = new Thread(ThreadFunc);
            _thread.Start();
        }

        public void Dispose()
        {
            _listener.Stop();
        }

        private void ThreadFunc()
        {
            var client = _listener.AcceptTcpClient();
            var stream = client.GetStream();
            WriteToStream(stream, (Greeting ?? "220 Zergatul.Ftp test server") + Constants.TelnetEndOfLine);
            while (true)
            {
                List<byte> bytes = new List<byte>();
                string cmd;
                while (true)
                {
                    int b = stream.ReadByte();
                    if (b == -1)
                        return;
                    bytes.Add((byte)b);
                    cmd = Encoding.ASCII.GetString(bytes.ToArray());
                    if (cmd.EndsWith(Constants.TelnetEndOfLine))
                        break;
                }
                cmd = cmd.Substring(0, cmd.Length - 2);
                if (cmd == "QUIT")
                {
                    WriteToStream(stream, "220 Quit" + Constants.TelnetEndOfLine);
                    client.Close();
                    break;
                }
                if (ReplyFunction != null)
                {
                    WriteToStream(stream, ReplyFunction(cmd) + Constants.TelnetEndOfLine);
                }
            }
        }

        private void WriteToStream(NetworkStream stream, string data)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}