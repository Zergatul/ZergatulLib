using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Zergatul.Security.Tests.MessageDigest
{
    public abstract class NISTMDTest
    {
        protected abstract SecurityProvider[] Providers { get; }
        protected abstract string Name { get; }
        protected abstract string Algorithm { get; }
        protected abstract int Size { get; }

        protected virtual int ShortTestsCount => 2048;
        protected virtual int LongTestsCount => 513;

        [TestMethod]
        public void NIST_Short()
        {
            var text = File.ReadAllText($"MessageDigest/TestCases/{Algorithm}/ShortMsgKAT_{Size}.txt");
            var r = new Regex(@"Len = (?<len>\d+)\s+Msg = (?<msg>[0-9A-F]+)\s+MD = (?<MD>[0-9A-F]+)", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var matches = r.Matches(text);
                    Assert.IsTrue(matches.Count == ShortTestsCount);
                    foreach (Match m in matches)
                    {
                        int len = int.Parse(m.Groups["len"].Value);
                        if (len % 8 != 0)
                            continue;

                        var digest = md.Digest(BitHelper.HexToBytes(m.Groups["msg"].Value).Take(len / 8).ToArray());
                        Assert.IsTrue(string.Equals(BitHelper.BytesToHex(digest), m.Groups["MD"].Value, StringComparison.InvariantCultureIgnoreCase));
                        md.Reset();
                    }
                }
        }

        [TestMethod]
        public void NIST_Long()
        {
            var text = File.ReadAllText($"MessageDigest/TestCases/{Algorithm}/LongMsgKAT_{Size}.txt");
            var r = new Regex(@"Len = (?<len>\d+)\s+Msg = (?<msg>[0-9A-F]+)\s+MD = (?<MD>[0-9A-F]+)", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var matches = r.Matches(text);
                    Assert.IsTrue(matches.Count == LongTestsCount);
                    foreach (Match m in matches)
                    {
                        if (int.Parse(m.Groups["len"].Value) % 8 != 0)
                            continue;

                        var digest = md.Digest(BitHelper.HexToBytes(m.Groups["msg"].Value));
                        Assert.IsTrue(string.Equals(BitHelper.BytesToHex(digest), m.Groups["MD"].Value, StringComparison.InvariantCultureIgnoreCase));
                        md.Reset();
                    }
                }
        }

        [TestMethod]
        public virtual void NIST_MonteCarlo()
        {
            var text = File.ReadAllText($"MessageDigest/TestCases/{Algorithm}/MonteCarlo_{Size}.txt");

            var seed = BitHelper.HexToBytes(Regex.Match(text, @"Seed = ([0-9A-F]+)", RegexOptions.IgnoreCase).Groups[1].Value);
            var r = new Regex(@"(j|count) = \d+\s+MD = (?<MD>[0-9A-F]+)", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var matches = r.Matches(text);
                    Assert.IsTrue(matches.Count == 100);

                    byte[] msg = seed;
                    for (int i = 0; i < 100; i++)
                    {
                        byte[] hash = null;
                        for (int j = 0; j < 1000; j++)
                        {
                            hash = md.Digest(msg);
                            md.Reset();
                            byte[] temp = ByteArray.SubArray(msg, 0, 128 - hash.Length);
                            msg = ByteArray.Concat(hash, temp);
                        }

                        Assert.IsTrue(ByteArray.Equals(hash, BitHelper.HexToBytes(matches[i].Groups["MD"].Value)));
                    }
                }
        }

        [TestMethod]
        public virtual void NIST_ExtremelyLong()
        {
            var text = File.ReadAllText($"MessageDigest/TestCases/{Algorithm}/ExtremelyLongMsgKAT_{Size}.txt");
            var r = new Regex(@"Repeat = (?<repeat>\d+)\s+Text = (?<text>.+?)\s+MD = (?<hash>[0-9A-F]+)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var m = r.Match(text);
            byte[] data = Encoding.ASCII.GetBytes(m.Groups["text"].Value);

            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    int repeat = int.Parse(m.Groups["repeat"].Value);
                    for (int i = 0; i < repeat; i++)
                        md.Update(data);

                    byte[] digest = md.Digest();

                    Assert.IsTrue(ByteArray.Equals(digest, BitHelper.HexToBytes(m.Groups["hash"].Value)));
                }
        }
    }
}