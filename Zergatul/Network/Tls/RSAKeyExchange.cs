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
        public override bool ServerKeyExchangeRequired => false;

        #region ServerKeyExchange

        public override ServerKeyExchange GenerateServerKeyExchange()
        {
            return null;
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
            // TODO: why constant here???
            BitHelper.GetBytes((ushort)ProtocolVersion.Tls12, ByteOrder.BigEndian, PreMasterSecret, 0);
            Random.GetBytes(PreMasterSecret, 2, 46);

            throw new NotImplementedException();

            return message;
        }

        public override ClientKeyExchange ReadClientKeyExchange(BinaryReader reader)
        {
            var message = new ClientKeyExchange();
            message.EncryptedPreMasterSecret = reader.ReadBytes(reader.ReadShort());

            var algo = SecurityParameters.ServerCertificate.PrivateKey.ResolveAlgorithm();
            var rsa = algo as RSA;
            if (rsa == null)
                throw new TlsStreamException("For RSA key exchange certificate must also use RSA");

            byte[] data = rsa.Encryption.Decrypt(message.EncryptedPreMasterSecret);

            return message;
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}