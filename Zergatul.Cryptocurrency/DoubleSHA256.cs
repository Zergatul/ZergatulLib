using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptocurrency
{
    internal class DoubleSHA256 : SHA256
    {
        protected override byte[] InternalStateToBytes()
        {
            var sha256 = new SHA256();
            sha256.Update(base.InternalStateToBytes());
            return sha256.ComputeHash();
        }

        public static byte[] Hash(byte[] data)
        {
            var dsha256 = new DoubleSHA256();
            dsha256.Update(data);
            return dsha256.ComputeHash();
        }
    }
}