using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    class TlsStreamException : Exception
    {
        public TlsStreamException(string message) : base(message)
        {

        }
    }
}
