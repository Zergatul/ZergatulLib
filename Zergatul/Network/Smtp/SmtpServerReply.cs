using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Smtp
{
    public class SmtpServerReply
    {
        public SmtpReplyCode Code;
        public string RawMessage;
        public string Message;

        public SmtpServerReply(SmtpReplyCode code, string raw, string message)
        {
            this.Code = code;
            this.RawMessage = raw;
            this.Message = message;
        }
    }
}