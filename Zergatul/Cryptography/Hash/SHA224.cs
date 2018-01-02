using System;
using System.Runtime.InteropServices;
using Zergatul.Network;

namespace Zergatul.Cryptography.Hash
{
#if !UseOpenSSL

    public class SHA224 : SHA2_32Bit
    {
        public override int HashSize => 28;
        public override OID OID => OID.JointISOITUT.Country.US.Organization.Gov.CSOR.NISTAlgorithm.HashAlgs.SHA224;

        protected override void Init()
        {
            h0 = 0xC1059ED8;
            h1 = 0x367CD507;
            h2 = 0x3070DD17;
            h3 = 0xF70E5939;
            h4 = 0xFFC00B31;
            h5 = 0x68581511;
            h6 = 0x64F98FA7;
            h7 = 0xBEFA4FA4;
        }
    }

#else

    public class SHA224 : AbstractOpenSSLHash
    {
        public override int BlockSize => 64;
        public override int HashSize => 28;
        public override OID OID => OID.JointISOITUT.Country.US.Organization.Gov.CSOR.NISTAlgorithm.HashAlgs.SHA224;

        protected override int GetContextSize() => Marshal.SizeOf(typeof(OpenSSL.SHA256_CTX));
        protected override void ContextInit() => OpenSSL.SHA224_Init(_context);
        protected override void ContextUpdate(byte[] data) => OpenSSL.SHA224_Update(_context, data, data.Length);
        protected override void ContextFinal(byte[] digest) => OpenSSL.SHA224_Final(digest, _context);
    }

#endif
}