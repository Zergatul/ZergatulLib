using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Math.EllipticCurves;
using Zergatul.Network.Tls.Extensions;
using Zergatul.Network.Tls.Messages;
using Zergatul.Security;

namespace Zergatul.Network.Tls
{
    // https://tools.ietf.org/html/rfc4492
    internal class ECDHEKeyExchange : AbstractTlsKeyExchange
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

        private ECPDiffieHellman _ecdh;
        private AbstractTlsSignature _signature;

        public ECDHEKeyExchange(AbstractTlsSignature signature)
        {
            this._signature = signature;
        }

        public override void SetRandom(SecureRandom random)
        {
            base.SetRandom(random);
            _signature.Random = random;
        }

        #region ServerKeyExchange

        public override ServerKeyExchange GenerateServerKeyExchange()
        {
            var message = new ServerKeyExchange();

            _ecdh = ECDHERoutine.GenerateServerKeyExchange(message, Random);

            // TODO, get sign and hash from clienthello
            message.SignAndHashAlgo = new SignatureAndHashAlgorithm
            {
                Hash = HashAlgorithm.SHA256,
                Signature = _signature.Algorithm
            };

            var algo = SecurityParameters.ServerCertificate.PrivateKey.ResolveAlgorithm();

            var hash = message.SignAndHashAlgo.Hash.Resolve();
            var data = ByteArray.Concat(SecurityParameters.ClientRandom, SecurityParameters.ServerRandom, message.ECParams.ToBytes());

            message.Signature = _signature.CreateSignature(algo.ToSignature(), hash, data);

            return message;
        }

        public override ServerKeyExchange ReadServerKeyExchange(BinaryReader reader)
        {
            var message = new ServerKeyExchange();

            ECDHERoutine.ReadServerKeyExchange(message, reader);

            message.SignAndHashAlgo = new SignatureAndHashAlgorithm();
            message.SignAndHashAlgo.Read(reader);

            message.Signature = reader.ReadBytes(reader.ReadShort());

            var algo = SecurityParameters.ServerCertificate.PublicKey.ResolveAlgorithm();

            var hash = message.SignAndHashAlgo.Hash.Resolve();
            var data = ByteArray.Concat(SecurityParameters.ClientRandom, SecurityParameters.ServerRandom, message.ECParams.ToBytes());

            if (!_signature.VerifySignature(algo.ToSignature(), hash, data, message.Signature))
                throw new InvalidSignatureException();

            PreMasterSecret = ECDHERoutine.GetSharedSecretAsClient(message, Random, out _ecdh);

            return message;
        }

        public override void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer)
        {
            ECDHERoutine.WriteServerKeyExchange(message, writer);

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

            ECDHERoutine.GenerateClientKeyExchange(message, _ecdh);

            return message;
        }

        public override ClientKeyExchange ReadClientKeyExchange(BinaryReader reader)
        {
            var message = new ClientKeyExchange();

            PreMasterSecret = ECDHERoutine.ReadClientKeyExchange(message, reader, _ecdh);

            return message;
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            ECDHERoutine.WriteClientKeyExchange(message, writer);
        }

        #endregion
    }
}