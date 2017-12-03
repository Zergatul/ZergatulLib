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
            return FromScriptHash(hash);
        }

        public static P2SHAddress FromScriptHash(byte[] hash)
        {
            return new P2SHAddress
            {
                _value = Base58Encoding.Encode(5, hash)
            };
        }
    }
}