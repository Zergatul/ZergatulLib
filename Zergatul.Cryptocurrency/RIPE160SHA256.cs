using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptocurrency
{
    public class RIPE160SHA256 : SHA256
    {
        protected override byte[] InternalStateToBytes()
        {
            var ripemd160 = new RIPEMD160();
            ripemd160.Update(base.InternalStateToBytes());
            return ripemd160.ComputeHash();
        }
    }
}