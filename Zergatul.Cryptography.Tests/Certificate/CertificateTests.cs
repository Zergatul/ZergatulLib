using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Certificate;
using Zergatul.Network;
using Zergatul.Cryptography.Certificate.Extensions;

namespace Zergatul.Cryptography.Tests.Certificate
{
    [TestClass]
    public class CertificateTests
    {
        [TestMethod]
        public void ToolsIetfOrg()
        {
            var cert = new X509Certificate("Certificate/tools.ietf.org.crt");

            Assert.IsTrue(cert.SerialNumberString == "66F4C727C910EF68");
            Assert.IsTrue(cert.SignatureAlgorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.SHA256WithRSA);

            Assert.IsTrue(cert.NotBefore.ToUniversalTime() == new DateTime(2017, 10, 1, 11, 34, 0, DateTimeKind.Utc));
            Assert.IsTrue(cert.NotAfter.ToUniversalTime() == new DateTime(2018, 11, 30, 23, 34, 19, DateTimeKind.Utc));

            Assert.IsTrue(cert.Issuer["CN"] == "Starfield Secure Certificate Authority - G2");
            Assert.IsTrue(cert.Issuer["OU"] == "http://certs.starfieldtech.com/repository/");
            Assert.IsTrue(cert.Issuer["O"] == "Starfield Technologies, Inc.");
            Assert.IsTrue(cert.Issuer["L"] == "Scottsdale");
            Assert.IsTrue(cert.Issuer["ST"] == "Arizona");
            Assert.IsTrue(cert.Issuer["C"] == "US");

            Assert.IsTrue(cert.Subject["CN"] == "*.tools.ietf.org");
            Assert.IsTrue(cert.Subject["OU"] == "Domain Control Validated");

            Assert.IsTrue(cert.PublicKey.Algorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.RSA);
            Assert.IsTrue(cert.PublicKey.Key.Length == 270);

            Assert.IsTrue(cert.Extensions.Get<BasicConstraints>().Critical);
            Assert.IsFalse(cert.Extensions.Get<BasicConstraints>().CA);

            Assert.IsFalse(cert.Extensions.Get<ExtendedKeyUsage>().Critical);
            Assert.IsTrue(cert.Extensions.Get<ExtendedKeyUsage>().KeyPurposes.SequenceEqual(new OID[]
            {
                OID.ISO.IdentifiedOrganization.DOD.Internet.Security.Mechanisms.PKIX.KP.ServerAuth,
                OID.ISO.IdentifiedOrganization.DOD.Internet.Security.Mechanisms.PKIX.KP.ClientAuth
            }));

            Assert.IsTrue(cert.Extensions.Get<KeyUsage>().Critical);
            Assert.IsTrue(cert.Extensions.Get<KeyUsage>().DigitalSignature);
            Assert.IsTrue(cert.Extensions.Get<KeyUsage>().KeyEncipherment);

            Assert.IsFalse(cert.Extensions.Get<CRLDistributionPoints>().Critical);
            Assert.IsTrue(cert.Extensions.Get<CRLDistributionPoints>()
                .Points
                .SelectMany(p => p.Name.FullName.List.Select(n => n.UniformResourceIdentifier))
                .SequenceEqual(new[] { "http://crl.starfieldtech.com/sfig2s1-67.crl" }));

            Assert.IsFalse(cert.Extensions.Get<CertificatePolicies>().Critical);
            Assert.IsTrue(cert.Extensions.Get<CertificatePolicies>().List[0].PolicyIdentifier.DotNotation == "2.16.840.1.114414.1.7.23.1");
            Assert.IsTrue(cert.Extensions.Get<CertificatePolicies>().List[1].PolicyIdentifier.DotNotation == "2.23.140.1.2.1");

            Assert.IsFalse(cert.Extensions.Get<AuthorityInformationAccess>().Critical);
            Assert.IsTrue(cert.Extensions.Get<AuthorityInformationAccess>().Descriptions[0].AccessMethod ==
                OID.ISO.IdentifiedOrganization.DOD.Internet.Security.Mechanisms.PKIX.AD.OCSP);
            Assert.IsTrue(cert.Extensions.Get<AuthorityInformationAccess>().Descriptions[0].AccessLocation.UniformResourceIdentifier ==
                "http://ocsp.starfieldtech.com/");
            Assert.IsTrue(cert.Extensions.Get<AuthorityInformationAccess>().Descriptions[1].AccessMethod ==
                OID.ISO.IdentifiedOrganization.DOD.Internet.Security.Mechanisms.PKIX.AD.CAIssuers);
            Assert.IsTrue(cert.Extensions.Get<AuthorityInformationAccess>().Descriptions[1].AccessLocation.UniformResourceIdentifier ==
                "http://certificates.starfieldtech.com/repository/sfig2.crt");

            Assert.IsFalse(cert.Extensions.Get<AuthorityKeyIdentifier>().Critical);
            Assert.IsTrue(cert.Extensions.Get<AuthorityKeyIdentifier>().KeyIdentifier.SequenceEqual(
                BitHelper.HexToBytes("254581685026383d3b2d2cbecd6ad9b63db36663")));

            Assert.IsFalse(cert.Extensions.Get<SubjectAlternativeName>().Critical);
            Assert.IsTrue(cert.Extensions.Get<SubjectAlternativeName>().Names.List.Select(n => n.DNSName).SequenceEqual(new[]
            {
                "*.tools.ietf.org",
                "tools.ietf.org"
            }));

            Assert.IsFalse(cert.Extensions.Get<SubjectKeyIdentifier>().Critical);
            Assert.IsTrue(cert.Extensions.Get<SubjectKeyIdentifier>().KeyIdentifier.SequenceEqual(
                BitHelper.HexToBytes("ad8ab41c0751d7928907b0b784622f36557a5f4d")));
        }

        [TestMethod]
        public void StarfieldSecureCertificateAuthority()
        {
            var cert = new X509Certificate("Certificate/StarfieldSecureCertificateAuthority.crt");

            Assert.IsTrue(cert.SerialNumberString == "07");
            Assert.IsTrue(cert.SignatureAlgorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.SHA256WithRSA);

            Assert.IsTrue(cert.NotBefore.ToUniversalTime() == new DateTime(2011, 5, 3, 7, 0, 0, DateTimeKind.Utc));
            Assert.IsTrue(cert.NotAfter.ToUniversalTime() == new DateTime(2031, 5, 3, 7, 0, 0, DateTimeKind.Utc));

            Assert.IsTrue(cert.Issuer["CN"] == "Starfield Root Certificate Authority - G2");
            Assert.IsTrue(cert.Issuer["O"] == "Starfield Technologies, Inc.");
            Assert.IsTrue(cert.Issuer["L"] == "Scottsdale");
            Assert.IsTrue(cert.Issuer["ST"] == "Arizona");
            Assert.IsTrue(cert.Issuer["C"] == "US");

            Assert.IsTrue(cert.Subject["CN"] == "Starfield Secure Certificate Authority - G2");
            Assert.IsTrue(cert.Subject["OU"] == "http://certs.starfieldtech.com/repository/");
            Assert.IsTrue(cert.Subject["O"] == "Starfield Technologies, Inc.");
            Assert.IsTrue(cert.Subject["L"] == "Scottsdale");
            Assert.IsTrue(cert.Subject["ST"] == "Arizona");
            Assert.IsTrue(cert.Subject["C"] == "US");

            Assert.IsTrue(cert.PublicKey.Algorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.RSA);
            Assert.IsTrue(cert.PublicKey.Key.Length == 270);

            Assert.IsTrue(cert.Extensions.Get<BasicConstraints>().Critical);
            Assert.IsTrue(cert.Extensions.Get<BasicConstraints>().CA);

            Assert.IsTrue(cert.Extensions.Get<KeyUsage>().Critical);
            Assert.IsTrue(cert.Extensions.Get<KeyUsage>().KeyCertSign);
            Assert.IsTrue(cert.Extensions.Get<KeyUsage>().CRLSign);

            Assert.IsFalse(cert.Extensions.Get<SubjectKeyIdentifier>().Critical);
            Assert.IsTrue(cert.Extensions.Get<SubjectKeyIdentifier>().KeyIdentifier.SequenceEqual(
                BitHelper.HexToBytes("254581685026383d3b2d2cbecd6ad9b63db36663")));

            Assert.IsFalse(cert.Extensions.Get<AuthorityKeyIdentifier>().Critical);
            Assert.IsTrue(cert.Extensions.Get<AuthorityKeyIdentifier>().KeyIdentifier.SequenceEqual(
                BitHelper.HexToBytes("7c0c321fa7d9307fc47d68a362a8a1ceab075b27")));

            Assert.IsFalse(cert.Extensions.Get<AuthorityInformationAccess>().Critical);
            Assert.IsTrue(cert.Extensions.Get<AuthorityInformationAccess>().Descriptions[0].AccessMethod ==
                OID.ISO.IdentifiedOrganization.DOD.Internet.Security.Mechanisms.PKIX.AD.OCSP);
            Assert.IsTrue(cert.Extensions.Get<AuthorityInformationAccess>().Descriptions[0].AccessLocation.UniformResourceIdentifier ==
                "http://ocsp.starfieldtech.com/");

            Assert.IsFalse(cert.Extensions.Get<CRLDistributionPoints>().Critical);
            Assert.IsTrue(cert.Extensions.Get<CRLDistributionPoints>()
                .Points
                .SelectMany(p => p.Name.FullName.List.Select(n => n.UniformResourceIdentifier))
                .SequenceEqual(new[] { "http://crl.starfieldtech.com/sfroot-g2.crl" }));

            Assert.IsFalse(cert.Extensions.Get<CertificatePolicies>().Critical);
            Assert.IsTrue(cert.Extensions.Get<CertificatePolicies>().List[0].PolicyIdentifier.DotNotation == "2.5.29.32.0");
        }

        [TestMethod]
        public void StarfieldRootCertificateAuthority()
        {
            var cert = new X509Certificate("Certificate/StarfieldRootCertificateAuthority.crt");

            Assert.IsTrue(cert.SerialNumberString == "00");
            Assert.IsTrue(cert.SignatureAlgorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.SHA256WithRSA);

            Assert.IsTrue(cert.NotBefore.ToUniversalTime() == new DateTime(2009, 9, 1, 0, 0, 0, DateTimeKind.Utc));
            Assert.IsTrue(cert.NotAfter.ToUniversalTime() == new DateTime(2037, 12, 31, 23, 59, 59, DateTimeKind.Utc));

            Assert.IsTrue(cert.Issuer["CN"] == "Starfield Root Certificate Authority - G2");
            Assert.IsTrue(cert.Issuer["O"] == "Starfield Technologies, Inc.");
            Assert.IsTrue(cert.Issuer["L"] == "Scottsdale");
            Assert.IsTrue(cert.Issuer["ST"] == "Arizona");
            Assert.IsTrue(cert.Issuer["C"] == "US");

            Assert.IsTrue(cert.Subject["CN"] == "Starfield Root Certificate Authority - G2");
            Assert.IsTrue(cert.Subject["O"] == "Starfield Technologies, Inc.");
            Assert.IsTrue(cert.Subject["L"] == "Scottsdale");
            Assert.IsTrue(cert.Subject["ST"] == "Arizona");
            Assert.IsTrue(cert.Subject["C"] == "US");

            Assert.IsTrue(cert.PublicKey.Algorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.RSA);
            Assert.IsTrue(cert.PublicKey.Key.Length == 270);

            Assert.IsTrue(cert.Extensions.Get<BasicConstraints>().Critical);
            Assert.IsTrue(cert.Extensions.Get<BasicConstraints>().CA);

            Assert.IsTrue(cert.Extensions.Get<KeyUsage>().Critical);
            Assert.IsTrue(cert.Extensions.Get<KeyUsage>().KeyCertSign);
            Assert.IsTrue(cert.Extensions.Get<KeyUsage>().CRLSign);

            Assert.IsFalse(cert.Extensions.Get<SubjectKeyIdentifier>().Critical);
            Assert.IsTrue(cert.Extensions.Get<SubjectKeyIdentifier>().KeyIdentifier.SequenceEqual(
                BitHelper.HexToBytes("7c0c321fa7d9307fc47d68a362a8a1ceab075b27")));
        }
    }
}