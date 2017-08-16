using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal class TLSCiphertext
    {
        public ContentType Type;
        public ProtocolVersion Version;
        public byte[] Fragment;
        public ushort Length => (ushort)(Fragment?.Length ?? 0);
    }
}