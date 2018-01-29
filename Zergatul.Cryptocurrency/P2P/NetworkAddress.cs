using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.P2P
{
    public class NetworkAddress : ShortNetworkAddress
    {
        public uint Time;

        public override void Serialize(List<byte> buffer)
        {
            buffer.AddRange(BitHelper.GetBytes(Time, ByteOrder.LittleEndian));

            base.Serialize(buffer);
        }

        public override void Deserialize(byte[] buffer, ref int index)
        {
            Time = BitHelper.ToUInt32(buffer, index, ByteOrder.LittleEndian);
            index += 4;

            base.Deserialize(buffer, ref index);
        }
    }
}