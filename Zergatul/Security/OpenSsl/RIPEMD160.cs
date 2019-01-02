using System.Runtime.InteropServices;

namespace Zergatul.Security.OpenSsl
{
    class RIPEMD160 : AbstractMessageDigest
    {
        private static readonly int _contextSize = Marshal.SizeOf(typeof(OpenSsl.RIPEMD160_CTX));

        public override int DigestLength => 20;
        protected override int ContextSize => _contextSize;

        protected override void DoFinal(byte[] digest) => OpenSsl.RIPEMD160_Final(digest, _context);
        public override void Reset() => OpenSsl.RIPEMD160_Init(_context);
        public override void Update(byte[] data) => OpenSsl.RIPEMD160_Update(_context, data, data.Length);
    }
}