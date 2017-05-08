using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class ClientKeyExchange : HandshakeBody
    {
        private TlsStream _stream;

        public ClientDiffieHellmanPublic DHPublic;

        public override ushort Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ClientKeyExchange(TlsStream stream)
        {

        }

        public override void Read(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteTo(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
