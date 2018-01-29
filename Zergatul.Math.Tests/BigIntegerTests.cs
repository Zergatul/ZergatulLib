using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zergatul.Math;
using Zergatul;

namespace Zergatul.Math.Tests
{
    [TestClass]
    public class BigIntegerTests
    {
        #region Constructors

        //[TestMethod]
        //public void ConstructorFromBytesBE_1()
        //{
        //    var bi = new BigInteger(new byte[] { 0, 0, 0, 0 }, ByteOrder.BigEndian);
        //    Assert.IsTrue(bi._words.Length == 0);
        //}

        //[TestMethod]
        //public void ConstructorFromBytesBE_2()
        //{
        //    var bi = new BigInteger(Enumerable.Repeat((byte)0, 1000).Concat(new byte[] { 2 }).ToArray(), ByteOrder.BigEndian);
        //    Assert.IsTrue(bi._words.Length == 1);
        //    Assert.IsTrue(bi._words[0] == 2);
        //}

        //[TestMethod]
        //public void ConstructorFromBytesBE_3()
        //{
        //    var bi = new BigInteger(Enumerable.Repeat((byte)0, 1000).Concat(new byte[] { 1, 0, 0, 0, 2 }).ToArray(), ByteOrder.BigEndian);
        //    Assert.IsTrue(bi._words.Length == 2);
        //    Assert.IsTrue(bi._words[0] == 2);
        //    Assert.IsTrue(bi._words[1] == 1);
        //}

        //[TestMethod]
        //public void ConstructorFromBytesBE_4()
        //{
        //    var bi = new BigInteger(Enumerable.Repeat((byte)0, 1000).Concat(new byte[] { 1, 0, 0, 0, 0, 2 }).ToArray(), ByteOrder.BigEndian);
        //    Assert.IsTrue(bi._words.Length == 2);
        //    Assert.IsTrue(bi._words[0] == 2);
        //    Assert.IsTrue(bi._words[1] == 256);
        //}

        //[TestMethod]
        //public void ConstructorFromBytesBE_5()
        //{
        //    var bi = new BigInteger(Enumerable.Repeat((byte)0, 1000).Concat(new byte[] { 1, 0, 0, 0, 0, 0, 2 }).ToArray(), ByteOrder.BigEndian);
        //    Assert.IsTrue(bi._words.Length == 2);
        //    Assert.IsTrue(bi._words[0] == 2);
        //    Assert.IsTrue(bi._words[1] == 256 * 256);
        //}

        //[TestMethod]
        //public void ConstructorFromBytesBE_6()
        //{
        //    var bi = new BigInteger(Enumerable.Repeat((byte)0, 1000).Concat(new byte[] { 1, 0, 0, 0, 0, 0, 0, 2 }).ToArray(), ByteOrder.BigEndian);
        //    Assert.IsTrue(bi._words.Length == 2);
        //    Assert.IsTrue(bi._words[0] == 2);
        //    Assert.IsTrue(bi._words[1] == 256 * 256 * 256);
        //}

        //[TestMethod]
        //public void ConstructorFromBytesLE_1()
        //{
        //    var bi = new BigInteger(new byte[] { 0, 0, 0, 0 }, ByteOrder.LittleEndian);
        //    Assert.IsTrue(bi._words.Length == 0);
        //}

        //[TestMethod]
        //public void ConstructorFromBytesLE_2()
        //{
        //    var bi = new BigInteger(new byte[] { 2 }.Concat(Enumerable.Repeat((byte)0, 1000)).ToArray(), ByteOrder.LittleEndian);
        //    Assert.IsTrue(bi._words.Length == 1);
        //    Assert.IsTrue(bi._words[0] == 2);
        //}

        //[TestMethod]
        //public void ConstructorFromBytesLE_3()
        //{
        //    var bi = new BigInteger(new byte[] { 2, 0, 0, 0, 1 }.Concat(Enumerable.Repeat((byte)0, 1000)).ToArray(), ByteOrder.LittleEndian);
        //    Assert.IsTrue(bi._words.Length == 2);
        //    Assert.IsTrue(bi._words[0] == 2);
        //    Assert.IsTrue(bi._words[1] == 1);
        //}

        //[TestMethod]
        //public void ConstructorFromBytesLE_4()
        //{
        //    var bi = new BigInteger(new byte[] { 2, 0, 0, 0, 0, 1 }.Concat(Enumerable.Repeat((byte)0, 1000)).ToArray(), ByteOrder.LittleEndian);
        //    Assert.IsTrue(bi._words.Length == 2);
        //    Assert.IsTrue(bi._words[0] == 2);
        //    Assert.IsTrue(bi._words[1] == 256);
        //}

        //[TestMethod]
        //public void ConstructorFromBytesLE_5()
        //{
        //    var bi = new BigInteger(new byte[] { 2, 0, 0, 0, 0, 0, 1 }.Concat(Enumerable.Repeat((byte)0, 1000)).ToArray(), ByteOrder.LittleEndian);
        //    Assert.IsTrue(bi._words.Length == 2);
        //    Assert.IsTrue(bi._words[0] == 2);
        //    Assert.IsTrue(bi._words[1] == 256 * 256);
        //}

        //[TestMethod]
        //public void ConstructorFromBytesLE_6()
        //{
        //    var bi = new BigInteger(new byte[] { 2, 0, 0, 0, 0, 0, 0, 1 }.Concat(Enumerable.Repeat((byte)0, 1000)).ToArray(), ByteOrder.LittleEndian);
        //    Assert.IsTrue(bi._words.Length == 2);
        //    Assert.IsTrue(bi._words[0] == 2);
        //    Assert.IsTrue(bi._words[1] == 256 * 256 * 256);
        //}

        [TestMethod]
        public void ConstructorFromInt32()
        {
            var bi = new BigInteger(int.MinValue);
            Assert.IsTrue(bi.ToString() == "-2147483648");
        }

        [TestMethod]
        public void ConstructorFromInt64()
        {
            var bi = new BigInteger(long.MinValue);
            Assert.IsTrue(bi.ToString() == "-9223372036854775808");

            bi = new BigInteger((long)uint.MaxValue);
            Assert.IsTrue(bi.ToString() == "4294967295");

            bi = new BigInteger((long)uint.MaxValue + 1);
            Assert.IsTrue(bi.ToString() == "4294967296");
        }

        #endregion

        #region ToBytes

        [TestMethod]
        public void ToBytes_1()
        {
            var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Assert.IsTrue(data.SequenceEqual(new BigInteger(data, ByteOrder.BigEndian).ToBytes(ByteOrder.BigEndian)));
            Assert.IsTrue(data.SequenceEqual(new BigInteger(data, ByteOrder.LittleEndian).ToBytes(ByteOrder.LittleEndian)));

            data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Assert.IsTrue(data.SequenceEqual(new BigInteger(data, ByteOrder.BigEndian).ToBytes(ByteOrder.BigEndian)));
            Assert.IsTrue(data.SequenceEqual(new BigInteger(data, ByteOrder.LittleEndian).ToBytes(ByteOrder.LittleEndian)));

            data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            Assert.IsTrue(data.SequenceEqual(new BigInteger(data, ByteOrder.BigEndian).ToBytes(ByteOrder.BigEndian)));
            Assert.IsTrue(data.SequenceEqual(new BigInteger(data, ByteOrder.LittleEndian).ToBytes(ByteOrder.LittleEndian)));

            data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            Assert.IsTrue(data.SequenceEqual(new BigInteger(data, ByteOrder.BigEndian).ToBytes(ByteOrder.BigEndian)));
            Assert.IsTrue(data.SequenceEqual(new BigInteger(data, ByteOrder.LittleEndian).ToBytes(ByteOrder.LittleEndian)));
        }

        [TestMethod]
        public void ToBytes_2()
        {
            var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Assert.IsTrue(Enumerable.Repeat((byte)0, 10).Concat(data).SequenceEqual(new BigInteger(data, ByteOrder.BigEndian).ToBytes(ByteOrder.BigEndian, data.Length + 10)));
            Assert.IsTrue(data.Concat(Enumerable.Repeat((byte)0, 10)).SequenceEqual(new BigInteger(data, ByteOrder.LittleEndian).ToBytes(ByteOrder.LittleEndian, data.Length + 10)));
        }

        #endregion

        #region ToString

        #region Base 2

        [TestMethod]
        public void ToString2_1()
        {
            var bi = new BigInteger(new byte[] { 255, 0, 0, 0, 0, 255 }, ByteOrder.BigEndian);
            Assert.IsTrue(bi.ToString(2) == Convert.ToString(0xFF00000000FF, 2));
        }

        [TestMethod]
        public void ToString2_2()
        {
            var bi = new BigInteger(new byte[] { 255, 0, 0, 255 }, ByteOrder.BigEndian);
            Assert.IsTrue(bi.ToString(2) == Convert.ToString(0xFF0000FF, 2));
        }

        #endregion

        #region Base 4

        [TestMethod]
        public void ToString4_1()
        {
            var bi = BigInteger.Parse("86780890812314");
            Assert.IsTrue(bi.ToString(4) == "103232311000022320332122");
        }

        [TestMethod]
        public void ToString4_2()
        {
            var bi = BigInteger.Parse("86780890812314037509314750918615613984561");
            Assert.IsTrue(bi.ToString(4) == "33330012223321330201121330203303303102320320021331231210223210330301");
        }

        #endregion

        #region Base 10

        [TestMethod]
        public void ToString10_1()
        {
            var bi = new BigInteger(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 }, ByteOrder.BigEndian);
            Assert.IsTrue(bi.ToString() == "18446744073709551616");
        }

        [TestMethod]
        public void ToString10_2()
        {
            var bi = new BigInteger(new byte[] { 1, 0, 0, 0, 0 }, ByteOrder.BigEndian);
            Assert.IsTrue(bi.ToString() == "4294967296");
        }

        [TestMethod]
        public void ToString10_3()
        {
            var bi = new BigInteger(new byte[] { 11 }, ByteOrder.BigEndian);
            Assert.IsTrue(bi.ToString() == "11");
        }

        [TestMethod]
        public void ToString10_4()
        {
            var bi = new BigInteger(BitHelper.HexToBytes("05e9c3dd0c07aac76179ebc76a6c78d4d67c6c160a7f297f31"), ByteOrder.BigEndian);
            Assert.IsTrue(bi.ToString() == "37117412046184165215328310413872196888161402012543360794417");
        }

        #endregion

        #region Base 15

        [TestMethod]
        public void ToString15_1()
        {
            var bi = BigInteger.Parse("15");
            Assert.IsTrue(bi.ToString(15).ToUpper() == "10");
        }

        [TestMethod]
        public void ToString15_5()
        {
            var bi = BigInteger.Parse("86780890812314037509314750918615613984561");
            Assert.IsTrue(bi.ToString(15).ToUpper() == "8E1658DA57EB9D352DDAD5C4ACC6D33B80B");
        }

        #endregion

        #region Base 16

        [TestMethod]
        public void ToString16_1()
        {
            var bi = new BigInteger(new byte[] { 1, 0, 0, 0, 0 }, ByteOrder.BigEndian);
            Assert.IsTrue(bi.ToString(16) == "100000000");
        }

        #endregion

        #endregion

        #region Parse

        [TestMethod]
        public void ContructorFromString_Zero()
        {
            string number = new string('0', 1000);
            for (int radix = 2; radix <= 36; radix++)
                Assert.IsTrue(BigInteger.Parse(number, radix) == BigInteger.Zero);
        }

        [TestMethod]
        public void ContructorFromString10_1()
        {
            string number = "1234567890";
            var bi = BigInteger.Parse(number);
            Assert.IsTrue(bi.ToString() == number);
        }

        [TestMethod]
        public void ContructorFromString10_2()
        {
            string number = "1" + new string('5', 20000);
            var bi = BigInteger.Parse(number);
            Assert.IsTrue(bi.ToString() == number);
        }

        [TestMethod]
        public void ContructorFromString2_1()
        {
            string number = "100000000000000000000000000000000";
            var bi = BigInteger.Parse(number, 2);
            Assert.IsTrue(bi.ToString() == System.Math.Pow(2, 32).ToString());
        }

        //[TestMethod]
        //public void ContructorFromString16_1()
        //{
        //    string number = "FFFFFFFFFFFF";
        //    var bi = BigInteger.Parse(number, 16);

        //    Assert.IsTrue(bi._words.Length == 2);
        //    Assert.IsTrue(bi._words[0] == 0xFFFFFFFF);
        //    Assert.IsTrue(bi._words[1] == 0xFFFF);
        //}

        #endregion

        #region BitSizeOfArray

        //[TestMethod]
        //public void BitSizeOfArray_1()
        //{
        //    Assert.IsTrue(BigInteger.BitSizeOfArray(new byte[0], ByteOrder.BigEndian) == 0);
        //    Assert.IsTrue(BigInteger.BitSizeOfArray(new byte[10], ByteOrder.BigEndian) == 0);
        //}

        //[TestMethod]
        //public void BitSizeOfArray_2()
        //{
        //    Assert.IsTrue(BigInteger.BitSizeOfArray(BitHelper.HexToBytes("80"), ByteOrder.BigEndian) == 8);
        //    Assert.IsTrue(BigInteger.BitSizeOfArray(BitHelper.HexToBytes("7f"), ByteOrder.BigEndian) == 7);

        //    Assert.IsTrue(BigInteger.BitSizeOfArray(BitHelper.HexToBytes("00800000"), ByteOrder.BigEndian) == 24);
        //    Assert.IsTrue(BigInteger.BitSizeOfArray(BitHelper.HexToBytes("00007f0000"), ByteOrder.BigEndian) == 23);
        //}

        #endregion

        #region Equals

        [TestMethod]
        public void Equals_1()
        {
            Assert.IsTrue(new BigInteger(1) == new BigInteger(1));
        }

        [TestMethod]
        public void Equals_2()
        {
            Assert.IsTrue(new BigInteger(1) != BigInteger.Zero);
        }

        [TestMethod]
        public void Equals_3()
        {
            int zeros = 10000;
            var bi1 = BigInteger.Parse("1" + new string('0', zeros));
            var bi2 = BigInteger.Parse("1" + new string('0', zeros));
            Assert.IsTrue(bi1 == bi2);
        }

        [TestMethod]
        public void Equals_4()
        {
            int zeros = 10000;
            var bi1 = BigInteger.Parse("1" + new string('0', zeros));
            var bi2 = BigInteger.Parse("1" + new string('0', zeros - 1) + "1");
            Assert.IsTrue(bi1 != bi2);
        }

        #endregion

        #region CompareTo BigInteger

        [TestMethod]
        public void CompareTo_1()
        {
            Assert.IsTrue(BigInteger.Zero.CompareTo(BigInteger.Zero) == 0);
        }

        [TestMethod]
        public void CompareTo_2()
        {
            Assert.IsTrue(new BigInteger(123).CompareTo(new BigInteger(124)) < 0);
        }

        [TestMethod]
        public void CompareTo_3()
        {
            Assert.IsTrue(new BigInteger(123).CompareTo(new BigInteger(122)) > 0);
        }

        [TestMethod]
        public void CompareTo_4()
        {
            string hex1 = "1" + new string('F', 200000) + "A";
            string hex2 = "1" + new string('F', 200000) + "B";
            Assert.IsTrue(BigInteger.Parse(hex1, 16).CompareTo(BigInteger.Parse(hex2, 16)) < 0);
            Assert.IsTrue(BigInteger.Parse(hex2, 16).CompareTo(BigInteger.Parse(hex1, 16)) > 0);
        }

        #endregion

        #region CompareTo Int32

        [TestMethod]
        public void CompareToInt32_1()
        {
            Assert.IsTrue(BigInteger.Zero.CompareTo(0) == 0);
            Assert.IsTrue(BigInteger.Zero.CompareTo(1) == -1);
            Assert.IsTrue(BigInteger.Zero.CompareTo(-1) == 1);
        }

        [TestMethod]
        public void CompareToInt32_2()
        {
            Assert.IsTrue(BigInteger.One.CompareTo(0) == 1);
            Assert.IsTrue(BigInteger.One.AdditiveInverse().CompareTo(0) == -1);
            Assert.IsTrue(BigInteger.Parse("12312314234125125").CompareTo(0) == 1);
            Assert.IsTrue(BigInteger.Parse("-12312314234125125").CompareTo(0) == -1);
        }

        [TestMethod]
        public void CompareToInt32_3()
        {
            Assert.IsTrue(BigInteger.One.CompareTo(1) == 0);
            Assert.IsTrue(BigInteger.One.AdditiveInverse().CompareTo(1) == -1);
            Assert.IsTrue(BigInteger.Parse("12312314234125125").CompareTo(1) == 1);
            Assert.IsTrue(BigInteger.Parse("-12312314234125125").CompareTo(1) == -1);
        }

        [TestMethod]
        public void CompareToInt32_4()
        {
            Assert.IsTrue(BigInteger.One.CompareTo(-1) == 1);
            Assert.IsTrue(BigInteger.One.AdditiveInverse().CompareTo(-1) == 0);
            Assert.IsTrue(BigInteger.Parse("12312314234125125").CompareTo(-1) == 1);
            Assert.IsTrue(BigInteger.Parse("-12312314234125125").CompareTo(-1) == -1);
        }

        #endregion

        #region Shift Left

        [TestMethod]
        public void ShiftLeft_1()
        {
            var bin = "1010101010011001000100101010100100101001010101010101010010100101000011110001010101010101010";
            var value1 = BigInteger.Parse(bin, 2);
            var value2 = BigInteger.Parse(bin + new string('0', 104), 2);
            Assert.IsTrue((value1 << 104) == value2);
        }

        #endregion

        #region Shift Left

        [TestMethod]
        public void ShiftRight_1()
        {
            var value1 = new BigInteger(new uint[] { 0x1, 0x0, 0x0, 0x20 }, ByteOrder.BigEndian);
            var value2 = new BigInteger(new uint[] { 0x80000000, 0x0, 0x10 }, ByteOrder.BigEndian);
            Assert.IsTrue((value1 >> 1) == value2);
        }

        #endregion

        #region Add

        [TestMethod]
        public void Add_1()
        {
            Assert.IsTrue(BigInteger.Zero + BigInteger.Zero == BigInteger.Zero);
        }

        [TestMethod]
        public void Add_2()
        {
            Assert.IsTrue(BigInteger.Zero + BigInteger.One == BigInteger.One);
        }

        [TestMethod]
        public void Add_3()
        {
            int zeros = 5000;
            string number = "9" + new string('0', zeros);
            var bi = BigInteger.Parse(number);
            BigInteger result = BigInteger.Zero;
            for (int i = 0; i < 1000; i++)
                result = result + bi;

            Assert.IsTrue(result.ToString() == number + "000");
        }

        [TestMethod]
        public void Add_4()
        {
            var x = new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF }, ByteOrder.BigEndian);
            Assert.IsTrue((x + 1).ToString(16) == "10000000000000000000000000000000000000000");
        }

        #endregion

        #region Substract

        [TestMethod]
        public void Substract_1()
        {
            var x = BigInteger.Parse("1071047203874102875891750374509837508173045730847513947518937459817345173045");
            var y = x - 1;
            Assert.IsTrue(y.ToString() == "1071047203874102875891750374509837508173045730847513947518937459817345173044");
        }

        #endregion

        #region Multiply

        [TestMethod]
        public void Multiply_1()
        {
            Assert.IsTrue(new BigInteger(long.MaxValue) * BigInteger.Zero == BigInteger.Zero);
            Assert.IsTrue(BigInteger.Zero * new BigInteger(long.MaxValue) == BigInteger.Zero);
        }

        [TestMethod]
        public void Multiply_2()
        {
            Assert.IsTrue(new BigInteger(long.MaxValue) * BigInteger.One == new BigInteger(long.MaxValue));
            Assert.IsTrue(BigInteger.One * new BigInteger(long.MaxValue) == new BigInteger(long.MaxValue));
        }

        [TestMethod]
        public void Multiply_3()
        {
            string dec1 = "7" + new string('5', 300);
            string dec2 = "1" + new string('0', 50);
            string result = "7" + new string('5', 300) + new string('0', 50);
            Assert.IsTrue(BigInteger.Parse(dec1) * BigInteger.Parse(dec2) == BigInteger.Parse(result));
        }

        [TestMethod]
        public void Multiply_4()
        {
            string hex1 = "1" + new string('0', 8);
            string hex2 = "2" + new string('0', 8);
            string result = "2" + new string('0', 16);
            Assert.IsTrue(BigInteger.Parse(hex1, 16) * BigInteger.Parse(hex2, 16) == BigInteger.Parse(result, 16));
        }

        [TestMethod]
        public void Multiply_5()
        {
            Assert.IsTrue(BigInteger.Parse("08396905339595156620") * BigInteger.Parse("13568775824401428918") == BigInteger.Parse("113935726171686031717971661160007137160"));
        }

        [TestMethod]
        public void Multiply_6()
        {
            Assert.IsTrue(2 * BigInteger.Parse("100") == 200);
            Assert.IsTrue(2 * BigInteger.Parse("-100") == -200);
            Assert.IsTrue(-2 * BigInteger.Parse("100") == -200);
            Assert.IsTrue(-2 * BigInteger.Parse("-100") == 200);
        }

        #endregion

        #region Division

        [TestMethod]
        public void Division_1()
        {
            BigInteger quotient, remainder;
            BigInteger.Division(BigInteger.One, new BigInteger(2), out quotient, out remainder);
            Assert.IsTrue(quotient.IsZero);
            Assert.IsTrue(remainder.ToString() == "1");
        }

        [TestMethod]
        public void Division_2()
        {
            var bi1 = new BigInteger(Enumerable.Repeat((byte)244, 500).ToArray(), ByteOrder.BigEndian);
            var bi2 = new BigInteger(Enumerable.Repeat((byte)245, 500).ToArray(), ByteOrder.BigEndian);
            BigInteger quotient, remainder;
            BigInteger.Division(bi1, bi2, out quotient, out remainder);
            Assert.IsTrue(quotient.IsZero);
            Assert.IsTrue(remainder == bi1);
        }

        [TestMethod]
        public void Division_3()
        {
            var bi1 = BigInteger.Parse("100000001", 16);
            var bi2 = new BigInteger(2);
            BigInteger quotient, remainder;
            BigInteger.Division(bi1, bi2, out quotient, out remainder);
            Assert.IsTrue(quotient.ToString(16) == "80000000");
            Assert.IsTrue(remainder.ToString() == "1");
        }

        [TestMethod]
        public void Division_4()
        {
            var bi1 = BigInteger.Parse("18190758438987288201");
            var bi2 = BigInteger.Parse("7771411298");
            BigInteger quotient, remainder;
            BigInteger.Division(bi1, bi2, out quotient, out remainder);
            Assert.IsTrue(quotient.ToString() == "2340727795");
            Assert.IsTrue(remainder.ToString() == "7381660291");
        }

        [TestMethod]
        public void Division_5()
        {
            var bi1 = BigInteger.Parse("33983299299734985002");
            var bi2 = BigInteger.Parse("3671279634");
            BigInteger quotient, remainder;
            BigInteger.Division(bi1, bi2, out quotient, out remainder);
            Assert.IsTrue(quotient.ToString() == "9256527066");
            Assert.IsTrue(remainder.ToString() == "759411158");
        }

        [TestMethod]
        public void Division_6()
        {
            var bi1 = BigInteger.Parse("24912195018622299722");
            var bi2 = BigInteger.Parse("3783002497");
            BigInteger quotient, remainder;
            BigInteger.Division(bi1, bi2, out quotient, out remainder);
            Assert.IsTrue(quotient.ToString() == "6585297006");
            Assert.IsTrue(remainder.ToString() == "1437675740");
        }

        [TestMethod]
        public void Division_7()
        {
            var bi1 = BigInteger.Parse("89693542179547309145");
            var bi2 = BigInteger.Parse("9907093449");
            BigInteger quotient, remainder;
            BigInteger.Division(bi1, bi2, out quotient, out remainder);
            Assert.IsTrue(quotient.ToString() == "9053466855");
            Assert.IsTrue(remainder.ToString() == "9638176250");
        }

        [TestMethod]
        public void Division_8()
        {
            var bi1 = BigInteger.Parse("32999900863646250090");
            var bi2 = BigInteger.Parse("9341814507");
            BigInteger quotient, remainder;
            BigInteger.Division(bi1, bi2, out quotient, out remainder);
            Assert.IsTrue(quotient.ToString() == "3532493696");
            Assert.IsTrue(remainder.ToString() == "8467402218");
        }

        [TestMethod]
        public void Division_9()
        {
            var bi1 = BigInteger.Parse("281982656586273440378");
            var bi2 = BigInteger.Parse("57508465279");
            BigInteger quotient, remainder;
            BigInteger.Division(bi1, bi2, out quotient, out remainder);
            Assert.IsTrue(quotient.ToString() == "4903324323");
            Assert.IsTrue(remainder.ToString() == "5351759261");
        }

        [TestMethod]
        public void Division_10()
        {
            var bi1 = new BigInteger(997);
            var bi2 = new BigInteger(997);
            BigInteger quotient, remainder;
            BigInteger.Division(bi1, bi2, out quotient, out remainder);
            Assert.IsTrue(quotient == 1);
            Assert.IsTrue(remainder == 0);
        }

        [TestMethod]
        public void Division_11()
        {
            var bi1 = BigInteger.Parse("-3709922803365947854249406587936891007271240697659934190189352926534182226656176161307276765759961107574741733679712927872190108668553201928124260700254843641852246257622014545666253549165226937810500934814536469243449757983154383");
            var bi2 = BigInteger.Parse("7fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffed", 16);
            Assert.IsTrue((bi1 % bi2) == 0);
        }

        #endregion

        #region Extended Euclidean Int64

        //[TestMethod]
        //public void ExtendedEuclideanInt64_1()
        //{
        //    long a = 4294967296;
        //    long b = 3411137177;
        //    var result = BigInteger.ExtendedEuclideanInt64(a, b);
        //    Assert.IsTrue(checked(a * result.x + b * result.y) == result.d);
        //}

        #endregion

        #region Modular Exponentiation

        [TestMethod]
        public void ModPow_1()
        {
            var bi1 = BigInteger.Parse("078362760750068396170");
            var bi2 = BigInteger.Parse("274919547864220404034");
            var bi3 = BigInteger.Parse("74296421505141971816");
            Assert.IsTrue(BigInteger.ModularExponentiation(bi1, bi2, bi3).ToString() == "62733211883538321040");
        }

        [TestMethod]
        public void ModPow_2()
        {
            var bi1 = BigInteger.Parse("078362760750068396170");
            var bi2 = BigInteger.Parse("274919547864220404034");
            var bi3 = BigInteger.Parse("74296421505141971816");
            Assert.IsTrue(BigInteger.ModularExponentiation(bi1, bi2, bi3).ToString() == "62733211883538321040");
        }

        [TestMethod]
        public void ModPow_3()
        {
            var bi1 = BigInteger.Parse("3812984915");
            var bi2 = BigInteger.Parse("9545293744");
            var bi3 = BigInteger.Parse("385942420801");
            Assert.IsTrue(BigInteger.ModularExponentiation(bi1, bi2, bi3).ToString() == "317273175837");
        }

        [TestMethod]
        public void ModPow_4()
        {
            var bi1 = BigInteger.Parse("163338342975806799406");
            var bi2 = BigInteger.Parse("02827803280063344426");
            var bi3 = BigInteger.Parse("76224379996895040153");
            Assert.IsTrue(BigInteger.ModularExponentiation(bi1, bi2, bi3).ToString() == "50608764524184452200");
        }

        [TestMethod]
        public void ModPow_5()
        {
            var bi1 = BigInteger.Parse("777");
            var bi2 = BigInteger.Parse("6");
            var bi3 = BigInteger.Parse("18446744073709551629");
            Assert.IsTrue(BigInteger.ModularExponentiation(bi1, bi2, bi3).ToString() == "220052401647189489");
        }

        #endregion

        #region Modular Inverse

        [TestMethod]
        public void ModInverse_1()
        {
            var prime = new BigInteger(131);
            var a = new BigInteger(129);
            var x = BigInteger.ModularInverse(a, prime);
            Assert.IsTrue(a * x % prime == 1);
        }

        [TestMethod]
        public void ModInverse_2()
        {
            var prime = BigInteger.Parse("10293180515138650139860915713987518037501983548136548091635139465109364509183645961345613049241");
            var a = BigInteger.Parse("10193180515138650139860915713987518037501983548136548091635139465109364509183645961345613049241");
            var x = BigInteger.ModularInverse(a, prime);
            Assert.IsTrue(a * x % prime == 1);
        }

        [TestMethod]
        public void ModInverse_3()
        {
            var prime = BigInteger.Parse("99801848086134853174517340857134875134514650137846805173648571364751634075601375183745817304895713894901");
            var a = BigInteger.Parse("101909877096775573180515138650139860915713987518037501983548136548091635139465109364509183645961345613049241");
            var x = BigInteger.ModularInverse(a, prime);
            Assert.IsTrue(a * x % prime == 1);
        }

        #endregion

        #region Modular Square Root

        [TestMethod]
        public void ModSqrt_1()
        {
            var prime = new BigInteger(131);
            var value = new BigInteger(18);
            var sqrt = BigInteger.ModularSquareRoot(value, prime);
            Assert.IsNull(sqrt);
        }

        [TestMethod]
        public void ModSqrt_2()
        {
            var prime = new BigInteger(991); // mod 4 = 3
            var value = new BigInteger(13);
            var sqrt = BigInteger.ModularSquareRoot(value, prime);
            Assert.IsTrue(sqrt * sqrt % prime == value);
        }

        [TestMethod]
        public void ModSqrt_3()
        {
            var prime = new BigInteger(997); // mod 8 = 5
            var value = new BigInteger(13);
            var sqrt = BigInteger.ModularSquareRoot(value, prime);
            Assert.IsTrue(sqrt * sqrt % prime == value);
        }

        [TestMethod]
        public void ModSqrt_4()
        {
            var prime = new BigInteger(281); // mod 8 = 1
            var value = new BigInteger(280);
            var sqrt = BigInteger.ModularSquareRoot(value, prime);
            Assert.IsTrue(sqrt * sqrt % prime == value);
        }

        [TestMethod]
        public void ModSqrt_5()
        {
            var prime = new BigInteger(113); // mod 8 = 1
            var value = new BigInteger(2);
            var sqrt = BigInteger.ModularSquareRoot(value, prime);
            Assert.IsTrue(sqrt * sqrt % prime == value);
        }

        [TestMethod]
        public void ModSqrt_6()
        {
            // mod 4 = 3
            var prime = BigInteger.Parse("340282366920938463463374607431768211507");
            var value = BigInteger.Parse("253147205716945551724961647724928675906");
            var sqrt = BigInteger.ModularSquareRoot(value, prime);
            Assert.IsTrue(sqrt * sqrt % prime == value);
        }

        [TestMethod]
        public void ModSqrt_7()
        {
            // mod 8 = 5
            var prime = BigInteger.Parse("340282366920938463463374607431768211621");
            /*var x = BigInteger.Random(prime, new DefaultRandom());
            var x2 = x * x % prime;*/
            var value = BigInteger.Parse("109739196097016756959209324015409862461");
            var sqrt = BigInteger.ModularSquareRoot(value, prime);
            Assert.IsTrue(sqrt * sqrt % prime == value);
        }

        [TestMethod]
        public void ModSqrt_8()
        {
            // mod 8 = 1
            var prime = BigInteger.Parse("1180591620717411303449");
            var value = BigInteger.Parse("13");
            var sqrt = BigInteger.ModularSquareRoot(value, prime);
            Assert.IsTrue(sqrt * sqrt % prime == value);
        }

        [TestMethod]
        public void ModSqrt_9()
        {
            // mod 8 = 1
            var prime = BigInteger.Parse("60246720987560982760987268597209710987510378758165891640598136405164059613049571093847518093740513929");
            var value = BigInteger.Parse("50246720987560982760987268597209710987510378758165891640598136405164059613049571093847518093740513929");
            var sqrt = BigInteger.ModularSquareRoot(value, prime);
            Assert.IsTrue(sqrt * sqrt % prime == value);
        }

        [TestMethod]
        public void ModSqrt_10()
        {
            // secp192r1
            // mod 8 = 1
            var prime = new BigInteger(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFE, 0xFFFFFFFF, 0xFFFFFFFF }, ByteOrder.BigEndian);
            var value = BigInteger.Parse("2927373548741373060228559338194823606712523754856353507713");
            var sqrt = BigInteger.ModularSquareRoot(value, prime);
            Assert.IsTrue(sqrt * sqrt % prime == value);
        }

        #endregion

        #region Bit Size

        [TestMethod]
        public void BitSize_1()
        {
            Assert.IsTrue(BigInteger.Zero.BitSize == 0);
        }

        [TestMethod]
        public void BitSize_2()
        {
            Assert.IsTrue(BigInteger.One.BitSize == 1);
        }

        [TestMethod]
        public void BitSize_3()
        {
            Assert.IsTrue(new BigInteger(2).BitSize == 2);
        }

        [TestMethod]
        public void BitSize_4()
        {
            Assert.IsTrue(new BigInteger(1L << 32).BitSize == 33);
        }

        #endregion
    }
}