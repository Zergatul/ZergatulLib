using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Http
{
    public class KeepAliveConnection
    {
        public Stream Stream { get; private set; }

        public void Close()
        {

        }
    }
}