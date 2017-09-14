using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric
{
    public class KeyStream : Stream
    {
        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        private Func<byte[]> _getNextBlock;
        private byte[] _block;
        private int _index;

        public KeyStream(Func<byte[]> getNextBlock)
        {
            this._getNextBlock = getNextBlock;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                if (_block == null || _index >= _block.Length)
                {
                    _block = _getNextBlock();
                    _index = 0;
                }

                int length = System.Math.Min(_block.Length - _index, count);
                Array.Copy(_block, _index, buffer, offset, length);

                _index += length;
                offset += length;
                count -= length;
            }

            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}