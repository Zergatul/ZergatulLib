using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zergatul.Cryptography;
using Zergatul.Math;
using Zergatul.Net.Tls.Extensions;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal class TLS_DHE_RSA_WITH_AES_256_GCM_SHA384 : CipherSuite
    {
        private BigInteger _g;
        private BigInteger _p;
        private BigInteger _Ys;
        private int _dhKeyLength;

        private byte[] _preMasterSecret;
        private byte[] _masterSecret;

        public TLS_DHE_RSA_WITH_AES_256_GCM_SHA384() :
            base(CipherSuiteType.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384)
        {

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

        public override ClientKeyExchange GetClientKeyExchange()
        {
            var dh = new DiffieHellman(_g, _p, _Ys, new DefaultSecureRandom());
            dh.CalculateForBSide();
            _preMasterSecret = dh.ZZ.ToBytes(ByteOrder.BigEndian, _dhKeyLength);
            return new ClientKeyExchange
            {
                DHPublic = new ClientDiffieHellmanPublic
                {
                    DH_Yc = dh.Yb.ToBytes(ByteOrder.BigEndian, _dhKeyLength)
                }
            };
        }

        public override void CalculateMasterSecret(byte[] clientRandom, byte[] serverRandom)
        {
            // RFC 5246 // Page 63
            // The master secret is always exactly 48 bytes in length.
            _masterSecret = PseudoRandomFunction(_preMasterSecret, "master secret", clientRandom.Concat(serverRandom))
                .Take(48).ToArray();

            // RFC 5246 // Page 63
            // The pre_master_secret should be deleted from memory once the master_secret has been computed.
            for (int i = 0; i < _preMasterSecret.Length; i++)
                _preMasterSecret[i] = 0;
        }

        public override Finished GetFinished(IEnumerable<byte> data)
        {
            return new Finished
            {
                Data = PseudoRandomFunction(_masterSecret, "client finished", Hash(data)).Take(12).ToArray()
            };
        }

        private byte[] Hash(IEnumerable<byte> data)
        {
            var algo = new System.Security.Cryptography.SHA256Managed();
            return algo.ComputeHash(data.ToArray());
        }

        private byte[] HMACHash(byte[] secret, IEnumerable<byte> seed)
        {
            return Hash(secret.Select(b => (byte)(b ^ 0x5C)).Concat(Hash(secret.Select(b => (byte)(b ^ 0x36)).Concat(seed))));
        }

        private byte[] PHash(byte[] secret, IEnumerable<byte> seed)
        {
            return HMACHash(secret, seed.Concat(seed));
        }

        public override byte[] PseudoRandomFunction(byte[] secret, string label, IEnumerable<byte> seed)
        {
            return PHash(secret, Encoding.ASCII.GetBytes(label).Concat(seed));
        }
    }
}