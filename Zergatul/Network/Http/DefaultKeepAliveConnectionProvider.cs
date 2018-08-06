using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Http
{
    public class DefaultKeepAliveConnectionProvider : KeepAliveConnectionProvider
    {
        public static KeepAliveConnectionProvider Instance { get; private set; } = new DefaultKeepAliveConnectionProvider();

        private DefaultKeepAliveConnectionProvider()
        {

        }

        public override KeepAliveConnection GetConnection(Uri uri)
        {
            throw new NotImplementedException();
        }
    }
}