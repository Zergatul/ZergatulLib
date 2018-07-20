using System.Runtime.InteropServices;

namespace Zergatul.Security.OpenSsl
{
    class SHA256 : AbstractMessageDigest
    {
        private static readonly int _contextSize = Marshal.SizeOf(typeof(OpenSsl.SHA256_CTX));

        public override int DigestLength => 32;
        protected override int ContextSize => _contextSize;

        protected override void DoFinal(byte[] digest) => OpenSsl.SHA256_Final(digest, _context);
        public override void Reset() => OpenSsl.SHA256_Init(_context);
        public override void Update(byte[] data) => OpenSsl.SHA256_Update(_context, data, data.Length);
    }
}