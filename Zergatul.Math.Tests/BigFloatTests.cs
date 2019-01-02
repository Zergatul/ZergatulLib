using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Math.Tests
{
    [TestClass]
    public class BigFloatTests
    {
        [TestMethod]
        public void BigReal_Constructor_Double()
        {
            //var real = new BigReal(0, 100);
            //Assert.IsTrue(real.ToString() == "0");

            //real = new BigReal(1, 100);
            //Assert.IsTrue(real.ToString() == "1");

            //real = new BigReal(2, 100);
            //Assert.IsTrue(real.ToString() == "2");

            //real = new BigReal(-4, 100);
            //Assert.IsTrue(real.ToString() == "-4");

            var real = new BigFloat(1000000, 100);
            Assert.IsTrue(real.ToString() == "1000000");
        }
    }
}