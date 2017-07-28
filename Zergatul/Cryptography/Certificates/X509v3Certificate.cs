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
        public X509Extension[] Extensions { get; private set; }
        public bool HasPrivateKey => PrivateKey != null;
        public string Issuer { get; private set; }
        public DateTime NotBefore { get; private set; }
        public DateTime NotAfter { get; private set; }
        public PrivateKey PrivateKey { get; private set; }
        public PublicKey PublicKey { get; private set; }
        public byte[] SerialNumber { get; private set; }
        public string SerialNumberString => BitHelper.BytesToHex(SerialNumber).ToUpper();
        public OID SignatureAlgorithm { get; private set; }
        public string Thumbprint { get; private set; }

        public X509v3Certificate(byte[] data)
        {
            using (var ms = new MemoryStream(data))
                ReadFromStream(ms);
        }

        public X509v3Certificate(Stream stream)
        {
            ReadFromStream(stream);
        }

        public X509v3Certificate(string filename)
        {
            using (var fs = File.OpenRead(filename))
                ReadFromStream(fs);
        }

        private void ReadFromStream(Stream stream)
        {
            var asn1 = ASN1Element.ReadFrom(stream);
            var syntax = ASN1CertificateSyntax.FromASN1Element(asn1);

            if (syntax.TbsCertificate.Version.Value != 2)
                throw new NotImplementedException("Only X509 v3 certificates are supported");

            SerialNumber = syntax.TbsCertificate.SerialNumber.Raw;

            NotBefore = syntax.TbsCertificate.Validity.NotBefore.Date;
            NotAfter = syntax.TbsCertificate.Validity.NotAfter.Date;

            if (syntax.TbsCertificate.Issuer.RDN.Any(ra => ra.Any(r => !(r.Value is PrintableString))))
                throw new NotImplementedException("Only Printable String is supported");

            var issuerParts = syntax.TbsCertificate.Issuer.RDN.SelectMany(ra => ra);
            Issuer = string.Join(", ",
                issuerParts.Reverse().Select(r => r.Type.OID.ShortName + "=" + ((PrintableString)r.Value).Value));
        }
    }
}