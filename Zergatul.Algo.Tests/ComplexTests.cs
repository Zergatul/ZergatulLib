using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Algo.Tests
{
    [TestClass]
    public class ComplexTests
    {
        [TestMethod]
        public void MultiplyTest()
        {
            var x = new Complex(0, -1);
            x = x * x;
            Assert.IsTrue(x.Real == -1);
            Assert.IsTrue(x.Imaginary == 0);
        }
    }
}