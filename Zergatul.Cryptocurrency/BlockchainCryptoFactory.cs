using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency
{
    public abstract class BlockchainCryptoFactory
    {
        public abstract BlockBase GetBlock();
        public abstract TransactionBase GetTransaction();
        public virtual P2PKAddressBase GetP2PKAddress() => null;
        public abstract P2PKHAddressBase GetP2PKHAddress();
        public abstract P2SHAddressBase GetP2SHAddress();
        public virtual P2WPKHAddressBase GetP2WPKHAddress() => null;
        public virtual P2SHP2WPKHAddressBase GetP2SHP2WPKHAddress() => null;
        public abstract ulong Multiplier { get; }

        public static readonly BlockchainCryptoFactory Bitcoin = new Bitcoin.BitcoinFactory();
        public static readonly BlockchainCryptoFactory BitcoinTestnet = new Bitcoin.Testnet.BitcoinTestnetFactory();
        public static readonly BlockchainCryptoFactory BitcoinGold = new BitcoinGold.BitcoinGoldFactory();
        public static readonly BlockchainCryptoFactory Litecoin = new Litecoin.LitecoinFactory();
        public static readonly BlockchainCryptoFactory Verge = new Verge.VergeFactory();
        public static readonly BlockchainCryptoFactory Zcash = new Zcash.ZcashFactory();
    }
}