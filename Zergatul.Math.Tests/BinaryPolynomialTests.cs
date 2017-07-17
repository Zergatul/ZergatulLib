using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Zergatul.Math.Tests
{
    [TestClass]
    public class BinaryPolynomialTests
    {
        [TestMethod]
        public void Add_1()
        {
            var p1 = BinaryPolynomial.FromPowers(0);
            var p2 = BinaryPolynomial.FromPowers(0);
            Assert.IsTrue((p1 + p2).ToString() == "0");
        }

        [TestMethod]
        public void Add_2()
        {
            var p1 = BinaryPolynomial.FromPowers(0);
            var p2 = BinaryPolynomial.FromPowers(1);
            Assert.IsTrue((p1 + p2).ToString() == "x+1");
        }

        [TestMethod]
        public void Add_3()
        {
            var p1 = BinaryPolynomial.FromPowers(5, 4, 3, 2, 1, 0);
            var p2 = BinaryPolynomial.FromPowers(4, 3, 2, 1);
            Assert.IsTrue((p1 + p2).ToString() == "x⁵+1");
        }

        [TestMethod]
        public void ModularMultiplication_1()
        {
            var f = BinaryPolynomial.FromPowers(4, 1, 0);
            var g = BinaryPolynomial.FromPowers(1);

            var gpow = new BinaryPolynomial[16];
            gpow[1] = g;
            for (int i = 2; i < gpow.Length; i++)
                gpow[i] = BinaryPolynomial.ModularMultiplication(gpow[i - 1], g, f);

            Assert.IsTrue(gpow[2].ToString() == "x²");
            Assert.IsTrue(gpow[3].ToString() == "x³");
            Assert.IsTrue(gpow[4].ToString() == "x+1");
            Assert.IsTrue(gpow[5].ToString() == "x²+x");
            Assert.IsTrue(gpow[6].ToString() == "x³+x²");
            Assert.IsTrue(gpow[7].ToString() == "x³+x+1");
            Assert.IsTrue(gpow[8].ToString() == "x²+1");
            Assert.IsTrue(gpow[9].ToString() == "x³+x");
            Assert.IsTrue(gpow[10].ToString() == "x²+x+1");
            Assert.IsTrue(gpow[11].ToString() == "x³+x²+x");
            Assert.IsTrue(gpow[12].ToString() == "x³+x²+x+1");
            Assert.IsTrue(gpow[13].ToString() == "x³+x²+1");
            Assert.IsTrue(gpow[14].ToString() == "x³+1");
            Assert.IsTrue(gpow[15].ToString() == "1");
        }

        [TestMethod]
        public void ModularReduction_1()
        {
            var p = BinaryPolynomial.FromPowers(200, 1);
            var m = BinaryPolynomial.FromPowers(5);

            Assert.IsTrue((p % m).ToString() == "x");
        }

        [TestMethod]
        public void ModularReduction_2()
        {
            var p = BinaryPolynomial.FromPowers(65, 1);
            var m = BinaryPolynomial.FromPowers(64, 0);

            Assert.IsTrue((p % m).ToString() == "0");
        }

        [TestMethod]
        public void ModularReduction_3()
        {
            var p = BinaryPolynomial.FromPowers(67, 63, 0);
            var m = BinaryPolynomial.FromPowers(65, 62, 0);

            Assert.IsTrue((p % m).ToString() == "x⁶⁴+x⁶³+x²+1");
        }


        [TestMethod]
        public void ModularReduction_4()
        {
            var p = BinaryPolynomial.FromPowers(70, 5, 4, 3, 2, 1, 0);
            var r = BinaryPolynomial.FromPowers(67, 66, 65, 64, 63, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0);
            var m = BinaryPolynomial.FromPowers(68, 60, 3, 2, 1, 0);

            Assert.IsTrue((p * m + r) % m == r);
        }

        [TestMethod]
        public void ModularReduction_5()
        {
            var p = BinaryPolynomial.FromPowers(300, 295, 200, 195, 150, 100, 90, 80, 70, 60, 50, 40, 30, 20, 15, 10, 5, 4, 3, 2, 1, 0);
            var r = BinaryPolynomial.FromPowers(264, 250, 240, 220, 101, 96, 34, 35, 30, 13, 8, 7, 5, 2, 1, 0);
            var m = BinaryPolynomial.FromPowers(265, 5, 4, 3, 2, 1, 0);

            Assert.IsTrue((p * m + r) % m == r);
        }

        [TestMethod]
        public void Multiplication_1()
        {
            var p1 = BinaryPolynomial.FromPowers(4, 0);
            var p2 = BinaryPolynomial.FromPowers(5, 0);
            Assert.IsTrue((p1 * p2).ToString() == "x⁹+x⁵+x⁴+1");
        }

        [TestMethod]
        public void Multiplication_2()
        {
            var p1 = BinaryPolynomial.FromPowers(256, 74, 38, 3, 0);
            var p2 = BinaryPolynomial.FromPowers(65, 1, 0);
            Assert.IsTrue((p1 * p2).ToString() == "x³²¹+x²⁵⁷+x²⁵⁶+x¹³⁹+x¹⁰³+x⁷⁵+x⁷⁴+x⁶⁸+x⁶⁵+x³⁹+x³⁸+x⁴+x³+x+1");
        }

        [TestMethod]
        public void Multiplication_3()
        {
            var p1 = BinaryPolynomial.FromPowers(65, 61, 0);
            var p2 = BinaryPolynomial.FromPowers(65, 61, 0);
            Assert.IsTrue((p1 * p2).ToString() == "x¹³⁰+x¹²²+1");
        }

        [TestMethod]
        public void Multiplication_4()
        {
            var p1 = BinaryPolynomial.FromPowers(70, 5, 4, 3, 2, 1, 0);
            var p2 = BinaryPolynomial.FromPowers(68, 60, 3, 2, 1, 0);
            Assert.IsTrue((p1 * p2).ToString() == "x¹³⁸+x¹³⁰+x⁶⁹+x⁶⁸+x⁶⁵+x⁶⁴+x⁶³+x⁶²+x⁶¹+x⁶⁰+x⁸+x⁶+x²+1");
        }

        [TestMethod]
        public void Square_1()
        {
            Assert.IsTrue(BinaryPolynomial.Square(BinaryPolynomial.FromPowers()).ToString() == "0");
        }

        [TestMethod]
        public void Square_2()
        {
            Assert.IsTrue(BinaryPolynomial.Square(BinaryPolynomial.FromPowers(0)).ToString() == "1");
        }

        [TestMethod]
        public void Square_3()
        {
            var p = BinaryPolynomial.FromPowers(500, 100, 1, 0);
            Assert.IsTrue(BinaryPolynomial.Square(p).ToString() == "x¹⁰⁰⁰+x²⁰⁰+x²+1");
        }

        [TestMethod]
        public void ModularInverse_1()
        {
            var p = BinaryPolynomial.FromPowers(1);
            var m = BinaryPolynomial.FromPowers(2, 0);
            var inv = BinaryPolynomial.ModularInverse(p, m);
            Assert.IsTrue(BinaryPolynomial.ModularMultiplication(p, inv, m).ToString() == "1");
        }

        [TestMethod]
        public void ModularInverse_2()
        {
            var p = BinaryPolynomial.FromPowers(2, 0);
            var m = BinaryPolynomial.FromPowers(3, 0);
            var inv = BinaryPolynomial.ModularInverse(p, m);
            Assert.IsTrue(inv == null);
        }

        [TestMethod]
        public void ModularInverse_3()
        {
            var p = BinaryPolynomial.FromPowers(2, 1, 0);
            var m = BinaryPolynomial.FromPowers(5, 0);
            var inv = BinaryPolynomial.ModularInverse(p, m);
            Assert.IsTrue(BinaryPolynomial.ModularMultiplication(p, inv, m).ToString() == "1");
        }

        [TestMethod]
        public void ModularInverse_4()
        {
            var p = BinaryPolynomial.FromPowers(32, 30, 0);
            var m = BinaryPolynomial.FromPowers(65, 60, 0);
            var inv = BinaryPolynomial.ModularInverse(p, m);
            Assert.IsTrue(BinaryPolynomial.ModularMultiplication(p, inv, m).ToString() == "1");
        }
    }
}