using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class TxInput : TxInputBase
    {
        public TxInput()
            : base(BlockchainCryptoFactory.Bitcoin)
        {

        }
    }
}