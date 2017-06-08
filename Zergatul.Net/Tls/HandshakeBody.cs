using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal abstract class HandshakeBody
    {
        public abstract ushort Length { get; }
        public abstract bool Encrypted { get; }
        public abstract void Read(BinaryReader reader);
        public abstract void WriteTo(BinaryWriter writer);
    }
}
