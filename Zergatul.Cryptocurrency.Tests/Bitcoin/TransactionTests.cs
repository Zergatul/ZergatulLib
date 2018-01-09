using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Bitcoin;

namespace Zergatul.Cryptocurrency.Tests.Bitcoin
{
    [TestClass]
    public class TransactionTests
    {
        private static ITransactionRepository<Transaction> _repository;

        [TestMethod]
        public void Parse1()
        {
            var tx = Transaction.FromHex("01000000013edff2a0a837f864695159d17c92cc588148845c877f0a98651779c05d65e7ce000000006a4730440220165fff9ba94070672ae4d86dda2d66c484f171ab97f6af5ff919c563c65ca17d022035cedddc003b96465db8d24a62e41ee4cfd983083532b6b7d028f44bea7cf1490121029513dea9bae3a0c6beb25bfe07242025ce46567793e7bbbe91537502d6c2d505ffffffff0210037d01000000001976a914a2971b8954610ab1cbf82d577bea9f927b7cca2688ac3063bb0b000000001976a91438c6174002d44124c2d7ef37c0dbf1fcd844c48b88ac00000000");

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
        public void Parse2()
        {
            var tx = Transaction.FromHex("0100000002d8c8df6a6fdd2addaf589a83d860f18b44872d13ee6ec3526b2b470d42a96d4d000000008b483045022100b31557e47191936cb14e013fb421b1860b5e4fd5d2bc5ec1938f4ffb1651dc8902202661c2920771fd29dd91cd4100cefb971269836da4914d970d333861819265ba014104c54f8ea9507f31a05ae325616e3024bd9878cb0a5dff780444002d731577be4e2e69c663ff2da922902a4454841aa1754c1b6292ad7d317150308d8cce0ad7abffffffff2ab3fa4f68a512266134085d3260b94d3b6cfd351450cff021c045a69ba120b2000000008b4830450220230110bc99ef311f1f8bda9d0d968bfe5dfa4af171adbef9ef71678d658823bf022100f956d4fcfa0995a578d84e7e913f9bb1cf5b5be1440bcede07bce9cd5b38115d014104c6ec27cffce0823c3fecb162dbd576c88dd7cda0b7b32b0961188a392b488c94ca174d833ee6a9b71c0996620ae71e799fc7c77901db147fa7d97732e49c8226ffffffff02c0175302000000001976a914a3d89c53bb956f08917b44d113c6b2bcbe0c29b788acc01c3d09000000001976a91408338e1d5e26db3fce21b011795b1c3c8a5a5d0788ac00000000");

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
        public void ParseP2SH_1()
        {
            var tx = Transaction.FromHex("0100000001da75479f893cccfaa8e4558b28ec7cb4309954389f251f2212eabad7d7fda342000000006a473044022048d1468895910edafe53d4ec4209192cc3a8f0f21e7b9811f83b5e419bfb57e002203fef249b56682dbbb1528d4338969abb14583858488a3a766f609185efe68bca0121031a455dab5e1f614e574a2f4f12f22990717e93899695fb0d81e4ac2dcfd25d00ffffffff01301b0f000000000017a914e9c3dd0c07aac76179ebc76a6c78d4d67c6c160a8700000000");

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
            var tx = Transaction.FromHex("010000000001010000000000000000000000000000000000000000000000000000000000000000ffffffff3903f99607232f706f6f6c2e626974636f696e2e636f6d2f5573652f426974636f696e2f436173682f10d78bc800341e8aeebfde2beb202aa500ffffffff02ab46f255000000001976a914a7ea99e1245c4d54d6efce374434b2e091fc481788ac0000000000000000266a24aa21a9ed6b594a5bb845e30fccd404350ed73181278758cc9ec62d077c1f1c4642288a5b0120000000000000000000000000000000000000000000000000000000000000000000000000");

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
            var tx = Transaction.FromHex("010000000001076f21233c0d120e1749fc70c147bb30cabf4206866dc545c6534618006203ead9000000001716001432bedb841cb7e72bc65d7fd56c25514164e7e6b1ffffffff00d32f2f82ee5d6f7544fa816561bcfc9a14a4ca98aaf93de9c48e10519c7daf000000001716001432bedb841cb7e72bc65d7fd56c25514164e7e6b1ffffffff550a214b51573601949c778f6c2e8572cf2fd7ff5d868295748e2e39977545a20100000017160014475aa69550cb501ce7ab5bdc236d243aa749fa83ffffffff8a4340aa98e83b5c1d04b1c933fb21bf4afa801f38be0cccd91fe7019908d96e0100000017160014475aa69550cb501ce7ab5bdc236d243aa749fa83ffffffff69ed7f8453a7fb1918fcecf8592fe564f2f83683de169c9514fdd5bcd4622aa60000000017160014475aa69550cb501ce7ab5bdc236d243aa749fa83ffffffff08f638deea7685d045ea6a624fea823214e65a9cecb82d93f7798411e95c072f0100000017160014475aa69550cb501ce7ab5bdc236d243aa749fa83ffffffff78cae63cd68e06f352e396120065dd25ad493bfbff17f9690d30b575535a44130100000017160014475aa69550cb501ce7ab5bdc236d243aa749fa83ffffffff02002f6859000000001976a914e5b6daf8eab44f0effee3c52affa6500ca03a84088acfcb4c9020000000017a914c59cf95ab43f2686f4cbe7af88838935edf419c7870247304402201f24520b1bbb71461bf33875d8ba8e6d5713badae59bbd0eee79bcf0e3aad7650220488b035a24110509c556a63066386a81b582a8739af202d1cb690a89704f4d21012103f42a8468172f9683d30a067fb9a4893017e8425620e39258988e4977faf5f50c0247304402200d1514ae61dcd17ae1cdcdf6ece8d2e8ae9b68005a29de5e95b4d3fc693e2b00022037e75a0663133c6894ac33b4fb594b1ddc3946c2b3bf2373d37b942c8abb3298012103f42a8468172f9683d30a067fb9a4893017e8425620e39258988e4977faf5f50c0247304402203a903f1c1635643223e2a5e1549564329bc8d8604c2e35bdf8d0289f61899f4f022030c3a82092615e2c97b25584fca9a26b87d42234aa50e0d86ba72d604065ab0c01210283181b032610a20ceb2db3d4bfbd7a0a79388faf7385dab8322689fc0405eb7d024730440220782b37da64cbc268e70f5cc5f0ca70ef9e1fd1da6ce338e05ac4d2a7a219010802206750babec829125b96db7d69ff1eea2d581413db5ed8d121ab02e2465140936e01210283181b032610a20ceb2db3d4bfbd7a0a79388faf7385dab8322689fc0405eb7d024830450221009897df24e10eec5c48a1e254ca1ea2ec02bc9ba8ef4dd33027975f120f66cf0e02203208c185fb896ef4ab867cd7b501a98e4a476b7d7a9f715d4701d8a56fdce03801210283181b032610a20ceb2db3d4bfbd7a0a79388faf7385dab8322689fc0405eb7d0247304402200f4510586c726fa0adcba00e67274c73f30eee34aa6dc123a3e612c237eb2972022070187e317a48c868277bbcb239f5a92d0c8f38144e9f4a542de48317ee606d5701210283181b032610a20ceb2db3d4bfbd7a0a79388faf7385dab8322689fc0405eb7d02483045022100d87c5980eccd71c6ec30c579a66294290e5abf6607ced7c736474ea391a739b402207cc78f465de880e66536b100d008bbfab8e0020e96270ef1a9e4bbc827a2da3301210283181b032610a20ceb2db3d4bfbd7a0a79388faf7385dab8322689fc0405eb7d00000000");

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
        public void ParseVersion2()
        {
            var tx = _repository.GetTransaction("9b82698866bb9626ca4fb6c0becfbb3f2232c65b815795c8929c1adf91278499");
        }

        [TestMethod]
        public void ParseGenesis()
        {
            var tx = Transaction.FromHex("01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff4d04ffff001d0104455468652054696d65732030332f4a616e2f32303039204368616e63656c6c6f72206f6e206272696e6b206f66207365636f6e64206261696c6f757420666f722062616e6b73ffffffff0100f2052a01000000434104678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5fac00000000");

            Assert.IsTrue(tx.IDString == "4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b");
            Assert.IsTrue(tx.IsCoinbase);

            Assert.IsTrue(tx.Outputs.Count == 1);
            Assert.IsTrue(tx.Outputs[0].Address.Value == "1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa");
            Assert.IsTrue(tx.Outputs[0].AmountBTC == 50);
        }

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _repository = new TestTransactionRepository();
        }
    }
}