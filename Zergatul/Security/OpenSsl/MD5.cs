using System.Runtime.InteropServices;

namespace Zergatul.Security.OpenSsl
{
    class MD5 : AbstractMessageDigest
    {
        private static readonly int _contextSize = Marshal.SizeOf(typeof(OpenSsl.MD5_CTX));

        public override int DigestLength => 16;
        protected override int ContextSize => _contextSize;

        protected override void DoFinal(byte[] digest) => OpenSsl.MD5_Final(digest, _context);
        public override void Reset() => OpenSsl.MD5_Init(_context);
        public override void Update(byte[] data) => OpenSsl.MD5_Update(_context, data, data.Length);
    }
}