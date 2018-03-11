using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Generator;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Network.Asn1.Structures
{
    public class PKCS12Store
    {
        public int Version;
        public ContentInfo AuthSafe;
        public MACData MACData;

        public ContentInfo[] Parts;

        public static PKCS12Store Parse(Asn1Element element, string password)
        {
            var result = new PKCS12Store();

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotInRange(seq.Elements.Count, 2, 3);

            var ver = seq.Elements[0] as Integer;
            ParseException.ThrowIfNull(ver);
            result.Version = (int)ver.Value;

            result.AuthSafe = ContentInfo.Parse(seq.Elements[1]);

            if (seq.Elements.Count == 3)
            {
                result.MACData = MACData.Parse(seq.Elements[2]);

                var generator = new PKCS12v11();
                var hash = AbstractHash.Resolve(result.MACData.MAC.Algorithm.Algorithm);
                var macKey = generator.GenerateParameter(
                    hash,
                    PKCS12v11.IDIntegrityKey,
                    generator.FormatPassword(password),
                    result.MACData.MACSalt,
                    result.MACData.Iterations,
                    hash.HashSize);

                var hmac = new HMAC(hash, macKey);
                var calculatedMAC = hmac.ComputeHash(result.AuthSafe.Data);

                if (!calculatedMAC.SequenceEqual(result.MACData.MAC.Digest))
                    throw new ParseException("Invalid password or corrupted data");
            }

            result.ParseAuthSafe(password);

            return result;
        }

        private void ParseAuthSafe(string password)
        {
            if (AuthSafe.Data != null)
            {
                var element = Asn1Element.ReadFrom(AuthSafe.Data);
                var seq = element as Sequence;
                ParseException.ThrowIfNull(seq);

                Parts = seq.Elements.Select(e => ContentInfo.Parse(e)).ToArray();

                for (int i = 0; i < Parts.Length; i++)
                    Parts[i].Decrypt(password);
            }
            else
                throw new NotImplementedException();
        }
    }
}