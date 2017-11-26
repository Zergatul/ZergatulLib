using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.IO;
using Zergatul.Network;
using Zergatul.Network.ASN1;
using Zergatul.Network.ASN1.Structures;

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
            using (var ms = new MemoryStream(data))
                ReadFromStreamX509(ms);
        }

        public X509Certificate(Stream stream)
        {
            ReadFromStreamX509(stream);
        }

        public X509Certificate(ASN1Element element)
        {
            ParseX509(element);
        }

        public X509Certificate(string filename, string password = null)
        {
            using (var fs = File.OpenRead(filename))
                switch (Path.GetExtension(filename))
                {
                    case ".cer":
                        ReadFromStreamX509(fs);
                        break;
                    case ".pfx":
                    case ".p12":
                        ReadFromStreamPKCS12(fs, password);
                        break;
                    default:
                        throw new NotSupportedException();
                }
        }

        public bool IsSelfSigned()
        {
            if (Extensions == null || Extensions.Length == 0)
                return true;

            var authKeyId = Extensions.OfType<AuthorityKeyIdentifier>().SingleOrDefault();
            var subjKeyId = Extensions.OfType<SubjectKeyIdentifier>().SingleOrDefault();

            if (authKeyId == null)
                return true;

            if (subjKeyId != null && subjKeyId.KeyIdentifier.SequenceEqual(authKeyId.KeyIdentifier))
                return true;

            return false;
        }

        private void ParseX509(ASN1Element element)
        {
            var syntax = Network.ASN1.Structures.X509.Certificate.Parse(element);

            if (syntax.TBSCertificate.Version != 2)
                throw new NotImplementedException("Only X509 v3 certificates are supported");

            SerialNumber = syntax.TBSCertificate.SerialNumber;
            NotBefore = syntax.TBSCertificate.Validity.NotBefore;
            NotAfter = syntax.TBSCertificate.Validity.NotAfter;

            Issuer = FormatName(syntax.TBSCertificate.Issuer);
            Subject = FormatName(syntax.TBSCertificate.Subject);

            SignatureAlgorithm = syntax.SignatureAlgorithm.Algorithm;

            PublicKey = new PublicKey(syntax.TBSCertificate.SubjectPublicKeyInfo);

            Extensions = syntax.TBSCertificate.Extensions?.Select(ext => X509Extension.Parse(ext)).DefaultIfEmpty().ToArray();
        }

        private void ReadFromStreamX509(Stream stream)
        {
            var interception = new InterceptionStream(stream);

            var asn1 = ASN1Element.ReadFrom(interception);
            ParseX509(asn1);

            this.RawData = interception.ReadBytes.ToArray();
        }

        private void ReadFromStreamPKCS12(Stream stream, string password)
        {
            var asn1 = ASN1Element.ReadFrom(stream);
            var pkcs12 = PKCS12Store.Parse(asn1, password);

            var certs = pkcs12.Parts.SelectMany(p => p.Bags.Where(b => b.CertBag != null)).Select(bag => bag.CertBag.X509Certificate).ToArray();
            var keys = pkcs12.Parts.SelectMany(p => p.Bags.Where(b => b.PKCS8ShroudedKeyBag != null)).Select(bag => bag.PKCS8ShroudedKeyBag.PrivateKey).ToArray();

            if (certs.Length == 0)
                throw new CertificateParseException("PFX store doesn't contain certificate");
            if (certs.Length > 1)
                throw new NotImplementedException();

            ReadFromStreamX509(new MemoryStream(certs[0].RawData));
            if (keys.Length > 1)
                throw new NotImplementedException();

            if (keys.Length == 1)
                PrivateKey = new PrivateKey(this, keys[0]);
        }

        private static string FormatName(Network.ASN1.Structures.X509.Name name)
        {
            return string.Join(", ",
                name.RDN.SelectMany(rdn => rdn.Attributes).Reverse().Select(r => (r.Type.ShortName ?? "OID." + r.Type.DotNotation) + "=" + r.Value.Value));
        }
    }
}