using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    class GenericBlockCiphertext : GenericCiphertext
    {
        public ByteArray IV;
        public ByteArray MAC;
        public ByteArray Padding;
        public byte PaddingLength;
    }
}
