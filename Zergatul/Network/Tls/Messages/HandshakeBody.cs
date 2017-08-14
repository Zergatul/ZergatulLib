using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Messages
{
    internal abstract class HandshakeBody
    {
        public HandshakeType Type { get; private set; }

        public HandshakeBody(HandshakeType type)
        {
            this.Type = type;
        }

        public abstract void Read(BinaryReader reader);
        public abstract void WriteTo(BinaryWriter writer);
    }
}