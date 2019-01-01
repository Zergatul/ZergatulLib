using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Zergatul.Security.Tests.SymmetricCipher
{
    [TestClass]
    public class AESTests
    {
        private static SecurityProvider[] _providers = new SecurityProvider[]
        {
            new OpenSslProvider()
        };

        private static Regex _regex = new Regex(@"KEY = (?<key>\w+)\s{2}PLAINTEXT = (?<plain>\w+)\s{2}CIPHERTEXT = (?<cipher>\w+)", RegexOptions.Multiline);
        private static Regex _regexIV = new Regex(@"KEY = (?<key>\w+)\s{2}IV = (?<iv>\w+)\s{2}PLAINTEXT = (?<plain>\w+)\s{2}CIPHERTEXT = (?<cipher>\w+)", RegexOptions.Multiline);

        [TestMethod]
        public void TestECB128()
        {
            var text1 = File.ReadAllText("SymmetricCipher/AESTestData/ECBVarKey128.rsp");
            var text2 = File.ReadAllText("SymmetricCipher/AESTestData/ECBVarTxt128.rsp");
            var cases = _regex.Matches(text1).Cast<Match>().Concat(_regex.Matches(text2).Cast<Match>()).Select(m =>
            {
                return new
                {
                    key = BitHelper.HexToBytes(m.Groups["key"].Value),
                    plain = BitHelper.HexToBytes(m.Groups["plain"].Value),
                    cipher = BitHelper.HexToBytes(m.Groups["cipher"].Value)
                };
            }).ToList();

            Assert.IsTrue(cases.Count == 256);

            foreach (var provider in _providers)
            {
                byte[] result = new byte[256];
                foreach (var c in cases)
                {
                    // Encrypt
                    using (var aes = provider.GetSymmetricCipher(SymmetricCiphers.AES))
                    {
                        aes.InitForEncryption(c.key, new SymmetricCipherParameters
                        {
                            Mode = BlockCipherMode.ECB
                        });
                        int len = aes.Update(c.plain, c.plain.Length, result);
                        Assert.IsTrue(len == c.cipher.Length);
                        Assert.IsTrue(result.Take(len).SequenceEqual(c.cipher));
                    }

                    // Decrypt
                    using (var aes = provider.GetSymmetricCipher(SymmetricCiphers.AES))
                    {
                        aes.InitForDecryption(c.key, new SymmetricCipherParameters
                        {
                            Mode = BlockCipherMode.ECB
                        });
                        int len = aes.Update(c.cipher, c.cipher.Length, result);
                        Assert.IsTrue(len == c.plain.Length);
                        Assert.IsTrue(result.Take(len).SequenceEqual(c.plain));
                    }
                }
            }
        }

        [TestMethod]
        public void TestCBC128()
        {
            var text1 = File.ReadAllText("SymmetricCipher/AESTestData/CBCVarKey128.rsp");
            var text2 = File.ReadAllText("SymmetricCipher/AESTestData/CBCVarTxt128.rsp");
            var cases = _regexIV.Matches(text1).Cast<Match>().Concat(_regexIV.Matches(text2).Cast<Match>()).Select(m =>
            {
                return new
                {
                    key = BitHelper.HexToBytes(m.Groups["key"].Value),
                    iv = BitHelper.HexToBytes(m.Groups["iv"].Value),
                    plain = BitHelper.HexToBytes(m.Groups["plain"].Value),
                    cipher = BitHelper.HexToBytes(m.Groups["cipher"].Value)
                };
            }).ToList();

            Assert.IsTrue(cases.Count == 256);

            foreach (var provider in _providers)
            {
                byte[] result = new byte[256];
                foreach (var c in cases)
                {
                    // Encrypt
                    using (var aes = provider.GetSymmetricCipher(SymmetricCiphers.AES))
                    {
                        aes.InitForEncryption(c.key, new SymmetricCipherParameters
                        {
                            Mode = BlockCipherMode.CBC,
                            IV = c.iv
                        });
                        int len = aes.Update(c.plain, c.plain.Length, result);
                        Assert.IsTrue(len == c.cipher.Length);
                        Assert.IsTrue(result.Take(len).SequenceEqual(c.cipher));
                    }

                    // Decrypt
                    using (var aes = provider.GetSymmetricCipher(SymmetricCiphers.AES))
                    {
                        aes.InitForDecryption(c.key, new SymmetricCipherParameters
                        {
                            Mode = BlockCipherMode.CBC,
                            IV = c.iv
                        });
                        int len = aes.Update(c.cipher, c.cipher.Length, result);
                        Assert.IsTrue(len == 16);
                        Assert.IsTrue(result.Take(len).SequenceEqual(c.plain));
                    }
                }
            }
        }
    }
}