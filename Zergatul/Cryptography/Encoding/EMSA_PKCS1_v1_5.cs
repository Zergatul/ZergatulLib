using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Certificate;
using Zergatul.Network.ASN1;
using Zergatul.Network.ASN1.Structures;

namespace Zergatul.Cryptography.Encoding
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc8017#section-9.2
    /// </summary>
    public class EMSA_PKCS1_v1_5
    {
        public AlgorithmIdentifier DigestAlgorithm { get; private set; }
        public byte[] Digest { get; private set; }

        public static EMSA_PKCS1_v1_5 TryParse(byte[] data)
        {
            if (data.Length < 11)
                return null;

            if (data[0] != 0 || data[1] != 1)
                return null;

            int index = 2;
            int psLength = 0;
            while (index < data.Length && data[index] == 0xFF)
            {
                index++;
                psLength++;
            }
            if (psLength < 8)
                return null;

            if (index >= data.Length)
                return null;

            if (data[index] != 0)
                return null;

            index++;

            if (index >= data.Length)
                return null;

            ASN1Element element;

            using (var ms = new MemoryStream(data, index, data.Length - index))
                try
                {
                    element = ASN1Element.ReadFrom(ms);
                    if (ms.Position != ms.Length)
                        return null;
                }
                catch (EndOfStreamException)
                {
                    return null;
                }

            var seq = element as Sequence;
            if (seq == null)
                return null;
            if (seq.Elements.Count != 2)
                return null;

            var ai = AlgorithmIdentifier.Parse(seq.Elements[0]);
            if (ai == null)
                return null;

            if (!(seq.Elements[1] is OctetString))
                return null;

            return new EMSA_PKCS1_v1_5
            {
                DigestAlgorithm = ai,
                Digest = ((OctetString)seq.Elements[1]).Raw
            };
        }
    }
}