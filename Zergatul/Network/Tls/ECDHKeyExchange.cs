using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal class ECDHKeyExchange : AbstractTlsKeyExchange
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
            var ecdhServer = (ECPDiffieHellman)algo.ToKeyExchange();

            var ecdhClient = new ECPDiffieHellman();
            ecdhClient.Random = Random;
            ecdhClient.Parameters = ecdhServer.Parameters;
            ecdhClient.GenerateKeyPair(0);

            message.ECDH_Yc = ecdhClient.PublicKey.Point.ToUncompressed();

            var secret = ecdhClient.CalculateSharedSecret(ecdhServer.PublicKey);
            PreMasterSecret = ByteArray.SubArray(secret, 1, secret.Length - 1);

            return message;
        }

        public override ClientKeyExchange ReadClientKeyExchange(BinaryReader reader)
        {
            var message = new ClientKeyExchange();
            message.ECDH_Yc = reader.ReadBytes(reader.ReadByte());

            var algo = SecurityParameters.ServerCertificate.PrivateKey.ResolveAlgorithm();
            var ecdh = (ECPDiffieHellman)algo.ToKeyExchange();

            var secret = ecdh.CalculateSharedSecret(new ECPPublicKey(ECPointGeneric.Parse(message.ECDH_Yc, ecdh.Parameters.Curve).PFECPoint));
            PreMasterSecret = ByteArray.SubArray(secret, 1, secret.Length - 1);

            return message;
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            writer.WriteByte((byte)message.ECDH_Yc.Length);
            writer.WriteBytes(message.ECDH_Yc);
        }

        #endregion
    }
}