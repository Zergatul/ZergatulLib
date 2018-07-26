using System;
using System.Collections.Generic;
using Zergatul.Network;

namespace Zergatul.Cryptography.Hash
{
    public abstract class AbstractHash
    {
        /// <summary>
        /// Block size in bytes
        /// </summary>
        public abstract int BlockSize { get; }

        /// <summary>
        /// Hash size in bytes
        /// </summary>
        public abstract int HashSize { get; }

        public abstract OID OID { get; }

        protected List<byte> _buffer;
        protected ulong _totalBytes;
        protected byte[] _block;

        public AbstractHash()
        {
            _buffer = new List<byte>();
            _totalBytes = 0;
            _block = new byte[BlockSize];

            Init();
        }

        protected AbstractHash(bool empty)
        {
            _buffer = new List<byte>();
            _totalBytes = 0;
        }

        public void Reset()
        {
            _buffer.Clear();
            _totalBytes = 0;

            Init();
        }

        public void Update(byte[] data)
        {
            if (data == null)
                return;

            _buffer.AddRange(data);
            _totalBytes += (ulong)data.LongLength;

            ProcessBuffer();
        }

        public void Update(byte[] data, int index, int length)
        {
            if (index + length > data.Length)
                throw new InvalidOperationException();

            for (int i = 0; i < length; i++)
                _buffer.Add(data[index + i]);
            _totalBytes += (ulong)length;

            ProcessBuffer();
        }

        public byte[] ComputeHash()
        {
            AddPadding();

            if (_buffer.Count % BlockSize != 0)
                throw new Exception("Invalid padding");

            ProcessBuffer();

            return InternalStateToBytes();
        }

        protected virtual void ProcessBuffer()
        {
            while (_buffer.Count >= BlockSize)
            {
                _buffer.CopyTo(0, _block, 0, BlockSize);
                ProcessBlock();
                _buffer.RemoveRange(0, BlockSize);
            }
        }

        protected abstract void Init();
        protected abstract void ProcessBlock();
        protected abstract void AddPadding();
        protected abstract byte[] InternalStateToBytes();

        public static AbstractHash Resolve(OID oid)
        {
            if (oid == OID.ISO.IdentifiedOrganization.OIW.SECSIG.Algorithms.SHA1)
                return new SHA1();
            else
                throw new NotImplementedException();
        }
    }

#if UseOpenSSL

    public abstract class AbstractOpenSSLHash : AbstractHash
    {
        protected IntPtr _context;

        protected override void Init()
        {
            if (_context == IntPtr.Zero)
                this._context = OpenSSL.CRYPTO_malloc(GetContextSize(), null, 0);

            ContextInit();
        }

        protected override void AddPadding()
        {
            if (_buffer.Count > 0)
            {
                ContextUpdate(_buffer.ToArray());
                _buffer.Clear();
            }
        }

        protected override void ProcessBlock()
        {
            ContextUpdate(_block);
        }

        protected override byte[] InternalStateToBytes()
        {
            byte[] digest = new byte[HashSize];
            ContextFinal(digest);
            return digest;
        }

        ~AbstractOpenSSLHash()
        {
            OpenSSL.CRYPTO_free(_context);
        }

        protected abstract int GetContextSize();
        protected abstract void ContextInit();
        protected abstract void ContextUpdate(byte[] data);
        protected abstract void ContextFinal(byte[] digest);
    }

#endif
}