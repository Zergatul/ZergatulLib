using System;

namespace Zergatul.Security.OpenSsl
{
    abstract class AbstractMessageDigest : MessageDigest
    {
        protected IntPtr _context;
        protected abstract int ContextSize { get; }

        protected AbstractMessageDigest()
        {
            this._context = OpenSsl.CRYPTO_malloc(ContextSize, null, 0);
            Reset();
        }

        public override byte[] Digest()
        {
            byte[] digest = new byte[DigestLength];
            DoFinal(digest);
            return digest;
        }

        public override byte[] Digest(byte[] data)
        {
            Update(data);
            return Digest();
        }

        public override byte[] Digest(byte[] data, int offset, int length)
        {
            Update(data, offset, length);
            return Digest();
        }

        public override void Update(byte[] data, int offset, int length)
        {
            data = ByteArray.SubArray(data, offset, length);
            Update(data);
        }

        protected abstract void DoFinal(byte[] digest);

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            OpenSsl.CRYPTO_free(_context);
            _context = IntPtr.Zero;

            base.Dispose(disposing);
        }
    }
}