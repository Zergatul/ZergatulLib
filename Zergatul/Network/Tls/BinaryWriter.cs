using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    internal class BinaryWriter
    {
        private List<byte> _list;
        private List<byte> _tracking;

        public BinaryWriter(List<byte> list)
        {
            this._list = list;
        }

        public void WriteByte(byte value)
        {
            ListAdd(value);
        }

        public void WriteBytes(byte[] value)
        {
            if (value == null)
                return;

            ListAddRange(value);
        }

        public void WriteShort(ushort value)
        {
            ListAdd((byte)((value >> 8) & 0xFF));
            ListAdd((byte)(value & 0xFF));
        }

        public void WriteUInt24(int value)
        {
            if (value < 0)
                throw new InvalidOperationException("value must be >= 0");
            if (value >= (1 << 24))
                throw new InvalidOperationException("value cannot be greater than max uint24");

            ListAdd((byte)((value >> 16) & 0xFF));
            ListAdd((byte)((value >> 8) & 0xFF));
            ListAdd((byte)(value & 0xFF));
        }

        public void WriteUInt32(uint value)
        {
            ListAdd((byte)((value >> 24) & 0xFF));
            ListAdd((byte)((value >> 16) & 0xFF));
            ListAdd((byte)((value >> 8) & 0xFF));
            ListAdd((byte)(value & 0xFF));
        }

        private void ListAdd(byte value)
        {
            _list.Add(value);
            _tracking?.Add(value);
        }

        private void ListAddRange(IEnumerable<byte> collection)
        {
            _list.AddRange(collection);
            _tracking?.AddRange(collection);
        }

        public void StartTracking(List<byte> data)
        {
            _tracking = data;
        }

        public void StopTracking()
        {
            _tracking = null;
        }

        public IDisposable WriteShortLengthOfBlock()
        {
            var result = new BlockLength();
            WriteShort(0);
            result.Writer = this;
            result.Position = _list.Count;
            if (_tracking != null)
                result.TrackingPosition = _tracking.Count;
            return result;
        }

        private class BlockLength : IDisposable
        {
            public int Position;
            public int TrackingPosition;
            public BinaryWriter Writer;

            public void Dispose()
            {
                int length = Writer._list.Count - Position;
                Writer._list[Position - 2] = (byte)(length >> 8);
                Writer._list[Position - 1] = (byte)(length);
                if (Writer._tracking != null)
                {
                    Writer._tracking[TrackingPosition - 2] = Writer._list[Position - 2];
                    Writer._tracking[TrackingPosition - 1] = Writer._list[Position - 1];
                }
            }
        }
    }
}