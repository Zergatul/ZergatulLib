using System;
using System.Collections.Generic;

namespace Zergatul.Cryptocurrency.P2P
{
    class TxMessage : Message
    {
        public override string Command => "tx";

        public byte[] Raw;

        public override void DeserializePayload(byte[] buffer)
        {
            Raw = buffer;
        }

        protected override void SerializePayload(List<byte> buffer)
        {
            buffer.AddRange(Raw);
        }
    }
}