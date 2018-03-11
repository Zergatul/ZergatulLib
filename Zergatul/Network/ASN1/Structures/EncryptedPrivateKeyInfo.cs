using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Symmetric;
using Zergatul.Cryptography.Symmetric.CipherMode;
using Zergatul.Cryptography.Generator;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Network.Asn1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5208#section-6
    /// </summary>
    public class EncryptedPrivateKeyInfo
    {
        public AlgorithmIdentifier Algorithm { get; private set; }
        public byte[] EncryptedData { get; private set; }
        public PrivateKeyInfo PrivateKey { get; private set; }

        public static EncryptedPrivateKeyInfo Parse(Asn1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 2);

            var result = new EncryptedPrivateKeyInfo { Algorithm = AlgorithmIdentifier.Parse(seq.Elements[0]) };

            var os = seq.Elements[1] as OctetString;
            ParseException.ThrowIfNull(os);
            result.EncryptedData = os.Data;

            return result;
        }

        public void Decrypt(string password)
        {
            byte[] data;
            if (Algorithm.Algorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS12.PKCS12PBE.PBEWithSHA1And3KeyTripleDESCBC)
            {
                var @params = PKCS12PBEParams.Parse(Algorithm.Parameters);

                var generator = new PKCS12v11();
                var hash = new SHA1();
                var pwd = generator.FormatPassword(password);

                byte[] key = generator.GenerateParameter(hash, PKCS12v11.IDKeyMaterial, pwd, @params.Salt, @params.Iterations, 24);
                byte[] iv = generator.GenerateParameter(hash, PKCS12v11.IDInitialValue, pwd, @params.Salt, @params.Iterations, 8);

                var tripleDES = new TripleDESEDE();
                var dec = new CBC().CreateDecryptor(tripleDES, key);
                data = dec.Decrypt(iv, EncryptedData);
            }
            else
                throw new NotImplementedException();

            PrivateKey = PrivateKeyInfo.Parse(Asn1Element.ReadFrom(data));
        }
    }
}