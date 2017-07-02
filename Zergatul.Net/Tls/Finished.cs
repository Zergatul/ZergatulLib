using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class Finished : HandshakeBody
    {
        public ByteArray Data;

        public Finished() : base(HandshakeType.Finished) { }

        public override void Read(BinaryReader reader)
        {
            Data = new ByteArray(reader.ReadToEnd());
        }

        public override void WriteTo(BinaryWriter writer)
        {
            writer.WriteBytes(Data);
        }
    }
}
