using System.Runtime.InteropServices;

namespace Zergatul.Security.OpenSsl
{
    class MD4 : AbstractMessageDigest
    {
        private static readonly int _contextSize = Marshal.SizeOf(typeof(OpenSsl.MD4_CTX));

        public override int DigestLength => 16;
        protected override int ContextSize => _contextSize;

        protected override void DoFinal(byte[] digest) => OpenSsl.MD4_Final(digest, _context);
        public override void Reset() => OpenSsl.MD4_Init(_context);
        public override void Update(byte[] data) => OpenSsl.MD4_Update(_context, data, data.Length);
    }
}