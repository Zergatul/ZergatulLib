using System.Runtime.InteropServices;

namespace Zergatul.Security.OpenSsl
{
    class SHA1 : AbstractMessageDigest
    {
        private static readonly int _contextSize = Marshal.SizeOf(typeof(OpenSsl.SHA_CTX));

        public override int DigestLength => 20;
        protected override int ContextSize => _contextSize;

        protected override void DoFinal(byte[] digest) => OpenSsl.SHA1_Final(digest, _context);
        public override void Reset() => OpenSsl.SHA1_Init(_context);
        public override void Update(byte[] data) => OpenSsl.SHA1_Update(_context, data, data.Length);
    }
}