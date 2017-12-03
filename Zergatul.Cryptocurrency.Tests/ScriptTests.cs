using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptocurrency.Bitcoin;

namespace Zergatul.Cryptocurrency.Tests
{
    [TestClass]
    public class ScriptTests
    {
        [TestMethod]
        public void P2PKHTest()
        {
            var script = Script.FromHex("76A91489ABCDEFABBAABBAABBAABBAABBAABBAABBAABBA88AC");
            Assert.IsTrue(script.IsPayToPublicKeyHash);
        }

        //[TestMethod]
        //public void P2PKTest()
        //{
        //    var script = Script.FromHex("76A91489ABCDEFABBAABBAABBAABBAABBAABBAABBAABBA88AC");
        //    Assert.IsTrue(script.IsPayToPublicKeyHash);
        //}
    }
}