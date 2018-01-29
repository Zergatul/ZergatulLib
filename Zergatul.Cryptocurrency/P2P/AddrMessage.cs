using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.P2P
{
    public class AddrMessage : Message
    {
        public override string Command => "addr";

        public NetworkAddress[] Addresses;

        protected override void SerializePayload(List<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override void DeserializePayload(byte[] buffer)
        {
            int index = 0;
            int count = VarLengthInt.ParseInt32(buffer, ref index);

            Addresses = new NetworkAddress[count];
            for (int i = 0; i < count; i++)
            {
                Addresses[i] = new NetworkAddress();
                Addresses[i].Deserialize(buffer, ref index);
            }
        }
    }
}