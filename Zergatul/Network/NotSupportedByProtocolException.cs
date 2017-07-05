using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network
{
    public class NotSupportedByProtocolException : Exception
    {
        public NotSupportedByProtocolException(string message)
            : base(message)
        {
        }
    }
}