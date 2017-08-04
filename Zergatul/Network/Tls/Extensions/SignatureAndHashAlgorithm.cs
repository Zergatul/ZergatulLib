using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Extensions
{
    internal class SignatureAndHashAlgorithm
    {
        public HashAlgorithm Hash;
        public SignatureAlgorithm Signature;

        public void Read(BinaryReader reader)
        {
            Hash = (HashAlgorithm)reader.ReadByte();
            Signature = (SignatureAlgorithm)reader.ReadByte();
        }
    }
}
