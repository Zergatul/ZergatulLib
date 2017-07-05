using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Ftp
{
    public class FtpServerException : Exception
    {
        public FtpReplyCode Code { get; private set; }

        public FtpServerException(string message)
            : base(message)
        {
        }

        public FtpServerException(string message, FtpReplyCode code)
            : base(message)
        {
            this.Code = code;
        }
    }
}