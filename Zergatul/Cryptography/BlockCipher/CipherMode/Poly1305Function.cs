using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.BlockCipher.CipherMode
{
    public class Poly1305Function : Stream
    {
        private static readonly BigInteger P = new BigInteger(new uint[] { 0x3, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFB }, ByteOrder.BigEndian);

        private BigInteger _accumulator;
        private BigInteger _r;
        private BigInteger _s;
        private int _length;
        private byte[] _block = new byte[16];
        private int _index;

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length => _length;

        public override long Position
        {
            get
            {
                return _length;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        private static void ClampR(byte[] r)
        {
            r[3] &= 15;
            r[7] &= 15;
            r[11] &= 15;
            r[15] &= 15;
            r[4] &= 252;
            r[8] &= 252;
            r[12] &= 252;
        }

        private static void SplitKey(byte[] key, out BigInteger r, out BigInteger s)
        {
            byte[] rb = new byte[16];
            byte[] sb = new byte[16];

            Array.Copy(key, 0, rb, 0, 16);
            Array.Copy(key, 16, sb, 0, 16);

            ClampR(rb);

            r = new BigInteger(rb, ByteOrder.LittleEndian);
            s = new BigInteger(sb, ByteOrder.LittleEndian);
        }

        public void Init(byte[] key)
        {
            SplitKey(key, out _r, out _s);
            _accumulator = BigInteger.Zero;
            _index = 0;
        }

        private void ProcessBlock()
        {
            _index = 0;
            var bnum = new BigInteger(_block, ByteOrder.LittleEndian) + (BigInteger.One << 128);
            _accumulator = (_accumulator + bnum) * _r % P;
        }

        public byte[] ComputeResult()
        {
            if (_index > 0)
            {
                var bnum = new BigInteger(_block, 0, _index, ByteOrder.LittleEndian) + (BigInteger.One << (_index * 8));
                _accumulator = (_accumulator + bnum) * _r % P;
            }

            _accumulator += _s;
            return _accumulator.ToBytes(ByteOrder.LittleEndian, 16);
        }

        public override void Flush()
        {
            
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                int length = System.Math.Min(16 - _index, count);
                Array.Copy(buffer, offset, _block, _index, length);

                _index += length;
                _length += length;
                count -= length;
                offset += length;

                if (_index == 16)
                    ProcessBlock();
            }
        }

        public void PadZeros()
        {
            if (_index > 0)
            {
                for (; _index < 16; _index++)
                    _block[_index] = 0;
                ProcessBlock();
            }
        }
    }
}