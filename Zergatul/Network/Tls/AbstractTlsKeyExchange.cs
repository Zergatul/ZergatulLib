using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal abstract class AbstractTlsKeyExchange
    {
        public ISecureRandom Random;
        public byte[] PreMasterSecret;
        public SecurityParameters SecurityParameters;
        public TlsStreamSettings Settings;

        public abstract bool ServerKeyExchangeRequired { get; }

        public abstract ServerKeyExchange GenerateServerKeyExchange();
        public abstract ServerKeyExchange ReadServerKeyExchange(BinaryReader reader);
        public abstract void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer);

        public abstract ClientKeyExchange GenerateClientKeyExchange();
        public abstract ClientKeyExchange ReadClientKeyExchange(BinaryReader reader);
        public abstract void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer);
    }
}