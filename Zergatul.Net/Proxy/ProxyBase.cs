using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Proxy
{
    public abstract class ProxyBase
    {
        public abstract bool AllowConnectionsByDomainName { get; }
        public virtual bool ResolveDnsLocally { get; set; }

        protected IPAddress _serverAddress;
        protected string _serverHostname;
        protected int _serverPort;

        public IPAddress ServerAddress { get { return _serverAddress; } }
        public string ServerHostName { get { return _serverHostname; } }
        public int ServerPort { get { return _serverPort; } }

        protected ProxyBase()
        {
            this.ResolveDnsLocally = false;
        }

        protected ProxyBase(IPAddress address, int port)
            : this()
        {
            this._serverAddress = address;
            this._serverPort = port;
        }

        protected ProxyBase(string hostname, int port)
            : this()
        {
            this._serverHostname = hostname;
            this._serverPort = port;
        }

        public abstract TcpClient CreateConnection(IPAddress address, int port, TcpClient tcp);

        public abstract TcpClient CreateConnection(string hostname, int port, TcpClient tcp);

        public virtual TcpClient CreateConnection(IPAddress address, int port)
        {
            return CreateConnection(address, port, null);
        }

        public virtual TcpClient CreateConnection(string hostname, int port)
        {
            return CreateConnection(hostname, port, null);
        }

        public abstract TcpListener CreateListener(int port);
    }
}