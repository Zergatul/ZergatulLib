using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    /// <summary>
    /// Pay-to-script-hash
    /// </summary>
    public class P2SHAddress : Address
    {
        public static P2SHAddress FromScript(byte[] scriptData)
        {
            var ripesha = new RIPE160SHA256();
            ripesha.Update(scriptData);
            byte[] hash = ripesha.ComputeHash();
            return new P2SHAddress
            {
                _value = "3" + Base58Encoding.Encode(hash)
            };
        }
    }
}