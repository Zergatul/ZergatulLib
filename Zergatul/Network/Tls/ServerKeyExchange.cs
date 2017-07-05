using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Tls.CipherSuites;
using Zergatul.Network.Tls.Extensions;

namespace Zergatul.Network.Tls
{
    internal class ServerKeyExchange : HandshakeBody
    {
        public ServerDHParams Params;
        public ServerECDHParams ECParams;

        public SignatureAndHashAlgorithm SignAndHashAlgo;
        public byte[] Signature;

        private AbstractCipherSuite _cipher;

        public ServerKeyExchange(AbstractCipherSuite cipher)
            : base(HandshakeType.ServerKeyExchange)
        {
            this._cipher = cipher;
        }

        public override void Read(BinaryReader reader)
        {
            _cipher.ReadServerKeyExchange(this, reader);
        }

        public override void WriteTo(BinaryWriter writer)
        {
            _cipher.WriteServerKeyExchange(this, writer);
        }
    }
}
