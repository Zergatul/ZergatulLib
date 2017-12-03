﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    /// <summary>
    /// Pay-to-pubkey-hash
    /// </summary>
    public class P2PKHAddress : Address
    {
        public static P2PKHAddress FromPublicKey(byte[] pubkeyData)
        {
            var ripesha = new RIPE160SHA256();
            ripesha.Update(pubkeyData);
            byte[] hash = ripesha.ComputeHash();
            return FromPublicKeyHash(hash);
        }

        public static P2PKHAddress FromPublicKeyHash(byte[] hash)
        {
            return new P2PKHAddress
            {
                _value = "1" + Base58Encoding.Encode(0, hash)
            };
        }
    }
}