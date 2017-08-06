using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc7292#appendix-C
    /// </summary>
    public class PKCS12PBEParams
    {
        public byte[] Salt { get; private set; }
        public int Iterations { get; private set; }

        public static PKCS12PBEParams Parse(ASN1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 2);

            var os = seq.Elements[0] as OctetString;
            ParseException.ThrowIfNull(os);

            var @int = seq.Elements[1] as Integer;
            ParseException.ThrowIfNull(@int);

            return new PKCS12PBEParams
            {
                Salt = os.Raw,
                Iterations = (int)@int.Value
            };
        }
    }
}