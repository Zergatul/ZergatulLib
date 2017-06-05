using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class TLSCiphertext
    {
        public ContentType Type;
        public ProtocolVersion Version;
        public ushort Length;
        public GenericCiphertext Fragment;
    }
}
