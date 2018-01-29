using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.P2P
{
    public class PongMessage : Message
    {
        public override string Command => "pong";

        public ulong Nonce;

        protected override void SerializePayload(List<byte> buffer)
        {
            buffer.AddRange(BitHelper.GetBytes(Nonce, ByteOrder.LittleEndian));
        }

        public override void DeserializePayload(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}