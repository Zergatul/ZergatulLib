using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Certificate;

namespace Zergatul.Cryptography.Tests.Certificate
{
    [TestClass]
    public class ChainTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var certs = new[]
            {
                new X509Certificate("Certificate/tools.ietf.org.crt"),
                new X509Certificate("Certificate/StarfieldSecureCertificateAuthority.crt"),
                new X509Certificate("Certificate/StarfieldRootCertificateAuthority.crt")
            };
            var chain = X509Chain.Build(certs);
        }
    }
}
