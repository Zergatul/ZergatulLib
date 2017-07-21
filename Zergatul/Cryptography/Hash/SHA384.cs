using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Hash
{
    public class SHA384 : SHA2_64Bit
    {
        public override int HashSize => 48;

        protected override void Init()
        {
            h0 = 0xcbbb9d5dc1059ed8;
            h1 = 0x629a292a367cd507;
            h2 = 0x9159015a3070dd17;
            h3 = 0x152fecd8f70e5939;
            h4 = 0x67332667ffc00b31;
            h5 = 0x8eb44a8768581511;
            h6 = 0xdb0c2e0d64f98fa7;
            h7 = 0x47b5481dbefa4fa4;
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
            return list.ToArray();
        }
    }
}