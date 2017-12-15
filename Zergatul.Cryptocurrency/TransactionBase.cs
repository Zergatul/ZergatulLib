using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptocurrency.Bitcoin;

namespace Zergatul.Cryptocurrency
{
    public abstract class TransactionBase
    {
        public byte[] Raw { get; protected set; }

        protected byte[] _id;
        public byte[] ID
        {
            get
            {
                if (_id == null)
                {
                    var hash = new DoubleSHA256();
                    hash.Update(Raw);
                    _id = hash.ComputeHash();
                    Array.Reverse(_id);
                }
                return _id;
            }
        }
        public string IDString => BitHelper.BytesToHex(ID);
    }
}