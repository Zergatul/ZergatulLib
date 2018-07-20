using System.Runtime.InteropServices;

namespace Zergatul.Security.OpenSsl
{
    class SHA512 : AbstractMessageDigest
    {
        private static readonly int _contextSize = Marshal.SizeOf(typeof(OpenSsl.SHA512_CTX));

        public override int DigestLength => 64;
        protected override int ContextSize => _contextSize;

        protected override void DoFinal(byte[] digest) => OpenSsl.SHA512_Final(digest, _context);
        public override void Reset() => OpenSsl.SHA512_Init(_context);
        public override void Update(byte[] data) => OpenSsl.SHA512_Update(_context, data, data.Length);
    }
}