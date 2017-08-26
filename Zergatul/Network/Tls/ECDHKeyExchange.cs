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
            var ecdsa = algo as ECDSA;
            if (ecdsa == null)
                throw new TlsStreamException("For ECDH key exchange certificate must use ECDSA");
            var ecdhServer = ecdsa.ToECDH();

            var ecdhClient = new ECDiffieHellman();
            ecdhClient.Random = Random;
            ecdhClient.Parameters = ecdhServer.Parameters;
            ecdhClient.GenerateKeys();

            message.ECDH_Yc = ecdhClient.PublicKey.ToBytes();

            ecdhClient.KeyExchange.CalculateSharedSecret(ecdhServer.PublicKey);

            PreMasterSecret = ecdhClient.KeyExchange.SharedSecret.XToBytes();

            return message;
        }

        public override ClientKeyExchange ReadClientKeyExchange(BinaryReader reader)
        {
            var message = new ClientKeyExchange();
            message.ECDH_Yc = reader.ReadBytes(reader.ReadByte());

            var algo = SecurityParameters.ServerCertificate.PrivateKey.ResolveAlgorithm();
            var ecdsa = algo as ECDSA;
            if (ecdsa == null)
                new TlsStreamException("For ECDH key exchange certificate must use ECDSA");
            var ecdh = ecdsa.ToECDH();

            ecdh.KeyExchange.CalculateSharedSecret(ECPointGeneric.Parse(message.ECDH_Yc, ecdh.Parameters));

            PreMasterSecret = ecdh.KeyExchange.SharedSecret.XToBytes();

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