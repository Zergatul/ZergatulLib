﻿using System;
using System.Runtime.InteropServices;

namespace Zergatul.Security.OpenSsl.MessageDigest
{
    abstract class AbstractMessageDigest : Security.MessageDigest
    {
        public override int BlockLength => Native.EVP_MD_block_size(_md);
        public override int DigestLength => Native.EVP_MD_size(_md);

        private IntPtr _md_ctx;
        private IntPtr _md;

        protected AbstractMessageDigest()
        {
            _md_ctx = Native.EVP_MD_CTX_new();
            Init();
        }

        private void Init()
        {
            _md = CreateMD();
            if (Native.EVP_DigestInit_ex(_md_ctx, _md, IntPtr.Zero) != 1)
                throw new OpenSslException();
        }

        protected abstract IntPtr CreateMD();

        public override byte[] Digest()
        {
            ThrowIfDisposed();

            byte[] digest = new byte[DigestLength];
            if (Native.EVP_DigestFinal_ex(_md_ctx, digest, IntPtr.Zero) != 1)
                throw new OpenSslException();
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

        public override void Reset()
        {
            ThrowIfDisposed();

            if (Native.EVP_MD_CTX_reset(_md_ctx) != 1)
                throw new OpenSslException();

            Init();
        }

        public override void Update(byte[] data)
        {
            ThrowIfDisposed();

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (Native.EVP_DigestUpdate(_md_ctx, data, data.Length) != 1)
                throw new OpenSslException();
        }

        public override void Update(byte[] data, int offset, int length)
        {
            ThrowIfDisposed();

            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                if (Native.EVP_DigestUpdate(_md_ctx, handle.AddrOfPinnedObject() + offset, length) != 1)
                    throw new OpenSslException();
            }
            finally
            {
                handle.Free();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (_md_ctx != IntPtr.Zero)
            {
                Native.EVP_MD_CTX_free(_md_ctx);
                _md_ctx = IntPtr.Zero;
            }

            base.Dispose(disposing);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MessageDigest));
        }
    }
}