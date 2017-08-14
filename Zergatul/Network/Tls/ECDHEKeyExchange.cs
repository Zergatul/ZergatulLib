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
    internal class ECDHEKeyExchange : AbstractTlsKeyExchange
    {
        ECDiffieHellman _ecdh;

        public override void GetServerKeyExchange(ServerKeyExchange message)
        {
            var namedCurve = NamedCurve.secp256r1;

            _ecdh = new ECDiffieHellman();
            _ecdh.Parameters = ResolveCurve(namedCurve);
            _ecdh.GenerateKeys(Random);

            message.ECParams = new ServerECDHParams
            {
                CurveParams = new ECParameters
                {
                    CurveType = ECCurveType.NamedCurve,
                    NamedCurve = namedCurve
                },
                Point = _ecdh.PublicKey.ToBytes()
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
            writer.WriteBytes(message.ECParams.ToBytes());
            writer.WriteByte((byte)message.SignAndHashAlgo.Hash);
            writer.WriteByte((byte)message.SignAndHashAlgo.Signature);
            writer.WriteShort((ushort)message.Signature.Length);
            writer.WriteBytes(message.Signature);
        }

        public override byte[] GetDataToSign(ServerKeyExchange message)
        {
            return message.ECParams.ToBytes();
        }

        public override void GenerateClientKeyExchange(ClientKeyExchange message)
        {
            message.ECDH_Yc = _ecdh.PublicKey.ToBytes();

            PreMasterSecret = new ByteArray(_ecdh.KeyExchange.SharedSecret.XToBytes());
        }

        public override void ReadClientKeyExchange(ClientKeyExchange message, BinaryReader reader)
        {
            message.ECDH_Yc = reader.ReadBytes(reader.ReadByte());

            _ecdh.KeyExchange.CalculateSharedSecret(ECPointGeneric.Parse(message.ECDH_Yc, _ecdh.Parameters));

            /*
                All ECDH calculations (including parameter and key generation as well
                as the shared secret calculation) are performed according to [6]
                using the ECKAS-DH1 scheme with the identity map as key derivation
                function (KDF), so that the premaster secret is the x-coordinate of
                the ECDH shared secret elliptic curve point represented as an octet
                string.  Note that this octet string (Z in IEEE 1363 terminology) as
                output by FE2OSP, the Field Element to Octet String Conversion
                Primitive, has constant length for any given field; leading zeros
                found in this octet string MUST NOT be truncated.
            */
            PreMasterSecret = new ByteArray(_ecdh.KeyExchange.SharedSecret.XToBytes());
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            writer.WriteByte((byte)message.ECDH_Yc.Length);
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