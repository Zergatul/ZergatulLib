using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.Ethereum
{
    public class Transaction
    {
        public Address From { get; set; }
        public Address To { get; set; }

        public int Nonce { get; set; }
        public int GasPrice { get; set; }
        public int GasLimit { get; set; }
        public long ValueWei { get; set; }
        public decimal ValueEther { get; set; }
        public byte[] Data { get; set; }

        public void ParseHex(string hex) => Parse(BitHelper.HexToBytes(hex));

        public void Parse(byte[] data)
        {
            var rdl = RdlEncoding.Decode(data);
        }
    }
}