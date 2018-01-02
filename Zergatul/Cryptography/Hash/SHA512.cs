using System;
using System.Runtime.InteropServices;
using Zergatul.Network;

namespace Zergatul.Cryptography.Hash
{
#if !UseOpenSSL

    public class SHA512 : SHA2_64Bit
    {
        public override int HashSize => 64;
        public override OID OID => OID.JointISOITUT.Country.US.Organization.Gov.CSOR.NISTAlgorithm.HashAlgs.SHA512;

        protected override void Init()
        {
            h0 = 0x6A09E667F3BCC908;
            h1 = 0xBB67AE8584CAA73B;
            h2 = 0x3C6EF372FE94F82B;
            h3 = 0xA54FF53A5F1D36F1;
            h4 = 0x510E527FADE682D1;
            h5 = 0x9B05688C2B3E6C1F;
            h6 = 0x1F83D9ABFB41BD6B;
            h7 = 0x5BE0CD19137E2179;
        }
    }

#else

    public class SHA512 : AbstractOpenSSLHash
    {
        public override int BlockSize => 64;
        public override int HashSize => 64;
        public override OID OID => OID.JointISOITUT.Country.US.Organization.Gov.CSOR.NISTAlgorithm.HashAlgs.SHA512;

        protected override int GetContextSize() => Marshal.SizeOf(typeof(OpenSSL.SHA512_CTX));
        protected override void ContextInit() => OpenSSL.SHA512_Init(_context);
        protected override void ContextUpdate(byte[] data) => OpenSSL.SHA512_Update(_context, data, data.Length);
        protected override void ContextFinal(byte[] digest) => OpenSSL.SHA512_Final(digest, _context);
    }

#endif
}