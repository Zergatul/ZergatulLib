using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Ftp
{
    public class FtpException : Exception
    {
        public FtpException(string message)
            : base(message)
        {
        }
    }
}