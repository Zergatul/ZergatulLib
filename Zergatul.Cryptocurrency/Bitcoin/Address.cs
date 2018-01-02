using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public abstract class Address
    {
        protected string _value;

        protected abstract byte _prefix { get; }

        public string Value => _value;
        public byte[] Hash => GetHash();

        protected virtual byte[] GetHash()
        {
            byte[] bytes = Base58Encoding.Decode(Value);
            if (bytes.Length != 21)
                throw new InvalidOperationException();
            if (bytes[0] != _prefix)
                throw new InvalidOperationException();
            return ByteArray.SubArray(bytes, 1, 20);
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

            if (bytes.Length != 21)
                return false;
            if (bytes[0] != _prefix)
                return false;

            return true;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}