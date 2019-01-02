namespace Zergatul.Cryptocurrency.Base
{
    public static class SignatureType
    {
        public const int SIGHASH_ALL = 0x00000001;
        public const int SIGHASH_NONE = 0x00000002;
        public const int SIGHASH_SINGLE = 0x00000003;
        public const int SIGHASH_ANYONECANPAY = 0x00000080;
    }
}