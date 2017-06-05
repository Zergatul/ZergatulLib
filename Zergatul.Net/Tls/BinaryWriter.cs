using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class BinaryWriter
    {
        private List<byte> _list;

        public BinaryWriter(List<byte> list)
        {
            this._list = list;
        }

        public void WriteByte(byte value)
        {
            _list.Add(value);
        }

        public void WriteBytes(byte[] value)
        {
            if (value == null)
                return;

            _list.AddRange(value);
        }

        public void WriteBytes(ByteArray value)
        {
            if (value == null)
                return;

            value.AddTo(_list);
        }

        public void WriteShort(ushort value)
        {
            _list.Add((byte)((value >> 8) & 0xFF));
            _list.Add((byte)(value & 0xFF));
        }

        public void WriteUInt24(int value)
        {
            if (value < 0)
                throw new InvalidOperationException("value must be >= 0");
            if (value >= (1 << 24))
                throw new InvalidOperationException("value cannot be greater than max uint24");

            _list.Add((byte)((value >> 16) & 0xFF));
            _list.Add((byte)((value >> 8) & 0xFF));
            _list.Add((byte)(value & 0xFF));
        }

        public void WriteUInt32(uint value)
        {
            _list.Add((byte)((value >> 24) & 0xFF));
            _list.Add((byte)((value >> 16) & 0xFF));
            _list.Add((byte)((value >> 8) & 0xFF));
            _list.Add((byte)(value & 0xFF));
        }
    }
}