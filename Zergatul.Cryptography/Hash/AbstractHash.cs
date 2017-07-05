using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Hash
{
    public abstract class AbstractHash
    {
        public abstract int BlockSize { get; }
        public abstract int HashSize { get; }

        protected List<byte> _buffer;
        protected long _totalBytes;
        protected byte[] _block;

        public AbstractHash()
        {
            _buffer = new List<byte>();
            _block = new byte[BlockSize];
            _totalBytes = 0;

            Init();
        }

        public void Reset()
        {
            _buffer.Clear();
            _totalBytes = 0;

            Init();
        }

        public void Update(byte[] data)
        {
            _buffer.AddRange(data);
            _totalBytes += data.LongLength;

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

        private void ProcessBuffer()
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
    }
}