using System.IO;

namespace Zergatul.Network.Ftp
{
    public class FtpControlStreamReader : ControlStreamReader
    {
        public FtpControlStreamReader(Stream stream)
            : base(stream)
        {

        }

        public FtpServerReply ReadServerReply()
        {
            var reply = ReadServerReplyRaw();
            return new FtpServerReply((FtpReplyCode)reply.Code, reply.Raw);
        }
    }
}