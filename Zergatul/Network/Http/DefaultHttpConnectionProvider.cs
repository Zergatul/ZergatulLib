using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Proxy;

namespace Zergatul.Network.Http
{
    public class DefaultHttpConnectionProvider : HttpConnectionProvider
    {
        public override Http1Connection GetHttp1Connection(Uri uri, ProxyBase proxy)
        {
            throw new NotImplementedException();
        }

        public override Http2Connection GetHttp2Connection(Uri uri, ProxyBase proxy)
        {
            throw new NotImplementedException();
        }
    }
}