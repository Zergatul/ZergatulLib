using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.P2P
{
    class SendHeadersMessage : Message
    {
        public override string Command => "sendheaders";

        public override void DeserializePayload(byte[] buffer)
        {
            
        }

        protected override void SerializePayload(List<byte> buffer)
        {
            
        }
    }
}