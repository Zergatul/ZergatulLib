using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Zergatul.Algo.NumberTheory;

namespace Zergatul.Algo.Tests.NumberTheory
{
    [TestClass]
    public class PrimeTests
    {
        [TestMethod]
        public void MillerRabinTest()
        {
            var rnd = new Random();

            Assert.IsFalse(Prime.MillerRabinProbablePrimeTest(121, 5, rnd));
            Assert.IsFalse(Prime.MillerRabinProbablePrimeTest(123, 5, rnd));
            Assert.IsFalse(Prime.MillerRabinProbablePrimeTest(12345678901, 5, rnd));
            Assert.IsFalse(Prime.MillerRabinProbablePrimeTest(12345678903, 5, rnd));
            Assert.IsFalse(Prime.MillerRabinProbablePrimeTest(12345678907, 5, rnd));
            Assert.IsFalse(Prime.MillerRabinProbablePrimeTest(12345678909, 5, rnd));
            Assert.IsFalse(Prime.MillerRabinProbablePrimeTest(12345678911, 5, rnd));
            Assert.IsFalse(Prime.MillerRabinProbablePrimeTest(12345678913, 5, rnd));
            Assert.IsFalse(Prime.MillerRabinProbablePrimeTest(12345678917, 5, rnd));
            Assert.IsFalse(Prime.MillerRabinProbablePrimeTest(12345678919, 5, rnd));
            Assert.IsFalse(Prime.MillerRabinProbablePrimeTest(12345678921, 5, rnd));
            

            Assert.IsTrue(Prime.MillerRabinProbablePrimeTest(127, 5, rnd));
            Assert.IsTrue(Prime.MillerRabinProbablePrimeTest(12345678923, 20, rnd));
        }
    }
}