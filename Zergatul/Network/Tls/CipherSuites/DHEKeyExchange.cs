using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Math;
using Zergatul.Network.Tls.Extensions;

namespace Zergatul.Network.Tls.CipherSuites
{
    internal class DHEKeyExchange : AbstractKeyExchange
    {
        private BigInteger _g;
        private BigInteger _p;
        private BigInteger _Xs;
        private BigInteger _Ys;

        public override void GetServerKeyExchange(ServerKeyExchange message)
        {
            // RFC 3526, Group #14
            _p = new BigInteger("FFFFFFFFFFFFFFFFC90FDAA22168C234C4C6628B80DC1CD129024E088A67CC74020BBEA63B139B22514A08798E3404DDEF9519B3CD3A431B302B0A6DF25F14374FE1356D6D51C245E485B576625E7EC6F44C42E9A637ED6B0BFF5CB6F406B7EDEE386BFB5A899FA5AE9F24117C4B1FE649286651ECE45B3DC2007CB8A163BF0598DA48361C55D39A69163FA8FD24CF5F83655D23DCA3AD961C62F356208552BB9ED529077096966D670C354E4ABC9804F1746C08CA18217C32905E462E36CE3BE39E772C180E86039B2783A2EC07A28FB5C55DF06F4C52C9DE2BCBF6955817183995497CEA956AE515D2261898FA051015728E5A8AACAA68FFFFFFFFFFFFFFFF", 16);
            _g = new BigInteger(2);

            var dh = new DiffieHellmanOld(_g, _p, Random);
            dh.CalculateForASideStep1();
            _Xs = dh.Xa;
            _Ys = dh.Ya;

            message.Params = new ServerDHParams
            {
                DH_p = _p.ToBytes(ByteOrder.BigEndian),
                DH_g = _g.ToBytes(ByteOrder.BigEndian),
                DH_Ys = _Ys.ToBytes(ByteOrder.BigEndian)
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

            _g = new BigInteger(message.Params.DH_g, ByteOrder.BigEndian);
            _p = new BigInteger(message.Params.DH_p, ByteOrder.BigEndian);
            _Ys = new BigInteger(message.Params.DH_Ys, ByteOrder.BigEndian);
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
            var dh = new DiffieHellmanOld(_g, _p, Random);
            dh.Ya = _Ys;
            dh.CalculateForBSide();

            message.DHPublic = new ClientDiffieHellmanPublic
            {
                DH_Yc = dh.Yb.ToBytes(ByteOrder.BigEndian)
            };

            PreMasterSecret = new ByteArray(dh.ZZ.ToBytes(ByteOrder.BigEndian));
        }

        public override void ReadClientKeyExchange(ClientKeyExchange message, BinaryReader reader)
        {
            message.DHPublic = new ClientDiffieHellmanPublic
            {
                DH_Yc = reader.ReadBytes(reader.ReadShort())
            };

            var dh = new DiffieHellmanOld(_g, _p, Random);
            dh.Xa = _Xs;
            dh.Yb = new BigInteger(message.DHPublic.DH_Yc, ByteOrder.BigEndian);
            dh.CalculateForASideStep2();

            PreMasterSecret = new ByteArray(dh.ZZ.ToBytes(ByteOrder.BigEndian));
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            writer.WriteShort((ushort)message.DHPublic.DH_Yc.Length);
            writer.WriteBytes(message.DHPublic.DH_Yc);
        }
    }
}