using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Math.EllipticCurves.BinaryField;

namespace Zergatul.Math.Tests
{
    [TestClass]
    public class EllipticBinaryFieldTests
    {
        [TestMethod]
        public void Validate_Curves()
        {
            EllipticCurve.sect163k1.g.Validate();
            EllipticCurve.sect163r1.g.Validate();
            EllipticCurve.sect163r2.g.Validate();

            EllipticCurve.sect233k1.g.Validate();
            EllipticCurve.sect233r1.g.Validate();

            EllipticCurve.sect239k1.g.Validate();

            EllipticCurve.sect283k1.g.Validate();
            EllipticCurve.sect283r1.g.Validate();

            EllipticCurve.sect409k1.g.Validate();
            EllipticCurve.sect409r1.g.Validate();

            EllipticCurve.sect571k1.g.Validate();
            EllipticCurve.sect571r1.g.Validate();
        }
    }
}
