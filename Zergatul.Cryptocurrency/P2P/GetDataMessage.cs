using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.P2P
{
    public class GetDataMessage : Message
    {
        public override string Command => "getdata";

        public InvVect[] Inventory;

        protected override void SerializePayload(List<byte> buffer)
        {
            buffer.AddRange(VarLengthInt.Serialize(Inventory.Length));

            for (int i = 0; i < Inventory.Length; i++)
            {
                buffer.AddRange(BitHelper.GetBytes((uint)Inventory[0].Type, ByteOrder.LittleEndian));
                buffer.AddRange(Inventory[0].Hash.Reverse());
            }
        }

        public override void DeserializePayload(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}