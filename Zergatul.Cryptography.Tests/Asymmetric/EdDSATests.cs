using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Math.EdwardsCurves;

namespace Zergatul.Cryptography.Tests.Asymmetric
{
    [TestClass]
    public class EdDSATests
    {
        [TestMethod]
        public void Ed25519Tests()
        {
            var eddsa = new EdDSA();
            eddsa.Parameters = new EdDSAParameters
            {
                Curve = EdCurve.Ed25519
            };
            eddsa.PrivateKey = new EdDSAPrivateKey
            {
                Value = BitHelper.HexToBytes("9d61b19deffd5a60ba844af492ec2cc44449c5697b326919703bac031cae7f60")
            };

            // a=36144925721603087658594284515452164870581325872720374094707712194495455132720
            // prefix=9b4f0afe280b746a778684e75442502057b7473a03f08f96f5a38e9287e01f8f
            // A=d75a980182b10ab7d54bfed3c964073a0ee172f3daa62325af021a68f707511a
            // r=2015312323900227545039973048451112962192803170366412514267238242679089760755
            // h=1958233733501237659471134851339390337284068724042047466985993338226439154310
            byte[] signature = eddsa.Sign(new byte[0]);

            Assert.IsTrue(ByteArray.Equals(signature, BitHelper.HexToBytes("e5564300c360ac729086e2cc806e828a84877f1eb8e5d974d873e065224901555fb8821590a33bacc61e39701cf9b46bd25bf5f0595bbe24655141438e7a100b")));
        }
    }
}