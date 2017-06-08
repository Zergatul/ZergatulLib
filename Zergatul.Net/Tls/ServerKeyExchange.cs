using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Net.Tls.CipherSuites;
using Zergatul.Net.Tls.Extensions;

namespace Zergatul.Net.Tls
{
    internal class ServerKeyExchange : HandshakeBody
    {
        private CipherSuite _cipher;

        public ServerDHParams Params;
        public ServerECDHParams ECParams;

        public SignatureAndHashAlgorithm SignAndHashAlgo;
        public byte[] Signature;

        public override ushort Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override bool Encrypted => false;

        public ServerKeyExchange(CipherSuite cipher)
        {
            this._cipher = cipher;
        }

        public override void Read(BinaryReader reader)
        {
            _cipher.ReadServerKeyExchange(this, reader);
        }

        public override void WriteTo(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
