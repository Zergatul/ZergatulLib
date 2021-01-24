using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Bitcoin;
using Zergatul.Security;
using Zergatul.Security.OpenSsl;
using Zergatul.Security.Tests;

namespace Zergatul.Cryptocurrency.Tests.Bitcoin
{
    [TestClass]
    public class TransactionTests
    {
        private static ITransactionRepository<Transaction> _repository;

        [TestMethod]
        public void Test1()
        {
            var tx = new Transaction();
            Assert.IsTrue(tx.TryParseHex("01000000013edff2a0a837f864695159d17c92cc588148845c877f0a98651779c05d65e7ce000000006a4730440220165fff9ba94070672ae4d86dda2d66c484f171ab97f6af5ff919c563c65ca17d022035cedddc003b96465db8d24a62e41ee4cfd983083532b6b7d028f44bea7cf1490121029513dea9bae3a0c6beb25bfe07242025ce46567793e7bbbe91537502d6c2d505ffffffff0210037d01000000001976a914a2971b8954610ab1cbf82d577bea9f927b7cca2688ac3063bb0b000000001976a91438c6174002d44124c2d7ef37c0dbf1fcd844c48b88ac00000000"));

            Assert.IsTrue(tx.IDString == "1c7e556b3545827a8e1adaed75d869486801a6075d07d7a444fb27f2cbab3192");
            Assert.IsFalse(tx.IsCoinbase);

            Assert.IsTrue(tx.Inputs.Count == 1);
            Assert.IsTrue(tx.Inputs[0].Address.Value == "1PqVTcQNDuyY1b7DwfXkJPvTmQd2SzSuK6");
            Assert.IsTrue(tx.Inputs[0].PrevTxOutIndex == 0);
            Assert.IsTrue(tx.Inputs[0].PrevTxIDString == "cee7655dc0791765980a7f875c84488158cc927cd159516964f837a8a0f2df3e");

            Assert.IsTrue(tx.Outputs.Count == 2);
            Assert.IsTrue(tx.Outputs[0].Address.Value == "1FphVDRQrVjmeo8j4oe8VWmN7rg8Cmo1fe");
            Assert.IsTrue(tx.Outputs[0].AmountBTC == 0.2497m);
            Assert.IsTrue(tx.Outputs[1].Address.Value == "16BC9LNatuiXAzwVH6u6apM1yjx7x2sEQ1");
            Assert.IsTrue(tx.Outputs[1].AmountBTC == 1.9683m);
        }

        [TestMethod]
        public void Test2()
        {
            var tx = new Transaction();
            Assert.IsTrue(tx.TryParseHex("0100000002d8c8df6a6fdd2addaf589a83d860f18b44872d13ee6ec3526b2b470d42a96d4d000000008b483045022100b31557e47191936cb14e013fb421b1860b5e4fd5d2bc5ec1938f4ffb1651dc8902202661c2920771fd29dd91cd4100cefb971269836da4914d970d333861819265ba014104c54f8ea9507f31a05ae325616e3024bd9878cb0a5dff780444002d731577be4e2e69c663ff2da922902a4454841aa1754c1b6292ad7d317150308d8cce0ad7abffffffff2ab3fa4f68a512266134085d3260b94d3b6cfd351450cff021c045a69ba120b2000000008b4830450220230110bc99ef311f1f8bda9d0d968bfe5dfa4af171adbef9ef71678d658823bf022100f956d4fcfa0995a578d84e7e913f9bb1cf5b5be1440bcede07bce9cd5b38115d014104c6ec27cffce0823c3fecb162dbd576c88dd7cda0b7b32b0961188a392b488c94ca174d833ee6a9b71c0996620ae71e799fc7c77901db147fa7d97732e49c8226ffffffff02c0175302000000001976a914a3d89c53bb956f08917b44d113c6b2bcbe0c29b788acc01c3d09000000001976a91408338e1d5e26db3fce21b011795b1c3c8a5a5d0788ac00000000"));

            Assert.IsTrue(tx.IDString == "9021b49d445c719106c95d561b9c3fac7bcb3650db67684a9226cd7fa1e1c1a0");
            Assert.IsFalse(tx.IsCoinbase);

            Assert.IsTrue(tx.Inputs.Count == 2);
            Assert.IsTrue(tx.Inputs[0].Address.Value == "12kNvejJPzT3x7Vkz81z7wpQpVF32nMeJR");
            Assert.IsTrue(tx.Inputs[0].PrevTxOutIndex == 0);
            Assert.IsTrue(tx.Inputs[0].PrevTxIDString == "4d6da9420d472b6b52c36eee132d87448bf160d8839a58afdd2add6f6adfc8d8");
            Assert.IsTrue(tx.Inputs[1].Address.Value == "1MVFBXnqWRAS3wNokffUXY14muBrhfxJD7");
            Assert.IsTrue(tx.Inputs[1].PrevTxOutIndex == 0);
            Assert.IsTrue(tx.Inputs[1].PrevTxIDString == "b220a19ba645c021f0cf501435fd6c3b4db960325d0834612612a5684ffab32a");

            Assert.IsTrue(tx.Outputs.Count == 2);
            Assert.IsTrue(tx.Outputs[0].Address.Value == "1FwLde9A8xyiboJkmjpBnVUYi1DTbXi8yf");
            Assert.IsTrue(tx.Outputs[0].AmountBTC == 0.39m);
            Assert.IsTrue(tx.Outputs[1].Address.Value == "1kNAebmDfKcAfEEC2cRgyVFw5jRSjsAyk");
            Assert.IsTrue(tx.Outputs[1].AmountBTC == 1.55m);
        }

        [TestMethod]
        public void TestP2SH()
        {
            var tx = new Transaction();
            Assert.IsTrue(tx.TryParseHex("0100000001da75479f893cccfaa8e4558b28ec7cb4309954389f251f2212eabad7d7fda342000000006a473044022048d1468895910edafe53d4ec4209192cc3a8f0f21e7b9811f83b5e419bfb57e002203fef249b56682dbbb1528d4338969abb14583858488a3a766f609185efe68bca0121031a455dab5e1f614e574a2f4f12f22990717e93899695fb0d81e4ac2dcfd25d00ffffffff01301b0f000000000017a914e9c3dd0c07aac76179ebc76a6c78d4d67c6c160a8700000000"));

            Assert.IsTrue(tx.IDString == "40eee3ae1760e3a8532263678cdf64569e6ad06abc133af64f735e52562bccc8");
            Assert.IsFalse(tx.IsCoinbase);

            Assert.IsTrue(tx.Inputs.Count == 1);
            Assert.IsTrue(tx.Inputs[0].Address.Value == "12hgRT8xznZdbvhMwNrRzUC6aUXpkCopo5");
            Assert.IsTrue(tx.Inputs[0].PrevTxOutIndex == 0);
            Assert.IsTrue(tx.Inputs[0].PrevTxIDString == "42a3fdd7d7baea12221f259f38549930b47cec288b55e4a8facc3c899f4775da");

            Assert.IsTrue(tx.Outputs.Count == 1);
            Assert.IsTrue(tx.Outputs[0].Address.Value == "3P14159f73E4gFr7JterCCQh9QjiTjiZrG");
            Assert.IsTrue(tx.Outputs[0].AmountBTC == 0.0099m);
        }

        [TestMethod]
        public void ParseSegWit1()
        {
            var tx = new Transaction();
            Assert.IsTrue(tx.TryParseHex("010000000001010000000000000000000000000000000000000000000000000000000000000000ffffffff3903f99607232f706f6f6c2e626974636f696e2e636f6d2f5573652f426974636f696e2f436173682f10d78bc800341e8aeebfde2beb202aa500ffffffff02ab46f255000000001976a914a7ea99e1245c4d54d6efce374434b2e091fc481788ac0000000000000000266a24aa21a9ed6b594a5bb845e30fccd404350ed73181278758cc9ec62d077c1f1c4642288a5b0120000000000000000000000000000000000000000000000000000000000000000000000000"));

            Assert.IsTrue(tx.IDString == "69efdcb4169c4f09a3e83ec9bafef534b9ec3bbcf1e6b921f95a0b6f3f356301");
            Assert.IsTrue(tx.IsCoinbase);

            Assert.IsTrue(tx.Outputs.Count == 2);
            Assert.IsTrue(tx.Outputs[0].Address.Value == "1GJrtQWQNxNxoSLqMG2LXfYU7EH2gvQJek");
            Assert.IsTrue(tx.Outputs[0].AmountBTC == 14.41941163m);
            Assert.IsTrue(tx.Outputs[1].AmountBTC == 0);
        }

        [TestMethod]
        public void ParseSegWit2()
        {
            var tx = new Transaction();
            Assert.IsTrue(tx.TryParseHex("010000000001076f21233c0d120e1749fc70c147bb30cabf4206866dc545c6534618006203ead9000000001716001432bedb841cb7e72bc65d7fd56c25514164e7e6b1ffffffff00d32f2f82ee5d6f7544fa816561bcfc9a14a4ca98aaf93de9c48e10519c7daf000000001716001432bedb841cb7e72bc65d7fd56c25514164e7e6b1ffffffff550a214b51573601949c778f6c2e8572cf2fd7ff5d868295748e2e39977545a20100000017160014475aa69550cb501ce7ab5bdc236d243aa749fa83ffffffff8a4340aa98e83b5c1d04b1c933fb21bf4afa801f38be0cccd91fe7019908d96e0100000017160014475aa69550cb501ce7ab5bdc236d243aa749fa83ffffffff69ed7f8453a7fb1918fcecf8592fe564f2f83683de169c9514fdd5bcd4622aa60000000017160014475aa69550cb501ce7ab5bdc236d243aa749fa83ffffffff08f638deea7685d045ea6a624fea823214e65a9cecb82d93f7798411e95c072f0100000017160014475aa69550cb501ce7ab5bdc236d243aa749fa83ffffffff78cae63cd68e06f352e396120065dd25ad493bfbff17f9690d30b575535a44130100000017160014475aa69550cb501ce7ab5bdc236d243aa749fa83ffffffff02002f6859000000001976a914e5b6daf8eab44f0effee3c52affa6500ca03a84088acfcb4c9020000000017a914c59cf95ab43f2686f4cbe7af88838935edf419c7870247304402201f24520b1bbb71461bf33875d8ba8e6d5713badae59bbd0eee79bcf0e3aad7650220488b035a24110509c556a63066386a81b582a8739af202d1cb690a89704f4d21012103f42a8468172f9683d30a067fb9a4893017e8425620e39258988e4977faf5f50c0247304402200d1514ae61dcd17ae1cdcdf6ece8d2e8ae9b68005a29de5e95b4d3fc693e2b00022037e75a0663133c6894ac33b4fb594b1ddc3946c2b3bf2373d37b942c8abb3298012103f42a8468172f9683d30a067fb9a4893017e8425620e39258988e4977faf5f50c0247304402203a903f1c1635643223e2a5e1549564329bc8d8604c2e35bdf8d0289f61899f4f022030c3a82092615e2c97b25584fca9a26b87d42234aa50e0d86ba72d604065ab0c01210283181b032610a20ceb2db3d4bfbd7a0a79388faf7385dab8322689fc0405eb7d024730440220782b37da64cbc268e70f5cc5f0ca70ef9e1fd1da6ce338e05ac4d2a7a219010802206750babec829125b96db7d69ff1eea2d581413db5ed8d121ab02e2465140936e01210283181b032610a20ceb2db3d4bfbd7a0a79388faf7385dab8322689fc0405eb7d024830450221009897df24e10eec5c48a1e254ca1ea2ec02bc9ba8ef4dd33027975f120f66cf0e02203208c185fb896ef4ab867cd7b501a98e4a476b7d7a9f715d4701d8a56fdce03801210283181b032610a20ceb2db3d4bfbd7a0a79388faf7385dab8322689fc0405eb7d0247304402200f4510586c726fa0adcba00e67274c73f30eee34aa6dc123a3e612c237eb2972022070187e317a48c868277bbcb239f5a92d0c8f38144e9f4a542de48317ee606d5701210283181b032610a20ceb2db3d4bfbd7a0a79388faf7385dab8322689fc0405eb7d02483045022100d87c5980eccd71c6ec30c579a66294290e5abf6607ced7c736474ea391a739b402207cc78f465de880e66536b100d008bbfab8e0020e96270ef1a9e4bbc827a2da3301210283181b032610a20ceb2db3d4bfbd7a0a79388faf7385dab8322689fc0405eb7d00000000"));

            Assert.IsTrue(tx.IDString == "0ebe2d98b077160dcfd0920030eefb69edb5e5da22730790e2c186a698b41580");
            Assert.IsFalse(tx.IsCoinbase);

            Assert.IsTrue(tx.Inputs.Count == 7);
            Assert.IsTrue(tx.Inputs[0].PrevTxIDString == "d9ea036200184653c645c56d860642bfca30bb47c170fc49170e120d3c23216f");
            Assert.IsTrue(tx.Inputs[0].PrevTxOutIndex == 0);
            Assert.IsTrue(tx.Inputs[1].PrevTxIDString == "af7d9c51108ec4e93df9aa98caa4149afcbc616581fa44756f5dee822f2fd300");
            Assert.IsTrue(tx.Inputs[1].PrevTxOutIndex == 0);
            Assert.IsTrue(tx.Inputs[2].PrevTxIDString == "a2457597392e8e749582865dffd72fcf72852e6c8f779c94013657514b210a55");
            Assert.IsTrue(tx.Inputs[2].PrevTxOutIndex == 1);
            Assert.IsTrue(tx.Inputs[3].PrevTxIDString == "6ed9089901e71fd9cc0cbe381f80fa4abf21fb33c9b1041d5c3be898aa40438a");
            Assert.IsTrue(tx.Inputs[3].PrevTxOutIndex == 1);
            Assert.IsTrue(tx.Inputs[4].PrevTxIDString == "a62a62d4bcd5fd14959c16de8336f8f264e52f59f8ecfc1819fba753847fed69");
            Assert.IsTrue(tx.Inputs[4].PrevTxOutIndex == 0);
            Assert.IsTrue(tx.Inputs[5].PrevTxIDString == "2f075ce9118479f7932db8ec9c5ae6143282ea4f626aea45d08576eade38f608");
            Assert.IsTrue(tx.Inputs[5].PrevTxOutIndex == 1);
            Assert.IsTrue(tx.Inputs[6].PrevTxIDString == "13445a5375b5300d69f917fffb3b49ad25dd65001296e352f3068ed63ce6ca78");
            Assert.IsTrue(tx.Inputs[6].PrevTxOutIndex == 1);

            Assert.IsTrue(tx.Outputs.Count == 2);
            Assert.IsTrue(tx.Outputs[0].Address.Value == "1Mwcnwf6bVGF7mgeD2xDbsxLWhQSt3CXXi");
            Assert.IsTrue(tx.Outputs[0].AmountBTC == 15m);
            Assert.IsTrue(tx.Outputs[1].Address.Value == "3Khu65hgB6gZyAdwzfQhgdtN7Bsvg9UzCx");
            Assert.IsTrue(tx.Outputs[1].AmountBTC == 0.467735m);
        }

        [TestMethod]
        public void VerifyP2WPKH()
        {
            var tx = new Transaction();
            Assert.IsTrue(tx.TryParseHex("01000000000102fff7f7881a8099afa6940d42d1e7f6362bec38171ea3edf433541db4e4ad969f00000000494830450221008b9d1dc26ba6a9cb62127b02742fa9d754cd3bebf337f7a55d114c8e5cdd30be022040529b194ba3f9281a99f2b1c0a19c0489bc22ede944ccf4ecbab4cc618ef3ed01eeffffffef51e1b804cc89d182d279655c3aa89e815b1b309fe287d9b2b55d57b90ec68a0100000000ffffffff02202cb206000000001976a9148280b37df378db99f66f85c95a783a76ac7a6d5988ac9093510d000000001976a9143bde42dbee7e4dbe6a21b2d50ce2f0167faa815988ac000247304402203609e17b84f6a7d30c80bfa610b5b4542f32a8a0d5447a12fb1366d7f01cc44a0220573a954c4518331561406f90300e8f3358f51928d43c212a8caed02de67eebee0121025476c2e83188368da1ff3e292e7acafcdb3566bb0ad253f62fc70f07aeee635711000000"));

            byte[] rawScript = BitHelper.HexToBytes("2103c9f4836b9a4f77fc0d81f7bcb01b7f1b35916864b9476c241ce9fc198bd25432ac");
            var prevTx1 = new Transaction
            {
                Outputs = new System.Collections.Generic.List<TxOutput>
                {
                    new TxOutput
                    {
                        AmountBTC = 6.25m,
                        ScriptRaw = rawScript,
                        Script = Script.FromBytes(rawScript)
                    }
                }
            };
            TestHelper.SetPrivateField(prevTx1, "_id", tx.Inputs[0].PrevTx);
            tx.Inputs[0].PrevTransaction = prevTx1;

            rawScript = BitHelper.HexToBytes("00141d0f172a0ecb48aee1be1f2687d2963ae33f71a1");
            var prevTx2 = new Transaction
            {
                Outputs = new System.Collections.Generic.List<TxOutput>
                {
                    new TxOutput(),
                    new TxOutput
                    {
                        AmountBTC = 6m,
                        ScriptRaw = rawScript,
                        Script = Script.FromBytes(rawScript)
                    }
                }
            };
            TestHelper.SetPrivateField(prevTx2, "_id", tx.Inputs[1].PrevTx);
            tx.Inputs[1].PrevTransaction = prevTx2;

            Assert.IsTrue(tx.Inputs[1].Verify());
        }

        [TestMethod]
        public void VerifyP2SH_P2WPKH()
        {
            var tx = new Transaction();
            Assert.IsTrue(tx.TryParseHex("01000000000101db6b1b20aa0fd7b23880be2ecbd4a98130974cf4748fb66092ac4d3ceb1a5477010000001716001479091972186c449eb1ded22b78e40d009bdf0089feffffff02b8b4eb0b000000001976a914a457b684d7f0d539a46a45bbc043f35b59d0d96388ac0008af2f000000001976a914fd270b1ee6abcaea97fea7ad0402e8bd8ad6d77c88ac02473044022047ac8e878352d3ebbde1c94ce3a10d057c24175747116f8288e5d794d12d482f0220217f36a485cae903c713331d877c1f64677e3622ad4010726870540656fe9dcb012103ad1d8e89212f0b92c74d23bb710c00662ad1470198ac48c43f7d6f93a2a2687392040000"));

            byte[] rawScript = BitHelper.HexToBytes("a9144733f37cf4db86fbc2efed2500b4f4e49f31202387");
            var prevTx = new Transaction
            {
                Outputs = new System.Collections.Generic.List<TxOutput>
                {
                    new TxOutput(),
                    new TxOutput
                    {
                        AmountBTC = 10m,
                        ScriptRaw = rawScript,
                        Script = Script.FromBytes(rawScript)
                    }
                }
            };
            TestHelper.SetPrivateField(prevTx, "_id", tx.Inputs[0].PrevTx);
            tx.Inputs[0].PrevTransaction = prevTx;

            Assert.IsTrue(tx.Inputs[0].Verify());
        }

        [TestMethod]
        public void ParseV2()
        {
            var tx = _repository.GetTransaction("9b82698866bb9626ca4fb6c0becfbb3f2232c65b815795c8929c1adf91278499");
        }

        [TestMethod]
        public void ParseGenesis()
        {
            var tx = new Transaction();
            Assert.IsTrue(tx.TryParseHex("01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff4d04ffff001d0104455468652054696d65732030332f4a616e2f32303039204368616e63656c6c6f72206f6e206272696e6b206f66207365636f6e64206261696c6f757420666f722062616e6b73ffffffff0100f2052a01000000434104678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5fac00000000"));

            Assert.IsTrue(tx.IDString == "4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b");
            Assert.IsTrue(tx.IsCoinbase);

            Assert.IsTrue(tx.Outputs.Count == 1);
            Assert.IsTrue(tx.Outputs[0].Address.Value == "1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa");
            Assert.IsTrue(tx.Outputs[0].AmountBTC == 50);
        }

        [TestMethod]
        public void VerifyP2WPKH2()
        {
            var tx = _repository.GetTransaction("ae8a4150e4c5719e48ce8f88fbd20b5f3b5adc2e9a1b08c17d3088f2d0b945db");

            Assert.IsTrue(tx.IDString == "ae8a4150e4c5719e48ce8f88fbd20b5f3b5adc2e9a1b08c17d3088f2d0b945db");

            Assert.IsTrue(tx.Inputs.Count == 1);
            Assert.IsTrue(tx.Inputs[0].Address.Value == "bc1q52zucsn7al3nr7unq8y2hs52sy0z6phqgw5mrl");

            Assert.IsTrue(tx.Outputs.Count == 3);
            Assert.IsTrue(tx.Outputs[0].Address.Value == "bc1qkymumss6zj0rxy9l3v5vqxqwwffy8jjsw3c9cm");
            Assert.IsTrue(tx.Outputs[0].AmountBTC == 0.002m);
            Assert.IsTrue(tx.Outputs[1].Address.Value == "bc1qt727hkz4d28xdyl4pt7y9l4sxl0prfqsugdfwr");
            Assert.IsTrue(tx.Outputs[1].AmountBTC == 1.00038784m);
            Assert.IsTrue(tx.Outputs[2].Address.Value == "bc1q9vklkehvff3ftyr2rxy3uhppaset3hvn5r6caq");
            Assert.IsTrue(tx.Outputs[2].AmountBTC == 5.99778785m);

            Assert.IsTrue(tx.Verify(_repository));
        }

        [TestMethod]
        public void VerifyP2SHP2WPKH()
        {
            var tx = _repository.GetTransaction("2e066b452c0fd68163694143c35391478cb6298270f111b21d78dc31022ca402");

            Assert.IsTrue(tx.IDString == "2e066b452c0fd68163694143c35391478cb6298270f111b21d78dc31022ca402");

            Assert.IsTrue(tx.Inputs.Count == 1);
            Assert.IsTrue(tx.Inputs[0].Address.Value == "3E6wDvWHDWmsnz4uS5GBy4cRVLWFegW3nW");

            Assert.IsTrue(tx.Outputs.Count == 2);
            Assert.IsTrue(tx.Outputs[0].Address.Value == "1C7VZuzzCoGDZCvAaRH1kxLFd87aytJBG4");
            Assert.IsTrue(tx.Outputs[0].AmountBTC == 0.02104789m);
            Assert.IsTrue(tx.Outputs[1].Address.Value == "34dWaijnipjjZ6YyMnf1ZJs3UF5PSFMxuF");
            Assert.IsTrue(tx.Outputs[1].AmountBTC == 0.04500147m);

            Assert.IsTrue(tx.Verify(_repository));
        }

        [TestMethod]
        public void VerifyP2SHP2WPKH2()
        {
            var tx = _repository.GetTransaction("31e89ec088dfbfc778afa66b954d04f36e07687656978a3c15e499dc81e18960");

            Assert.IsTrue(tx.IDString == "31e89ec088dfbfc778afa66b954d04f36e07687656978a3c15e499dc81e18960");

            Assert.IsTrue(tx.Inputs.Count == 1);
            Assert.IsTrue(tx.Inputs[0].Address.Value == "37mZZjrA1zihRVNDTjuWQivm4aaND1zxZ8");

            Assert.IsTrue(tx.Outputs.Count == 2);
            Assert.IsTrue(tx.Outputs[0].Address.Value == "3BbJEWWsCPbZEqCfEs98sXtwfY7wpw3Y39");
            Assert.IsTrue(tx.Outputs[0].AmountBTC == 0.01700000m);
            Assert.IsTrue(tx.Outputs[1].Address.Value == "3Hp1NG7qVCwiyjBXCo7b2QwGaFhcP45xJj");
            Assert.IsTrue(tx.Outputs[1].AmountBTC == 5.61213956m);

            Assert.IsTrue(tx.Verify(_repository));
        }

        [TestMethod]
        public void Sign1Test()
        {
            // SHA256(my-testnet-bitcoins)
            // n2fTiBzAQZrjKdmDi3dzV8JxzMYDmLsdUx
            // send back to mv4rnyY3Su5gjcDNzbMLKBQkBicCtHUtFB

            var sha256 = MessageDigest.GetInstance(MessageDigests.SHA256);
            sha256.Update(Encoding.ASCII.GetBytes("my-testnet-bitcoins"));
            var key = sha256.Digest();

            var prevtx = new Cryptocurrency.Bitcoin.Testnet.Transaction();
            Assert.IsTrue(prevtx.TryParseHex("0200000000010140bd2b54d4ec327579f0dbb6f4a95381c45d133959d0b659e500788fafef18bb0000000017160014dddbcf7e8b482a3894aaac55c09aded3e72515cffeffffff026101e400000000001976a914e7f6864669556005b2caef6926896549aa4bd76988accbf1f0276f00000017a914580416bdccba76fd7a8b037d15c26f9729181416870247304402206b81c68dda027a2705822f83b85bb868cf169d54d92ae43bd14706aada97e8dc02205904fb10e094589e06de3105bf15fdf2b8288fc5dcecee54c6cc4e79fa8e7b9f012103e4867c884214a8954b191577506b1c7c185c4a48972fe6202f2ca4601c27968d2a0c1600"));
            Assert.IsTrue(prevtx.IDString == "7987e7ae907aaf29eea115ebf2253ae4027f2d5918ef67e9f5ffee5231a05694");

            Assert.IsTrue(prevtx.Inputs.Count == 1);
            Assert.IsTrue(prevtx.Inputs[0].Address.Value == "2Myxr8yTcFEV6GTstvsi16U8sRstN6UnUDY");

            Assert.IsTrue(prevtx.Outputs.Count == 2);
            Assert.IsTrue(prevtx.Outputs[0].Address.Value == "n2fTiBzAQZrjKdmDi3dzV8JxzMYDmLsdUx");
            Assert.IsTrue(prevtx.Outputs[0].AmountBTC == 0.14942561m);
            Assert.IsTrue(prevtx.Outputs[1].Address.Value == "2N1GcMGAC2tS2LLeGwzrSacBVaF4Ac1Vrfj");
            Assert.IsTrue(prevtx.Outputs[1].AmountBTC == 4774.11471819m);

            var tx = new Cryptocurrency.Bitcoin.Testnet.Transaction
            {
                Version = 2,
                IsSegWit = false
            };

            var inputAddr = new Cryptocurrency.Bitcoin.Testnet.P2PKHAddress();
            inputAddr.FromPrivateKey(new Secp256k1PrivateKey(key, true));

            tx.Inputs = new System.Collections.Generic.List<Cryptocurrency.Bitcoin.Testnet.TxInput>
            {
                new Cryptocurrency.Bitcoin.Testnet.TxInput
                {
                    Address = inputAddr,
                    PrevTransaction = prevtx,
                    PrevTxOutIndex = 0,
                    SequenceNo = 0xFFFFFFFF
                }
            };

            tx.Outputs = new System.Collections.Generic.List<Cryptocurrency.Bitcoin.Testnet.TxOutput>
            {
                new Cryptocurrency.Bitcoin.Testnet.TxOutput
                {
                    Address = inputAddr,
                    AmountBTC = 0.14922561m
                }
            };

            Assert.IsTrue(tx.FeeBTC == 0.0002m);

            var random = BitHelper.HexToBytes("1100110011001100110011001100110011001100110011001100110011001100");
            using (MockSecureRandom.Use(random))
            {
                tx.Sign();
            }

            var serialized = tx.ToBytes();
            Assert.IsTrue(BitHelper.BytesToHex(serialized) == "02000000019456a03152eefff5e967ef18592d7f02e43a25f2eb15a1ee29af7a90aee78779000000006b483045022100822251144db90ecdab676008873ed9bd4132f3e6e93b8b5c57fb795389fdc15702200a1ae8f4ad03cffdf6e828703a00ecb8ea220f343534cc64d1ef77647712e0ff0121024014cdc58a85820d0f5bfe731b3becb985bebdcf7bae2dabb1fb735a66eb81a8ffffffff0141b3e300000000001976a914e7f6864669556005b2caef6926896549aa4bd76988ac00000000");

            var tx2 = new Cryptocurrency.Bitcoin.Testnet.Transaction();
            Assert.IsTrue(tx2.TryParse(serialized));
            Assert.IsTrue(tx2.IDString == "58b5efb59094849dec23c01ab51cf0a509a6eaf6fda17f2a04a67a6d70be2267");
            tx2.Inputs[0].PrevTransaction = prevtx;
            Assert.IsTrue(tx2.Verify());
        }

        [TestMethod]
        public void Sign2Test()
        {
            // SHA256(my-testnet-bitcoins)
            // n2fTiBzAQZrjKdmDi3dzV8JxzMYDmLsdUx
            // send back to mv4rnyY3Su5gjcDNzbMLKBQkBicCtHUtFB

            var sha256 = MessageDigest.GetInstance(MessageDigests.SHA256);
            sha256.Update(Encoding.ASCII.GetBytes("my-testnet-bitcoins"));
            var key = sha256.Digest();

            var prevtx = new Cryptocurrency.Bitcoin.Testnet.Transaction();
            Assert.IsTrue(prevtx.TryParseHex("02000000019456a03152eefff5e967ef18592d7f02e43a25f2eb15a1ee29af7a90aee78779000000006b483045022100822251144db90ecdab676008873ed9bd4132f3e6e93b8b5c57fb795389fdc15702200a1ae8f4ad03cffdf6e828703a00ecb8ea220f343534cc64d1ef77647712e0ff0121024014cdc58a85820d0f5bfe731b3becb985bebdcf7bae2dabb1fb735a66eb81a8ffffffff0141b3e300000000001976a914e7f6864669556005b2caef6926896549aa4bd76988ac00000000"));
            Assert.IsTrue(prevtx.IDString == "58b5efb59094849dec23c01ab51cf0a509a6eaf6fda17f2a04a67a6d70be2267");

            Assert.IsTrue(prevtx.Inputs.Count == 1);
            Assert.IsTrue(prevtx.Inputs[0].Address.Value == "n2fTiBzAQZrjKdmDi3dzV8JxzMYDmLsdUx");

            Assert.IsTrue(prevtx.Outputs.Count == 1);
            Assert.IsTrue(prevtx.Outputs[0].Address.Value == "n2fTiBzAQZrjKdmDi3dzV8JxzMYDmLsdUx");
            Assert.IsTrue(prevtx.Outputs[0].AmountBTC == 0.14922561m);

            var tx = new Cryptocurrency.Bitcoin.Testnet.Transaction
            {
                Version = 2,
                IsSegWit = false
            };

            var inputAddr = new Cryptocurrency.Bitcoin.Testnet.P2PKHAddress();
            inputAddr.FromPrivateKey(new Secp256k1PrivateKey(key, true));

            var addr2 = new Cryptocurrency.Bitcoin.Testnet.P2SHP2WPKHAddress();
            addr2.FromPrivateKey(key);

            Assert.IsTrue(addr2.Value == "2NAW9gmpw5XMUso7CJakncP1yHvo5sTRTmg");

            tx.Inputs = new System.Collections.Generic.List<Cryptocurrency.Bitcoin.Testnet.TxInput>
            {
                new Cryptocurrency.Bitcoin.Testnet.TxInput
                {
                    Address = inputAddr,
                    PrevTransaction = prevtx,
                    PrevTxOutIndex = 0,
                    SequenceNo = 0xFFFFFFFF
                }
            };

            tx.Outputs = new System.Collections.Generic.List<Cryptocurrency.Bitcoin.Testnet.TxOutput>
            {
                new Cryptocurrency.Bitcoin.Testnet.TxOutput
                {
                    Address = inputAddr,
                    AmountBTC = 0.148m
                },
                new Cryptocurrency.Bitcoin.Testnet.TxOutput
                {
                    Address = addr2,
                    AmountBTC = 0.001m
                }
            };

            Assert.IsTrue(tx.FeeBTC == 0.00022561m);

            var random = BitHelper.HexToBytes("1100110011001100110011001100110011001100110011001100110011001100");
            using (MockSecureRandom.Use(random))
            {
                tx.Sign();
            }

            var serialized = tx.ToBytes();
            Assert.IsTrue(BitHelper.BytesToHex(serialized) == "02000000016722be706d7aa6042a7fa1fdf6eaa609a5f01cb51ac023ec9d849490b5efb558000000006b483045022100822251144db90ecdab676008873ed9bd4132f3e6e93b8b5c57fb795389fdc1570220037bd9b33e7009b065d9d6b34f1bf781916f48fd66b9c56f1896b0e7931ef7310121024014cdc58a85820d0f5bfe731b3becb985bebdcf7bae2dabb1fb735a66eb81a8ffffffff0280d4e100000000001976a914e7f6864669556005b2caef6926896549aa4bd76988aca08601000000000017a914bd4cc9657cd2c5de3ed0999f8599e699b46a01de8700000000");

            var tx2 = new Cryptocurrency.Bitcoin.Testnet.Transaction();
            Assert.IsTrue(tx2.TryParse(serialized));
            Assert.IsTrue(tx2.IDString == "3c67f649f39913c9dd8b02a6d512ddebb74fdba9c137aacd7268ecd65bbd216b");
            Assert.IsTrue(tx2.Outputs[1].Address.Value == "2NAW9gmpw5XMUso7CJakncP1yHvo5sTRTmg");
            tx2.Inputs[0].PrevTransaction = prevtx;
            Assert.IsTrue(tx2.Verify());
        }

        [TestMethod]
        public void SignSegwit1Test()
        {
            // SHA256(my-testnet-bitcoins)
            // n2fTiBzAQZrjKdmDi3dzV8JxzMYDmLsdUx
            // send back to mv4rnyY3Su5gjcDNzbMLKBQkBicCtHUtFB

            var sha256 = MessageDigest.GetInstance(MessageDigests.SHA256);
            sha256.Update(Encoding.ASCII.GetBytes("my-testnet-bitcoins"));
            var key = sha256.Digest();

            var prevtx = new Cryptocurrency.Bitcoin.Testnet.Transaction();
            Assert.IsTrue(prevtx.TryParseHex("02000000016722be706d7aa6042a7fa1fdf6eaa609a5f01cb51ac023ec9d849490b5efb558000000006b483045022100822251144db90ecdab676008873ed9bd4132f3e6e93b8b5c57fb795389fdc1570220037bd9b33e7009b065d9d6b34f1bf781916f48fd66b9c56f1896b0e7931ef7310121024014cdc58a85820d0f5bfe731b3becb985bebdcf7bae2dabb1fb735a66eb81a8ffffffff0280d4e100000000001976a914e7f6864669556005b2caef6926896549aa4bd76988aca08601000000000017a914bd4cc9657cd2c5de3ed0999f8599e699b46a01de8700000000"));

            var tx = new Cryptocurrency.Bitcoin.Testnet.Transaction
            {
                Version = 2,
                IsSegWit = true
            };

            var addr = new Cryptocurrency.Bitcoin.Testnet.P2SHP2WPKHAddress();
            addr.FromPrivateKey(key);

            Assert.IsTrue(addr.Value == "2NAW9gmpw5XMUso7CJakncP1yHvo5sTRTmg");

            tx.Inputs = new System.Collections.Generic.List<Cryptocurrency.Bitcoin.Testnet.TxInput>
            {
                new Cryptocurrency.Bitcoin.Testnet.TxInput
                {
                    Address = addr,
                    PrevTransaction = prevtx,
                    PrevTxOutIndex = 1,
                    SequenceNo = 0xFFFFFFFF
                }
            };

            tx.Outputs = new System.Collections.Generic.List<Cryptocurrency.Bitcoin.Testnet.TxOutput>
            {
                new Cryptocurrency.Bitcoin.Testnet.TxOutput
                {
                    Address = addr,
                    AmountBTC = 0.0008m
                }
            };

            Assert.IsTrue(tx.FeeBTC == 0.0002m);

            var random = BitHelper.HexToBytes("1100110011001100110011001100110011001100110011001100110011001100");
            using (MockSecureRandom.Use(random))
            {
                tx.Sign();
            }

            var serialized = tx.ToBytes();
            Assert.IsTrue(BitHelper.BytesToHex(serialized) == "020000000001016b21bd5bd6ec6872cdaa37c1a9db4fb7ebdd12d5a6028bddc91399f349f6673c0100000017160014e7f6864669556005b2caef6926896549aa4bd769ffffffff01803801000000000017a914bd4cc9657cd2c5de3ed0999f8599e699b46a01de8702483045022100822251144db90ecdab676008873ed9bd4132f3e6e93b8b5c57fb795389fdc1570220463fdbaaecb5c117613f8c24668e1213e270070ac2837129b1f83c8ee770810e0121024014cdc58a85820d0f5bfe731b3becb985bebdcf7bae2dabb1fb735a66eb81a800000000");

            var tx2 = new Cryptocurrency.Bitcoin.Testnet.Transaction();
            Assert.IsTrue(tx2.TryParse(serialized));
            Assert.IsTrue(tx2.IDString == "f647c091631644e524a22d32047fee5e2cc29e8ed70751f0ac74da7e502feaa8");
            Assert.IsTrue(tx2.Outputs[0].Address.Value == "2NAW9gmpw5XMUso7CJakncP1yHvo5sTRTmg");
            tx2.Inputs[0].PrevTransaction = prevtx;
            Assert.IsTrue(tx2.Verify());
        }

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            SecurityProvider.UnregisterAll();
            SecurityProvider.Register(new OpenSslProvider());

            _repository = new SimpleTransactionRepository<Transaction>("Bitcoin/Transactions.txt");
        }
    }
}