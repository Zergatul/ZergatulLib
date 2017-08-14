using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Messages
{
    internal class ChangeCipherSpec : ContentMessage
    {
        public override void Read(BinaryReader reader)
        {
            if (reader.ReadByte() != 1)
                throw new TlsStreamException("Invalid ChangeCipherSpec message");
        }

        public override void Write(BinaryWriter writer)
        {
            writer.WriteByte(1);
        }
    }
}