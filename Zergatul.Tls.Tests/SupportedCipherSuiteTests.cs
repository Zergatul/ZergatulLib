using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Network.Tls;
using Zergatul.Cryptography;
using System.Collections.Generic;
using System.Diagnostics;

namespace Zergatul.Tls.Tests
{
    [TestClass]
    public class SupportedCipherSuiteTests
    {
        [TestMethod]
        public void Test()
        {
            List<CipherSuite> notSupported = new List<CipherSuite>();
            foreach (var cs in TlsStream.SupportedCipherSuites)
            {
                try
                {
                    var obj = AbstractCipherSuite.Resolve(cs, new SecurityParameters(), Role.Client, new DefaultSecureRandom());
                }
                catch (NotImplementedException)
                {
                    notSupported.Add(cs);
                }
            }
            foreach (var cs in notSupported)
                Debug.WriteLine(cs);
            Assert.IsTrue(notSupported.Count == 0);
        }
    }
}