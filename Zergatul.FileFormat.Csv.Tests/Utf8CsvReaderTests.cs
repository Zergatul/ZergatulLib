using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Zergatul.FileFormat.Csv.Tests
{
    [TestClass]
    public class Utf8CsvReaderTests
    {
        const string CRLF = "\r\n";
        const string CR = "\r";
        const string LF = "\n";

        [TestMethod]
        public void SimpleTest()
        {
            byte[] data = Encoding.ASCII.GetBytes(
                "abc,qwerty,0" + CRLF +
                "\"\",\"-\"" + CR + 
                "ROFLMAO" + LF + "" +
                "GG");
            var tokens = EnumerateTokens(data).ToArray();

            Assert.IsTrue(tokens.Length == 10);
            Assert.IsTrue(tokens[0] == (CsvTokenType.Data, "abc"));
            Assert.IsTrue(tokens[1] == (CsvTokenType.Data, "qwerty"));
            Assert.IsTrue(tokens[2] == (CsvTokenType.Data, "0"));
            Assert.IsTrue(tokens[3] == (CsvTokenType.EndOfLine, CRLF));
            Assert.IsTrue(tokens[4] == (CsvTokenType.Data, ""));
            Assert.IsTrue(tokens[5] == (CsvTokenType.Data, "-"));
            Assert.IsTrue(tokens[6] == (CsvTokenType.EndOfLine, CR));
            Assert.IsTrue(tokens[7] == (CsvTokenType.Data, "ROFLMAO"));
            Assert.IsTrue(tokens[8] == (CsvTokenType.EndOfLine, LF));
            Assert.IsTrue(tokens[9] == (CsvTokenType.Data, "GG"));
        }

        [TestMethod]
        public void EndTest1()
        {
            byte[] data = Encoding.ASCII.GetBytes("a");
            var tokens = EnumerateTokens(data).ToArray();

            Assert.IsTrue(tokens.Length == 1);
            Assert.IsTrue(tokens[0] == (CsvTokenType.Data, "a"));
        }

        [TestMethod]
        public void EndTest2()
        {
            byte[] data = Encoding.ASCII.GetBytes("\"\"");
            var tokens = EnumerateTokens(data).ToArray();

            Assert.IsTrue(tokens.Length == 1);
            Assert.IsTrue(tokens[0] == (CsvTokenType.Data, ""));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void EndTest3()
        {
            byte[] data = Encoding.ASCII.GetBytes("\"abc");
            var tokens = EnumerateTokens(data).ToArray();
        }

        [TestMethod]
        public void EndTest4()
        {
            byte[] data = Encoding.ASCII.GetBytes(CRLF);
            var tokens = EnumerateTokens(data).ToArray();

            Assert.IsTrue(tokens.Length == 1);
            Assert.IsTrue(tokens[0] == (CsvTokenType.EndOfLine, CRLF));
        }

        [TestMethod]
        public void EndTest6()
        {
            byte[] data = Encoding.ASCII.GetBytes(CR);
            var tokens = EnumerateTokens(data).ToArray();

            Assert.IsTrue(tokens.Length == 1);
            Assert.IsTrue(tokens[0] == (CsvTokenType.EndOfLine, CR));
        }

        [TestMethod]
        public void EndTest7()
        {
            byte[] data = Encoding.ASCII.GetBytes(LF);
            var tokens = EnumerateTokens(data).ToArray();

            Assert.IsTrue(tokens.Length == 1);
            Assert.IsTrue(tokens[0] == (CsvTokenType.EndOfLine, LF));
        }

        [TestMethod]
        public void MultilineTest()
        {
            byte[] data = Encoding.ASCII.GetBytes(
                "abc,\"qwerty" + CRLF + "lol\"");
            var tokens = EnumerateTokens(data).ToArray();

            Assert.IsTrue(tokens.Length == 2);
            Assert.IsTrue(tokens[0] == (CsvTokenType.Data, "abc"));
            Assert.IsTrue(tokens[1] == (CsvTokenType.Data, $"qwerty{CRLF}lol"));
        }

        private static IEnumerable<(CsvTokenType, string)> EnumerateTokens(byte[] data)
        {
            var reader = new Utf8CsvReader();
            for (int i = 0; i < data.Length; i++)
            {
                reader.AddData(data.AsMemory(i, 1));
                while (reader.Read())
                    yield return (reader.TokenType, Encoding.ASCII.GetString(reader.Value.Span));
            }

            if (reader.End())
                yield return (reader.TokenType, Encoding.ASCII.GetString(reader.Value.Span));
        }
    }
}