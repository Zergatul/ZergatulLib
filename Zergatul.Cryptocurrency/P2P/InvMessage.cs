using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.P2P
{
    public class InvMessage : Message
    {
        public override string Command => "inv";

        public InvVect[] Inventory;

        protected override void SerializePayload(List<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override void DeserializePayload(byte[] buffer)
        {
            int index = 0;

            int count = checked((int)VarLengthInt.Parse(buffer, ref index));
            Inventory = new InvVect[count];
            for (int i = 0; i < count; i++)
            {
                Inventory[i] = new InvVect();

                Inventory[i].Type = (VectorType)BitHelper.ToUInt32(buffer, index, ByteOrder.LittleEndian);
                index += 4;

                Inventory[i].Hash = ByteArray.SubArray(buffer, index, 32);
                index += 32;
            }
        }
    }
}