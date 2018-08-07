using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.IO;
using Stream = System.IO.Stream;

namespace Zergatul.Network.Tls
{
    internal class BinaryReader
    {
        private Stream _stream;
        private byte[] _array;
        private byte[] _buffer = new byte[4];
        private int _bufIndex = 0;

        internal int Position { get; private set; }
        private int? _limit;

        private List<List<byte>> _tracking = new List<List<byte>>();

        public BinaryReader(Stream stream)
        {
            this._stream = stream;
            this._array = null;

            Position = 0;
        }

        public BinaryReader(byte[] array)
        {
            this._stream = null;
            this._array = array;

            Position = 0;
        }

        public bool IsDataAvailable()
        {
            if (_stream != null)
            {
                if (_bufIndex > 0)
                    return true;
                _bufIndex = _stream.Read(_buffer, 0, 1);
                return _bufIndex > 0;
            }

            if (_array != null)
                return Position < _array.Length;

            throw new InvalidOperationException();
        }

        private void FillBuffer(int count)
        {
            if (count > _buffer.Length)
                _buffer = new byte[count];

            if (_stream != null)
            {
                int totalRead = _bufIndex;
                while (true)
                {
                    totalRead += _stream.Read(_buffer, totalRead, count - totalRead);
                    if (totalRead == 0)
                        throw new EndOfStreamException();
                    if (totalRead == count)
                        break;
                }
                _bufIndex = 0;
            }

            if (_array != null)
            {
                Array.Copy(_array, Position, _buffer, 0, count);
            }

            Position += count;

            if (_tracking != null)
                for (int t = 0; t < _tracking.Count; t++)
                    for (int i = 0; i < count; i++)
                        _tracking[t].Add(_buffer[i]);
        }

        public byte ReadByte()
        {
            FillBuffer(1);
            return _buffer[0];
        }

        public byte[] ReadBytes(int count)
        {
            FillBuffer(count);

            var result = new byte[count];
            Array.Copy(_buffer, result, count);

            return result;
        }

        public ushort ReadShort()
        {
            FillBuffer(2);
            return (ushort)(_buffer[0] << 8 | _buffer[1]);
        }

        public int ReadUInt24()
        {
            FillBuffer(3);
            return (_buffer[0] << 16) | (_buffer[1] << 8) | _buffer[2];
        }

        public uint ReadUInt32()
        {
            FillBuffer(4);
            return (uint)((_buffer[0] << 24) | (_buffer[1] << 16) | (_buffer[2] << 8) | _buffer[3]);
        }

        public byte[] ReadToEnd()
        {
            int count = -1;
            if (_limit != null && Position <= _limit.Value)
                count = _limit.Value - Position;
            else
            {
                var stream = _stream as LimitedReadStream;
                if (stream != null)
                    count = (int)(stream.Length - stream.Position);
            }

            if (count < 0)
                throw new InvalidOperationException();

            return ReadBytes(count);
        }

        public ReadCounter StartCounter(int totalBytes)
        {
            return new ReadCounter(this, totalBytes);
        }

        public void StartTracking(List<byte> data)
        {
            this._tracking.Add(data);
        }

        public void StopTracking()
        {
            this._tracking.RemoveAt(this._tracking.Count - 1);
        }

        public IDisposable SetReadLimit(int totalBytes)
        {
            _limit = Position + totalBytes;
            return new ReadLimit(this);
        }

        private class ReadLimit : IDisposable
        {
            private BinaryReader _br;

            public ReadLimit(BinaryReader br)
            {
                this._br = br;
            }

            public void Dispose()
            {
                if (_br.Position != _br._limit)
                    throw new InvalidOperationException();
                _br._limit = null;
            }
        }
    }
}