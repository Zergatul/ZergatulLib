using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.P2P
{
    public class HeadersMessage : Message
    {
        public override string Command => "headers";

        protected override void SerializePayload(List<byte> buffer)
        {
            buffer.Add(0);
        }

        public override void DeserializePayload(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}