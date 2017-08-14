using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal abstract class AbstractTlsSymmetricCipher
    {
        public SecurityParameters SecurityParameters;

        public abstract void ApplySecurityParameters();
        public abstract void Init(TlsConnectionKeys keys, Role role);

        public abstract byte[] Encrypt(ContentType type, ProtocolVersion version, ulong seqnum, byte[] data);
        public abstract byte[] Decrypt(ContentType type, ProtocolVersion version, ulong seqnum, byte[] data);

        public TLSCompressed Compress(ContentType type, ProtocolVersion version, byte[] data)
        {
            return new TLSCompressed
            {
                Type = type,
                Version = version,
                Fragment = data
            };
        }

        public TLSPlaintext Decompress(TLSCompressed data)
        {
            return new TLSPlaintext
            {
                Type = data.Type,
                Version = data.Version,
                Fragment = data.Fragment
            };
        }
    }
}