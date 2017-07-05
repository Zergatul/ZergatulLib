using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    internal class ServerHelloDone : HandshakeBody
    {
        public ServerHelloDone() : base(HandshakeType.ServerHelloDone) { }

        public override void Read(BinaryReader reader)
        {

        }

        public override void WriteTo(BinaryWriter writer)
        {

        }
    }
}
