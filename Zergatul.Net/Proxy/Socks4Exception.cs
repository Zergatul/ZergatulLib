using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Proxy
{
    public class Socks4Exception : Exception
    {
        public Socks4Exception(string message)
            : base(message)
        {
        }
    }
}