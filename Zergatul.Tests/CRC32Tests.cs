using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Zergatul.IO;

namespace Zergatul.Tests
{
    [TestClass]
    public class CRC32Tests
    {
        [TestMethod]
        public void SimpleTest()
        {
            var crc32 = new CRC32(CRC32Parameters.IEEE8023);
            crc32.Update(Encoding.ASCII.GetBytes("123456789"));
            Assert.IsTrue(crc32.GetCheckSum() == 0xCBF43926);
        }
    }
}