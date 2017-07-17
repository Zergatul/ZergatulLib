using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Math;
using Zergatul.Network.Tls.Extensions;

namespace Zergatul.Network.Tls.CipherSuites
{
    internal class DHEKeyExchange : AbstractKeyExchange
    {
        private DiffieHellman _dh;
        private BigInteger _sharedSecret;

        public override void GetServerKeyExchange(ServerKeyExchange message)
        {
            _dh = new DiffieHellman();
            _dh.Parameters = DiffieHellmanParameters.Group14; // 2048 bit key
            _dh.GenerateKeys(Random);

            message.Params = new ServerDHParams
            {
                DH_p = _dh.Parameters.p.ToBytes(ByteOrder.BigEndian),
                DH_g = _dh.Parameters.g.ToBytes(ByteOrder.BigEndian),
                DH_Ys = _dh.PublicKey.ToBytes(ByteOrder.BigEndian)
            };
            message.SignAndHashAlgo = new SignatureAndHashAlgorithm
            {
                Hash = HashAlgorithm.SHA1,
                Signature = SignatureAlgorithm.RSA
            };
        }

        public override void ReadServerKeyExchange(ServerKeyExchange message, BinaryReader reader)
        {
            message.Params = new ServerDHParams
            {
                DH_p = reader.ReadBytes(reader.ReadShort()),
                DH_g = reader.ReadBytes(reader.ReadShort()),
                DH_Ys = reader.ReadBytes(reader.ReadShort())
            };
            message.SignAndHashAlgo = new SignatureAndHashAlgorithm
            {
                Hash = (HashAlgorithm)reader.ReadByte(),
                Signature = (SignatureAlgorithm)reader.ReadByte()
            };
            message.Signature = reader.ReadBytes(reader.ReadShort());

            _dh = new DiffieHellman();
            _dh.Parameters = new DiffieHellmanParameters(new BigInteger(message.Params.DH_g, ByteOrder.BigEndian), new BigInteger(message.Params.DH_p, ByteOrder.BigEndian));
            _dh.GenerateKeys(Random);
            _dh.KeyExchange.CalculateSharedSecret(new BigInteger(message.Params.DH_Ys, ByteOrder.BigEndian));
        }

        public override void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer)
        {
            writer.WriteBytes(message.Params.ToArray());
            writer.WriteByte((byte)message.SignAndHashAlgo.Hash);
            writer.WriteByte((byte)message.SignAndHashAlgo.Signature);
            writer.WriteShort((ushort)message.Signature.Length);
            writer.WriteBytes(message.Signature);
        }

        public override void GetClientKeyExchange(ClientKeyExchange message)
        {
            message.DHPublic = new ClientDiffieHellmanPublic
            {
                DH_Yc = _dh.PublicKey.ToBytes(ByteOrder.BigEndian)
            };

            PreMasterSecret = new ByteArray(_dh.KeyExchange.SharedSecret.ToBytes(ByteOrder.BigEndian));
        }

        public override void ReadClientKeyExchange(ClientKeyExchange message, BinaryReader reader)
        {
            message.DHPublic = new ClientDiffieHellmanPublic
            {
                DH_Yc = reader.ReadBytes(reader.ReadShort())
            };

            _dh.KeyExchange.CalculateSharedSecret(new BigInteger(message.DHPublic.DH_Yc, ByteOrder.BigEndian));

            PreMasterSecret = new ByteArray(_dh.KeyExchange.SharedSecret.ToBytes(ByteOrder.BigEndian));
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            writer.WriteShort((ushort)message.DHPublic.DH_Yc.Length);
            writer.WriteBytes(message.DHPublic.DH_Yc);
        }
    }
}