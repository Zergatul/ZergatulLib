using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1.Structures.PKCS7
{
    class IssuerAndSerialNumber
    {
        public X509.Name Issuer { get; private set; }
        public byte[] SerialNumber { get; private set; }

        public static IssuerAndSerialNumber Parse(Asn1Element element)
        {
            var result = new IssuerAndSerialNumber();

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 2);

            result.Issuer = X509.Name.Parse(seq.Elements[0]);

            var @int = seq.Elements[1] as Integer;
            ParseException.ThrowIfNull(@int);
            result.SerialNumber = @int.Data;

            return result;
        }
    }
}