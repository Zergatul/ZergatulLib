using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.Tests.Hash
{
    [TestClass]
    public class StreebogTests
    {
        [TestMethod]
        public void TestEmpty()
        {
            Assert.IsTrue(Hash256("") ==
                "3f539a213e97c802cc229d474c6aa32a825a360b2a933a949fd925208d9ce1bb");

            Assert.IsTrue(Hash512("") ==
                "8e945da209aa869f0455928529bcae4679e9873ab707b55315f56ceb98bef0a7362f715528356ee83cda5f2aac4c6ad2ba3a715c1bcd81cb8e9f90bf4c1c1a8a");
        }

        [TestMethod]
        public void TestFox()
        {
            Assert.IsTrue(Hash256("The quick brown fox jumps over the lazy dog") ==
                "3e7dea7f2384b6c5a3d0e24aaa29c05e89ddd762145030ec22c71a6db8b2c1f4");
            Assert.IsTrue(Hash256("The quick brown fox jumps over the lazy dog.") ==
                "36816a824dcbe7d6171aa58500741f2ea2757ae2e1784ab72c5c3c6c198d71da");

            Assert.IsTrue(Hash512("The quick brown fox jumps over the lazy dog") ==
                "d2b793a0bb6cb5904828b5b6dcfb443bb8f33efc06ad09368878ae4cdc8245b97e60802469bed1e7c21a64ff0b179a6a1e0bb74d92965450a0adab69162c00fe");
            Assert.IsTrue(Hash512("The quick brown fox jumps over the lazy dog.") ==
                "fe0c42f267d921f940faa72bd9fcf84f9f1bd7e9d055e9816e4c2ace1ec83be82d2957cd59b86e123d8f5adee80b3ca08a017599a9fc1a14d940cf87c77df070");
        }

        [TestMethod]
        public void Test256_1()
        {
            Assert.IsTrue(Hash256(BitHelper.HexToBytes("323130393837363534333231303938373635343332313039383736353433323130393837363534333231303938373635343332313039383736353433323130")) ==
                "00557be5e584fd52a449b16b0251d05d27f94ab76cbaa6da890b59d8ef1e159d");
        }

        [TestMethod]
        public void Test512_1()
        {
            Assert.IsTrue(Hash512(BitHelper.HexToBytes("323130393837363534333231303938373635343332313039383736353433323130393837363534333231303938373635343332313039383736353433323130")) ==
                "486f64c1917879417fef082b3381a4e211c324f074654c38823a7b76f830ad00fa1fbae42b1285c0352f227524bc9ab16254288dd6863dccd5b9f54a1ad0541b");
        }

        private static string Hash256(string input) => Helper.Hash<Streebog256>(input);
        private static string Hash256(byte[] data) => Helper.Hash<Streebog256>(data);
        private static string Hash512(string input) => Helper.Hash<Streebog512>(input);
        private static string Hash512(byte[] data) => Helper.Hash<Streebog512>(data);
    }
}
