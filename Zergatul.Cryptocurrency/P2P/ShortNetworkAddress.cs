using System;
using System.Collections.Generic;
using System.Net;

namespace Zergatul.Cryptocurrency.P2P
{
    public class ShortNetworkAddress
    {
        private static readonly byte[] IPv4Prefix = BitHelper.HexToBytes("00000000000000000000FFFF");

        public ulong Services;
        public IPAddress IP;
        public ushort Port;

        public virtual void Serialize(List<byte> buffer)
        {
            buffer.AddRange(BitHelper.GetBytes(Services, ByteOrder.LittleEndian));

            if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                buffer.AddRange(IPv4Prefix);
                byte[] addr = IP.GetAddressBytes();
                if (addr.Length != 4)
                    throw new InvalidOperationException();
                buffer.AddRange(addr);
            }
            else if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                byte[] addr = IP.GetAddressBytes();
                if (addr.Length != 16)
                    throw new InvalidOperationException();
                buffer.AddRange(addr);
            }
            else
                throw new InvalidOperationException();

            buffer.AddRange(BitHelper.GetBytes(Port, ByteOrder.BigEndian));
        }

        public virtual void Deserialize(byte[] buffer, ref int index)
        {
            Services = BitHelper.ToUInt64(buffer, index, ByteOrder.LittleEndian);
            index += 8;

            if (ByteArray.IsSubArray(buffer, IPv4Prefix, index))
            {
                IP = new IPAddress(ByteArray.SubArray(buffer, index + 12, 4));
            }
            else
            {
                IP = new IPAddress(ByteArray.SubArray(buffer, index, 16));
            }
            index += 16;

            Port = BitHelper.ToUInt16(buffer, index, ByteOrder.BigEndian);
            index += 2;
        }
    }
}