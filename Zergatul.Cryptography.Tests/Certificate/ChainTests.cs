using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Certificate;

namespace Zergatul.Cryptography.Tests.Certificate
{
    [TestClass]
    public class ChainTests
    {
        [TestMethod]
        public void ToolsIetfOrg()
        {
            var certs = new[]
            {
                new X509Certificate("Certificate/tools.ietf.org.crt"),
                new X509Certificate("Certificate/StarfieldSecureCertificateAuthority.crt"),
                new X509Certificate("Certificate/StarfieldRootCertificateAuthority.crt")
            };
            var store = new SimpleRootCertificateStore(certs[2]);
            var chain = X509Tree.Build(certs, store);
            Assert.IsTrue(chain.Validate(certs[0]));
        }
    }
}