using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Ethereum;
using Zergatul.Cryptography.Hash;
using Zergatul.Cryptography.Asymmetric;

namespace Zergatul.Cryptocurrency.Tests.Ethereum
{
    [TestClass]
    public class TransactionTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            // https://github.com/ethereum/wiki/wiki/%5BEnglish%5D-RLP
            // https://ropsten.etherscan.io/getRawTx?tx=0x8b69a0ca303305a92d8d028704d65e4942b7ccc9a99917c8c9e940c9d57a9662
            // https://medium.com/@codetractio/inside-an-ethereum-transaction-fa94ffca912f
            var addr = new Address();
            addr.FromPrivateKey("c0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0dec0de");
            Assert.IsTrue(addr.Value == "0x53ae893e4b22d707943299a8d0c844df0e3d5557");

            var tx = new Transaction();
            tx.Nonce = 0;
            tx.GasPrice = 20000000000;
            tx.GasLimit = 100000;
            tx.To = new Address();
            tx.To.Parse("0x687422eEA2cB73B5d3e242bA5456b782919AFc85");
            tx.ValueWei = 1000;
            tx.Data = BitHelper.HexToBytes("c0de");

            Assert.IsTrue(BitHelper.BytesToHex(tx.GetSignHash()) == "6a74f15f29c3227c5d1d2e27894da58d417a484ef53bc7aa57ee323b42ded656");

            ECPDSA ecdsa = new ECPDSA();
            ecdsa.Parameters = new ECPDSAParameters(Math.EllipticCurves.PrimeField.EllipticCurve.secp256k1);
            byte[] pub = ecdsa.RecoverPublicKey(
                0x1C,
                BitHelper.HexToBytes("668ed6500efd75df7cb9c9b9d8152292a75453ec2d11030b0eec42f6a7ace602"),
                BitHelper.HexToBytes("3efcbbf4d53e0dfa4fde5c6d9a73221418652abc66dff7fddd78b81cc28b9fbf"));

            var from = new Address();
            from.FromPublicKey(pub);
            Assert.IsTrue(from.Value == "0x53ae893e4b22d707943299a8d0c844df0e3d5557");
        }
    }
}