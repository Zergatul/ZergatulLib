using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;
using Zergatul.Network;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    internal class DoubleSHA256 : SHA256
    {
        protected override byte[] InternalStateToBytes()
        {
            var sha256 = new SHA256();
            sha256.Update(base.InternalStateToBytes());
            return sha256.ComputeHash();
        }
    }
}