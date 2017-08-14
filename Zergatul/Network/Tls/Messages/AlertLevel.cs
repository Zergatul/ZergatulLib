using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Messages
{
    internal enum AlertLevel : byte
    {
        Warning = 1,
        Fatal = 2
    }
}