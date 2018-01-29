using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.Cryptocurrency.P2P
{
    public class VersionMessage : Message
    {
        public override string Command => "version";

        public int Version;
        public ulong Services;
        public ulong Timestamp;
        public ShortNetworkAddress AddrRecv;
        public ShortNetworkAddress AddrFrom;
        public ulong Nonce;
        public string UserAgent;
        public int StartHeight;
        public bool Relay;

        protected override void SerializePayload(List<byte> buffer)
        {
            buffer.AddRange(BitHelper.GetBytes(Version, ByteOrder.LittleEndian));
            buffer.AddRange(BitHelper.GetBytes(Services, ByteOrder.LittleEndian));
            buffer.AddRange(BitHelper.GetBytes(Timestamp, ByteOrder.LittleEndian));
            AddrRecv.Serialize(buffer);
            AddrFrom.Serialize(buffer);
            buffer.AddRange(BitHelper.GetBytes(Nonce, ByteOrder.LittleEndian));
            buffer.AddRange(VarLengthInt.Serialize(UserAgent.Length));
            buffer.AddRange(Encoding.ASCII.GetBytes(UserAgent));
            buffer.AddRange(BitHelper.GetBytes(StartHeight, ByteOrder.LittleEndian));
            buffer.Add((byte)(Relay ? 1 : 0));
        }

        public override void DeserializePayload(byte[] buffer)
        {
            int index = 0;

            Version = BitHelper.ToInt32(buffer, index, ByteOrder.LittleEndian);
            index += 4;

            Services = BitHelper.ToUInt64(buffer, index, ByteOrder.LittleEndian);
            index += 8;

            Timestamp = BitHelper.ToUInt64(buffer, index, ByteOrder.LittleEndian);
            index += 8;

            AddrRecv = new ShortNetworkAddress();
            AddrRecv.Deserialize(buffer, ref index);

            AddrFrom = new ShortNetworkAddress();
            AddrFrom.Deserialize(buffer, ref index);

            Nonce = BitHelper.ToUInt64(buffer, index, ByteOrder.LittleEndian);
            index += 8;

            int length = checked((int)VarLengthInt.Parse(buffer, ref index));
            UserAgent = Encoding.ASCII.GetString(buffer, index, length);
            index += length;

            StartHeight = BitHelper.ToInt32(buffer, index, ByteOrder.LittleEndian);
            index += 4;

            Relay = buffer[index] == 1;
            index += 1;

            if (index != buffer.Length)
                throw new InvalidOperationException();
        }
    }
}