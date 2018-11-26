using System;

namespace Zergatul.Cryptocurrency.Base
{
    public abstract class Base58AddressBase : AddressBase
    {
        protected abstract byte[] _prefix { get; }

        public byte[] Hash => GetHash();

        protected virtual byte[] GetHash()
        {
            byte[] bytes = Base58Encoding.Decode(Value);
            if (bytes.Length != 20 + _prefix.Length)
                throw new InvalidOperationException();
            if (!ByteArray.IsSubArray(bytes, _prefix, 0))
                throw new InvalidOperationException();
            return ByteArray.SubArray(bytes, _prefix.Length, 20);
        }

        protected virtual bool Validate()
        {
            byte[] bytes;
            try
            {
                bytes = Base58Encoding.Decode(Value);
            }
            catch (Base58InvalidCheckSumException)
            {
                return false;
            }

            if (bytes.Length != 20 + _prefix.Length)
                return false;
            if (!ByteArray.IsSubArray(bytes, _prefix, 0))
                return false;

            return true;
        }
    }
}