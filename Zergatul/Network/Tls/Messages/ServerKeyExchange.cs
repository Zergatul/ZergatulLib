using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Tls.Extensions;

namespace Zergatul.Network.Tls.Messages
{
    internal class ServerKeyExchange : HandshakeBody
    {
        public ServerDHParams Params;
        public ServerECDHParams ECParams;

        public SignatureAndHashAlgorithm SignAndHashAlgo;
        public byte[] Signature;

        public ServerKeyExchange()
            : base(HandshakeType.ServerKeyExchange)
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