using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Ftp
{
    public class FtpServerFeaturesReply : FtpServerReply
    {
        public FtpServerFeaturesReply(FtpReplyCode code, string message)
            : base(code, message)
        {

        }

        public string[] Features;
    }
}