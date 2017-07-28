using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificates
{
    public class X509v3Certificate
    {
        public X509v3Certificate(byte[] data)
        {
            using (var ms = new MemoryStream(data))
                ReadFromStream(ms);
        }

        public X509v3Certificate(Stream stream)
        {
            ReadFromStream(stream);
        }

        private void ReadFromStream(Stream stream)
        {
            ASN1Element asn1 = ASN1Element.ReadFrom(stream);
            if (asn1 is Sequence)
            {
                var certificate = (Sequence)asn1;
                if (certificate.Elements.Count == 3)
                {

                }
            }

            throw new CertificateParseException();
        }

        private bool IsCertificate(ASN1Element element)
        {

        }

        private bool IsSignatureAlgorithm(ASN1Element element)
        {
            if (element is Sequence)
            {
                var ai = (Sequence)element;
                if (ai.Length >= 2)
                    return ai.Elements[0] is ObjectIdentifier;
            }

            return false;
        }
    }
}