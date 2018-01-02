using System;
using System.Runtime.InteropServices;
using Zergatul.Network;

namespace Zergatul.Cryptography.Hash
{
#if !UseOpenSSL

    public class SHA384 : SHA2_64Bit
    {
        public override int HashSize => 48;
        public override OID OID => OID.JointISOITUT.Country.US.Organization.Gov.CSOR.NISTAlgorithm.HashAlgs.SHA384;

        protected override void Init()
        {
            h0 = 0xcbbb9d5dc1059ed8;
            h1 = 0x629a292a367cd507;
            h2 = 0x9159015a3070dd17;
            h3 = 0x152fecd8f70e5939;
            h4 = 0x67332667ffc00b31;
            h5 = 0x8eb44a8768581511;
            h6 = 0xdb0c2e0d64f98fa7;
            h7 = 0x47b5481dbefa4fa4;
        }
    }

#else

    public class SHA384 : AbstractOpenSSLHash
    {
        public override int BlockSize => 64;
        public override int HashSize => 48;
        public override OID OID => OID.JointISOITUT.Country.US.Organization.Gov.CSOR.NISTAlgorithm.HashAlgs.SHA384;

        protected override int GetContextSize() => Marshal.SizeOf(typeof(OpenSSL.SHA512_CTX));
        protected override void ContextInit() => OpenSSL.SHA384_Init(_context);
        protected override void ContextUpdate(byte[] data) => OpenSSL.SHA384_Update(_context, data, data.Length);
        protected override void ContextFinal(byte[] digest) => OpenSSL.SHA384_Final(digest, _context);
    }

#endif
}