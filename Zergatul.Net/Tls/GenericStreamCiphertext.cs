using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    class GenericStreamCiphertext : GenericCiphertext
    {
        public ByteArray Content;
        public ByteArray MAC;
    }
}
