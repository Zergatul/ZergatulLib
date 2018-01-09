using System;
using Zergatul.Cryptocurrency.Bitcoin;

namespace Zergatul.Cryptocurrency
{
    public abstract class TransactionBase
    {
        public byte[] RawSegWit { get; protected set; }
        public byte[] RawOriginal { get; protected set; }

        protected byte[] _id;
        public byte[] ID
        {
            get
            {
                if (_id == null)
                {
                    var hash = new DoubleSHA256();
                    hash.Update(RawOriginal);
                    _id = hash.ComputeHash();
                    Array.Reverse(_id);
                }
                return _id;
            }
        }
        public string IDString => BitHelper.BytesToHex(ID);
    }
}