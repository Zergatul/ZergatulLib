using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Proxy
{
    public class HttpsProxy : HttpProxy
    {
        public HttpsProxy(IPAddress address, int port)
            : base(address, port)
        {
        }

        public HttpsProxy(string hostname, int port)
            : base(hostname, port)
        {
        }

        protected override Stream GetStream(TcpClient tcp)
        {
            var stream = base.GetStream(tcp);
            var ssl = new System.Net.Security.SslStream(stream, false, (a, b, c, d) => true);
            ssl.AuthenticateAsClient(_serverHostname ?? _serverAddress.ToString());
            return ssl;
        }
    }
}