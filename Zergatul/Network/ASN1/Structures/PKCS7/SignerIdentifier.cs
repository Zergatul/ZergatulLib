using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1.Structures.PKCS7
{
    class SignerIdentifier
    {
        public IssuerAndSerialNumber IssuerAndSerialNumber { get; private set; }
        public byte[] SubjectKeyIdentifier { get; private set; }

        public static SignerIdentifier Parse(ASN1Element element, SignerInfo info)
        {
            var result = new SignerIdentifier();

            switch (info.Version)
            {
                case CMSVersion.v1:
                    result.IssuerAndSerialNumber = IssuerAndSerialNumber.Parse(element);
                    break;
                case CMSVersion.v3:
                    var octetstr = element as OctetString;
                    ParseException.ThrowIfNull(octetstr);
                    result.SubjectKeyIdentifier = octetstr.Data;
                    break;
                default:
                    throw new ParseException();
            }

            return result;
        }
    }
}