using System.Collections.Generic;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal class PSKKeyExchange : AbstractTlsKeyExchange
    {
        public override MessageInfo ServerCertificateMessage => MessageInfo.Forbidden;
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
            message.PSKIdentityHint = Parameters.PSKIdentityHint;
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

            PreMasterSecret = CreateSharedSecret(null, psk.Secret);

            return message;
        }

        public override ClientKeyExchange ReadClientKeyExchange(BinaryReader reader)
        {
            if (Parameters.GetPSKByIdentity == null)
                throw new TlsStreamException("TlsStream.Settings.GetPSKByIdentity is null");

            var message = new ClientKeyExchange();
            message.PSKIdentity = reader.ReadBytes(reader.ReadShort());

            var psk = Parameters.GetPSKByIdentity(message.PSKIdentity);
            PreMasterSecret = CreateSharedSecret(null, psk.Secret);

            return message;
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            writer.WriteShort((ushort)message.PSKIdentity.Length);
            writer.WriteBytes(message.PSKIdentity);
        }

        #endregion

        public static byte[] CreateSharedSecret(byte[] otherSecret, byte[] psk)
        {
            if (otherSecret == null)
                otherSecret = new byte[psk.Length];

            var list = new List<byte>();
            var bw = new BinaryWriter(list);

            bw.WriteShort((ushort)otherSecret.Length);
            bw.WriteBytes(otherSecret);

            bw.WriteShort((ushort)psk.Length);
            bw.WriteBytes(psk);

            return list.ToArray();
        }
    }
}