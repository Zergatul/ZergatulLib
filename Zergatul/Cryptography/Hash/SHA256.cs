﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Hash
{
    public class SHA256 : SHA2_32Bit
    {
        public override int HashSize => 32;

        protected override void Init()
        {
            h0 = 0x6A09E667;
            h1 = 0xBB67AE85;
            h2 = 0x3C6EF372;
            h3 = 0xA54FF53A;
            h4 = 0x510E527F;
            h5 = 0x9B05688C;
            h6 = 0x1F83D9AB;
            h7 = 0x5BE0CD19;
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
            list.AddRange(BitHelper.GetBytes(h7, ByteOrder.BigEndian));
            return list.ToArray();
        }
    }
}