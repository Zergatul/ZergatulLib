using System;
using System.Runtime.InteropServices;
using Zergatul.Network;

namespace Zergatul.Cryptography.Hash
{
#if !UseOpenSSL

    public class SHA256 : SHA2_32Bit
    {
        public override int HashSize => 32;
        public override OID OID => OID.JointISOITUT.Country.US.Organization.Gov.CSOR.NISTAlgorithm.HashAlgs.SHA256;

        protected override void Init()
        {
            h0 = 0x6A09E667;
            h1 = 0xBB67AE85;
            h2 = 0x3C6EF372;
            h3 = 0xA54FF53A;
            h4 = 0x510E527F;
            h5 = 0x9B05688C;
            h6 = 0x1F83D9AB;
            h7 = 0x5BE0CD19;
        }
    }
#else

    public class SHA256 : AbstractOpenSSLHash
    {
        public override int BlockSize => 64;
        public override int HashSize => 32;
        public override OID OID => OID.JointISOITUT.Country.US.Organization.Gov.CSOR.NISTAlgorithm.HashAlgs.SHA256;

        protected override int GetContextSize() => Marshal.SizeOf(typeof(OpenSSL.SHA256_CTX));
        protected override void ContextInit() => OpenSSL.SHA256_Init(_context);
        protected override void ContextUpdate(byte[] data) => OpenSSL.SHA256_Update(_context, data, data.Length);
        protected override void ContextFinal(byte[] digest) => OpenSSL.SHA256_Final(digest, _context);
    }

#endif
}