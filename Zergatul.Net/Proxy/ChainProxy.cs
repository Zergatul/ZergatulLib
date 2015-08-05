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

        public bool AllowConnectionsByDomainName
        {
            get
            {
                return _proxies.Last().AllowConnectionsByDomainName;
            }
        }

        public ChainProxy(params ProxyBase[] proxies)
        {
            if (proxies.Length == 0)
                throw new ArgumentException();
            this._proxies = proxies;
        }

        public TcpClient CreateConnection(IPAddress address, int port, TcpClient tcp = null)
        {
            if (tcp != null)
                throw new NotImplementedException();

            if (_proxies.Length == 1)
                return _proxies.First().CreateConnection(address, port);

            for (int i = 0; i < _proxies.Length - 1; i++)
                ;//tcp = _proxies[i].CreateConnection(_proxies[i + 1].
        }

        public TcpClient CreateConnection(string hostname, int port, TcpClient tcp = null)
        {
            throw new NotImplementedException();
        }

        public TcpListener CreateListener(int port)
        {
            throw new NotImplementedException();
        }
    }
}