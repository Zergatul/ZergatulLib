using System.Runtime.InteropServices;

namespace Zergatul.Security.OpenSsl
{
    class SHA224 : AbstractMessageDigest
    {
        private static readonly int _contextSize = Marshal.SizeOf(typeof(OpenSsl.SHA256_CTX));

        public override int DigestLength => 28;
        protected override int ContextSize => _contextSize;

        protected override void DoFinal(byte[] digest) => OpenSsl.SHA224_Final(digest, _context);
        public override void Reset() => OpenSsl.SHA224_Init(_context);
        public override void Update(byte[] data) => OpenSsl.SHA224_Update(_context, data, data.Length);
    }
}