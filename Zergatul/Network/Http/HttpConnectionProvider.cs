using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Http
{
    public abstract class HttpConnectionProvider
    {
        public abstract Http1Connection GetHttp1Connection(Uri uri, Proxy.ProxyBase proxy);
    }
}