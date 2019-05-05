using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SIMD512 : AbstractMessageDigest
    {
        public override int BlockLength => 128;
        public override int DigestLength => 64;

        protected uint a0, a1, a2, a3, a4, a5, a6, a7;
        protected uint b0, b1, b2, b3, b4, b5, b6, b7;
        protected uint c0, c1, c2, c3, c4, c5, c6, c7;
        protected uint d0, d1, d2, d3, d4, d5, d6, d7;

        protected ulong lengthLo, lengthHi;

        public SIMD512()
        {
            buffer = new byte[128];
            Reset();
        }

        public override void Reset()
        {
            //s00 = 0x0BA16B95;
            //s01 = 0x72F999AD;
            //s02 = 0x9FECC2AE;
            //s03 = 0xBA3264FC;
            //s04 = 0x5E894929;
            //s05 = 0x8E9F30E5;
            //s06 = 0x2F1DAA37;
            //s07 = 0xF0F2C558;
            //s08 = 0xAC506643;
            //s09 = 0xA90635A5;
            //s0a = 0xE25B878B;
            //s0b = 0xAAB7878F;
            //s0c = 0x88817F7A;
            //s0d = 0x0A02892B;
            //s0e = 0x559A7550;
            //s0f = 0x598F657E;
            //s00 = 0x7EEF60A1;
            //s01 = 0x6B70E3E8;
            //s02 = 0x9C1714D1;
            //s03 = 0xB958E2A8;
            //s04 = 0xAB02675E;
            //s05 = 0xED1C014F;
            //s06 = 0xCD8D65BB;
            //s07 = 0xFDB7A257;
            //s08 = 0x09254899;
            //s09 = 0xD699C7BC;
            //s0a = 0x9019B6DC;
            //s0b = 0x2B9022E4;
            //s0c = 0x8FA14956;
            //s0d = 0x21BF9BD3;
            //s0e = 0xB94D0943;
            //s0f = 0x6FFDDC22;

            bufOffset = 0;
            lengthHi = lengthLo = 0;
        }

        public override byte[] Digest()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessBlock()
        {
            throw new NotImplementedException();
        }
    }
}