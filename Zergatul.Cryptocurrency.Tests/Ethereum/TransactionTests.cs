using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Ethereum;
using Zergatul.Cryptography.Hash;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Math.EllipticCurves.PrimeField;
using Zergatul.Math;
using Zergatul.Security;
using System.Text;
using Zergatul.Tests;

namespace Zergatul.Cryptocurrency.Tests.Ethereum
{
    [TestClass]
    public class TransactionTests
    {
        [TestMethod]
        public void Test1()
        {
            var addr = new Address();
            addr.FromPrivateKey("c0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0de");
            Assert.IsTrue(addr.Value == "0x53Ae893e4b22D707943299a8d0C844Df0e3d5557");

            var tx = new Transaction();
            tx.Nonce = 0;
            tx.GasPrice = 20000000000;
            tx.GasLimit = 100000;
            tx.To = new Address();
            tx.To.Parse("0x687422eEA2cB73B5d3e242bA5456b782919AFc85");
            tx.ValueWei = 1000;
            tx.Data = BitHelper.HexToBytes("c0de");

            byte[] signHash = tx.GetSignHash();
            Assert.IsTrue(BitHelper.BytesToHex(signHash) == "6a74f15f29c3227c5d1d2e27894da58d417a484ef53bc7aa57ee323b42ded656");
        }

        [TestMethod]
        public void Test2()
        {
            var tx = new Transaction();
            tx.ParseHex("f869808504a817c800830186a094687422eea2cb73b5d3e242ba5456b782919afc858203e882c0de1ca0668ed6500efd75df7cb9c9b9d8152292a75453ec2d11030b0eec42f6a7ace602a03efcbbf4d53e0dfa4fde5c6d9a73221418652abc66dff7fddd78b81cc28b9fbf");

            Assert.IsTrue(tx.Nonce == 0);
            Assert.IsTrue(tx.GasPrice == 20000000000);
            Assert.IsTrue(tx.GasLimit == 100000);
            Assert.IsTrue(tx.To.Value == "0x687422eEA2cB73B5d3e242bA5456b782919AFc85");
            Assert.IsTrue(tx.ValueWei == 1000);
            Assert.IsTrue(ByteArray.Equals(tx.Data, BitHelper.HexToBytes("c0de")));
            Assert.IsTrue(tx.v == 0x1C);
            Assert.IsTrue(ByteArray.Equals(tx.r, BitHelper.HexToBytes("668ed6500efd75df7cb9c9b9d8152292a75453ec2d11030b0eec42f6a7ace602")));
            Assert.IsTrue(ByteArray.Equals(tx.s, BitHelper.HexToBytes("3efcbbf4d53e0dfa4fde5c6d9a73221418652abc66dff7fddd78b81cc28b9fbf")));
            Assert.IsTrue(tx.From.Value == "0x53Ae893e4b22D707943299a8d0C844Df0e3d5557");
            Assert.IsTrue(tx.IdString == "0x8b69a0ca303305a92d8d028704d65e4942b7ccc9a99917c8c9e940c9d57a9662");

            Assert.IsTrue(tx.VerifySignature());

            tx.r[15] ^= 1;
            Assert.IsFalse(tx.VerifySignature());
        }

        [TestMethod]
        public void Test3()
        {
            var tx = new Transaction();
            tx.ParseHex("f86b2984b2d05e0082520894001a36a77a56981287dcd806668663b988af96a888054b4008cdd2d000801ca044af12e5f073e15d7e5e5297ee23ea217bb13a9eeb1fd01923e407e20a297881a07a50ecfc6021add35e5efee0a77a2e503b3986e695c16f60ec8675f083ca9ac5");

            Assert.IsTrue(tx.Nonce == 41);
            Assert.IsTrue(tx.GasPrice == 3000000000);
            Assert.IsTrue(tx.GasLimit == 21000);
            Assert.IsTrue(tx.To.Value == "0x001A36a77a56981287DCD806668663B988aF96A8");
            Assert.IsTrue(tx.ValueEther == 0.381469m);
            Assert.IsTrue(tx.Data.Length == 0);
            Assert.IsTrue(tx.From.Value == "0xB2EC9899eD168d0cFf1892a1Ba3F309304675259");
            Assert.IsTrue(tx.IdString == "0xa4a3392aee5ba8c80ca1f8b70a8acafb0b9920720cc46c95fb18d5525e0ba7c8");

            Assert.IsTrue(tx.VerifySignature());

            tx.r[15] ^= 1;
            Assert.IsFalse(tx.VerifySignature());
        }

        [TestMethod]
        public void Test4()
        {
            var tx = new Transaction();
            tx.IsEIP155 = true;
            tx.ChainId = Chain.Ropsten;
            tx.ParseHex("f86f830c1d7784b2d05e008304cb2694e09b30a039465ebc7dff8cc4846e7f6830ff8691880de0b6b3a7640000801ba07831e4d97f314b00d8f22e72bd97e2217cce0126d0465ea053f0b3e5f679e43da0372e483df14c74825a4a3a7aa923899eeed56d25842e4921d8acfef070662d5f");

            Assert.IsTrue(tx.Nonce == 793975);
            Assert.IsTrue(tx.GasPrice == 3000000000);
            Assert.IsTrue(tx.GasLimit == 314150);
            Assert.IsTrue(tx.To.Value == "0xe09b30a039465EBc7dFF8cC4846E7F6830FF8691");
            Assert.IsTrue(tx.ValueEther == 1);
            Assert.IsTrue(tx.Data.Length == 0);
            Assert.IsTrue(tx.From.Value == "0x687422eEA2cB73B5d3e242bA5456b782919AFc85");
            Assert.IsTrue(tx.IdString == "0x6a71183f673364d2e0c655331c58c5ee6c758cf438018e68bc2fe0fe2eeaa116");

            Assert.IsTrue(tx.VerifySignature());

            // test high S
            var s = new BigInteger(tx.s, ByteOrder.BigEndian);
            tx.s = (EllipticCurve.secp256k1.n - s).ToBytes(ByteOrder.BigEndian);

            Assert.IsFalse(tx.VerifySignature());
        }

        [TestMethod]
        public void Test5()
        {
            var tx = new Transaction();
            tx.IsEIP155 = true;
            tx.ChainId = Chain.Ropsten;

            tx.Nonce = 0;
            tx.GasPrice = 5000000000;
            tx.GasLimit = 21000;
            tx.To = new Address();
            tx.To.Parse("0xDa7227a2f8D181bBaa633a28A9dD65663D9B38c3");
            tx.ValueEther = 0.000001m;

            using (StaticRandomTestProvider.Use(null))
            {
                tx.Sign(GetKey());
            }
        }

        private byte[] GetKey()
        {
            var sha256 = MessageDigest.GetInstance(MessageDigests.SHA256);
            return sha256.Digest(Encoding.ASCII.GetBytes("my-testnet-eth"));
        }
    }
}