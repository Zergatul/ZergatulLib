using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.P2P
{
    public class PingMessage : Message
    {
        public override string Command => "ping";

        public ulong Nonce;

        protected override void SerializePayload(List<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override void DeserializePayload(byte[] buffer)
        {
            if (buffer.Length != 8)
                throw new InvalidOperationException();

            Nonce = BitHelper.ToUInt64(buffer, 0, ByteOrder.LittleEndian);
        }
    }
}