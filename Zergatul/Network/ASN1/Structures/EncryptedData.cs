using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Symmetric;
using Zergatul.Cryptography.Symmetric.CipherMode;
using Zergatul.Cryptography.Generator;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Network.ASN1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc2315#section-13
    /// </summary>
    public class EncryptedData
    {
        public int Version { get; private set; }
        public EncryptedContentInfo Content { get; private set; }

        public static EncryptedData Parse(ASN1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 2);

            var i = seq.Elements[0] as Integer;
            ParseException.ThrowIfNull(i);

            return new EncryptedData
            {
                Version = (int)i.Value,
                Content = EncryptedContentInfo.Parse(seq.Elements[1])
            };
        }

        public byte[] Decrypt(string password)
        {
            if (Content.EncryptionAlgorithm.Algorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS12.PKCS12PBE.PBEWithSHA1And40BitRC2CBC)
            {
                var @params = PKCS12PBEParams.Parse(Content.EncryptionAlgorithm.Parameters);

                var generator = new PKCS12v11();
                var hash = new SHA1();
                var pwd = generator.FormatPassword(password);

                byte[] key = generator.GenerateParameter(hash, PKCS12v11.IDKeyMaterial, pwd, @params.Salt, @params.Iterations, 5);
                byte[] iv = generator.GenerateParameter(hash, PKCS12v11.IDInitialValue, pwd, @params.Salt, @params.Iterations, 8);

                var rc2 = new RC2(5, 40);
                var dec = new CBC().CreateDecryptor(rc2, key);
                return dec.Decrypt(iv, Content.Content);
            }
            else
                throw new NotImplementedException();
        }
    }
}