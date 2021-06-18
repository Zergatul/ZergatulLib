using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Algo.NumberTheory;

namespace Zergatul.Algo.Tests.NumberTheory
{
    [TestClass]
    public class JacobiSymbolTests
    {
        [TestMethod]
        public void Test1()
        {
            Assert.IsTrue(JacobiSymbol.Calculate(158, 235) == -1);
        }
    }
}