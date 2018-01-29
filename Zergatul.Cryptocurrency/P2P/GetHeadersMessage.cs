using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.P2P
{
    public class GetHeadersMessage : Message
    {
        public override string Command => "getheaders";

        public uint Version;
        public byte[][] BlockLocatorHashes;
        public byte[] HashStop;

        protected override void SerializePayload(List<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override void DeserializePayload(byte[] buffer)
        {
            int index = 0;

            Version = BitHelper.ToUInt32(buffer, index, ByteOrder.LittleEndian);
            index += 4;

            int count = checked((int)VarLengthInt.Parse(buffer, ref index));
            BlockLocatorHashes = new byte[count][];
            for (int i = 0; i < count; i++)
            {
                BlockLocatorHashes[i] = ByteArray.SubArray(buffer, index, 32);
                Array.Reverse(BlockLocatorHashes[i]);
                index += 32;
            }

            HashStop = ByteArray.SubArray(buffer, index, 32);
            index += 32;

            if (buffer.Length != index)
                throw new InvalidOperationException();
        }
    }
}