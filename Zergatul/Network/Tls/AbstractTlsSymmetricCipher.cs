using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal abstract class AbstractTlsSymmetricCipher
    {
        public SecurityParameters SecurityParameters;
        public ISecureRandom Random;

        public abstract void ApplySecurityParameters();
        public abstract void Init(TlsConnectionKeys keys, Role role);

        public byte[] Encrypt(ContentType type, ProtocolVersion version, ulong seqnum, byte[] data)
        {
            var plaintext = new TLSPlaintext
            {
                Type = type,
                Version = version,
                Fragment = data
            };
            var compressed = Compress(type, version, data);
            var ciphertext = new TLSCiphertext
            {
                Type = type,
                Version = version,
                Fragment = Encrypt(compressed, seqnum)
            };
            return ciphertext.Fragment;
        }

        public byte[] Decrypt(ContentType type, ProtocolVersion version, ulong seqnum, byte[] data)
        {
            var ciphertext = new TLSCiphertext
            {
                Type = type,
                Version = version,
                Fragment = data
            };
            var compressed = new TLSCompressed
            {
                Type = type,
                Version = version,
                Fragment = Decrypt(ciphertext, seqnum)
            };
            var plaintext = Decompress(compressed);
            return plaintext.Fragment;
        }

        protected abstract byte[] Encrypt(TLSCompressed compressed, ulong seqnum);
        protected abstract byte[] Decrypt(TLSCiphertext ciphertext, ulong seqnum);

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