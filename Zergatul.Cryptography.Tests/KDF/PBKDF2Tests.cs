using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Hash;
using Zergatul.Cryptography.KDF;
using System.Text;

namespace Zergatul.Cryptography.Tests.KDF
{
    // https://tools.ietf.org/html/rfc6070
    [TestClass]
    public class PBKDF2Tests
    {
        [TestMethod]
        public void PBKDF2_Vector1()
        {
            Test("password", "salt", 1, 20, "0c60c80f961f0e71f3a9b524af6012062fe037a6");
        }

        [TestMethod]
        public void PBKDF2_Vector2()
        {
            Test("password", "salt", 2, 20, "ea6c014dc72d6f8ccd1ed92ace1d41f0d8de8957");
        }

        [TestMethod]
        public void PBKDF2_Vector3()
        {
            Test("password", "salt", 4096, 20, "4b007901b765489abead49d926f721d065a429c1");
        }

        [TestMethod]
        public void PBKDF2_Vector4()
        {
            Test("password", "salt", 16777216, 20, "eefe3d61cd4da4e4e9945b3d6ba2158c2634e984");
        }

        [TestMethod]
        public void PBKDF2_Vector5()
        {
            Test("passwordPASSWORDpassword", "saltSALTsaltSALTsaltSALTsaltSALTsalt", 4096, 25,
                "3d2eec4fe41c849b80c8d83662c0e44a8b291a964cf2f07038");
        }

        [TestMethod]
        public void PBKDF2_Vector6()
        {
            Test("pass\0word", "sa\0lt", 4096, 16, "56fa6aa75548099dcc37d7f03425e0c3");
        }

        private static void Test(string pwd, string salt, int iters, int len, string result)
        {
            var pbkdf2 = new PBKDF2(new SHA1());
            byte[] keys = pbkdf2.DeriveKeyBytes(
                Encoding.ASCII.GetBytes(pwd),
                Encoding.ASCII.GetBytes(salt),
                iters, (ulong)len);
            Assert.IsTrue(ByteArray.Equals(keys, BitHelper.HexToBytes(result)));
        }
    }
}