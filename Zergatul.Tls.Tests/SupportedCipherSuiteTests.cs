using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Network.Tls;
using System.Collections.Generic;
using System.Diagnostics;
using Zergatul.Security.Tls;

namespace Zergatul.Tls.Tests
{
    [TestClass]
    public class SupportedCipherSuiteTests
    {
        [TestMethod]
        public void Test()
        {
            List<CipherSuite> notSupported = new List<CipherSuite>();
            foreach (var cs in Network.Tls.TlsStream.SupportedCipherSuites)
            {
                try
                {
                    var obj = CipherSuiteBuilder.Resolve(cs);
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