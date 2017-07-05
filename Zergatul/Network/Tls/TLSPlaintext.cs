using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    internal class TLSPlaintext
    {
        public ContentType Type;
        public ProtocolVersion Version;
        public ushort Length;
        public ByteArray Fragment;
    }
}
