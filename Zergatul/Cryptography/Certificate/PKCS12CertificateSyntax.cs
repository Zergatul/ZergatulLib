using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.ASN1;
using Zergatul.Network.ASN1.Structures;

namespace Zergatul.Cryptography.Certificate
{
    internal class PKCS12CertificateSyntax
    {
        public int Version;
        public ContentInfo AuthSafe;
        public MACData MacData;

        public static PKCS12CertificateSyntax TryParse(ASN1Element element)
        {
            var result = new PKCS12CertificateSyntax();

            var seq = element as Sequence;
            if (seq == null)
                return null;
            if (seq.Elements.Count < 2 || seq.Elements.Count > 3)
                return null;

            var ver = seq.Elements[0] as Integer;
            if (ver == null)
                return null;
            result.Version = (int)ver.Value;

            result.AuthSafe = ContentInfo.TryParse(seq.Elements[1]);
            if (result.AuthSafe == null)
                return null;

            if (seq.Elements.Count == 3)
            {
                result.MacData = MACData.TryParse(seq.Elements[2]);
                if (result.MacData == null)
                    return null;
            }

            return result;
        }

        #region Nested Classes

        public class MACData
        {
            public DigestInfo MAC;
            public byte[] MACSalt;
            public int? Iterations;

            public static MACData TryParse(ASN1Element element)
            {
                var seq = element as Sequence;
                if (seq == null || seq.Elements.Count < 2)
                    return null;

                var mac = DigestInfo.TryParse(seq.Elements[0]);
                if (mac == null)
                    return null;

                var os = seq.Elements[1] as OctetString;
                if (os == null)
                    return null;

                Integer integer = null;
                if (seq.Elements.Count >= 3)
                {
                    integer = seq.Elements[2] as Integer;
                    if (integer == null)
                        return null;
                }

                return new MACData
                {
                    MAC = mac,
                    MACSalt = os.Raw,
                    Iterations = integer != null ? (int?)(int)integer.Value : null
                };
            }
        }

        #endregion
    }
}