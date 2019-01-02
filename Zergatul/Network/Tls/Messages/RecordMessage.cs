using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stream = System.IO.Stream;

namespace Zergatul.Network.Tls.Messages
{
    internal class RecordMessage
    {
        public ContentType RecordType;
        public ProtocolVersion Version;

        public List<ContentMessage> ContentMessages;
        public List<byte> ContentMessagesRaw;
    }
}