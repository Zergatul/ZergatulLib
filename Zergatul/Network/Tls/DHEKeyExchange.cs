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
        public override bool ServerKeyExchangeRequired => true;

        private DiffieHellman _dh;
        private AbstractTlsSignature _signature;

        public DHEKeyExchange(AbstractTlsSignature signature)
        {
            this._signature = signature;
        }

        #region ServerKeyExchange

        public override ServerKeyExchange GenerateServerKeyExchange()
        {
            var message = new ServerKeyExchange();

            _dh = new DiffieHellman();
            _dh.Random = Random;

            if (Settings.DHParameters != null)
                _dh.Parameters = Settings.DHParameters;
            else
                _dh.Parameters = TlsStreamSettings.Default.DHParameters;

            _dh.GenerateKeys();

            message.Params = new ServerDHParams
            {
                DH_p = _dh.Parameters.p.ToBytes(ByteOrder.BigEndian),
                DH_g = _dh.Parameters.g.ToBytes(ByteOrder.BigEndian),
                DH_Ys = _dh.PublicKey.ToBytes(ByteOrder.BigEndian)
            };

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

            message.Params = new ServerDHParams();
            message.Params.Read(reader);

            message.SignAndHashAlgo = new SignatureAndHashAlgorithm();
            message.SignAndHashAlgo.Read(reader);

            message.Signature = reader.ReadBytes(reader.ReadShort());

            var algo = SecurityParameters.ServerCertificate.PublicKey.ResolveAlgorithm();

            var hash = message.SignAndHashAlgo.Hash.Resolve();
            hash.Update(SecurityParameters.ClientRandom);
            hash.Update(SecurityParameters.ServerRandom);
            hash.Update(message.Params.ToBytes());

            if (!_signature.VerifySignature(algo, hash, message.Signature))
                throw new TlsStreamException("Invalid signature");

            _dh = new DiffieHellman();
            _dh.Random = Random;
            _dh.Parameters = new DiffieHellmanParameters(new BigInteger(message.Params.DH_g, ByteOrder.BigEndian), new BigInteger(message.Params.DH_p, ByteOrder.BigEndian));
            _dh.GenerateKeys();
            _dh.KeyExchange.CalculateSharedSecret(new BigInteger(message.Params.DH_Ys, ByteOrder.BigEndian));

            return message;
        }

        public override void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer)
        {
            writer.WriteBytes(message.Params.ToBytes());
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
            message.DH_Yc = _dh.PublicKey.ToBytes(ByteOrder.BigEndian);

            PreMasterSecret = _dh.KeyExchange.SharedSecret.ToBytes(ByteOrder.BigEndian);

            return message;
        }

        public override ClientKeyExchange ReadClientKeyExchange(BinaryReader reader)
        {
            var message = new ClientKeyExchange();
            message.DH_Yc = reader.ReadBytes(reader.ReadShort());

            _dh.KeyExchange.CalculateSharedSecret(new BigInteger(message.DH_Yc, ByteOrder.BigEndian));

            PreMasterSecret = _dh.KeyExchange.SharedSecret.ToBytes(ByteOrder.BigEndian);

            return message;
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            writer.WriteShort((ushort)message.DH_Yc.Length);
            writer.WriteBytes(message.DH_Yc);
        }

        #endregion
    }
}