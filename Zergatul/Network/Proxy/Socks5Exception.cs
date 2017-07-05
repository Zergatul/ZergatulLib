using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Proxy
{
    public class Socks5Exception : Exception
    {
        public Socks5Exception(string message)
            : base(message)
        {
        }
    }
}