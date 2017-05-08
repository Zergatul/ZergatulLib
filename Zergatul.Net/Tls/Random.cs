using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class Random
    {
        public uint GMTUnixTime;
        public byte[] RandomBytes;

        public void WriteTo(BinaryWriter writer)
        {
            writer.WriteUInt32(GMTUnixTime);
            writer.WriteBytes(RandomBytes);
        }

        public byte[] ToArray()
        {
            var list = new List<byte>();
            var writer = new BinaryWriter(list);
            writer.WriteUInt32(GMTUnixTime);
            writer.WriteBytes(RandomBytes);

            return list.ToArray();
        }
    }
}
