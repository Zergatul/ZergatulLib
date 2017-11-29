using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Math;
using Zergatul.Math.EllipticCurves.PrimeField;
using Zergatul.Cryptography.Asymmetric;

namespace Zergatul.Cryptography.Tests.Asymmetric
{
    [TestClass]
    public class GOSTR_34_10_2012Tests
    {
        [TestMethod]
        public void StandardTest1()
        {
            var curve = new EllipticCurve(
                p: BigInteger.Parse("8000000000000000000000000000000000000000000000000000000000000431", 16),
                a: new BigInteger(7),
                b: BigInteger.Parse("5FBFF498AA938CE739B8E022FBAFEF40563F6E6A3472FC2A514C0CE9DAE23B7E", 16),
                n: BigInteger.Parse("8000000000000000000000000000000150FE8A1892976154C59CFC193ACCF5B3", 16),
                g: new ECPoint
                {
                    x = new BigInteger(2),
                    y = BigInteger.Parse("8E2A8A0E65147D4BD6316030E16D19C85C97F0A9CA267122B96ABBCEA7E8FC8", 16)
                },
                h: 1);

            var gost = new GOSTR_34_10_2012();
            gost.Parameters = curve;
            gost.PrivateKey = BigInteger.Parse("7A929ADE789BB9BE10ED359DD39A72C11B60961F49397EEE1D19CE9891EC3B28", 16);
            gost.GeneratePublicKey();

            Assert.IsTrue(gost.PublicKey.x.ToString(16) == "7f2b49e270db6d90d8595bec458b50c58585ba1d4e9b788f6689dbd8e56fd80b");
            Assert.IsTrue(gost.PublicKey.y.ToString(16) == "26f1b489d6701dd185c8413a977b3cbbaf64d1c593d26627dffb101a87ff77da");

            var e = BigInteger.Parse("2DFBC1B372D89A1188C09C52E0EEC61FCE52032AB1022E8E67ECE6672B043EE5", 16);
            gost.Random = new StaticRandom(BitHelper.HexToBytes("77105C9B20BCD3122823C8CF6FCC7B956DE33814E95B7FE64FED924594DCEAB2"));
            var signature = gost.Signature.Sign(e);

            Assert.IsTrue(signature.r.ToString(16) == "41aa28d2f1ab148280cd9ed56feda41974053554a42767b83ad043fd39dc0493");
            Assert.IsTrue(signature.s.ToString(16) == "1456c64ba4642a1653c235a98a60249bcd6d3f746b631df928014f6c5bf9c40");

            Assert.IsTrue(gost.Signature.Verify(signature, e));

            Assert.IsFalse(gost.Signature.Verify(signature, e - 1));
            Assert.IsFalse(gost.Signature.Verify(signature, e + 1));

            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    Assert.IsFalse(gost.Signature.Verify(new GOSTRSignature
                    {
                        r = signature.r + i,
                        s = signature.s + j
                    }, e));
                }
        }

        [TestMethod]
        public void StandardTest2()
        {
            var curve = new EllipticCurve(
                p: BigInteger.Parse("4531ACD1FE0023C7550D267B6B2FEE80922B14B2FFB90F04D4EB7C09B5D2D15DF1D852741AF4704A0458047E80E4546D35B8336FAC224DD81664BBF528BE6373", 16),
                a: new BigInteger(7),
                b: BigInteger.Parse("1CFF0806A31116DA29D8CFA54E57EB748BC5F377E49400FDD788B649ECA1AC4361834013B2AD7322480A89CA58E0CF74BC9E540C2ADD6897FAD0A3084F302ADC", 16),
                n: BigInteger.Parse("4531ACD1FE0023C7550D267B6B2FEE80922B14B2FFB90F04D4EB7C09B5D2D15DA82F2D7ECB1DBAC719905C5EECC423F1D86E25EDBE23C595D644AAF187E6E6DF", 16),
                g: new ECPoint
                {
                    x = BigInteger.Parse("24D19CC64572EE30F396BF6EBBFD7A6C5213B3B3D7057CC825F91093A68CD762FD60611262CD838DC6B60AA7EEE804E28BC849977FAC33B4B530F1B120248A9A", 16),
                    y = BigInteger.Parse("2BB312A43BD2CE6E0D020613C857ACDDCFBF061E91E5F2C3F32447C259F39B2C83AB156D77F1496BF7EB3351E1EE4E43DC1A18B91B24640B6DBB92CB1ADD371E", 16)
                },
                h: 1);

            var gost = new GOSTR_34_10_2012();
            gost.Parameters = curve;
            gost.PrivateKey = BigInteger.Parse("BA6048AADAE241BA40936D47756D7C93091A0E8514669700EE7508E508B102072E8123B2200A0563322DAD2827E2714A2636B7BFD18AADFC62967821FA18DD4", 16);
            gost.GeneratePublicKey();

            Assert.IsTrue(gost.PublicKey.x.ToString(16) == "115dc5bc96760c7b48598d8ab9e740d4c4a85a65be33c1815b5c320c854621dd5a515856d13314af69bc5b924c8b4ddff75c45415c1d9dd9dd33612cd530efe1");
            Assert.IsTrue(gost.PublicKey.y.ToString(16) == "37c7c90cd40b0f5621dc3ac1b751cfa0e2634fa0503b3d52639f5d7fb72afd61ea199441d943ffe7f0c70a2759a3cdb84c114e1f9339fdf27f35eca93677beec");

            var e = BigInteger.Parse("3754F3CFACC9E0615C4F4A7C4D8DAB531B09B6F9C170C533A71D147035B0C5917184EE536593F4414339976C647C5D5A407ADEDB1D560C4FC6777D2972075B8C", 16);
            gost.Random = new StaticRandom(BitHelper.HexToBytes("0359E7F4B1410FEACC570456C6801496946312120B39D019D455986E364F365886748ED7A44B3E794434006011842286212273A6D14CF70EA3AF71BB1AE679F0"));
            var signature = gost.Signature.Sign(e);

            Assert.IsTrue(signature.r.ToString(16) == "2f86fa60a081091a23dd795e1e3c689ee512a3c82ee0dcc2643c78eea8fcacd35492558486b20f1c9ec197c90699850260c93bcbcd9c5c3317e19344e173ae36");
            Assert.IsTrue(signature.s.ToString(16) == "1081b394696ffe8e6585e7a9362d26b6325f56778aadbc081c0bfbe933d52ff5823ce288e8c4f362526080df7f70ce406a6eeb1f56919cb92a9853bde73e5b4a");

            Assert.IsTrue(gost.Signature.Verify(signature, e));

            Assert.IsFalse(gost.Signature.Verify(signature, e - 1));
            Assert.IsFalse(gost.Signature.Verify(signature, e + 1));

            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    Assert.IsFalse(gost.Signature.Verify(new GOSTRSignature
                    {
                        r = signature.r + i,
                        s = signature.s + j
                    }, e));
                }
        }
    }
}