using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1.Structures.X509
{
    class Name
    {
        public RelativeDistinguishedName[] RDN { get; private set; }

        public static Name Parse(ASN1Element element)
        {
            var result = new Name();

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);

            result.RDN = seq.Elements.Select(e => RelativeDistinguishedName.Parse(e)).ToArray();

            return result;
        }
    }
}