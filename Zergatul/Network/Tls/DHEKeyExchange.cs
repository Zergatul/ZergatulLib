using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Math;
using Zergatul.Network.Tls.Extensions;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal class DHEKeyExchange : AbstractTlsKeyExchange
    {
        public override MessageInfo ServerCertificateMessage => MessageInfo.Required;
        public override MessageInfo ServerKeyExchangeMessage => MessageInfo.Required;
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

        private DiffieHellman _dhServer;
        private DiffieHellman _dhClient;
        private AbstractTlsSignature _signature;

        public DHEKeyExchange(AbstractTlsSignature signature)
        {
            this._signature = signature;
        }

        public override void SetRandom(ISecureRandom random)
        {
            base.SetRandom(random);
            _signature.Random = random;
        }

        #region ServerKeyExchange

        public override ServerKeyExchange GenerateServerKeyExchange()
        {
            var message = new ServerKeyExchange();

            _dhServer = DHERoutine.GenerateServerKeyExchange(message, Random, Settings);

            // TODO, get sign and hash from clienthello
            message.SignAndHashAlgo = new SignatureAndHashAlgorithm
            {
                Hash = HashAlgorithm.SHA256,
                Signature = _signature.Algorithm
            };

            var algo = SecurityParameters.ServerCertificate.PrivateKey.ResolveAlgorithm();

            var hash = message.SignAndHashAlgo.Hash.Resolve();
            hash.Update(SecurityParameters.ClientRandom);
            hash.Update(SecurityParameters.ServerRandom);
            hash.Update(message.Params.ToBytes());

            message.Signature = _signature.CreateSignature(algo, hash);

            return message;
        }

        public override ServerKeyExchange ReadServerKeyExchange(BinaryReader reader)
        {
            var message = new ServerKeyExchange();

            DHERoutine.ReadServerKeyExchange(message, reader);

            message.SignAndHashAlgo = new SignatureAndHashAlgorithm();
            message.SignAndHashAlgo.Read(reader);

            message.Signature = reader.ReadBytes(reader.ReadShort());

            var algo = SecurityParameters.ServerCertificate.PublicKey.ResolveAlgorithm();

            var hash = message.SignAndHashAlgo.Hash.Resolve();
            hash.Update(SecurityParameters.ClientRandom);
            hash.Update(SecurityParameters.ServerRandom);
            hash.Update(message.Params.ToBytes());

            if (!_signature.VerifySignature(algo, hash, message.Signature))
                throw new InvalidSignatureException();

            _dhClient = DHERoutine.GetSharedSecretAsClient(message, Random);

            return message;
        }

        public override void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer)
        {
            DHERoutine.WriteServerKeyExchange(message, writer);

            writer.WriteByte((byte)message.SignAndHashAlgo.Hash);
            writer.WriteByte((byte)message.SignAndHashAlgo.Signature);
            writer.WriteShort((ushort)message.Signature.Length);
            writer.WriteBytes(message.Signature);
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