using System.IO;

namespace Zergatul.Network.Smtp
{
    public class SmtpControlStreamReader : ControlStreamReader
    {
        public SmtpControlStreamReader(Stream stream)
            : base(stream)
        {

        }

        public SmtpServerReply ReadServerReply()
        {
            var reply = ReadServerReplyRaw();
            return new SmtpServerReply((SmtpReplyCode)reply.Code, reply.Raw, reply.Parsed);
        }
    }
}