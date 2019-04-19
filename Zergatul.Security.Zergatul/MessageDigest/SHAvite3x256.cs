using System;
using static Zergatul.BitHelper;
using static Zergatul.Security.Zergatul.MessageDigest.SHAvite3Constants;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SHAvite3x256 : Security.MessageDigest
    {
        public override int BlockLength => 64;
        public override int DigestLength => 32;

        protected byte[] buffer;
        protected int bufOffset;
        protected int outsize;
        protected uint h0, h1, h2, h3, h4, h5, h6, h7;
        protected uint c0, c1;

        public SHAvite3x256()
        {
            buffer = new byte[64];
            Reset();
        }

        public override void Reset()
        {
            h0 = 0x49BB3E47;
            h1 = 0x2674860D;
            h2 = 0xA8B392AC;
            h3 = 0x021AC4E6;
            h4 = 0x409283CF;
            h5 = 0x620E5D86;
            h6 = 0x6D929DCB;
            h7 = 0x96CC2A8B;

            c0 = c1 = 0;

            bufOffset = 0;
        }

        public override void Update(byte[] data, int offset, int length)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            while (length > 0)
            {
                int copy = System.Math.Min(buffer.Length - bufOffset, length);
                Buffer.BlockCopy(data, offset, buffer, bufOffset, copy);

                offset += copy;
                length -= copy;
                bufOffset += copy;

                if (bufOffset == buffer.Length)
                {
                    if ((c0 += 512) == 0)
                    {
                        c1++;
                    }

                    bufOffset = 0;
                    ProcessBlock();
                }
            }
        }

        public override byte[] Digest()
        {
            Close(8);

            byte[] digest = new byte[32];
            GetBytes(h0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(h2, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(h3, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(h4, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(h5, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(h6, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(h7, ByteOrder.LittleEndian, digest, 0x1C);
            return digest;
        }

        protected void ProcessBlock()
        {
            uint p0 = h0;
            uint p1 = h1;
            uint p2 = h2;
            uint p3 = h3;
            uint p4 = h4;
            uint p5 = h5;
            uint p6 = h6;
            uint p7 = h7;

            uint rk0 = ToUInt32(buffer, 0x00, ByteOrder.LittleEndian);
            uint rk1 = ToUInt32(buffer, 0x04, ByteOrder.LittleEndian);
            uint rk2 = ToUInt32(buffer, 0x08, ByteOrder.LittleEndian);
            uint rk3 = ToUInt32(buffer, 0x0C, ByteOrder.LittleEndian);
            uint rk4 = ToUInt32(buffer, 0x10, ByteOrder.LittleEndian);
            uint rk5 = ToUInt32(buffer, 0x14, ByteOrder.LittleEndian);
            uint rk6 = ToUInt32(buffer, 0x18, ByteOrder.LittleEndian);
            uint rk7 = ToUInt32(buffer, 0x1C, ByteOrder.LittleEndian);
            uint rk8 = ToUInt32(buffer, 0x20, ByteOrder.LittleEndian);
            uint rk9 = ToUInt32(buffer, 0x24, ByteOrder.LittleEndian);
            uint rka = ToUInt32(buffer, 0x28, ByteOrder.LittleEndian);
            uint rkb = ToUInt32(buffer, 0x2C, ByteOrder.LittleEndian);
            uint rkc = ToUInt32(buffer, 0x30, ByteOrder.LittleEndian);
            uint rkd = ToUInt32(buffer, 0x34, ByteOrder.LittleEndian);
            uint rke = ToUInt32(buffer, 0x38, ByteOrder.LittleEndian);
            uint rkf = ToUInt32(buffer, 0x3C, ByteOrder.LittleEndian);

            uint x0, x1, x2, x3;
            uint t0, t1, t2, t3;

            #region Round 0

            x0 = p4 ^ rk0;
            x1 = p5 ^ rk1;
            x2 = p6 ^ rk2;
            x3 = p7 ^ rk3;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            x0 ^= rk4;
            x1 ^= rk5;
            x2 ^= rk6;
            x3 ^= rk7;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            x0 ^= rk8;
            x1 ^= rk9;
            x2 ^= rka;
            x3 ^= rkb;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            p0 ^= x0;
            p1 ^= x1;
            p2 ^= x2;
            p3 ^= x3;

            #endregion

            #region Round 1

            x0 = p0 ^ rkc;
            x1 = p1 ^ rkd;
            x2 = p2 ^ rke;
            x3 = p3 ^ rkf;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk1;
            t1 = rk2;
            t2 = rk3;
            t3 = rk0;
            rk1 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rk2 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rk3 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rk0 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk0;
            rk0 = rk1;
            rk1 = rk2;
            rk2 = rk3;
            rk3 = t0;
            rk0 ^= rkc ^ c0;
            rk1 ^= rkd ^ ~c1;
            rk2 ^= rke;
            rk3 ^= rkf;
            x0 ^= rk0;
            x1 ^= rk1;
            x2 ^= rk2;
            x3 ^= rk3;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk5;
            t1 = rk6;
            t2 = rk7;
            t3 = rk4;
            rk5 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rk6 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rk7 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rk4 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk4;
            rk4 = rk5;
            rk5 = rk6;
            rk6 = rk7;
            rk7 = t0;
            rk4 ^= rk0;
            rk5 ^= rk1;
            rk6 ^= rk2;
            rk7 ^= rk3;
            x0 ^= rk4;
            x1 ^= rk5;
            x2 ^= rk6;
            x3 ^= rk7;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            p4 ^= x0;
            p5 ^= x1;
            p6 ^= x2;
            p7 ^= x3;

            #endregion

            #region Round 2

            t0 = rk9;
            t1 = rka;
            t2 = rkb;
            t3 = rk8;
            rk9 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rka = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rkb = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rk8 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk8;
            rk8 = rk9;
            rk9 = rka;
            rka = rkb;
            rkb = t0;
            rk8 ^= rk4;
            rk9 ^= rk5;
            rka ^= rk6;
            rkb ^= rk7;
            x0 = p4 ^ rk8;
            x1 = p5 ^ rk9;
            x2 = p6 ^ rka;
            x3 = p7 ^ rkb;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rkd;
            t1 = rke;
            t2 = rkf;
            t3 = rkc;
            rkd = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rke = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rkf = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rkc = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rkc;
            rkc = rkd;
            rkd = rke;
            rke = rkf;
            rkf = t0;
            rkc ^= rk8;
            rkd ^= rk9;
            rke ^= rka;
            rkf ^= rkb;
            x0 ^= rkc;
            x1 ^= rkd;
            x2 ^= rke;
            x3 ^= rkf;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            rk0 ^= rkd;
            x0 ^= rk0;
            rk1 ^= rke;
            x1 ^= rk1;
            rk2 ^= rkf;
            x2 ^= rk2;
            rk3 ^= rk0;
            x3 ^= rk3;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            p0 ^= x0;
            p1 ^= x1;
            p2 ^= x2;
            p3 ^= x3;

            #endregion

            #region Round 3

            rk4 ^= rk1;
            x0 = p0 ^ rk4;
            rk5 ^= rk2;
            x1 = p1 ^ rk5;
            rk6 ^= rk3;
            x2 = p2 ^ rk6;
            rk7 ^= rk4;
            x3 = p3 ^ rk7;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            rk8 ^= rk5;
            x0 ^= rk8;
            rk9 ^= rk6;
            x1 ^= rk9;
            rka ^= rk7;
            x2 ^= rka;
            rkb ^= rk8;
            x3 ^= rkb;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            rkc ^= rk9;
            x0 ^= rkc;
            rkd ^= rka;
            x1 ^= rkd;
            rke ^= rkb;
            x2 ^= rke;
            rkf ^= rkc;
            x3 ^= rkf;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            p4 ^= x0;
            p5 ^= x1;
            p6 ^= x2;
            p7 ^= x3;

            #endregion

            #region Round 4

            t0 = rk1;
            t1 = rk2;
            t2 = rk3;
            t3 = rk0;
            rk1 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rk2 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rk3 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rk0 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk0;
            rk0 = rk1;
            rk1 = rk2;
            rk2 = rk3;
            rk3 = t0;
            rk0 ^= rkc;
            rk1 ^= rkd;
            rk2 ^= rke;
            rk3 ^= rkf;
            x0 = p4 ^ rk0;
            x1 = p5 ^ rk1;
            x2 = p6 ^ rk2;
            x3 = p7 ^ rk3;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk5;
            t1 = rk6;
            t2 = rk7;
            t3 = rk4;
            rk5 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rk6 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rk7 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rk4 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk4;
            rk4 = rk5;
            rk5 = rk6;
            rk6 = rk7;
            rk7 = t0;
            rk4 ^= rk0;
            rk5 ^= rk1;
            rk6 ^= rk2;
            rk7 ^= rk3;
            x0 ^= rk4;
            x1 ^= rk5;
            x2 ^= rk6;
            x3 ^= rk7;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk9;
            t1 = rka;
            t2 = rkb;
            t3 = rk8;
            rk9 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rka = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rkb = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rk8 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk8;
            rk8 = rk9;
            rk9 = rka;
            rka = rkb;
            rkb = t0;
            rk8 ^= rk4;
            rk9 ^= rk5 ^ c1;
            rka ^= rk6 ^ ~c0;
            rkb ^= rk7;
            x0 ^= rk8;
            x1 ^= rk9;
            x2 ^= rka;
            x3 ^= rkb;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            p0 ^= x0;
            p1 ^= x1;
            p2 ^= x2;
            p3 ^= x3;

            #endregion

            #region Round 5

            t0 = rkd;
            t1 = rke;
            t2 = rkf;
            t3 = rkc;
            rkd = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rke = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rkf = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rkc = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rkc;
            rkc = rkd;
            rkd = rke;
            rke = rkf;
            rkf = t0;
            rkc ^= rk8;
            rkd ^= rk9;
            rke ^= rka;
            rkf ^= rkb;
            x0 = p0 ^ rkc;
            x1 = p1 ^ rkd;
            x2 = p2 ^ rke;
            x3 = p3 ^ rkf;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            rk0 ^= rkd;
            x0 ^= rk0;
            rk1 ^= rke;
            x1 ^= rk1;
            rk2 ^= rkf;
            x2 ^= rk2;
            rk3 ^= rk0;
            x3 ^= rk3;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            rk4 ^= rk1;
            x0 ^= rk4;
            rk5 ^= rk2;
            x1 ^= rk5;
            rk6 ^= rk3;
            x2 ^= rk6;
            rk7 ^= rk4;
            x3 ^= rk7;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            p4 ^= x0;
            p5 ^= x1;
            p6 ^= x2;
            p7 ^= x3;

            #endregion

            #region Round 6

            rk8 ^= rk5;
            x0 = p4 ^ rk8;
            rk9 ^= rk6;
            x1 = p5 ^ rk9;
            rka ^= rk7;
            x2 = p6 ^ rka;
            rkb ^= rk8;
            x3 = p7 ^ rkb;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            rkc ^= rk9;
            x0 ^= rkc;
            rkd ^= rka;
            x1 ^= rkd;
            rke ^= rkb;
            x2 ^= rke;
            rkf ^= rkc;
            x3 ^= rkf;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk1;
            t1 = rk2;
            t2 = rk3;
            t3 = rk0;
            rk1 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rk2 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rk3 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rk0 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk0;
            rk0 = rk1;
            rk1 = rk2;
            rk2 = rk3;
            rk3 = t0;
            rk0 ^= rkc;
            rk1 ^= rkd;
            rk2 ^= rke;
            rk3 ^= rkf;
            x0 ^= rk0;
            x1 ^= rk1;
            x2 ^= rk2;
            x3 ^= rk3;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            p0 ^= x0;
            p1 ^= x1;
            p2 ^= x2;
            p3 ^= x3;

            #endregion

            #region Round 7

            t0 = rk5;
            t1 = rk6;
            t2 = rk7;
            t3 = rk4;
            rk5 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rk6 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rk7 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rk4 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk4;
            rk4 = rk5;
            rk5 = rk6;
            rk6 = rk7;
            rk7 = t0;
            rk4 ^= rk0;
            rk5 ^= rk1;
            rk6 ^= rk2 ^ c1;
            rk7 ^= rk3 ^ ~c0;
            x0 = p0 ^ rk4;
            x1 = p1 ^ rk5;
            x2 = p2 ^ rk6;
            x3 = p3 ^ rk7;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk9;
            t1 = rka;
            t2 = rkb;
            t3 = rk8;
            rk9 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rka = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rkb = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rk8 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk8;
            rk8 = rk9;
            rk9 = rka;
            rka = rkb;
            rkb = t0;
            rk8 ^= rk4;
            rk9 ^= rk5;
            rka ^= rk6;
            rkb ^= rk7;
            x0 ^= rk8;
            x1 ^= rk9;
            x2 ^= rka;
            x3 ^= rkb;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rkd;
            t1 = rke;
            t2 = rkf;
            t3 = rkc;
            rkd = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rke = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rkf = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rkc = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rkc;
            rkc = rkd;
            rkd = rke;
            rke = rkf;
            rkf = t0;
            rkc ^= rk8;
            rkd ^= rk9;
            rke ^= rka;
            rkf ^= rkb;
            x0 ^= rkc;
            x1 ^= rkd;
            x2 ^= rke;
            x3 ^= rkf;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            p4 ^= x0;
            p5 ^= x1;
            p6 ^= x2;
            p7 ^= x3;

            #endregion

            #region Round 8

            rk0 ^= rkd;
            x0 = p4 ^ rk0;
            rk1 ^= rke;
            x1 = p5 ^ rk1;
            rk2 ^= rkf;
            x2 = p6 ^ rk2;
            rk3 ^= rk0;
            x3 = p7 ^ rk3;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            rk4 ^= rk1;
            x0 ^= rk4;
            rk5 ^= rk2;
            x1 ^= rk5;
            rk6 ^= rk3;
            x2 ^= rk6;
            rk7 ^= rk4;
            x3 ^= rk7;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            rk8 ^= rk5;
            x0 ^= rk8;
            rk9 ^= rk6;
            x1 ^= rk9;
            rka ^= rk7;
            x2 ^= rka;
            rkb ^= rk8;
            x3 ^= rkb;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            p0 ^= x0;
            p1 ^= x1;
            p2 ^= x2;
            p3 ^= x3;

            #endregion

            #region Round 9

            rkc ^= rk9;
            x0 = p0 ^ rkc;
            rkd ^= rka;
            x1 = p1 ^ rkd;
            rke ^= rkb;
            x2 = p2 ^ rke;
            rkf ^= rkc;
            x3 = p3 ^ rkf;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk1;
            t1 = rk2;
            t2 = rk3;
            t3 = rk0;
            rk1 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rk2 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rk3 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rk0 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk0;
            rk0 = rk1;
            rk1 = rk2;
            rk2 = rk3;
            rk3 = t0;
            rk0 ^= rkc;
            rk1 ^= rkd;
            rk2 ^= rke;
            rk3 ^= rkf;
            x0 ^= rk0;
            x1 ^= rk1;
            x2 ^= rk2;
            x3 ^= rk3;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk5;
            t1 = rk6;
            t2 = rk7;
            t3 = rk4;
            rk5 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rk6 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rk7 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rk4 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk4;
            rk4 = rk5;
            rk5 = rk6;
            rk6 = rk7;
            rk7 = t0;
            rk4 ^= rk0;
            rk5 ^= rk1;
            rk6 ^= rk2;
            rk7 ^= rk3;
            x0 ^= rk4;
            x1 ^= rk5;
            x2 ^= rk6;
            x3 ^= rk7;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            p4 ^= x0;
            p5 ^= x1;
            p6 ^= x2;
            p7 ^= x3;

            #endregion

            #region Round 10

            t0 = rk9;
            t1 = rka;
            t2 = rkb;
            t3 = rk8;
            rk9 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rka = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rkb = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rk8 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rk8;
            rk8 = rk9;
            rk9 = rka;
            rka = rkb;
            rkb = t0;
            rk8 ^= rk4;
            rk9 ^= rk5;
            rka ^= rk6;
            rkb ^= rk7;
            x0 = p4 ^ rk8;
            x1 = p5 ^ rk9;
            x2 = p6 ^ rka;
            x3 = p7 ^ rkb;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rkd;
            t1 = rke;
            t2 = rkf;
            t3 = rkc;
            rkd = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            rke = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            rkf = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            rkc = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            t0 = rkc;
            rkc = rkd;
            rkd = rke;
            rke = rkf;
            rkf = t0;
            rkc ^= rk8 ^ c0;
            rkd ^= rk9;
            rke ^= rka;
            rkf ^= rkb ^ ~c1;
            x0 ^= rkc;
            x1 ^= rkd;
            x2 ^= rke;
            x3 ^= rkf;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            rk0 ^= rkd;
            x0 ^= rk0;
            rk1 ^= rke;
            x1 ^= rk1;
            rk2 ^= rkf;
            x2 ^= rk2;
            rk3 ^= rk0;
            x3 ^= rk3;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            p0 ^= x0;
            p1 ^= x1;
            p2 ^= x2;
            p3 ^= x3;

            #endregion

            #region Round 11

            rk4 ^= rk1;
            x0 = p0 ^ rk4;
            rk5 ^= rk2;
            x1 = p1 ^ rk5;
            rk6 ^= rk3;
            x2 = p2 ^ rk6;
            rk7 ^= rk4;
            x3 = p3 ^ rk7;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            rk8 ^= rk5;
            x0 ^= rk8;
            rk9 ^= rk6;
            x1 ^= rk9;
            rka ^= rk7;
            x2 ^= rka;
            rkb ^= rk8;
            x3 ^= rkb;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            rkc ^= rk9;
            x0 ^= rkc;
            rkd ^= rka;
            x1 ^= rkd;
            rke ^= rkb;
            x2 ^= rke;
            rkf ^= rkc;
            x3 ^= rkf;
            t0 = x0;
            t1 = x1;
            t2 = x2;
            t3 = x3;
            x0 = AES0[t0 & 0xFF] ^ AES1[(t1 >> 8) & 0xFF] ^ AES2[(t2 >> 16) & 0xFF] ^ AES3[(t3 >> 24) & 0xFF];
            x1 = AES0[t1 & 0xFF] ^ AES1[(t2 >> 8) & 0xFF] ^ AES2[(t3 >> 16) & 0xFF] ^ AES3[(t0 >> 24) & 0xFF];
            x2 = AES0[t2 & 0xFF] ^ AES1[(t3 >> 8) & 0xFF] ^ AES2[(t0 >> 16) & 0xFF] ^ AES3[(t1 >> 24) & 0xFF];
            x3 = AES0[t3 & 0xFF] ^ AES1[(t0 >> 8) & 0xFF] ^ AES2[(t1 >> 16) & 0xFF] ^ AES3[(t2 >> 24) & 0xFF];
            p4 ^= x0;
            p5 ^= x1;
            p6 ^= x2;
            p7 ^= x3;

            #endregion

            h0 ^= p0;
            h1 ^= p1;
            h2 ^= p2;
            h3 ^= p3;
            h4 ^= p4;
            h5 ^= p5;
            h6 ^= p6;
            h7 ^= p7;

        }

        protected void Close(int size)
        {
            c0 += (uint)(bufOffset << 3);

            uint count0 = c0;
            uint count1 = c1;

            if (bufOffset == 0)
            {
                c0 = c1 = 0;
            }

            buffer[bufOffset++] = 0x80;
            if (bufOffset > 54)
            {
                while (bufOffset < 64)
                    buffer[bufOffset++] = 0;

                bufOffset = 0;
                ProcessBlock();

                c0 = c1 = 0;
            }

            while (bufOffset < 54)
                buffer[bufOffset++] = 0;

            GetBytes(count0, ByteOrder.LittleEndian, buffer, 54);
            GetBytes(count1, ByteOrder.LittleEndian, buffer, 58);

            buffer[62] = (byte)(size << 5);
            buffer[63] = (byte)(size >> 3);

            ProcessBlock();
        }
    }
}