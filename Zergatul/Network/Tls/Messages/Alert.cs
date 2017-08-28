using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Messages
{
    internal class Alert : ContentMessage
    {
        public AlertLevel Level;
        public AlertDescription Description;

        public override void Read(BinaryReader reader)
        {
            Level = (AlertLevel)reader.ReadByte();
            Description = (AlertDescription)reader.ReadByte();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.WriteByte((byte)Level);
            writer.WriteByte((byte)Description);
        }
    }
}
