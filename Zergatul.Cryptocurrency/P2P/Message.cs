using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.P2P
{
    public abstract class Message
    {
        public abstract string Command { get; }
        protected abstract void SerializePayload(List<byte> buffer);
        public abstract void DeserializePayload(byte[] buffer);

        public void Serialize(List<byte> buffer, ProtocolSpecification spec)
        {
            if (Command.Length > 12)
                throw new InvalidOperationException();

            List<byte> plbuf = new List<byte>();
            SerializePayload(plbuf);
            byte[] payload = plbuf.ToArray();

            byte[] hash = DoubleSHA256.Hash(payload);

            buffer.AddRange(BitHelper.GetBytes(spec.Magic, ByteOrder.LittleEndian));
            buffer.AddRange(Encoding.ASCII.GetBytes(Command));
            for (int i = Command.Length; i < 12; i++)
                buffer.Add(0);
            buffer.AddRange(BitHelper.GetBytes(payload.Length, ByteOrder.LittleEndian));
            buffer.AddRange(hash.Take(4));
            buffer.AddRange(payload);
        }
    }
}