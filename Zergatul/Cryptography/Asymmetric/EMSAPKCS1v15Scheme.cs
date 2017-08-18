using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;
using Zergatul.Network;
using Zergatul.Network.ASN1;
using Zergatul.Network.ASN1.Structures;

namespace Zergatul.Cryptography.Asymmetric
{
    public class EMSAPKCS1v15Scheme : SignatureScheme<BigInteger, BigInteger>
    {
        public OID HashAlgorithmOID { get; set; }
        public int KeySize { get; set; }

        public override BigInteger EncodeData(byte[] data)
        {
            if (HashAlgorithmOID == null)
                throw new InvalidOperationException("EMSA PCKS1 v1.5 requires hash algorithm OID");

            var ai = new AlgorithmIdentifier(HashAlgorithmOID, new Null());
            var pkcs = new EMSA_PKCS1_v1_5(ai, data, (KeySize + 7) / 8);
            byte[] bytes = pkcs.ToBytes();

            return new BigInteger(bytes, ByteOrder.BigEndian);
        }

        public override byte[] DecodeData(BigInteger data)
        {
            byte[] bytes = new byte[(KeySize + 7) / 8];
            byte[] bigIntBytes =  data.ToBytes(ByteOrder.BigEndian, bytes.Length - 1);
            Array.Copy(bigIntBytes, 0, bytes, 1, bigIntBytes.Length);

            var pkcs = EMSA_PKCS1_v1_5.Parse(bytes);
            if (pkcs.DigestAlgorithm.Algorithm != HashAlgorithmOID)
                throw new ParseException();

            return pkcs.Digest;
        }

        public override byte[] SignatureToBytes(BigInteger signature)
        {
            return signature.ToBytes(ByteOrder.BigEndian);
        }

        public override BigInteger BytesToSignature(byte[] signature)
        {
            return new BigInteger(signature, ByteOrder.BigEndian);
        }
    }
}