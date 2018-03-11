using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Math.EllipticCurves.PrimeField;
using Zergatul.Cryptography.Hash;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Network.Asn1.Structures;
using Zergatul.Network.Asn1;
using Zergatul.Math;

namespace Zergatul.Cryptography.Tests.Asymmetric
{
    // https://tools.ietf.org/html/rfc4754#section-8
    [TestClass]
    public class ECPDSATests
    {
        [TestMethod]
        public void Vector1()
        {
            Test(
                curve: EllipticCurve.secp256r1,
                hash: new SHA256(),
                private_key: "DC51D386 6A15BACD E33D96F9 92FCA99D A7E6EF09 34E70975 59C27F16 14C88A7F",
                pubkey_x: "2442A5CC 0ECD015F A3CA31DC 8E2BBC70 BF42D60C BCA20085 E0822CB0 4235E970",
                pubkey_y: "6FC98BD7 E50211A4 A27102FA 3549DF79 EBCB4BF2 46B80945 CDDFE7D5 09BBFD7D",
                k: "9E56F509 196784D9 63D1C0A4 01510EE7 ADA3DCC5 DEE04B15 4BF61AF1 D5A6DECE",
                plain: "abc",
                r: "CB28E099 9B9C7715 FD0A80D8 E47A7707 9716CBBF 917DD72E 97566EA1 C066957C",
                s: "86FA3BB4 E26CAD5B F90B7F81 899256CE 7594BB1E A0C89212 748BFF3B 3D5B0315");
        }

        [TestMethod]
        public void Vector2()
        {
            Test(
                curve: EllipticCurve.secp384r1,
                hash: new SHA384(),
                private_key: "0BEB6466 34BA8773 5D77AE48 09A0EBEA 865535DE 4C1E1DCB 692E8470 8E81A5AF 62E528C3 8B2A81B3 5309668D 73524D9F",
                pubkey_x: "96281BF8 DD5E0525 CA049C04 8D345D30 82968D10 FEDF5C5A CA0C64E6 465A97EA 5CE10C9D FEC21797 41571072 1F437922",
                pubkey_y: "447688BA 94708EB6 E2E4D59F 6AB6D7ED FF9301D2 49FE49C3 3096655F 5D502FAD 3D383B91 C5E7EDAA 2B714CC9 9D5743CA",
                k: "B4B74E44 D71A13D5 68003D74 89908D56 4C7761E2 29C58CBF A1895009 6EB7463B 854D7FA9 92F934D9 27376285 E63414FA",
                plain: "abc",
                r: "FB017B91 4E291494 32D8BAC2 9A514640 B46F53DD AB2C6994 8084E293 0F1C8F7E 08E07C9C 63F2D21A 07DCB56A 6AF56EB3",
                s: "B263A130 5E057F98 4D38726A 1B468741 09F417BC A112674C 528262A4 0A629AF1 CBB9F516 CE0FA7D2 FF630863 A00E8B9F");
        }

        [TestMethod]
        public void Vector3()
        {
            Test(
                curve: EllipticCurve.secp521r1,
                hash: new SHA512(),
                private_key: "0065FDA3 409451DC AB0A0EAD 45495112 A3D813C1 7BFD34BD F8C1209D 7DF58491 20597779 060A7FF9 D704ADF7 8B570FFA D6F062E9 5C7E0C5D 5481C5B1 53B48B37 5FA1",
                pubkey_x: "0151518F 1AF0F563 517EDD54 85190DF9 5A4BF57B 5CBA4CF2 A9A3F647 4725A35F 7AFE0A6D DEB8BEDB CD6A197E 592D4018 8901CECD 650699C9 B5E456AE A5ADD190 52A8",
                pubkey_y: "006F3B14 2EA1BFFF 7E2837AD 44C9E4FF 6D2D34C7 3184BBAD 90026DD5 E6E85317 D9DF45CA D7803C6C 20035B2F 3FF63AFF 4E1BA64D 1C077577 DA3F4286 C58F0AEA E643",
                k: "00C1C2B3 05419F5A 41344D7E 4359933D 734096F5 56197A9B 244342B8 B62F46F9 373778F9 DE6B6497 B1EF825F F24F42F9 B4A4BD73 82CFC337 8A540B1B 7F0C1B95 6C2F",
                plain: "abc",
                r: "0154FD38 36AF92D0 DCA57DD5 341D3053 988534FD E8318FC6 AAAAB68E 2E6F4339 B19F2F28 1A7E0B22 C269D93C F8794A92 78880ED7 DBB8D936 2CAEACEE 54432055 2251",
                s: "017705A7 030290D1 CEB605A9 A1BB03FF 9CDD521E 87A696EC 926C8C10 C8362DF4 97536710 1F67D1CF 9BCCBF2F 3D239534 FA509E70 AAC851AE 01AAC68D 62F86647 2660");
        }

        private static void Test(EllipticCurve curve, AbstractHash hash, string private_key, string pubkey_x, string pubkey_y, string k, string plain, string r, string s)
        {
            private_key = private_key.Replace(" ", "").ToLower();
            pubkey_x = pubkey_x.Replace(" ", "").ToLower();
            pubkey_y = pubkey_y.Replace(" ", "").ToLower();
            k = k.Replace(" ", "").ToLower();
            r = r.Replace(" ", "").ToLower();
            s = s.Replace(" ", "").ToLower();

            var ecdsa1 = new ECPDSA();
            ecdsa1.Random = new StaticRandom(Dec(BitHelper.HexToBytes(private_key)));
            ecdsa1.Parameters = new ECPDSAParameters(curve);
            ecdsa1.Parameters.Hash = hash;
            ecdsa1.GenerateKeyPair(0);

            int len = pubkey_x.Length / 2;
            Assert.IsTrue(BitHelper.BytesToHex(ecdsa1.PublicKey.Point.x.ToBytes(ByteOrder.BigEndian, len)) == pubkey_x);
            Assert.IsTrue(BitHelper.BytesToHex(ecdsa1.PublicKey.Point.y.ToBytes(ByteOrder.BigEndian, len)) == pubkey_y);

            ecdsa1.Random = new StaticRandom(Dec(BitHelper.HexToBytes(k)));
            byte[] signature = ecdsa1.Sign(System.Text.Encoding.ASCII.GetBytes(plain));
            var ed = ECDSASignatureValue.Parse(Asn1Element.ReadFrom(signature));
            Assert.IsTrue(ed.r == new BigInteger(BitHelper.HexToBytes(r), ByteOrder.BigEndian));
            Assert.IsTrue(ed.s == new BigInteger(BitHelper.HexToBytes(s), ByteOrder.BigEndian));

            // *******************
            var ecdsa2 = new ECPDSA();
            ecdsa2.Parameters = new ECPDSAParameters(curve);
            ecdsa2.Parameters.Hash = hash;
            ecdsa2.PublicKey = ecdsa1.PublicKey;

            Assert.IsTrue(ecdsa2.Verify(System.Text.Encoding.ASCII.GetBytes(plain), signature));

            // change signature to assure verify hash return false
            signature[signature.Length - 1] ^= 1;

            Assert.IsFalse(ecdsa2.Verify(System.Text.Encoding.ASCII.GetBytes(plain), signature));
        }

        private static byte[] Dec(byte[] data)
        {
            return (new BigInteger(data, ByteOrder.BigEndian) - 1).ToBytes(ByteOrder.BigEndian, data.Length);
        }
    }
}