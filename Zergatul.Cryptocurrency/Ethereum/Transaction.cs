using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptocurrency.Ethereum
{
    public class Transaction
    {
        public Address From { get; set; }
        public Address To { get; set; }

        public int Nonce { get; set; }
        public long GasPrice { get; set; }
        public long GasLimit { get; set; }
        public byte[] Data { get; set; }

        public long ValueWei { get; set; }
        public decimal ValueEther
        {
            get
            {
                return ValueWei / 1e18m;
            }
            set
            {
                ValueWei = (long)(value * 1e18m);
            }
        }

        public void ParseHex(string hex) => Parse(BitHelper.HexToBytes(hex));

        public void Parse(byte[] data)
        {
            var rdl = Rlp.Decode(data);
        }

        public byte[] GetSignHash()
        {
            byte[] unsignedTx = Rlp.Encode(new RlpItem
            {
                Items = new[]
                {
                    new RlpItem(Nonce),
                    new RlpItem(GasPrice),
                    new RlpItem(GasLimit),
                    new RlpItem(To.Hash),
                    new RlpItem(ValueWei),
                    new RlpItem(Data)
                }
            });
            var keccak = new Keccak256();
            keccak.Update(unsignedTx);
            return keccak.ComputeHash();
        }
    }
}