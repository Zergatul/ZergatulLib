using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.OpenSsl;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.KeyDerivationFunction
{
    [TestClass]
    public class PBKDF2Tests
    {
        private static SecurityProvider[] _providers = new SecurityProvider[]
        {
            new ZergatulProvider(),
            new OpenSslProvider()
        };

        [TestMethod]
        public void RFCTest1()
        {
            Test("password", "salt", 1, "0c60c80f961f0e71f3a9b524af6012062fe037a6");
        }

        [TestMethod]
        public void RFCTest2()
        {
            Test("password", "salt", 2, "ea6c014dc72d6f8ccd1ed92ace1d41f0d8de8957");
        }

        [TestMethod]
        public void RFCTest3()
        {
            Test("password", "salt", 4096, "4b007901b765489abead49d926f721d065a429c1");
        }

        [TestMethod]
        public void RFCTest4()
        {
            Test("password", "salt", 16777216, "eefe3d61cd4da4e4e9945b3d6ba2158c2634e984");
        }

        [TestMethod]
        public void RFCTest5()
        {
            Test("passwordPASSWORDpassword", "saltSALTsaltSALTsaltSALTsaltSALTsalt", 4096,
                "3d2eec4fe41c849b80c8d83662c0e44a8b291a964cf2f07038");
        }

        [TestMethod]
        public void RFCTest6()
        {
            Test("pass\0word", "sa\0lt", 4096, "56fa6aa75548099dcc37d7f03425e0c3");
        }

        private static void Test(string pwd, string salt, int iterations, string hex)
        {
            byte[] bytes1 = BitHelper.HexToBytes(hex.Replace(" ", ""));

            foreach (var provider in _providers)
            {
                var kdf = provider.GetKeyDerivationFunction(KeyDerivationFunctions.PBKDF2);
                kdf.Init(new PBKDF2Parameters
                {
                    Password = Encoding.ASCII.GetBytes(pwd),
                    Salt = Encoding.ASCII.GetBytes(salt),
                    MessageDigest = "SHA1",
                    Iterations = iterations,
                    KeyLength = bytes1.Length
                });
                byte[] bytes2 = kdf.GetKeyBytes();
                Assert.IsTrue(ByteArray.Equals(bytes1, bytes2));
            };
        }
    }
}