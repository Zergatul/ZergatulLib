using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math.Tests
{
    [TestClass]
    public class Int128Tests
    {
        [TestMethod]
        public void ToStringTest()
        {
            Assert.IsTrue(new Int128(0).ToString() == "0");
            Assert.IsTrue(new Int128(1).ToString() == "1");
            Assert.IsTrue(new Int128(-1).ToString() == "-1");
            Assert.IsTrue(new Int128(int.MinValue).ToString() == int.MinValue.ToString());
        }
    }
}