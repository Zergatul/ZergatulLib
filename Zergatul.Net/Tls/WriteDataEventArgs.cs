using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class WriteDataEventArgs : EventArgs
    {
        public byte[] Data;
    }
}
