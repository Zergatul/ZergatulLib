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
        public static P2PKAddress FromPublicKey(byte[] pubkeyData)
        {
            var ripesha = new RIPE160SHA256();
            ripesha.Update(pubkeyData);
            byte[] hash = ripesha.ComputeHash();
            return new P2PKAddress
            {
                _value = "1" + Base58Encoding.Encode(0, hash)
            };
        }
    }
}