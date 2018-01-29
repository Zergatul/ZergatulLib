using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.P2P
{
    public class BlockMessage : Message
    {
        public override string Command => "block";

        public byte[] RawBlock;

        protected override void SerializePayload(List<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override void DeserializePayload(byte[] buffer)
        {
            RawBlock = buffer;
        }
    }
}