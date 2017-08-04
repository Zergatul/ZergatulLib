using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Math.EllipticCurves;
using Zergatul.Network.Tls.Extensions;

namespace Zergatul.Network.Tls
{
    // https://tools.ietf.org/html/rfc4492
    internal class ECDHEKeyExchange : AbstractKeyExchange
    {
        ECDiffieHellman _ecdh;

        public override void GetServerKeyExchange(ServerKeyExchange message)
        {
            _ecdh = new ECDiffieHellman();
            _ecdh.Parameters = Math.EllipticCurves.PrimeField.EllipticCurve.secp256k1;
            _ecdh.GenerateKeys(Random);

            message.ECParams = new ServerECDHParams
            {
                CurveParams = new ECParameters
                {
                    CurveType = ECCurveType.NamedCurve,
                    NamedCurve = NamedCurve.secp256k1
                },
                Point = _ecdh.PublicKey.ToBytes()
            };

            message.SignAndHashAlgo = new SignatureAndHashAlgorithm
            {
                Hash = HashAlgorithm.SHA1,
                Signature = SignatureAlgorithm.RSA
            };
        }

        public override void ReadServerKeyExchange(ServerKeyExchange message, BinaryReader reader)
        {
            message.ECParams = new ServerECDHParams();
            message.ECParams.Read(reader);

            message.SignAndHashAlgo = new SignatureAndHashAlgorithm();
            message.SignAndHashAlgo.Read(reader);

            message.Signature = reader.ReadBytes(reader.ReadShort());

            _ecdh = new ECDiffieHellman();
            _ecdh.Parameters = ResolveCurve(message.ECParams.CurveParams.NamedCurve);
            _ecdh.GenerateKeys(Random);
            _ecdh.KeyExchange.CalculateSharedSecret(ECPointGeneric.Parse(message.ECParams.Point, _ecdh.Parameters));
        }

        public override void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void GetClientKeyExchange(ClientKeyExchange message)
        {
            message.ECDH_Yc = _ecdh.PublicKey.ToBytes();

            if (_ecdh.KeyExchange.SharedSecret.PFECPoint != null)
            {
                var curve = (Math.EllipticCurves.PrimeField.EllipticCurve)_ecdh.Parameters;
                PreMasterSecret = new ByteArray(_ecdh.KeyExchange.SharedSecret.PFECPoint.x.ToBytes(ByteOrder.BigEndian, (curve.p.BitSize + 7) / 8));
            }
            if (_ecdh.KeyExchange.SharedSecret.BFECPoint != null)
            {
                var curve = (Math.EllipticCurves.BinaryField.EllipticCurve)_ecdh.Parameters;
                PreMasterSecret = new ByteArray(_ecdh.KeyExchange.SharedSecret.BFECPoint.x.ToBytes(ByteOrder.BigEndian, (curve.f.Degree + 7) / 8));
            }
        }

        public override void ReadClientKeyExchange(ClientKeyExchange message, BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            writer.WriteShort((ushort)message.ECDH_Yc.Length);
            writer.WriteBytes(message.ECDH_Yc);
        }

        private static IEllipticCurve ResolveCurve(NamedCurve curve)
        {
            switch (curve)
            {
                case NamedCurve.secp160k1: return Math.EllipticCurves.PrimeField.EllipticCurve.secp160k1;
                case NamedCurve.secp160r1: return Math.EllipticCurves.PrimeField.EllipticCurve.secp160r1;
                case NamedCurve.secp160r2: return Math.EllipticCurves.PrimeField.EllipticCurve.secp160r2;
                case NamedCurve.secp192k1: return Math.EllipticCurves.PrimeField.EllipticCurve.secp192k1;
                case NamedCurve.secp192r1: return Math.EllipticCurves.PrimeField.EllipticCurve.secp192r1;
                case NamedCurve.secp224k1: return Math.EllipticCurves.PrimeField.EllipticCurve.secp224k1;
                case NamedCurve.secp224r1: return Math.EllipticCurves.PrimeField.EllipticCurve.secp224r1;
                case NamedCurve.secp256k1: return Math.EllipticCurves.PrimeField.EllipticCurve.secp256k1;
                case NamedCurve.secp256r1: return Math.EllipticCurves.PrimeField.EllipticCurve.secp256r1;
                case NamedCurve.secp384r1: return Math.EllipticCurves.PrimeField.EllipticCurve.secp384r1;
                case NamedCurve.secp521r1: return Math.EllipticCurves.PrimeField.EllipticCurve.secp521r1;

                case NamedCurve.sect163k1: return Math.EllipticCurves.BinaryField.EllipticCurve.sect163k1;
                case NamedCurve.sect163r1: return Math.EllipticCurves.BinaryField.EllipticCurve.sect163r1;
                case NamedCurve.sect163r2: return Math.EllipticCurves.BinaryField.EllipticCurve.sect163r2;
                case NamedCurve.sect193r1: return Math.EllipticCurves.BinaryField.EllipticCurve.sect193r1;
                case NamedCurve.sect193r2: return Math.EllipticCurves.BinaryField.EllipticCurve.sect193r2;
                case NamedCurve.sect233k1: return Math.EllipticCurves.BinaryField.EllipticCurve.sect233k1;
                case NamedCurve.sect233r1: return Math.EllipticCurves.BinaryField.EllipticCurve.sect233r1;
                case NamedCurve.sect239k1: return Math.EllipticCurves.BinaryField.EllipticCurve.sect239k1;
                case NamedCurve.sect283k1: return Math.EllipticCurves.BinaryField.EllipticCurve.sect283k1;
                case NamedCurve.sect283r1: return Math.EllipticCurves.BinaryField.EllipticCurve.sect283r1;
                case NamedCurve.sect409k1: return Math.EllipticCurves.BinaryField.EllipticCurve.sect409k1;
                case NamedCurve.sect409r1: return Math.EllipticCurves.BinaryField.EllipticCurve.sect409r1;
                case NamedCurve.sect571k1: return Math.EllipticCurves.BinaryField.EllipticCurve.sect571k1;
                case NamedCurve.sect571r1: return Math.EllipticCurves.BinaryField.EllipticCurve.sect571r1;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}