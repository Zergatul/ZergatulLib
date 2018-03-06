using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Ftp
{
    public class FtpServerReply
    {
        public FtpReplyCode Code;
        public string Message;

        public FtpServerReply(FtpReplyCode code, string message)
        {
            this.Code = code;
            this.Message = message;
        }
    }
}