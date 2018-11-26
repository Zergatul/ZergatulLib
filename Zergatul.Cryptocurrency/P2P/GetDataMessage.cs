using System;
using System.Collections.Generic;
using System.Linq;

namespace Zergatul.Cryptocurrency.P2P
{
    public class GetDataMessage : Message
    {
        public override string Command => "getdata";

        public InvVect[] Inventory;

        public override void DeserializePayload(byte[] buffer)
        {
            int index = 0;
            Inventory = InvVect.Parse(buffer, ref index);

            if (buffer.Length != index)
                throw new InvalidOperationException();
        }

        protected override void SerializePayload(List<byte> buffer)
        {
            buffer.AddRange(VarLengthInt.Serialize(Inventory.Length));

            for (int i = 0; i < Inventory.Length; i++)
            {
                buffer.AddRange(BitHelper.GetBytes((uint)Inventory[0].Type, ByteOrder.LittleEndian));
                buffer.AddRange(Inventory[0].Hash.Reverse());
            }
        }
    }
}