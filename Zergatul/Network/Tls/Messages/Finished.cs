using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Messages
{
    internal class Finished : HandshakeBody
    {
        public byte[] Data;

        public Finished() : base(HandshakeType.Finished) { }

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
