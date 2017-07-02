using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Net.Tls.CipherSuites;

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

        private CipherSuite _cipher;

        public ClientKeyExchange(CipherSuite cipher)
        {
            this._cipher = cipher;
        }

        public override void Read(BinaryReader reader)
        {
            _cipher.ReadClientKeyExchange(this, reader);
        }

        public override void WriteTo(BinaryWriter writer)
        {
            _cipher.WriteClientKeyExchange(this, writer);
        }
    }
}
