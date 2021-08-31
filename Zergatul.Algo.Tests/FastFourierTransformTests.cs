using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Zergatul.Algo.Tests
{
    [TestClass]
    public class FastFourierTransformTests
    {
        static Complex[] Test1Poly = new Complex[]
        {
            1, 2, 3, 4
        };
        static Complex[] Test1Values = new Complex[]
        {
            (10, 0),
            (-2, -2),
            (-2, 0),
            (-2, 2)
        };

        static Complex[] Test2Poly = new Complex[]
        {
            7, 8, 7, 6, 2, 6, 9, 4, 9, 3, 3, 5, 6, 5, 9, 1, 8, 9, 7, 3, 8, 8, 9, 1, 7, 5, 9, 7, 5, 1, 2, 5
        };
        static Complex[] Test2Values = new Complex[]
        {
            (184, 0),
            (-7.03446576270651, -3.8315802221952),
            (0.493743663164145, 13.0448261521561),
            (-4.36066575949176, 5.70353831319435),
            (6.46446609406726, 7.60660171779821),
            (18.9766527309039, 15.985786774935),
            (16.5250430872078, 12.8415801289438),
            (-5.54177653786553, -14.4705442048078),
            (-3, 13),
            (1.9843938680308, -11.8126256796491),
            (-11.4539752753423, -1.88634193241408),
            (5.36696464895442, -12.6117956804696),
            (13.5355339059327, 13.6066017177982),
            (-4.18396174714327, -0.751908518479261),
            (-9.56481147502961, 2.31690409079822),
            (-13.2071414406821, 4.96847392669445),
            (30, 0),
            (-13.2071414406821, -4.96847392669445),
            (-9.56481147502962, -2.31690409079823),
            (-4.18396174714327, 0.75190851847926),
            (13.5355339059327, -13.6066017177982),
            (5.36696464895442, 12.6117956804696),
            (-11.4539752753423, 1.88634193241408),
            (1.9843938680308, 11.8126256796491),
            (-3, -13),
            (-5.54177653786553, 14.4705442048079),
            (16.5250430872078, -12.8415801289438),
            (18.9766527309039, -15.985786774935),
            (6.46446609406726, -7.60660171779821),
            (-4.36066575949176, -5.70353831319435),
            (0.493743663164142, -13.0448261521561),
            (-7.03446576270651, 3.83158022219521)
        };

        [TestMethod]
        public void Poly1Test()
        {
            var values = FastFourierTransform.Calculate((Complex[])Test1Poly.Clone());
            Assert.IsTrue(ComplexEquals(values, Test1Values));

            var poly = FastFourierTransform.InverseCalculate((Complex[])Test1Values.Clone());
            Assert.IsTrue(ComplexEquals(poly, Test1Poly));
        }

        [TestMethod]
        public void Poly2Test()
        {
            var values = FastFourierTransform.Calculate((Complex[])Test2Poly.Clone());
            Assert.IsTrue(ComplexEquals(values, Test2Values));

            var poly = FastFourierTransform.InverseCalculate((Complex[])Test2Values.Clone());
            Assert.IsTrue(ComplexEquals(poly, Test2Poly));
        }

        private static bool ComplexEquals(Complex[] c1, Complex[] c2)
        {
            if (c1.Length != c2.Length)
                return false;

            for (int i = 0; i < c1.Length; i++)
            {
                if (!c1[i].Equals(c2[i], 1e-10))
                    return false;
            }

            return true;
        }
    }
}