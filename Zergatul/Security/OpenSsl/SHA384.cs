using System.Runtime.InteropServices;

namespace Zergatul.Security.OpenSsl
{
    class SHA384 : AbstractMessageDigest
    {
        private static readonly int _contextSize = Marshal.SizeOf(typeof(OpenSsl.SHA512_CTX));

        public override int DigestLength => 48;
        protected override int ContextSize => _contextSize;

        protected override void DoFinal(byte[] digest) => OpenSsl.SHA384_Final(digest, _context);
        public override void Reset() => OpenSsl.SHA384_Init(_context);
        public override void Update(byte[] data) => OpenSsl.SHA384_Update(_context, data, data.Length);
    }
}