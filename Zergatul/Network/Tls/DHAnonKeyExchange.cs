using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal class DHAnonKeyExchange : AbstractTlsKeyExchange
    {
        public override MessageInfo ServerCertificateMessage => MessageInfo.Forbidden;
        public override MessageInfo ServerKeyExchangeMessage => MessageInfo.Required;
        public override MessageInfo CertificateRequestMessage => MessageInfo.Forbidden;
        public override MessageInfo ClientCertificateMessage => MessageInfo.Forbidden;
        public override MessageInfo CertificateverifyMessage => MessageInfo.Forbidden;

        private DiffieHellman _dhServer;
        private DiffieHellman _dhClient;

        #region ServerKeyExchange

        public override ServerKeyExchange GenerateServerKeyExchange()
        {
            var message = new ServerKeyExchange();

            _dhServer = DHERoutine.GenerateServerKeyExchange(message, Random, Settings);

            return message;
        }

        public override ServerKeyExchange ReadServerKeyExchange(BinaryReader reader)
        {
            var message = new ServerKeyExchange();

            DHERoutine.ReadServerKeyExchange(message, reader);
            _dhClient = DHERoutine.GetSharedSecretAsClient(message, Random);

            return message;
        }

        public override void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer)
        {
            DHERoutine.WriteServerKeyExchange(message, writer);
        }

        #endregion

        #region ClientKeyExchange

        public override ClientKeyExchange GenerateClientKeyExchange()
        {
            var message = new ClientKeyExchange();

            PreMasterSecret = DHERoutine.GenerateClientKeyExchange(message, _dhClient);

            return message;
        }

        public override ClientKeyExchange ReadClientKeyExchange(BinaryReader reader)
        {
            var message = new ClientKeyExchange();

            PreMasterSecret = DHERoutine.ReadClientKeyExchange(message, reader, _dhServer);

            return message;
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            DHERoutine.WriteClientKeyExchange(message, writer);
        }

        #endregion
    }
}