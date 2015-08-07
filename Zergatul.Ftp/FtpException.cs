using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Ftp
{
    public class FtpException : Exception
    {
        public FtpReplyCode Code { get; private set; }

        public FtpException(string message)
            : base(message)
        {
        }

        public FtpException(string message, FtpReplyCode code)
            : base(message)
        {
            this.Code = code;
        }
    }
}