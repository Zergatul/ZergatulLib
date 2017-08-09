using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
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
            var result = new ByteArray(new byte[0]);
            if (Fragment is GenericBlockCiphertext)
                result += (Fragment as GenericBlockCiphertext).IV;
            if (Fragment is GenericAEADCiphertext)
                result += (Fragment as GenericAEADCiphertext).NonceExplicit;
            result += Content;
            return result;
        }
    }
}