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
        public ByteArray Content;
        public GenericCiphertext Fragment;

        public ByteArray ToBytes()
        {
            var result = new ByteArray();
            if (Fragment is GenericBlockCiphertext)
                result += (Fragment as GenericBlockCiphertext).IV;
            result += Content;
            return result;
        }
    }
}