using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace Zergatul.Security.Tests.MessageDigest
{
    public abstract class NISTMDTestWithData : NISTMDTest
    {
        [TestMethod]
        public void NistTest()
        {
            List<byte> list = new List<byte>();
            foreach (var line in File.ReadAllLines("MessageDigest/TestCases/NISTData.txt"))
                list.AddRange(BitHelper.HexToBytes(line));
            byte[] data = list.ToArray();

            string[] digests = File.ReadAllLines($"MessageDigest/TestCases/{Algorithm}/NISTData{Size}.txt");

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    int index = 0;
                    for (int i = 0; i < 2048; i++)
                    {
                        if (i % 8 == 0)
                        {
                            var digest = md.Digest(data, index, i / 8);
                            Assert.IsTrue(BitHelper.BytesToHex(digest) == digests[i]);
                            md.Reset();
                        }
                        index += (i + 7) / 8;
                    }
                }
        }
    }
}