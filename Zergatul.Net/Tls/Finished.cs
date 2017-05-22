using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class Finished : HandshakeBody
    {
        public override ushort Length => (ushort)Data.Length;

        public byte[] Data;

        public override void Read(BinaryReader reader)
        {
            Data = reader.ReadToEnd();
        }

        public override void WriteTo(BinaryWriter writer)
        {
            writer.WriteBytes(Data);
        }
    }
}
