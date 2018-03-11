using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Zergatul.Network.Asn1;

namespace Zergatul.Network.Asn1.Tests
{
    [TestClass]
    public class ElementTests
    {
        [TestMethod]
        public void ObjectIdentifier_1()
        {
            var e = Asn1Element.ReadFrom(new byte[] { 0x06, 0x03, 0x01, 0x85, 0x03 });

            Assert.IsTrue(e.GetType() == typeof(ObjectIdentifier));
            Assert.IsTrue(((ObjectIdentifier)e).OID.DotNotation == "0.1.643");
        }

        [TestMethod]
        public void ObjectIdentifier_2()
        {
            var e = Asn1Element.ReadFrom(new byte[] { 0x06, 0x04, 0x01, 0x86, 0xF7, 0x0D });

            Assert.IsTrue(e.GetType() == typeof(ObjectIdentifier));
            Assert.IsTrue(((ObjectIdentifier)e).OID.DotNotation == "0.1.113549");
        }

        [TestMethod]
        public void ObjectIdentifier_3()
        {
            var e = Asn1Element.ReadFrom(new byte[] { 0x06, 0x04, 0x01, 0x83, 0x80, 0x00 });

            Assert.IsTrue(e.GetType() == typeof(ObjectIdentifier));
            Assert.IsTrue(((ObjectIdentifier)e).OID.DotNotation == "0.1.49152");
        }
    }
}