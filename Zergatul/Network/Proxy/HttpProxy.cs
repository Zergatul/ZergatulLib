using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Proxy
{
    public class HttpProxy : ProxyBase
    {
        public override bool AllowConnectionsByDomainName { get { return true; } }
        public override bool AllowListener { get { return false; } }

        public HttpProxy(IPAddress address, int port)
            : base(address, port)
        {
        }

        public HttpProxy(string hostname, int port)
            : base(hostname, port)
        {
        }

        public override TcpClient CreateConnection(IPAddress address, int port, TcpClient tcp)
        {
            tcp = ConnectToServer(tcp);

            var stream = tcp.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(string.Format("CONNECT {0}:{1}  HTTP/1.1{2}Host: {0}{2}{2}", address, port, Constants.TelnetEndOfLine));
            stream.Write(data, 0, data.Length);

            return tcp;
        }

        public override TcpClient CreateConnection(string hostname, int port, TcpClient tcp)
        {
            tcp = ConnectToServer(tcp);

            var stream = tcp.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(string.Format("CONNECT {0}:{1}  HTTP/1.1{2}Host: {0}{2}{2}", hostname, port, Constants.TelnetEndOfLine));
            stream.Write(data, 0, data.Length);

            byte[] buffer = new byte[1024];
            List<byte> response = new List<byte>();
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, 1024);
                for (int i = 0; i < bytesRead; i++)
                    response.Add(buffer[i]);
                if (bytesRead < 1024)
                    break;
            }
            string str = Encoding.ASCII.GetString(response.ToArray());

            return tcp;
        }

        public override TcpListener CreateListener(int port)
        {
            throw new NotImplementedException();
        }
    }
}