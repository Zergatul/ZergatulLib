using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Hash
{
    public class SHA224 : SHA2_32Bit
    {
        public override int HashSize => 28;

        protected override void Init()
        {
            h0 = 0xC1059ED8;
            h1 = 0x367CD507;
            h2 = 0x3070DD17;
            h3 = 0xF70E5939;
            h4 = 0xFFC00B31;
            h5 = 0x68581511;
            h6 = 0x64F98FA7;
            h7 = 0xBEFA4FA4;
        }

        protected override byte[] InternalStateToBytes()
        {
            var list = new List<byte>(HashSize);
            list.AddRange(BitHelper.GetBytes(h0, ByteOrder.BigEndian));
            list.AddRange(BitHelper.GetBytes(h1, ByteOrder.BigEndian));
            list.AddRange(BitHelper.GetBytes(h2, ByteOrder.BigEndian));
            list.AddRange(BitHelper.GetBytes(h3, ByteOrder.BigEndian));
            list.AddRange(BitHelper.GetBytes(h4, ByteOrder.BigEndian));
            list.AddRange(BitHelper.GetBytes(h5, ByteOrder.BigEndian));
            list.AddRange(BitHelper.GetBytes(h6, ByteOrder.BigEndian));
            return list.ToArray();
        }
    }
}