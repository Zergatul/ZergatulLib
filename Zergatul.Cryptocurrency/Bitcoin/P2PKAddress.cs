using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    /// <summary>
    /// Pay-to-pubkey
    /// </summary>
    public class P2PKAddress : Address
    {
        protected override byte _prefix => 0;

        public static P2PKAddress FromPublicKey(byte[] pubkeyData)
        {
            var ripesha = new RIPE160SHA256();
            ripesha.Update(pubkeyData);
            byte[] hash = ripesha.ComputeHash();

            var addr = new P2PKAddress();
            addr._value = Base58Encoding.Encode(addr._prefix, hash);
            return addr;
        }
    }
}