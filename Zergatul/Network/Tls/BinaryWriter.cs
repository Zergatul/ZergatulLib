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

        public void WriteBytes(ByteArray value)
        {
            ListAddRange(value.Array);
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
    }
}