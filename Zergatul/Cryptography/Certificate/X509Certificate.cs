using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zergatul.Cryptography.Certificate.Extensions;
using Zergatul.IO;
using Zergatul.Network;
using Zergatul.Network.Asn1;
using Zergatul.Network.Asn1.Structures;

namespace Zergatul.Cryptography.Certificate
{
    public class X509Certificate
    {
        #region Private static fields

        private const string BeginCertificate = "-----BEGIN CERTIFICATE-----";
        private const string EndCertificate = "-----END CERTIFICATE-----";

        #endregion

        public X509ExtensionsCollection Extensions { get; private set; }
        public bool HasPrivateKey => PrivateKey != null;
        public AttributesCollection Issuer { get; private set; }
        public AttributesCollection Subject { get; private set; }
        public DateTime NotBefore { get; private set; }
        public DateTime NotAfter { get; private set; }
        public PrivateKey PrivateKey { get; private set; }
        public PublicKey PublicKey { get; private set; }
        public byte[] SerialNumber { get; private set; }
        public string SerialNumberString => BitHelper.BytesToHex(SerialNumber).ToUpper();

        public OID SignatureAlgorithm { get; private set; }
        public byte[] Signature { get; private set; }
        public byte[] SignedData { get; private set; }

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

        public X509Certificate(Asn1Element element)
        {
            ParseX509(element);
        }

        public X509Certificate(string filename, string password = null)
        {
            byte[] bytes = File.ReadAllBytes(filename);
            MemoryStream ms;

            switch (Path.GetExtension(filename))
            {
                case ".cer":
                case ".crt":
                    byte[] base64Header = Encoding.ASCII.GetBytes(BeginCertificate);
                    if (ByteArray.IsSubArray(bytes, base64Header, 0))
                    {
                        string base64String = Encoding.ASCII.GetString(bytes);
                        base64String = base64String.Trim();
                        base64String = base64String.Substring(BeginCertificate.Length);
                        if (base64String.EndsWith(EndCertificate))
                            base64String = base64String.Substring(0, base64String.Length - EndCertificate.Length);
                        else
                            throw new InvalidOperationException();
                        bytes = Convert.FromBase64String(base64String);
                    }
                    ms = new MemoryStream(bytes);
                    ReadFromStreamX509(ms);
                    break;
                case ".pfx":
                case ".p12":
                    ms = new MemoryStream(bytes);
                    ReadFromStreamPKCS12(ms, password);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public bool IsSelfSigned()
        {
            if (Extensions.Count == 0)
                return true;

            var authKeyId = Extensions.Get<AuthorityKeyIdentifier>();
            var subjKeyId = Extensions.Get<SubjectKeyIdentifier>();

            if (authKeyId == null)
                return true;

            if (subjKeyId != null && ByteArray.Equals(subjKeyId.KeyIdentifier, authKeyId.KeyIdentifier))
                return true;

            return false;
        }

        // https://tools.ietf.org/html/rfc6125.html
        public bool IsHostAllowed(string host)
        {
            var patterns = new List<string>();

            patterns.AddRange(Subject.GetValuesByOID(OID.JointISOITUT.DS.AttributeType.CommonName));

            var subjAltName = Extensions.Get<SubjectAlternativeName>();
            if (subjAltName != null)
                patterns.AddRange(subjAltName.Names.List.Where(n => n.DNSName != null).Select(n => n.DNSName));

            return patterns.Any(p => IsHostMatchesPattern(host, p));
        }

        #region Private methods

        private void ParseX509(Asn1Element element)
        {
            var syntax = Network.Asn1.Structures.X509.Certificate.Parse(element);

            if (syntax.TBSCertificate.Version != 2)
                throw new NotImplementedException("Only X509 v3 certificates are supported");

            SerialNumber = syntax.TBSCertificate.SerialNumber;
            NotBefore = syntax.TBSCertificate.Validity.NotBefore;
            NotAfter = syntax.TBSCertificate.Validity.NotAfter;

            Issuer = new AttributesCollection(syntax.TBSCertificate.Issuer);
            Subject = new AttributesCollection(syntax.TBSCertificate.Subject);

            SignatureAlgorithm = syntax.SignatureAlgorithm.Algorithm;
            Signature = syntax.SignatureValue;
            SignedData = syntax.TBSCertificateRaw;

            PublicKey = new PublicKey(syntax.TBSCertificate.SubjectPublicKeyInfo);

            Extensions = new X509ExtensionsCollection(syntax.TBSCertificate.Extensions);
        }

        private void ReadFromStreamX509(Stream stream)
        {
            var asn1 = Asn1Element.ReadFrom(stream);
            ParseX509(asn1);

            this.RawData = asn1.Raw;
        }

        private void ReadFromStreamPKCS12(Stream stream, string password)
        {
            var asn1 = Asn1Element.ReadFrom(stream);
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

        // https://tools.ietf.org/html/rfc6125.html#section-6.4.3
        private bool IsHostMatchesPattern(string host, string pattern)
        {
            if (pattern.Contains("*"))
            {
                var r = new Regex(@"^(?<label>[\w*-]+)(?<ending>\.[\w*-.]+)?$");
                var match = r.Match(pattern);
                if (!match.Success)
                    throw new InvalidOperationException("Invalid host name");

                string label = match.Groups["label"].Value;
                string ending = match.Groups["ending"]?.Value;

                // The client SHOULD NOT attempt to match a presented identifier in
                // which the wildcard character comprises a label other than the
                // left - most label(e.g., do not match bar.*.example.net).
                if (ending != null && ending.Contains("*"))
                    return false;

                // If the wildcard character is the only character of the left-most
                // label in the presented identifier, the client SHOULD NOT compare
                // against anything but the left-most label of the reference
                // identifier(e.g., *.example.com would match foo.example.com but
                // not bar.foo.example.com or example.com).
                if (ending != null)
                {
                    if (host.EndsWith(ending))
                    {
                        host = host.Substring(0, host.LastIndexOf(ending));
                    }
                    else
                        return false;
                }

                if (host.Contains("."))
                    return false;

                r = new Regex(label.Replace("*", @"\w*"));
                return r.IsMatch(host);
            }
            else
                return string.Equals(host, pattern, StringComparison.InvariantCulture);
        }

        #endregion
    }
}