using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Math;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal class DHKeyExchange : AbstractTlsKeyExchange
    {
        public override MessageInfo ServerCertificateMessage => MessageInfo.Required;
        public override MessageInfo ServerKeyExchangeMessage => MessageInfo.Forbidden;
        public override MessageInfo CertificateRequestMessage
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override MessageInfo ClientCertificateMessage
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override MessageInfo CertificateverifyMessage
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #region ServerKeyExchange

        public override ServerKeyExchange GenerateServerKeyExchange()
        {
            throw new InvalidOperationException();
        }

        public override ServerKeyExchange ReadServerKeyExchange(BinaryReader reader)
        {
            throw new InvalidOperationException();
        }

        public override void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer)
        {
            throw new InvalidOperationException();
        }

        #endregion

        #region ClientKeyExchange

        public override ClientKeyExchange GenerateClientKeyExchange()
        {
            var message = new ClientKeyExchange();

            var algo = SecurityParameters.ServerCertificate.PublicKey.ResolveAlgorithm();
            var dhServer = algo.ToKeyExchange() as DiffieHellman;
            if (dhServer == null)
                new TlsStreamException("For DH key exchange certificate must also use DH");

            var dhClient = new DiffieHellman();
            dhClient.Random = Random;
            dhClient.Parameters = dhServer.Parameters;
            dhClient.GenerateKeyPair(0);

            message.DH_Yc = dhClient.PublicKey.Value_Raw;

            PreMasterSecret = dhClient.CalculateSharedSecret(dhServer.PublicKey);

            return message;
        }

        public override ClientKeyExchange ReadClientKeyExchange(BinaryReader reader)
        {
            var message = new ClientKeyExchange();
            message.DH_Yc = reader.ReadBytes(reader.ReadShort());

            var algo = SecurityParameters.ServerCertificate.PrivateKey.ResolveAlgorithm();
            var dh = algo.ToKeyExchange() as DiffieHellman;
            if (dh == null)
                new TlsStreamException("For DH key exchange certificate must also use DH");

            PreMasterSecret = dh.CalculateSharedSecret(new DiffieHellmanPublicKey(message.DH_Yc));

            return message;
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            writer.WriteShort((ushort)message.DH_Yc.Length);
            writer.WriteBytes(message.DH_Yc);
        }

        #endregion
    }
}