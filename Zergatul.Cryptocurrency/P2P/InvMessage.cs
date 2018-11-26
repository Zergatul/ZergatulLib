using System;
using System.Collections.Generic;

namespace Zergatul.Cryptocurrency.P2P
{
    public class InvMessage : Message
    {
        public override string Command => "inv";

        public InvVect[] Inventory;

        protected override void SerializePayload(List<byte> buffer)
        {
            buffer.AddRange(VarLengthInt.Serialize(Inventory.Length));
            foreach (var inv in Inventory)
            {
                buffer.AddRange(BitHelper.GetBytes((int)inv.Type, ByteOrder.LittleEndian));
                buffer.AddRange(inv.Hash);
            }
        }

        public override void DeserializePayload(byte[] buffer)
        {
            int index = 0;
            Inventory = InvVect.Parse(buffer, ref index);

            if (buffer.Length != index)
                throw new InvalidOperationException();
        }
    }
}