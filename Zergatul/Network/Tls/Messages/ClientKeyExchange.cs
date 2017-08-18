using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Messages
{
    internal class ClientKeyExchange : HandshakeBody
    {
        public byte[] DH_Yc;
        public byte[] ECDH_Yc;
        public byte[] EncryptedPreMasterSecret;

        public ClientKeyExchange()
            : base(HandshakeType.ClientKeyExchange)
        {
        }

        public override void Read(BinaryReader reader)
        {
            
        }

        public override void WriteTo(BinaryWriter writer)
        {
            
        }
    }
}