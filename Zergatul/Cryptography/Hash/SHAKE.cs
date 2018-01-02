using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;

namespace Zergatul.Cryptography.Hash
{
    public class SHAKE128 : Keccak
    {
        public override OID OID => null;

        public SHAKE128(int bits)
            : base(256, 0x1F, bits)
        {

        }
    }

    public class SHAKE256 : Keccak
    {
        public override OID OID => null;

        public SHAKE256(int bits)
            : base(512, 0x1F, bits)
        {

        }
    }
}