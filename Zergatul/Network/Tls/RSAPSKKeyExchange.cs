using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal class RSAPSKKeyExchange : AbstractTlsKeyExchange
    {
        public override MessageInfo ServerCertificateMessage => MessageInfo.Required;
        public override MessageInfo ServerKeyExchangeMessage => MessageInfo.CanBeOmitted;
        public override MessageInfo CertificateRequestMessage => MessageInfo.Forbidden;
        public override MessageInfo ClientCertificateMessage => MessageInfo.Forbidden;
        public override MessageInfo CertificateverifyMessage => MessageInfo.Forbidden;

        private byte[] _identityHint;

        #region ServerKeyExchange

        public override bool ShouldSendServerKeyExchange()
        {
            return Parameters.PSKIdentityHint != null;
        }

        public override ServerKeyExchange GenerateServerKeyExchange()
        {
            var message = new ServerKeyExchange();
            message.PSKIdentityHint = Parameters.PSKIdentityHint ?? new byte[0];

            return message;
        }

        public override ServerKeyExchange ReadServerKeyExchange(BinaryReader reader)
        {
            var message = new ServerKeyExchange();

            message.PSKIdentityHint = reader.ReadBytes(reader.ReadShort());
            this._identityHint = message.PSKIdentityHint;

            return message;
        }

        public override void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer)
        {
            writer.WriteShort((ushort)message.PSKIdentityHint.Length);
            writer.WriteBytes(message.PSKIdentityHint);
        }

        #endregion

        #region ClientKeyExchange

        public override ClientKeyExchange GenerateClientKeyExchange()
        {
            if (Parameters.GetPSKByHint == null)
                throw new TlsStreamException("TlsStream.Settings.GetPSKByHint is null");

            var psk = Parameters.GetPSKByHint(_identityHint);

            var message = new ClientKeyExchange();
            message.PSKIdentity = psk.Identity;

            byte[] otherSecret = new byte[48];
            BitHelper.GetBytes((ushort)ProtocolVersion.Tls12, ByteOrder.BigEndian, otherSecret, 0);
            Random.GetNextBytes(otherSecret, 2, 46);

            PreMasterSecret = PSKKeyExchange.CreateSharedSecret(otherSecret, psk.Secret);

            var algo = SecurityParameters.ServerCertificate.PublicKey.ResolveAlgorithm();
            var rsa = algo as RSAEncryption;
            if (rsa == null)
                new TlsStreamException("For RSA key exchange certificate must also use RSA");

            rsa.Parameters.Scheme = RSAEncryptionScheme.RSAES_PKCS1_v1_5;
            rsa.Random = Random;
            message.EncryptedPreMasterSecret = rsa.Encrypt(otherSecret);

            return message;
        }

        public override ClientKeyExchange ReadClientKeyExchange(BinaryReader reader)
        {
            if (Parameters.GetPSKByIdentity == null)
                throw new TlsStreamException("TlsStream.Settings.GetPSKByIdentity is null");

            var message = new ClientKeyExchange();
            message.PSKIdentity = reader.ReadBytes(reader.ReadShort());

            message.EncryptedPreMasterSecret = reader.ReadBytes(reader.ReadShort());

            var algo = SecurityParameters.ServerCertificate.PrivateKey.ResolveAlgorithm();
            var rsa = algo as RSAEncryption;
            if (rsa == null)
                throw new TlsStreamException("For RSA key exchange certificate must also use RSA");

            rsa.Parameters.Scheme = RSAEncryptionScheme.RSAES_PKCS1_v1_5;
            byte[] otherSecret = rsa.Decrypt(message.EncryptedPreMasterSecret);
            var version = (ProtocolVersion)BitHelper.ToUInt16(otherSecret, 0, ByteOrder.BigEndian);
            if (version != ProtocolVersion.Tls12)
                throw new TlsStreamException("Invalid PreMasterSecret");

            var psk = Parameters.GetPSKByIdentity(message.PSKIdentity);
            PreMasterSecret = PSKKeyExchange.CreateSharedSecret(otherSecret, psk.Secret);

            return message;
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            writer.WriteShort((ushort)message.PSKIdentity.Length);
            writer.WriteBytes(message.PSKIdentity);

            writer.WriteShort((ushort)message.EncryptedPreMasterSecret.Length);
            writer.WriteBytes(message.EncryptedPreMasterSecret);
        }

        #endregion
    }
}