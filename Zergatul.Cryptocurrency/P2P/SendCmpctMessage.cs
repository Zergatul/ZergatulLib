using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.P2P
{
    class SendCmpctMessage : Message
    {
        public override string Command => "sendcmpct";

        public bool Announce;
        public ulong Version;

        public override void DeserializePayload(byte[] buffer)
        {
            if (buffer.Length != 9)
                throw new InvalidOperationException();

            switch (buffer[0])
            {
                case 0: Announce = false; break;
                case 1: Announce = true; break;
                default:
                    throw new InvalidOperationException();
            }

            Version = BitHelper.ToUInt64(buffer, 1, ByteOrder.LittleEndian);
        }

        protected override void SerializePayload(List<byte> buffer)
        {
            throw new NotImplementedException();
        }
    }
}