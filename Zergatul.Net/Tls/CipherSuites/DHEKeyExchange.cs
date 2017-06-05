using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Math;
using Zergatul.Net.Tls.Extensions;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal class DHEKeyExchange : AbstractKeyExchange
    {
        private BigInteger _g;
        private BigInteger _p;
        private BigInteger _Ys;
        private int _dhKeyLength;

        private ISecureRandom _random;

        public DHEKeyExchange(ISecureRandom random)
        {
            this._random = random;
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

            _dhKeyLength = message.Params.DH_p.Length;
            _g = new BigInteger(message.Params.DH_g, ByteOrder.BigEndian);
            _p = new BigInteger(message.Params.DH_p, ByteOrder.BigEndian);
            _Ys = new BigInteger(message.Params.DH_Ys, ByteOrder.BigEndian);
        }

        public override ClientKeyExchangeResult GenerateClientKeyExchange()
        {
            var dh = new DiffieHellman(_g, _p, _Ys, _random);
            dh.CalculateForBSide();
            return new ClientKeyExchangeResult
            {
                PreMasterSecret = new ByteArray(dh.ZZ.ToBytes(ByteOrder.BigEndian, _dhKeyLength)),
                Message = new ClientKeyExchange
                {
                    DHPublic = new ClientDiffieHellmanPublic
                    {
                        DH_Yc = dh.Yb.ToBytes(ByteOrder.BigEndian, _dhKeyLength)
                    }
                }
            };
        }
    }
}