using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class ApplicationData : ContentMessage
    {
        public byte[] Data;

        public override void Read(BinaryReader reader)
        {
            Data = reader.ReadToEnd();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.WriteBytes(Data);
        }
    }
}