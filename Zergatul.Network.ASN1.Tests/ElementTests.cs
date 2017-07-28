using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Zergatul.Network.ASN1;

namespace Zergatul.Network.ASN1.Tests
{
    [TestClass]
    public class ElementTests
    {
        [TestMethod]
        public void ObjectIdentifier_1()
        {
            var ms = new MemoryStream(new byte[] { 0x06, 0x03, 0x01, 0x85, 0x03 });
            var e = ASN1Element.ReadFrom(ms);

            Assert.IsTrue(e.GetType() == typeof(ObjectIdentifier));
            Assert.IsTrue(((ObjectIdentifier)e).OID.Text == "0.1.643");
        }

        [TestMethod]
        public void ObjectIdentifier_2()
        {
            var ms = new MemoryStream(new byte[] { 0x06, 0x04, 0x01, 0x86, 0xF7, 0x0D });
            var e = ASN1Element.ReadFrom(ms);

            Assert.IsTrue(e.GetType() == typeof(ObjectIdentifier));
            Assert.IsTrue(((ObjectIdentifier)e).OID.Text == "0.1.113549");
        }

        [TestMethod]
        public void ObjectIdentifier_3()
        {
            var ms = new MemoryStream(new byte[] { 0x06, 0x04, 0x01, 0x83, 0x80, 0x00 });
            var e = ASN1Element.ReadFrom(ms);

            Assert.IsTrue(e.GetType() == typeof(ObjectIdentifier));
            Assert.IsTrue(((ObjectIdentifier)e).OID.Text == "0.1.49152");
        }
    }
}