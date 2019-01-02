using System;
using System.Collections.Generic;

namespace Zergatul.Cryptocurrency.P2P
{
    class FeeFilterMessage : Message
    {
        public override string Command => "feefilter";

        public ulong FeeRate;

        public override void DeserializePayload(byte[] buffer)
        {
            if (buffer.Length != 8)
                throw new InvalidOperationException();

            FeeRate = BitHelper.ToUInt64(buffer, 0, ByteOrder.LittleEndian);
        }

        protected override void SerializePayload(List<byte> buffer)
        {
            throw new NotImplementedException();
        }
    }
}