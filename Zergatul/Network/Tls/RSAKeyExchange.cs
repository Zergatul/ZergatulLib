using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal class RSAKeyExchange : AbstractTlsKeyExchange
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

            // https://tools.ietf.org/html/rfc5246#section-7.4.7.1
            /*
                struct {
                    ProtocolVersion client_version;
                    opaque random[46];
                } PreMasterSecret;
            */
            PreMasterSecret = new byte[48];
            BitHelper.GetBytes((ushort)ProtocolVersion.Tls12, ByteOrder.BigEndian, PreMasterSecret, 0);
            Random.GetBytes(PreMasterSecret, 2, 46);

            var algo = SecurityParameters.ServerCertificate.PublicKey.ResolveAlgorithm();
            var rsa = algo as RSAEncryption;
            if (rsa == null)
                new TlsStreamException("For RSA key exchange certificate must also use RSA");

            rsa.Parameters.Scheme = RSAEncryptionScheme.RSAES_PKCS1_v1_5;
            rsa.Random = Random;
            message.EncryptedPreMasterSecret = rsa.Encrypt(PreMasterSecret);

            return message;
        }

        public override ClientKeyExchange ReadClientKeyExchange(BinaryReader reader)
        {
            var message = new ClientKeyExchange();
            message.EncryptedPreMasterSecret = reader.ReadBytes(reader.ReadShort());

            var algo = SecurityParameters.ServerCertificate.PrivateKey.ResolveAlgorithm();
            var rsa = algo as RSAEncryption;
            if (rsa == null)
                throw new TlsStreamException("For RSA key exchange certificate must also use RSA");

            rsa.Parameters.Scheme = RSAEncryptionScheme.RSAES_PKCS1_v1_5;
            PreMasterSecret = rsa.Decrypt(message.EncryptedPreMasterSecret);
            var version = (ProtocolVersion)BitHelper.ToUInt16(PreMasterSecret, 0, ByteOrder.BigEndian);
            if (version != ProtocolVersion.Tls12)
                throw new TlsStreamException("Invalid PreMasterSecret");

            return message;
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            writer.WriteShort((ushort)message.EncryptedPreMasterSecret.Length);
            writer.WriteBytes(message.EncryptedPreMasterSecret);
        }

        #endregion
    }
}