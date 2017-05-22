using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class ReadDataEventArgs : EventArgs
    {
        public byte[] Data;
    }
}
