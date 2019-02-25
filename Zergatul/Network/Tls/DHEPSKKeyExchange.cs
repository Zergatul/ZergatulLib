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
    internal class DHEPSKKeyExchange : AbstractTlsKeyExchange
    {
        public override MessageInfo ServerCertificateMessage => MessageInfo.Forbidden;
        public override MessageInfo ServerKeyExchangeMessage => MessageInfo.Required;
        public override MessageInfo CertificateRequestMessage => MessageInfo.Forbidden;
        public override MessageInfo ClientCertificateMessage => MessageInfo.Forbidden;
        public override MessageInfo CertificateverifyMessage => MessageInfo.Forbidden;

        private DiffieHellman _dh;
        private byte[] _otherSecret;
        private byte[] _identityHint;

        #region ServerKeyExchange

        public override ServerKeyExchange GenerateServerKeyExchange()
        {
            var message = new ServerKeyExchange();
            message.PSKIdentityHint = Parameters.PSKIdentityHint ?? new byte[0];

            _dh = DHERoutine.GenerateServerKeyExchange(message, Random, Parameters);

            return message;
        }

        public override ServerKeyExchange ReadServerKeyExchange(BinaryReader reader)
        {
            var message = new ServerKeyExchange();

            message.PSKIdentityHint = reader.ReadBytes(reader.ReadShort());
            this._identityHint = message.PSKIdentityHint;

            DHERoutine.ReadServerKeyExchange(message, reader);
            _otherSecret = DHERoutine.GetSharedSecretAsClient(message, Random, out _dh);

            return message;
        }

        public override void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer)
        {
            writer.WriteShort((ushort)message.PSKIdentityHint.Length);
            writer.WriteBytes(message.PSKIdentityHint);

            DHERoutine.WriteServerKeyExchange(message, writer);
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

            DHERoutine.GenerateClientKeyExchange(message, _dh);
            PreMasterSecret = PSKKeyExchange.CreateSharedSecret(_otherSecret, psk.Secret);

            return message;
        }

        public override ClientKeyExchange ReadClientKeyExchange(BinaryReader reader)
        {
            if (Parameters.GetPSKByIdentity == null)
                throw new TlsStreamException("TlsStream.Settings.GetPSKByIdentity is null");

            var message = new ClientKeyExchange();
            message.PSKIdentity = reader.ReadBytes(reader.ReadShort());

            var psk = Parameters.GetPSKByIdentity(message.PSKIdentity);
            byte[] otherSecret = DHERoutine.ReadClientKeyExchange(message, reader, _dh);
            PreMasterSecret = PSKKeyExchange.CreateSharedSecret(otherSecret, psk.Secret);

            return message;
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            writer.WriteShort((ushort)message.PSKIdentity.Length);
            writer.WriteBytes(message.PSKIdentity);

            DHERoutine.WriteClientKeyExchange(message, writer);
        }

        #endregion
    }
}