using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Network.Tls.Messages;
using Zergatul.Security;
using Zergatul.Security.Tls;

namespace Zergatul.Network.Tls
{
    internal abstract class AbstractTlsKeyExchange
    {
        public SecureRandom Random { get; private set; }
        public byte[] PreMasterSecret;
        public SecurityParameters SecurityParameters;
        public TlsStreamParameters Parameters;

        public abstract MessageInfo ServerCertificateMessage { get; }
        public abstract MessageInfo ServerKeyExchangeMessage { get; }
        public abstract MessageInfo CertificateRequestMessage { get; }
        public abstract MessageInfo ClientCertificateMessage { get; }
        public abstract MessageInfo CertificateverifyMessage { get; }

        public virtual void SetRandom(SecureRandom random)
        {
            this.Random = random;
        }

        public virtual bool ShouldSendServerKeyExchange()
        {
            if (ServerKeyExchangeMessage == MessageInfo.Forbidden)
                return false;
            if (ServerKeyExchangeMessage == MessageInfo.Required)
                return true;
            throw new InvalidOperationException();
        }

        public abstract ServerKeyExchange GenerateServerKeyExchange();
        public abstract ServerKeyExchange ReadServerKeyExchange(BinaryReader reader);
        public abstract void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer);

        public abstract ClientKeyExchange GenerateClientKeyExchange();
        public abstract ClientKeyExchange ReadClientKeyExchange(BinaryReader reader);
        public abstract void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer);
    }
}