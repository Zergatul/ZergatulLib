using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Math.EllipticCurves.BinaryField;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Cryptography.Hash;
using System.Text;
using Zergatul.Network.ASN1.Structures;
using Zergatul.Network.ASN1;
using Zergatul.Math;

namespace Zergatul.Cryptography.Tests.Asymmetric
{
    // https://tools.ietf.org/html/rfc6979#appendix-A.2.8
    [TestClass]
    public class ECBDSATests
    {
        [TestMethod]
        public void Test163k1()
        {
            // TODO: don't work
            Test(
                curve: EllipticCurve.sect163k1,
                private_key: "000000009A4D6792295A7F730FC3F2B49CBC0F62E862272F",
                pubkey_x: "79AEE090DB05EC252D5CB4452F356BE198A4FF96F",
                pubkey_y: "782E29634DDC9A31EF40386E896BAA18B53AFA5A3",
                tests: new[]
                {
                    new[]
                    {
                        "SHA1",
                        "sample",
                        "09744429FA741D12DE2BE8316E35E84DB9E5DF1CD",
                        "30C45B80BA0E1406C4EFBBB7000D6DE4FA465D505",
                        "38D87DF89493522FC4CD7DE1553BD9DBBA2123011"
                    }
                }
            );
        }

        private static void Test(EllipticCurve curve, string private_key, string pubkey_x, string pubkey_y, string[][] tests)
        {
            private_key = private_key.Replace(" ", "").ToLower();
            pubkey_x = pubkey_x.Replace(" ", "").ToLower();
            pubkey_y = pubkey_y.Replace(" ", "").ToLower();

            var ecdsa1 = new ECBDSA();
            ecdsa1.Random = new StaticRandom(BitHelper.HexToBytes(private_key));
            ecdsa1.Parameters = new ECBDSAParameters(curve);
            ecdsa1.GenerateKeyPair(0);

            int len = pubkey_x.Length / 2;
            Assert.IsTrue(BitHelper.BytesToHex(ecdsa1.PublicKey.Point.x.ToBytes(ByteOrder.BigEndian, len)) == pubkey_x);
            Assert.IsTrue(BitHelper.BytesToHex(ecdsa1.PublicKey.Point.y.ToBytes(ByteOrder.BigEndian, len)) == pubkey_y);

            var ecdsa2 = new ECBDSA();
            ecdsa2.Parameters = new ECBDSAParameters(curve);
            ecdsa2.PublicKey = ecdsa1.PublicKey;

            foreach (var tc in tests)
            {
                ecdsa1.Parameters.Hash = ecdsa2.Parameters.Hash = ResolveHash(tc[0]);
                byte[] msg = Encoding.ASCII.GetBytes(tc[1]);
                ecdsa1.Random = new StaticRandom(BitHelper.HexToBytes(tc[2]));

                byte[] signature = ecdsa1.Sign(msg);

                var ed = ECDSASignatureValue.Parse(ASN1Element.ReadFrom(signature));
                Assert.IsTrue(ed.r == new BigInteger(BitHelper.HexToBytes(tc[3]), ByteOrder.BigEndian));
                Assert.IsTrue(ed.s == new BigInteger(BitHelper.HexToBytes(tc[4]), ByteOrder.BigEndian));

                Assert.IsTrue(ecdsa2.Verify(msg, signature));

                // change signature to assure verify hash return false
                signature[signature.Length - 1] ^= 1;

                Assert.IsFalse(ecdsa2.Verify(msg, signature));
            }
        }

        private static AbstractHash ResolveHash(string name)
        {
            switch (name)
            {
                case "SHA-1": return new SHA1();
                case "SHA-224": return new SHA224();
                case "SHA-256": return new SHA256();
                case "SHA-384": return new SHA384();
                case "SHA-512": return new SHA512();
            }
            return null;
        }
    }
}