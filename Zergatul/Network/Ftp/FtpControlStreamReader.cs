using System;
using System.Collections.Generic;
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

        public FtpServerReply ReadFeaturesReply()
        {
            var line = ReadLine();
            if (line.StartsWith("211-"))
            {
                var reply = new FtpServerFeaturesReply(FtpReplyCode.SystemStatus, line.Substring(4));
                var list = new List<string>();
                while (true)
                {
                    line = ReadLine();
                    if (line.StartsWith("211 "))
                    {
                        if (line.Equals("211 End", StringComparison.InvariantCultureIgnoreCase))
                            break;
                        else
                            throw new InvalidOperationException();
                    }

                    if (line[0] != ' ')
                        throw new InvalidOperationException();
                    list.Add(line.Substring(1));
                }
                reply.Features = list.ToArray();
                return reply;
            }
            else
                throw new NotImplementedException();
        }
    }
}