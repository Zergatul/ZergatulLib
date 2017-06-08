using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class ClientKeyExchange : HandshakeBody
    {
        public ClientDiffieHellmanPublic DHPublic;

        public override ushort Length
        {
            get
            {
                if (DHPublic != null)
                    return (ushort)(2 + DHPublic.DH_Yc.Length);
                throw new NotImplementedException();
            }
        }
        public override bool Encrypted => false;

        public override void Read(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteTo(BinaryWriter writer)
        {
            if (DHPublic != null)
            {
                writer.WriteShort((ushort)DHPublic.DH_Yc.Length);
                writer.WriteBytes(DHPublic.DH_Yc);
                return;
            }
            throw new NotImplementedException();
        }
    }
}
