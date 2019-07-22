using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Haval256Tests
    {
        private SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        private string Name => MessageDigests.Haval256;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "be417bb4dd5cfb76c7126f4f8eeb1553a449039307b1a3cd451dbfdc0fbbe330");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "976cd6254c337969e5913b158392a2921af16fca51f5601d486e0a9de01156e7");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "b89c551cdfe2e06dbd4cea2be1bc7d557416c58ebb4d07cbc94e49f710c55be4");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "896a7d08869dc2cd5d17fdfbc8af9de4a9ea92695886d1c8070e91de84ceeea5");
                    md.Reset();
                }
        }

        [TestMethod]
        public void DifferentLengthTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    byte[] data = new byte[] { (byte)'a' };

                    void Test(int count, string hash)
                    {
                        md.Reset();
                        for (int i = 0; i < count; i++)
                            md.Update(data, 0, 1);
                        var digest = md.Digest();
                        Assert.IsTrue(BitHelper.BytesToHex(digest) == hash);
                    }

                    Test(10, "50b3bb4d0c1739f6563e2a1a85de72827f86e29b5a9cc14092962363bf718424");
                    Test(11, "c1f55510424d1def8cd42d02106336ed40db6f25bb9407bbf35761ae274e0562");
                    Test(12, "31813a4ea2f25052b51bd1f414faf752e3effb224df97d4a789d0612da1be68c");
                    Test(13, "1715b9efa3055db467f3d6ab21b6efc165d29966ce588dcffdfaaaadb63b1ad7");
                    Test(14, "6b82ffcbb3a6724bd79628654678f7a8bb6176b1f2e4acd9ba1bac5cc5c253f8");
                    Test(15, "afbf7176e57d34b94ad273386652a688d44f6a27c0b1cf3a4827f21edc73c826");
                    Test(16, "995a764740605224a60f02a48181944216879b46a044a5678b7239a5ea1e0166");
                    Test(17, "a081446c7e694858779a18ee16d96a6d1c41fd01a3cd371090b4dcf357c04175");
                    Test(18, "14ef099abc1bc03ba74012f2bb1514b27dc7344e1c248f6b007b6ffbfaa533c8");
                    Test(19, "03163adff3709df6ee8aa8eb39e3c47a209c229fee24e102286d3fe6865212d8");
                    Test(20, "47e0e5eb4c7a5a736c2cee4a724dc7aa5756cb67f1f5f6ba259fc0bd887da64d");
                    Test(21, "04699584f713bfb4bc1afc743b243665aad0a97e30d1aa763a5189f70b7bd4f7");
                    Test(22, "97ee6d5bc2a0d4f7a51c02fb743dd6526750c77f31b8549a5605fcdc4f239a2c");
                    Test(23, "ee1e332f832c798627e2f11e3967d761d165b4c575deb63a4238b11fd103281b");
                    Test(24, "065c8561f2bb0415b1449c91d59fe96b86e21a6102f5d59d71795e4d21fd4ceb");
                    Test(25, "d40f7b3a48046161814728c526f3512ab7050083a31be85eb6c18593406a4700");
                    Test(26, "6925f39f0f20779538c27f790bf12203869ef262406fb58f93491ce9b56e0310");
                    Test(27, "945826e049c8407441c1bca1c61f535e876ebdf750c18026e3bf7ccb4ff62dfb");
                    Test(28, "bb7b4a0587f6893610ebabd52dd438571ffca6caf684bedc2552a413b6e8d049");
                    Test(29, "6ada9e1bb06c0d4588db68c32216b18cb115075e96520a7972cf138eeda9829b");
                    Test(30, "5a79b2b4e9474e4a026a082c5824086aa64913532f5f6716f2dc2db5748dcaf6");
                    Test(31, "9f88e1178b1d9adb587125fdef713c5fa9176f8fe9633758186ebef9883a5e06");
                    Test(32, "37a5593ea8ed233b329327c6ec069780b0682d8d4d6c4490ef8e070f9702e304");
                    Test(33, "60948d3091b0213282376d7d0b57f1ba89cc88eecb22c1b947d0b35db2e3c211");
                    Test(34, "872fd93c43f30a4d3ab31c7f7a5e10c72831da12b3b39f9115301db6273a3827");
                    Test(35, "f06f2421b5497ee15407c24b110621ef4b4a57c72429987a25b02ba16317d9c1");
                    Test(36, "fa03d14e83c0c9512c85209594866adf9a52ef098a6240905259568a2e0d2fc6");
                    Test(37, "dfa30ba22eeabfdc473052418f00ae31a3ddc3096d0e79aa1192dc0b218b9cd7");
                    Test(38, "371001f4610b2edb527274dc10aca64850351c9cf18e48bfd5d01bdd0b1e2369");
                    Test(39, "77fdba048aee53492559653d3a29c63beb687d900cd864a6087e82e335c0d14d");
                    Test(40, "5f949a5e5a472d462eb7626f50eb409f6737a94b71bc4a394591b378ebbbdaa5");
                    Test(41, "fba4dbf36662ba73b7ceb0033cd2c3486d55e367a76c43fd2a171ce95a958dcc");
                    Test(42, "a43aa803194e336b028c7f6b72b5e9271f8025af54825d43aa2754dea3b7930d");
                    Test(43, "ceafcb71daa95764d32a012dfe4247abb171aca79b87d8697c1ce4be7e5201da");
                    Test(44, "d5bb87fecda5197e0837ee28ff4d73173319553e2f990595f6b0479e71dc1c8a");
                    Test(45, "3a8eee4001fdbb3e6d209fdf42fd69c8b6a6722e40dfaea6c48f2daef59c45bf");
                    Test(46, "077d1951218060419b493224725499a1df960805cfdf825f53b6fadb5a7c6be1");
                    Test(47, "22c7aeee5e51ec519248387f80c4211452e979775cee203dbf49a93b135acc44");
                    Test(48, "ff7e0aeb762ee3f4f55a36ca6e4921edad4144eeb9eea375169981f7ff81cd6a");
                    Test(49, "f74662689eed0029d2143b179b1a52944facfdcecf788e4bea223d7657b41490");
                    Test(50, "95b654e097bf11e5c493f194c99de750cb9c91cc118c81d1f0e63e83c03987d1");
                    Test(51, "b520954491bb15cd6efcdb7c786d037c8847b46972d53999cd1f2eefb995b7c2");
                    Test(52, "b48447c9fc42a3d03e763ab4bed088e0daefa17147fab638e4e08e9fb4b7c3c8");
                    Test(53, "3e637b25f18d2514b564c6216861de7c0ad809f2786e9bc7e14cabadb9633e4f");
                    Test(54, "95eab7bfaf0e4b939d7efd8485de9a4a4c1cf9018ac0d539f3da6d58dd295497");
                    Test(55, "3492368f009965c4f9d20a036bf2d5dd877f2aacf5ff556136fb3f199ecff335");
                    Test(56, "0d7b0af580417cc385d2e12a64df5b1ebd4c728807bef874aedc051280bdffb5");
                    Test(57, "2e1b1b4d80916ab0b5dbccdc7c57802c6ab0190846000e9867c84c85edbda681");
                    Test(58, "f4b3a6902b1bec13f0ef173113e6a8c321b31cd8fe54f15a90f1bb3c28ffa034");
                    Test(59, "f6f664695ffb5dd5dec406a5669897c5671b06c583cd4d070daa9dde92f25eb1");
                    Test(60, "3cbf03df55e9247acda572d2f8f0686dbad910350a70e8417bbe00b8cd4908c3");
                    Test(61, "6312e154af0505f0f86e31f9922bbab9aec486ba37b5116a32c7908ecb7fd77f");
                    Test(62, "5e3a683571f8dd1d185a19a8bce7a62592d0710020448ceb6fe246310780d661");
                    Test(63, "6c2e4c3c52b0326f9f33b96d8c239bcf305df581555ff28b5900424289ceed00");
                    Test(64, "3ea25d82cd11c1163bf338e03dd006aec03851c61799be072284c51226f63cf3");
                    Test(65, "abe5e647abcaf46a428659cf741526731d35e9e10b815b2c8d0b636853ae1b28");
                    Test(66, "c0c0a19e94a2ccb9cbf13b35dcfb0303fe284f153bcb368d17886aca37e945dd");
                    Test(67, "0c6419798d0d67981abad2258476acb79c9d08ea5f0fdef5f779fb8f42cb969c");
                    Test(68, "15193d331dfdc057abf13370ed686e8eb1c9226bcea504858620ee99c3719f68");
                    Test(69, "2f20dcc784aafcb388cf68212a4a38cc3763b72c367ad90c99b3e2f21c17b5ff");
                    Test(70, "e0d29af34552291361d78f613267fb1f0b764576a2927f27abad0992d6ae7fc3");
                    Test(71, "764c358d0816bddc11de970bcd435a9b7072325e7f4bc76e4f20350efe1c60ef");
                    Test(72, "7888d0311ae39aac62c9f92030b58895bebbe42da3e70ab6b6c68455b38d3296");
                    Test(73, "5cc528fae748d0bc3b89bbeb183af91c7b324df9b5debb739b6abe0321b476a4");
                    Test(74, "6a8991b4e7a49fc656985683e7135ccb7be15f147bdc78835004688142624857");
                    Test(75, "b0e44b4d2badef90c135ee3bbee518cc05deedb2087db1bb1c47a965b98cb5f0");
                    Test(76, "bba79291957e9980875b67b0c89c82e849d7d1fcea09796b33d7faf385de8f1b");
                    Test(77, "31de1d35c9a5c8c642b867382c96bcf85eb1789c63e599b31474fd6f76db51a6");
                    Test(78, "122cf9ffffab68585f487af7ad751547f77c750e4c0a4735c9b7b9a9f91862a8");
                    Test(79, "cab0edb9c838aa617786455d67a2b29969401b4dc3669b49113dad7173f1c472");
                    Test(80, "846c88574b76bb256305074c03a3bd31ce630d27cfc60e7590a4961a43e09301");
                    Test(81, "3ceb732d84d224ea6d8656ab6dd1451797a5274131b5d4d0b1f43c04090ccb5a");
                    Test(82, "fdd4e07cf5939303faa36d7e94b03d79975018c569fd2edc9643c847ba8a9e07");
                    Test(83, "7f29d156efc2a29fd10bd525df6982e4e77542912df8130572a1527fe778f42f");
                    Test(84, "4b02db879d300f788d1bbfa3eca4bfce43d6734078cf00e066f5773b7fcf94c0");
                    Test(85, "5151f2861d7c26c91cb985df0a0a4f880295f36b4ab4e8450b9f94e9a0494a42");
                    Test(86, "9f4aa8edddf897a364e0291f7010e066ca252116bd783e584c51eeb028a317a3");
                    Test(87, "6f0bb5689d71aeea5912d80be0341d1c79520ce29a57f3adc5777b19bab38917");
                    Test(88, "adaa09ad35cb6ab513c67562e73964c398971f14eef1bc220543ac879cc46c38");
                    Test(89, "866199b6a20a15949ba202e1138ceb96cb61bf356c182778f17c47c7f6bad0e6");
                    Test(90, "0ad2dda540754615ed60a20473875e467ece66771a5fe7d323dcb5ba7af8b9c3");
                    Test(91, "bceac3be3c43d7a8b12798ff95e32eb4201a2b6edaaa2b58fdedd47828ed0566");
                    Test(92, "5f5f21b92782fb51b342a24e2c47c48199a381366b86700ca9139d2ba268e935");
                    Test(93, "fde25d2c0b975152b034ddfe86ab9648992c90b38233215a349365e9431ca5b9");
                    Test(94, "765ab59f7813593655f4e7ad3ecbdcd6ca986d3c513acf88f6cb3265b8bedcbe");
                    Test(95, "434abc75a8bc0cf9ad273fb6c63b5f09b43519b64470f193a6bc108ce1c9cdb5");
                    Test(96, "8a43cf55f17689071cd8836bbc143ff3a59cfbb58928b374ec6773f89fba7dd2");
                    Test(97, "14c8c23ac1dbe781b85aba23e9d416cd4831cccc608170e6cf97f75153be54b8");
                    Test(98, "7efd7d443fcb15739316cd12559d8c0777c030e1718439e3026758a1e97d82f1");
                    Test(99, "b4ebbc7bd906cbc4d7f9af27a92574bfc45e89712017deb440c6a6b1e8b8bf56");
                    Test(100, "f14a0b6e869696ce598a169789ace74653e318910d73ef337d65e9a943c78b79");
                    Test(101, "b982b16ee8a11f6a03ece5762fc42d25ac2a5f30e436c657d58a884cb0992090");
                    Test(102, "d72f1db33e35c5093648e25c3db943bcf7dbf7e3fb5b39cfcba3d7648a96e8d9");
                    Test(103, "ad86e45c79a5fac3f8e484fabd3faa7a6595e3be251ea47f6540f0fbd40e09e3");
                    Test(104, "4ab73188044ed15739de4ef7455d95af088e959ce4c68547b03870638160c527");
                    Test(105, "66b08745f7c78ed8c9709323eace46b80d430a730f9f35192fec4e4a9af38534");
                    Test(106, "196265b553ac2d0f05dc08a2cd1fe7bce662c3f97320141c60d6edf178e6fc6b");
                    Test(107, "52267c1bd20f2bdd4f86f7666e4767366bc65f1e5945b7253ea742a3cc3243a9");
                    Test(108, "38b5d23b1178547c018dfe15e8f1c5fbfbde5a9d0d5d3f9190d37ba577c492c4");
                    Test(109, "26596fa5cfbb90b67091d70bcfb20015c6dc10f55d42bb782519f2bf4b21b2c9");
                    Test(110, "b3dce09fc64b188b7b75f1d8621d8a33900cd71015b8a7ca39f7eb3caf2bbdf6");
                    Test(111, "0b6f3345b9bbf3acc12c6d66432bcbee5c6d37a0163cd3d1e59290d0f318eb5d");
                    Test(112, "9086ecb27a3ddf96e6f14f5f1e7820a359dc1648c2dd30daaa01c5afc1ff114a");
                    Test(113, "9a54720e0396c7120d22ec3d378d4f2193d62fa9edc863be74a572858c0c4b49");
                    Test(114, "8df705508fbe4e00f6abcb555aa0fa71d4cfc11b536c9cc157ef860a89d18105");
                    Test(115, "e38b21c14cdcc1f59f0a38e2204453a30781748ac34373d69a40ddc00cb389e8");
                    Test(116, "0b7c72ff2d9b5a27322bed39822372c4fa47b2ad8eb49d84c8c22bf62236a5df");
                    Test(117, "5ccc1fcc4aca9ab3a014732dbafee7ed7c0cf1028b4bbaffe92a9d78e634ee02");
                    Test(118, "34e1b82fdea5bf0d5eec513d8bc463d2eeab36a2af5f5183cc98b564e51431b0");
                    Test(119, "e317fa1b993386e8e4c38f6a9a3bb4ae1f903d00df13d16a86f13fbb68a11a28");
                    Test(120, "c9702f3b9008e5ddd4bdd4cf7914ebde346bf14a1132c677cb70cc30b4096399");
                    Test(121, "b60bc6b29c826fb7c310d3a7112a05ed1e2cceb3cc01de83b138766be092f592");
                    Test(122, "6924a1c188cd9ebaf8e1faec5d1e260be9e0aaba2ce72b462ebc97b7b07f7674");
                    Test(123, "ea2c66bbfecdfae799e501d5f2c953c77d3d2ecdaa0f2eb42fc76e8d6ccb3aac");
                    Test(124, "73caaeacf436138a9904bb1be082bfa07cb511867387397967228514f534d6a7");
                    Test(125, "08dbf3a28d0dee59a923254f8480d1e300d48b14e015c9dbd506988882ce5976");
                    Test(126, "bec0c91c08b1d1dfd36a1fb26ad6dec223ad80824335c1a967281ceeda7540d1");
                    Test(127, "f7eabeec467c8b56af40f90e799ea878d8ea7eff260d49982209364ad0e0c39d");
                    Test(128, "93390552a2d23df530a5918c95d095e3914cf476cd1d95bede099c7674b31efe");
                    Test(129, "a084bcc569ed32e30bb0c79e7b4f82be98c3934d2333ea7f6757c726382d6688");
                    Test(130, "99a7d9524b953a7b8b12162d5afce7b18d0b2b657544ddf9d3313bcaded38126");
                    Test(131, "c7b957b9a662c3a348c54070745c098c56cbd60137e78288a8b9ef5b72085966");
                    Test(132, "44bf862757e556a9edf766859ca52c641a3e2f762628eb905a994040640bca78");
                    Test(133, "74a0fd44aa87c8fda7a68c7850c2284d19f0b02b1b0363888b0d928365b077e5");
                    Test(134, "f3a12cf5bd181723365429ad4fd3df10c7cbb050da15be97b73d4b890473315d");
                    Test(135, "c9a310c66e4e5d283502c7784a7590c29477cbf7476885e3ad33fe7acbee3532");
                    Test(136, "950ed6bfceb55655d2ae8856d957d2d759976267e3b5701e5dfde30806222571");
                    Test(137, "c346542cb0015b475c53d396ed6b3ffe6d847f19cf822d441d4bb2230b698859");
                    Test(138, "780d7c76e7e47d8f9f11d80d41dc1410baf0d11653c31bb7d67d18d551d2bbf9");
                    Test(139, "e78f5e6749bc50b1b1ec32db0dfa13becf9866373b3370cac085aaea33b091b7");
                    Test(140, "db787055ff9fd0aa302ad47954a70310d418f0e0225322b037f738d5a4da1504");
                    Test(141, "803bc16cd003e3fc1324aec38d5e8391d3d03dacd11f1719977fa539b23f523b");
                    Test(142, "e1a8a73c8d0260c61d92c79744b5cf355968ffe172c0e28f0b1615b35c57876b");
                    Test(143, "0994c5e6339182141d820548fa47d53a15d0d4182a15dca242f238416140f1d1");
                    Test(144, "ceb2f809baf0ae083751e398d8326e5b5cf3837eae46b5d95c29ed6cade12b12");
                    Test(145, "2e27fe5baa1dc0883430fb14cf0f20b2942da22e0ecb673c517261eb056c2a8d");
                    Test(146, "34f3b057ec8bfff06b7605b9d0df2228d973c8be227ae87dd44c5b47fe936c9d");
                    Test(147, "370800a61403a0d09d02aa7b9a96787ddcd165871258fef068864ddf3a38f2b7");
                    Test(148, "349e3ee88d7ec05c6020ce798d4596d59c97ce2bcdbadaae5dd35f66080ea8dc");
                    Test(149, "8f9bbc8227e979a59689a313772b96685d8bd247fce0b60a17bb5132dc156351");
                    Test(150, "fe37ef02c2c7a443851e7e6c07ecfeccfc9d05af5ccb162323c988f1934b49df");
                    Test(151, "1b992f8cd5e8de45fcafc1c5b87ed3e32e9176da392c0e8caad0ef1def20c761");
                    Test(152, "586af59730c07bea78c335ee7e645a1f96b6342336712caf2e99ce748e361a50");
                    Test(153, "18d6078944c6e33f89267d8848375b37d3f9f7fe66bce35eafb6d6ed1f8277d9");
                    Test(154, "ec4d71483b06a369d9c3cb1a51c55a3c2611514356c622443042e63d0da9df97");
                    Test(155, "f808212e2a02f8e3724b9ccfce9f9987039a25212bfee45b542b8e4fc4b3213b");
                    Test(156, "de77e895c5fc731e3ec1a4f91d5204ebc53a66238081a837402dd200b44b5c13");
                    Test(157, "280db18d5448fa65140393d5e15d68fc0deae417cc6f84871a2d787e85d0c561");
                    Test(158, "5abde29941dfeec4900cba87727a6e16959908382684175fe186849c44675c15");
                    Test(159, "57f89bda0cc941c9e9b09308824f88801aaf3ae46ecbdf05b1b4091230717695");
                    Test(160, "c02eb7620a733588e33095293c9efdeda48907ae698130d4d8804d3e988249f2");
                    Test(161, "f5a28a58a9f9ded07f1cde3bf867518b17b444d98dd0df7594a6c12fb2133b09");
                    Test(162, "c3e38335ebaef5685815b1390cfde893749df21b90ce9b87d06f804eca25d20c");
                    Test(163, "40082a7d030f1d672d32a7ee60104b9d01889a05665a3f8ceb2f1ae95ff793eb");
                    Test(164, "2684b4deab58b29bfd5d68e4fd1511b4185278012cd16bd8a30afbb73dbb93e9");
                    Test(165, "779cfbbfb402ffcea12929e173fce4bdc2f237586160b9efad0fc9a230cd3681");
                    Test(166, "227fe0f975322e62d84a6bf278d5481d768d31730dc9384a0606c0e95006217f");
                    Test(167, "fa5d60a94742e25d41040bfc788354566f5183862a7125c7e6ca6e69e3742a7b");
                    Test(168, "8dd5039ccecbc91ee84005bf000cce4e31506821da15be9993b705b3cb6ca40a");
                    Test(169, "87ce92eda50a42c1405e2307f889c9c83d7831cadf6eb384bbca6c5f881246d7");
                    Test(170, "ab6782a867b7d63482410962efe3b806e825140c0dc417d5a18d83b251876ff4");
                    Test(171, "7333c2d144f61695cfe31ff3ba0119dc9dc4ed0dfdd0691063422b96040ba502");
                    Test(172, "b5a393aef58013ed69e99d54490102613609c739018a29eab127c989182e2bb8");
                    Test(173, "bff7978ddbb2f12d0077a0884772c0dc2c0e0dd2c2ce3fb3d8a6806f2451e8ef");
                    Test(174, "093b8d8e1a2428ba14023fff5edd196da938cf989dc68c73527e144580506b7f");
                    Test(175, "863e120ed100bd1e9c4f60c09d168c4868b3e41101421bd9f1aadd6311816894");
                    Test(176, "b3a41aa3a50ee8bbf0a3f27c572a956f503ac02d5f8bb0d8ee08062571bce8f8");
                    Test(177, "3863923ed333ddbdf271d8db4b1ce4877d716880488292e5d38ee183c15aeaee");
                    Test(178, "7cf69636cdc73a3688ff00277ef9d44d531a8ae4f00f749933fb380afb7ff61d");
                    Test(179, "bc6d65bce67d0a421cc6bd7b40772c598dedfb9e8311f0de5988f320a704b46c");
                    Test(180, "03b5ffca7530aeb742a5a9bf4c2aa10a3586000d737ae2538ed6ee0a7ba8cdd1");
                    Test(181, "daeb2d96b020c861a180f86718ee4888154b2f88c5f8ae90e6057789e41669b8");
                    Test(182, "1e680b8b464ccde3ff59ad6a50dec3d1c4ce05c5a777a3dfaa897681982f9a90");
                    Test(183, "1acc752d9cc1c531fb122e9b93a3624430059342ce48ba9ee67473a6550ce8cd");
                    Test(184, "da7a26e63c477c9d4f610d276456694cd7ae2f7209b75e629a5cf1807ab61947");
                    Test(185, "54e5cc23db9bba0a114f9e6cd18f8d3b70d7b3e87723c7a48ca41e766da85cf6");
                    Test(186, "8b7cc6aea1cec9647af9612b8645bfe89329017e6424854addda196fbff3d20c");
                    Test(187, "24e646069f3ab2a3e2b8d5abd995730cb90a9fbe5b166225030db64786cffe33");
                    Test(188, "d994ade74076d6ff560f0b97764a661e5cd2dc4591d61d98faa94ce2494da8db");
                    Test(189, "bab8ba7bc227dccff52e5f06f6e97dfad97bbcb2590e45207006e4d2a8537be2");
                    Test(190, "ba092971defad97f697a2dda24053482369457c37014dc4ca381696b1f2d6e0d");
                    Test(191, "8a8d52fed8fe5f07ca58bf9888af4e0b7280e3f421852e12f2966f7836292099");
                    Test(192, "ddc186324a0e76adbdc09aa06eb181e97b090601b64e9e7bcf43cb555b1d3f60");
                    Test(193, "b49c21ffcbe5fb1841048d6297aa94fc3d1e493374e79713523bf1ce46230307");
                    Test(194, "813a17ea275beeb3517420a9422aa6e8b48338f60bfb62bf3f0e244f82c11f38");
                    Test(195, "90a63af233f2fe728fad23186867622a86f94db7a9f5e479862c48192e965b92");
                    Test(196, "1467e85477fdc027e55d000cec502e9071311a1a3c407497b03b271471250a9e");
                    Test(197, "1c696cc23491559c77975c49e625ffd25a4ba365b68535c9509ce4962457a5c5");
                    Test(198, "fb8a1e0cf5e857a2d0f417609738d12953daff0083cdb688d783fb6f2da25170");
                    Test(199, "11056424e7752d656659836931e3074282a92559408d6aaaf483499a2768ddb3");
                    Test(200, "e3174c7f40a10a509605f5df076abbcbc4d076235cdd93f0ec83b7e52cff86c7");
                    Test(201, "fea77258bb1529bc5082f73135961b207a5b40d49040af9ccf9d509ca5ba598d");
                    Test(202, "9155f2dc3e8475039aca88315f79989a9c34996bb61317d82d7cb32339c5be52");
                    Test(203, "a5b0ff95de805d49f65a139df5b705e65a15445b80ba7b9f2dbb8c9f434fc62c");
                    Test(204, "d03e0e14f980f9a33fc2fdbd6e5ecefc7a768c01b9ea2e57c7419c3de7292c50");
                    Test(205, "b93c60b583684762c12339badb1a992697d218ee643792c5da4b53a5b2443bcf");
                    Test(206, "62a8c5f0cbb86b359cf0082f080705f7a72ccfec768bc90786860582b7f80c23");
                    Test(207, "f3c57ab37b8e8be151ec7709b54bbc7b9da3b04c7cd051865807bbfa254b477a");
                    Test(208, "91c0ad7d55608c09a8d12ba501870422516a1bfbb0b3b5e635220ab81342b68f");
                    Test(209, "4361360b6771563dd819ade676b5443fbd568f749042a0ae2d86a5e5a1557fa9");
                    Test(210, "6a68dc84585655055964e28d1a45323d84cab7e144b8ad514ecfe98143b51905");
                    Test(211, "a5ad5b1d80adb4764b66f9bc4bd5be90922b3102bd1424c1bdcfe8e59d4f32d3");
                    Test(212, "1fdd40488076781ca3e884156d3168000c9ad40d48a2332f5cf016bdbb549378");
                    Test(213, "8f58602957567c00a6979aec8694c8fbb5301a0b035c18d1d76051a8a84d3766");
                    Test(214, "8303a77d89d0bd7e642f6a0761d8cb1e7bce089cdd72e991ee066ce4ccbef0e3");
                    Test(215, "923834a8c6f748318fc400d979da7af0958332a977744d50d71f9bb8a7fb3241");
                    Test(216, "5eafa95eab44e23f36490461e94b6598dbfff3c8f9e5ab28abd7038eb678eb67");
                    Test(217, "306607265d541bcd531192d23cb1948eb6da56e0b9fbd32df3f177c17dc99e46");
                    Test(218, "5eb5643985769c7dfff7c8e1ee23271bd11fbf7e63936036ab4de4fc2944b8bb");
                    Test(219, "a8c79028b0360da584e59fd41aefb383b5099eeaf6fa1960af5ae6d1fc1e5fc8");
                    Test(220, "c87af5ca4f56df5625b91cfc4e46f4215867fc16ff875bb71d066f3b21c7bd5f");
                    Test(221, "f23873c0a8e59a134b7576d9e004302796e94a51491e11324ef5fd054d4a03ee");
                    Test(222, "3326ae5d43609daf54b7e51143bd94fd34fcac561f452fb68e0ad794026b9963");
                    Test(223, "bd56dea052620a062ac234c00f6e9969fd943d90338062ee248b551dd041070c");
                    Test(224, "9380decad5061939594441880fa7ca9f49a37436ca077d6c8549bbc0d593a40f");
                    Test(225, "527bfeb1c2a322dd5f1c082e0827efc65a007902d3ed7cb3bcd197cf4a53909c");
                    Test(226, "beb47192ba90173678077e8c8ab3e424c43b1ec950f0df0a760295214ddfa9b3");
                    Test(227, "bd00673549670eefdbb6aedd72f80ff4a80e70213a3751a65d9fef0f5834e104");
                    Test(228, "84cc7dd8ed0efd195e1aca2d770abfdfa0a42a86ed9006339185f176ef35833c");
                    Test(229, "c255d949c8c00592f618ce80a505a3d2ae95ebceaeade4e2ccb5fb3844831621");
                    Test(230, "0312bf5f097cb06930093888d5726cda1442f1d2363e6edeb4a2d58131a86a1b");
                    Test(231, "ce480b5c3fef5ef75392bc6f4ad88d094e82b4d3d5e6b1e2d6d8ebea5d8e32ab");
                    Test(232, "c280faa37ef93d354706358f7f74e428a893a5ad75064577ca21b368a485e0a5");
                    Test(233, "748268478021100c5bf0a78e3cd7aae25ef4ab084a9f3eac73e15439857e8af2");
                    Test(234, "91fd069f9edc1c1c79e8260db529bf4c0999b8e87499baeeb0de16f3aa9beedc");
                    Test(235, "11c3cf87833f345d899fc0cd0cb55a514b7bf9d7df3476bc9a3c67680b570e04");
                    Test(236, "5efac89a4b5de43ab8e5d915834e1c5e30662db02a495217eea79815c4a10a8a");
                    Test(237, "499626c8d845c4eb49e621d4216d22d5dd4c7e389b22f70b46c1880291aa488a");
                    Test(238, "1c6a45565cf7d2e11ab8eb30291c9a259071c9bb12607cc801366ca535a2dea6");
                    Test(239, "e9c2d919bcde3577bea4fa7b48080604e85eeeb85533c027a677e9be2e3a20f7");
                    Test(240, "4e624b64818a47e34fb0888b0ccffff2df6643c17f86526b1f2b9dfb79b2420e");
                    Test(241, "f37d8831a08b079044afdaca3e0d6bed8a3309efe15f8ad698ffa2234c1ab796");
                    Test(242, "4602755404c875982e991b627b5681dcda8ce815c6b91aa24cf95dc6a56fc5d0");
                    Test(243, "6fa3d84aa54d442c2d7fb42f213ad9675a6e0493d6f7d25cea51270bb0d62399");
                    Test(244, "a3aedbb80785c0341ee055df04d1cfa67920db97369f6428f9d71adfe5d1bb8c");
                    Test(245, "bfa3907ec44082d12f70378e0388530bce9e5dc3f13ceb05f308d1d6968a5a56");
                    Test(246, "579db91fdb8aa11df91178ed05665456b3f8795b1f15da459674ba4799ba8ce4");
                    Test(247, "419f95eefcbe38e0d2ce2a5df0848dcdcc9b169c858be32e4637ee5e4b7757e7");
                    Test(248, "dc18c9df73086463d3364a1f835212406b071ae5bd510a04436f6a30363c06d2");
                    Test(249, "f52cbaf7117299023390ef49e48b646eecc1ff173c75c5b9559f3e704b87dff4");
                    Test(250, "3f308b6aecef5ff4b949666e08060aa74e9b696777ea14b76d61d615372b88e4");
                    Test(251, "3abb1c0ed2451842a70d29ecee4a764bc1bd3c0451aa46f3f400e166272d1f72");
                    Test(252, "360ddf6e14e6db830b1c3fe3570b63458b1ce9dd7f1d2149057f4e0aaef521f0");
                    Test(253, "1f9feb1033b0685715f7d66ccfd0b1b20838f1cc2318a8b29fa517c2607b0e72");
                    Test(254, "703cd9ce76ae3e6c5d98b4b55a3520346a6312055af60b0b83401c261b37c829");
                    Test(255, "589ad049758344504a5f35421fa4549282cf0e443bbc9275d744d77489e2c0e0");
                    Test(256, "8c4296deaef674e672a746e24ed8a30908ae8f1887aa33a3c692b47a14b4b9e7");
                    Test(1000000, "3f2be6dd53dc7944290e8939192bcccc8077c99b622e0c20355942dd6a4ec009");
                }
        }
    }
}