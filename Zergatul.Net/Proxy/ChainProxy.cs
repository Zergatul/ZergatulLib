using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Proxy
{
    public class ChainProxy : ProxyBase
    {
        private ProxyBase[] _proxies;

        public override bool AllowConnectionsByDomainName
        {
            get
            {
                return _proxies.Last().AllowConnectionsByDomainName;
            }
        }

        public override bool AllowListener
        {
            get { return false; }
        }

        public ChainProxy(params ProxyBase[] proxies)
            : base()
        {
            if (proxies.Length == 0)
                throw new ArgumentException();
            this._proxies = proxies;
        }

        public override TcpClient CreateConnection(IPAddress address, int port, TcpClient tcp)
        {
            if (_proxies.Length == 1)
                return _proxies.First().CreateConnection(address, port, tcp);

            for (int i = 0; i < _proxies.Length - 1; i++)
            {
                if (_proxies[i + 1].ServerAddress != null)
                    tcp = _proxies[i].CreateConnection(_proxies[i + 1].ServerAddress, _proxies[i + 1].ServerPort, tcp);
                else
                    tcp = _proxies[i].CreateConnection(_proxies[i + 1].ServerHostName, _proxies[i + 1].ServerPort, tcp);
            }

            return _proxies.Last().CreateConnection(address, port, tcp);
        }

        public override TcpClient CreateConnection(string hostname, int port, TcpClient tcp)
        {
            throw new NotImplementedException();
        }

        public override TcpListener CreateListener(int port)
        {
            throw new NotImplementedException();
        }
    }
}