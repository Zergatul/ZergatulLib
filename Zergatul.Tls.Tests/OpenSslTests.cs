using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security;

namespace Zergatul.Tls.Tests
{
    [TestClass]
    public class OpenSslTests
    {
        static SecurityProvider _openssl = new OpenSslProvider();
        static SecurityProvider _default = new DefaultSecurityProvider();

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}