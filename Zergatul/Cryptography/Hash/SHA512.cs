using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Hash
{
    public class SHA512 : SHA2_64Bit
    {
        public override int HashSize => 64;
        public override OID OID => OID.SHA512;

        protected override void Init()
        {
            h0 = 0x6A09E667F3BCC908;
            h1 = 0xBB67AE8584CAA73B;
            h2 = 0x3C6EF372FE94F82B;
            h3 = 0xA54FF53A5F1D36F1;
            h4 = 0x510E527FADE682D1;
            h5 = 0x9B05688C2B3E6C1F;
            h6 = 0x1F83D9ABFB41BD6B;
            h7 = 0x5BE0CD19137E2179;
        }
    }
}