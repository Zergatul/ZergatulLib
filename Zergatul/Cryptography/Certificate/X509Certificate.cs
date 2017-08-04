using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificate
{
    public class X509Certificate
    {
        public X509Extension[] Extensions { get; private set; }
        public bool HasPrivateKey => PrivateKey != null;
        public string Issuer { get; private set; }
        public string Subject { get; private set; }
        public DateTime NotBefore { get; private set; }
        public DateTime NotAfter { get; private set; }
        public PrivateKey PrivateKey { get; private set; }
        public PublicKey PublicKey { get; private set; }
        public byte[] SerialNumber { get; private set; }
        public string SerialNumberString => BitHelper.BytesToHex(SerialNumber).ToUpper();
        public OID SignatureAlgorithm { get; private set; }
        public string Thumbprint { get; private set; }

        public byte[] RawData { get; private set; }

        public X509Certificate(byte[] data)
        {
            RawData = data;

            using (var ms = new MemoryStream(data))
                ReadFromStreamX509(ms);
        }

        public X509Certificate(Stream stream)
        {
            ReadFromStreamX509(stream);
        }

        public X509Certificate(string filename)
        {
            using (var fs = File.OpenRead(filename))
                switch (Path.GetExtension(filename))
                {
                    case ".cer":
                        ReadFromStreamX509(fs);
                        break;
                    case ".p12":
                        ReadFromStreamPKCS12(fs);
                        break;
                    default:
                        throw new NotSupportedException();
                }
        }

        private void ReadFromStreamX509(Stream stream)
        {
            var asn1 = ASN1Element.ReadFrom(stream);
            var syntax = X509CertificateSyntax.TryParse(asn1);

            if (syntax.TbsCertificate.Version != null && syntax.TbsCertificate.Version.Value != 2)
                throw new NotImplementedException("Only X509 v3 certificates are supported");

            SerialNumber = syntax.TbsCertificate.SerialNumber.Raw;

            NotBefore = syntax.TbsCertificate.Validity.NotBefore.Date;
            NotAfter = syntax.TbsCertificate.Validity.NotAfter.Date;

            Issuer = FormatName(syntax.TbsCertificate.Issuer);
            Subject = FormatName(syntax.TbsCertificate.Subject);

            SignatureAlgorithm = syntax.SignatureAlgorithm.Algorithm;

            PublicKey = new PublicKey(syntax.TbsCertificate.SubjectPublicKeyInfo);

            Extensions = syntax.TbsCertificate.Extensions?.Select(ext => X509Extension.Parse(ext)).DefaultIfEmpty().ToArray();
        }

        private void ReadFromStreamPKCS12(Stream stream)
        {
            var asn1 = ASN1Element.ReadFrom(stream);
            var syntax = PKCS12CertificateSyntax.TryParse(asn1);

            var data = ASN1Element.ReadFrom(syntax.AuthSafe.Data);
        }

        private static string FormatName(X509CertificateSyntax.Name name)
        {
            var parts = name.RDN.SelectMany(ra => ra);
            return string.Join(", ",
                parts.Reverse().Select(r => (r.Type.OID.ShortName ?? "OID." + r.Type.OID.DotNotation) + "=" + r.Value.Value));
        }
    }
}