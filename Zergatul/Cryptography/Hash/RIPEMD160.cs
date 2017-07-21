using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Hash
{
    public class RIPEMD160 : AbstractHash
    {
        public override int BlockSize => 64;
        public override int HashSize => 20;

        protected override void Init()
        {
            throw new NotImplementedException();
        }

        protected override void AddPadding()
        {
            _buffer.Add(0x80);
            while ((_buffer.Count + 8) % 64 != 0)
                _buffer.Add(0);
            ulong totalBits = _totalBytes * 8;
            _buffer.AddRange(BitHelper.GetBytes((uint)totalBits, ByteOrder.LittleEndian));
            _buffer.AddRange(BitHelper.GetBytes((uint)(totalBits >> 32), ByteOrder.LittleEndian));
        }

        protected override byte[] InternalStateToBytes()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessBlock()
        {
            throw new NotImplementedException();
        }
    }
}