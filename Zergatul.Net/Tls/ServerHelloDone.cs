using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class ServerHelloDone : HandshakeBody
    {
        public override ushort Length => 0;
        public override bool Encrypted => false;

        public override void Read(BinaryReader reader)
        {

        }

        public override void WriteTo(BinaryWriter writer)
        {

        }
    }
}
