using System;
using static Zergatul.BitHelper;
using static Zergatul.Security.Zergatul.MessageDigest.FugueConstants;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Fugue512 : Security.MessageDigest
    {
        public override int BlockLength => 4;
        public override int DigestLength => 64;

        private uint buffer;
        protected int bufLength;
        protected uint s00, s01, s02, s03, s04, s05, s06, s07, s08, s09, s10, s11;
        protected uint s12, s13, s14, s15, s16, s17, s18, s19, s20, s21, s22, s23;
        protected uint s24, s25, s26, s27, s28, s29, s30, s31, s32, s33, s34, s35;
        protected int rshift;
        protected ulong count;

        public Fugue512()
        {
            Reset();
        }

        public override void Reset()
        {
            s00 = s01 = s02 = s03 = s04 = s05 = s06 = s07 = s08 = s09 = 0;
            s10 = s11 = s12 = s13 = s14 = s15 = s16 = s17 = s18 = s19 = 0;
            s20 = 0x8807A57E;
            s21 = 0xE616AF75;
            s22 = 0xC5D3E4DB;
            s23 = 0xAC9AB027;
            s24 = 0xD915F117;
            s25 = 0xB6EECC54;
            s26 = 0x06E8020B;
            s27 = 0x4A92EFD1;
            s28 = 0xAAC6E2C9;
            s29 = 0xDDB21398;
            s30 = 0xCAE65838;
            s31 = 0x437F203F;
            s32 = 0x25EA78E7;
            s33 = 0x951FDDD6;
            s34 = 0xDA6ED11D;
            s35 = 0xE13E3567;

            rshift = 0;
            bufLength = 0;
            count = 0;
        }

        public override void Update(byte[] data, int offset, int length)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            count += (ulong)length << 3;

            while (offset < length)
            {
                buffer = (buffer << 8) | data[offset++];
                bufLength++;

                if (bufLength == 4)
                {
                    ProcessBlock(buffer);
                    bufLength = 0;
                }
            }
        }

        public override byte[] Digest()
        {
            if (bufLength != 0)
            {
                buffer = buffer << ((4 - bufLength) << 3);
                ProcessBlock(buffer);
            }

            ProcessBlock((uint)(count >> 32));
            ProcessBlock((uint)count);

            uint[] s = null;

            switch (rshift)
            {
                case 0:
                    s = new uint[36]
                    {
                        s00, s01, s02, s03, s04, s05, s06, s07, s08, s09, s10, s11,
                        s12, s13, s14, s15, s16, s17, s18, s19, s20, s21, s22, s23,
                        s24, s25, s26, s27, s28, s29, s30, s31, s32, s33, s34, s35,
                    };
                    break;

                case 1:
                    s = new uint[36]
                    {
                        s24, s25, s26, s27, s28, s29, s30, s31, s32, s33, s34, s35,
                        s00, s01, s02, s03, s04, s05, s06, s07, s08, s09, s10, s11,
                        s12, s13, s14, s15, s16, s17, s18, s19, s20, s21, s22, s23,
                    };
                    break;

                case 2:
                    s = new uint[36]
                    {
                        s12, s13, s14, s15, s16, s17, s18, s19, s20, s21, s22, s23,
                        s24, s25, s26, s27, s28, s29, s30, s31, s32, s33, s34, s35,
                        s00, s01, s02, s03, s04, s05, s06, s07, s08, s09, s10, s11,
                    };
                    break;
            }

            uint c0, c1, c2, c3, r0, r1, r2, r3, tmp;

            #region
            for (int i = 0; i < 32; i++)
            {
                ArrayRotateRight(s, 3);
                #region CMIX36
                s[0] ^= s[4];
                s[1] ^= s[5];
                s[2] ^= s[6];
                s[18] ^= s[4];
                s[19] ^= s[5];
                s[20] ^= s[6];
                #endregion
                #region SMIX
                c0 = 0;
                c1 = 0;
                c2 = 0;
                c3 = 0;
                r0 = 0;
                r1 = 0;
                r2 = 0;
                r3 = 0;
                tmp = mixtab0[s[0] >> 24];
                c0 ^= tmp;
                tmp = mixtab1[(s[0] >> 16) & 0xFF];
                c0 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[0] >> 8) & 0xFF];
                c0 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[0] & 0xFF];
                c0 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[1] >> 24];
                c1 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[1] >> 16) & 0xFF];
                c1 ^= tmp;
                tmp = mixtab2[(s[1] >> 8) & 0xFF];
                c1 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[1] & 0xFF];
                c1 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[2] >> 24];
                c2 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[2] >> 16) & 0xFF];
                c2 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[2] >> 8) & 0xFF];
                c2 ^= tmp;
                tmp = mixtab3[s[2] & 0xFF];
                c2 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[3] >> 24];
                c3 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[3] >> 16) & 0xFF];
                c3 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[3] >> 8) & 0xFF];
                c3 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[3] & 0xFF];
                c3 ^= tmp;
                s[0] = ((c0 ^ r0) & 0xFF000000)
                    | ((c1 ^ r1) & 0x00FF0000)
                    | ((c2 ^ r2) & 0x0000FF00)
                    | ((c3 ^ r3) & 0x000000FF);
                s[1] = ((c1 ^ (r0 << 8)) & 0xFF000000)
                    | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                    | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                    | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                s[2] = ((c2 ^ (r0 << 16)) & 0xFF000000)
                    | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                    | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                    | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                s[3] = ((c3 ^ (r0 << 24)) & 0xFF000000)
                    | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                    | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                    | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                #endregion
            }

            for (int i = 0; i < 13; i++)
            {
                s[4] ^= s[0];
                s[9] ^= s[0];
                s[18] ^= s[0];
                s[27] ^= s[0];
                ArrayRotateRight(s, 9);
                #region SMIX
                c0 = 0;
                c1 = 0;
                c2 = 0;
                c3 = 0;
                r0 = 0;
                r1 = 0;
                r2 = 0;
                r3 = 0;
                tmp = mixtab0[s[0] >> 24];
                c0 ^= tmp;
                tmp = mixtab1[(s[0] >> 16) & 0xFF];
                c0 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[0] >> 8) & 0xFF];
                c0 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[0] & 0xFF];
                c0 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[1] >> 24];
                c1 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[1] >> 16) & 0xFF];
                c1 ^= tmp;
                tmp = mixtab2[(s[1] >> 8) & 0xFF];
                c1 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[1] & 0xFF];
                c1 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[2] >> 24];
                c2 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[2] >> 16) & 0xFF];
                c2 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[2] >> 8) & 0xFF];
                c2 ^= tmp;
                tmp = mixtab3[s[2] & 0xFF];
                c2 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[3] >> 24];
                c3 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[3] >> 16) & 0xFF];
                c3 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[3] >> 8) & 0xFF];
                c3 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[3] & 0xFF];
                c3 ^= tmp;
                s[0] = ((c0 ^ r0) & 0xFF000000)
                    | ((c1 ^ r1) & 0x00FF0000)
                    | ((c2 ^ r2) & 0x0000FF00)
                    | ((c3 ^ r3) & 0x000000FF);
                s[1] = ((c1 ^ (r0 << 8)) & 0xFF000000)
                    | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                    | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                    | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                s[2] = ((c2 ^ (r0 << 16)) & 0xFF000000)
                    | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                    | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                    | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                s[3] = ((c3 ^ (r0 << 24)) & 0xFF000000)
                    | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                    | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                    | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                #endregion
                s[4] ^= s[0];
                s[10] ^= s[0];
                s[18] ^= s[0];
                s[27] ^= s[0];
                ArrayRotateRight(s, 9);
                #region SMIX
                c0 = 0;
                c1 = 0;
                c2 = 0;
                c3 = 0;
                r0 = 0;
                r1 = 0;
                r2 = 0;
                r3 = 0;
                tmp = mixtab0[s[0] >> 24];
                c0 ^= tmp;
                tmp = mixtab1[(s[0] >> 16) & 0xFF];
                c0 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[0] >> 8) & 0xFF];
                c0 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[0] & 0xFF];
                c0 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[1] >> 24];
                c1 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[1] >> 16) & 0xFF];
                c1 ^= tmp;
                tmp = mixtab2[(s[1] >> 8) & 0xFF];
                c1 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[1] & 0xFF];
                c1 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[2] >> 24];
                c2 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[2] >> 16) & 0xFF];
                c2 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[2] >> 8) & 0xFF];
                c2 ^= tmp;
                tmp = mixtab3[s[2] & 0xFF];
                c2 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[3] >> 24];
                c3 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[3] >> 16) & 0xFF];
                c3 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[3] >> 8) & 0xFF];
                c3 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[3] & 0xFF];
                c3 ^= tmp;
                s[0] = ((c0 ^ r0) & 0xFF000000)
                    | ((c1 ^ r1) & 0x00FF0000)
                    | ((c2 ^ r2) & 0x0000FF00)
                    | ((c3 ^ r3) & 0x000000FF);
                s[1] = ((c1 ^ (r0 << 8)) & 0xFF000000)
                    | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                    | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                    | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                s[2] = ((c2 ^ (r0 << 16)) & 0xFF000000)
                    | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                    | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                    | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                s[3] = ((c3 ^ (r0 << 24)) & 0xFF000000)
                    | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                    | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                    | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                #endregion
                s[4] ^= s[0];
                s[10] ^= s[0];
                s[19] ^= s[0];
                s[27] ^= s[0];
                ArrayRotateRight(s, 9);
                #region SMIX
                c0 = 0;
                c1 = 0;
                c2 = 0;
                c3 = 0;
                r0 = 0;
                r1 = 0;
                r2 = 0;
                r3 = 0;
                tmp = mixtab0[s[0] >> 24];
                c0 ^= tmp;
                tmp = mixtab1[(s[0] >> 16) & 0xFF];
                c0 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[0] >> 8) & 0xFF];
                c0 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[0] & 0xFF];
                c0 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[1] >> 24];
                c1 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[1] >> 16) & 0xFF];
                c1 ^= tmp;
                tmp = mixtab2[(s[1] >> 8) & 0xFF];
                c1 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[1] & 0xFF];
                c1 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[2] >> 24];
                c2 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[2] >> 16) & 0xFF];
                c2 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[2] >> 8) & 0xFF];
                c2 ^= tmp;
                tmp = mixtab3[s[2] & 0xFF];
                c2 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[3] >> 24];
                c3 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[3] >> 16) & 0xFF];
                c3 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[3] >> 8) & 0xFF];
                c3 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[3] & 0xFF];
                c3 ^= tmp;
                s[0] = ((c0 ^ r0) & 0xFF000000)
                    | ((c1 ^ r1) & 0x00FF0000)
                    | ((c2 ^ r2) & 0x0000FF00)
                    | ((c3 ^ r3) & 0x000000FF);
                s[1] = ((c1 ^ (r0 << 8)) & 0xFF000000)
                    | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                    | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                    | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                s[2] = ((c2 ^ (r0 << 16)) & 0xFF000000)
                    | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                    | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                    | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                s[3] = ((c3 ^ (r0 << 24)) & 0xFF000000)
                    | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                    | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                    | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                #endregion
                s[4] ^= s[0];
                s[10] ^= s[0];
                s[19] ^= s[0];
                s[28] ^= s[0];
                ArrayRotateRight(s, 8);
                #region SMIX
                c0 = 0;
                c1 = 0;
                c2 = 0;
                c3 = 0;
                r0 = 0;
                r1 = 0;
                r2 = 0;
                r3 = 0;
                tmp = mixtab0[s[0] >> 24];
                c0 ^= tmp;
                tmp = mixtab1[(s[0] >> 16) & 0xFF];
                c0 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[0] >> 8) & 0xFF];
                c0 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[0] & 0xFF];
                c0 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[1] >> 24];
                c1 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[1] >> 16) & 0xFF];
                c1 ^= tmp;
                tmp = mixtab2[(s[1] >> 8) & 0xFF];
                c1 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[1] & 0xFF];
                c1 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[2] >> 24];
                c2 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[2] >> 16) & 0xFF];
                c2 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[2] >> 8) & 0xFF];
                c2 ^= tmp;
                tmp = mixtab3[s[2] & 0xFF];
                c2 ^= tmp;
                r3 ^= tmp;
                tmp = mixtab0[s[3] >> 24];
                c3 ^= tmp;
                r0 ^= tmp;
                tmp = mixtab1[(s[3] >> 16) & 0xFF];
                c3 ^= tmp;
                r1 ^= tmp;
                tmp = mixtab2[(s[3] >> 8) & 0xFF];
                c3 ^= tmp;
                r2 ^= tmp;
                tmp = mixtab3[s[3] & 0xFF];
                c3 ^= tmp;
                s[0] = ((c0 ^ r0) & 0xFF000000)
                    | ((c1 ^ r1) & 0x00FF0000)
                    | ((c2 ^ r2) & 0x0000FF00)
                    | ((c3 ^ r3) & 0x000000FF);
                s[1] = ((c1 ^ (r0 << 8)) & 0xFF000000)
                    | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                    | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                    | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                s[2] = ((c2 ^ (r0 << 16)) & 0xFF000000)
                    | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                    | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                    | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                s[3] = ((c3 ^ (r0 << 24)) & 0xFF000000)
                    | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                    | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                    | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                #endregion
            }

            s[4] ^= s[0];
            s[9] ^= s[0];
            s[18] ^= s[0];
            s[27] ^= s[0];
            #endregion

            return InternalStateToDigest(s);
        }

        protected virtual byte[] InternalStateToDigest(uint[] s)
        {
            byte[] digest = new byte[64];
            GetBytes(s[0x01], ByteOrder.BigEndian, digest, 0x00);
            GetBytes(s[0x02], ByteOrder.BigEndian, digest, 0x04);
            GetBytes(s[0x03], ByteOrder.BigEndian, digest, 0x08);
            GetBytes(s[0x04], ByteOrder.BigEndian, digest, 0x0C);
            GetBytes(s[0x09], ByteOrder.BigEndian, digest, 0x10);
            GetBytes(s[0x0A], ByteOrder.BigEndian, digest, 0x14);
            GetBytes(s[0x0B], ByteOrder.BigEndian, digest, 0x18);
            GetBytes(s[0x0C], ByteOrder.BigEndian, digest, 0x1C);
            GetBytes(s[0x12], ByteOrder.BigEndian, digest, 0x20);
            GetBytes(s[0x13], ByteOrder.BigEndian, digest, 0x24);
            GetBytes(s[0x14], ByteOrder.BigEndian, digest, 0x28);
            GetBytes(s[0x15], ByteOrder.BigEndian, digest, 0x2C);
            GetBytes(s[0x1B], ByteOrder.BigEndian, digest, 0x30);
            GetBytes(s[0x1C], ByteOrder.BigEndian, digest, 0x34);
            GetBytes(s[0x1D], ByteOrder.BigEndian, digest, 0x38);
            GetBytes(s[0x1E], ByteOrder.BigEndian, digest, 0x3C);
            return digest;
        }

        private void ProcessBlock(uint p)
        {
            uint s00 = this.s00;
            uint s01 = this.s01;
            uint s02 = this.s02;
            uint s03 = this.s03;
            uint s04 = this.s04;
            uint s05 = this.s05;
            uint s06 = this.s06;
            uint s07 = this.s07;
            uint s08 = this.s08;
            uint s09 = this.s09;
            uint s10 = this.s10;
            uint s11 = this.s11;
            uint s12 = this.s12;
            uint s13 = this.s13;
            uint s14 = this.s14;
            uint s15 = this.s15;
            uint s16 = this.s16;
            uint s17 = this.s17;
            uint s18 = this.s18;
            uint s19 = this.s19;
            uint s20 = this.s20;
            uint s21 = this.s21;
            uint s22 = this.s22;
            uint s23 = this.s23;
            uint s24 = this.s24;
            uint s25 = this.s25;
            uint s26 = this.s26;
            uint s27 = this.s27;
            uint s28 = this.s28;
            uint s29 = this.s29;
            uint s30 = this.s30;
            uint s31 = this.s31;
            uint s32 = this.s32;
            uint s33 = this.s33;
            uint s34 = this.s34;
            uint s35 = this.s35;

            uint c0, c1, c2, c3, r0, r1, r2, r3, tmp;

            switch (rshift)
            {
                case 0:
                    rshift = 1;
                    #region
                    #region TIX4
                    s22 ^= s00;
                    s00 = p;
                    s08 ^= s00;
                    s01 ^= s24;
                    s04 ^= s27;
                    s07 ^= s30;
                    #endregion
                    #region CMIX36
                    s33 ^= s01;
                    s34 ^= s02;
                    s35 ^= s03;
                    s15 ^= s01;
                    s16 ^= s02;
                    s17 ^= s03;
                    #endregion
                    #region SMIX
                    c0 = 0;
                    c1 = 0;
                    c2 = 0;
                    c3 = 0;
                    r0 = 0;
                    r1 = 0;
                    r2 = 0;
                    r3 = 0;
                    tmp = mixtab0[s33 >> 24];
                    c0 ^= tmp;
                    tmp = mixtab1[(s33 >> 16) & 0xFF];
                    c0 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s33 >> 8) & 0xFF];
                    c0 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s33 & 0xFF];
                    c0 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s34 >> 24];
                    c1 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s34 >> 16) & 0xFF];
                    c1 ^= tmp;
                    tmp = mixtab2[(s34 >> 8) & 0xFF];
                    c1 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s34 & 0xFF];
                    c1 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s35 >> 24];
                    c2 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s35 >> 16) & 0xFF];
                    c2 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s35 >> 8) & 0xFF];
                    c2 ^= tmp;
                    tmp = mixtab3[s35 & 0xFF];
                    c2 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s00 >> 24];
                    c3 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s00 >> 16) & 0xFF];
                    c3 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s00 >> 8) & 0xFF];
                    c3 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s00 & 0xFF];
                    c3 ^= tmp;
                    s33 = ((c0 ^ r0) & 0xFF000000)
                        | ((c1 ^ r1) & 0x00FF0000)
                        | ((c2 ^ r2) & 0x0000FF00)
                        | ((c3 ^ r3) & 0x000000FF);
                    s34 = ((c1 ^ (r0 << 8)) & 0xFF000000)
                        | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                        | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                        | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                    s35 = ((c2 ^ (r0 << 16)) & 0xFF000000)
                        | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                        | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                        | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                    s00 = ((c3 ^ (r0 << 24)) & 0xFF000000)
                        | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                        | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                        | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                    #endregion
                    #region CMIX36
                    s30 ^= s34;
                    s31 ^= s35;
                    s32 ^= s00;
                    s12 ^= s34;
                    s13 ^= s35;
                    s14 ^= s00;
                    #endregion
                    #region SMIX
                    c0 = 0;
                    c1 = 0;
                    c2 = 0;
                    c3 = 0;
                    r0 = 0;
                    r1 = 0;
                    r2 = 0;
                    r3 = 0;
                    tmp = mixtab0[s30 >> 24];
                    c0 ^= tmp;
                    tmp = mixtab1[(s30 >> 16) & 0xFF];
                    c0 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s30 >> 8) & 0xFF];
                    c0 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s30 & 0xFF];
                    c0 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s31 >> 24];
                    c1 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s31 >> 16) & 0xFF];
                    c1 ^= tmp;
                    tmp = mixtab2[(s31 >> 8) & 0xFF];
                    c1 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s31 & 0xFF];
                    c1 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s32 >> 24];
                    c2 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s32 >> 16) & 0xFF];
                    c2 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s32 >> 8) & 0xFF];
                    c2 ^= tmp;
                    tmp = mixtab3[s32 & 0xFF];
                    c2 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s33 >> 24];
                    c3 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s33 >> 16) & 0xFF];
                    c3 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s33 >> 8) & 0xFF];
                    c3 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s33 & 0xFF];
                    c3 ^= tmp;
                    s30 = ((c0 ^ r0) & 0xFF000000)
                        | ((c1 ^ r1) & 0x00FF0000)
                        | ((c2 ^ r2) & 0x0000FF00)
                        | ((c3 ^ r3) & 0x000000FF);
                    s31 = ((c1 ^ (r0 << 8)) & 0xFF000000)
                        | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                        | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                        | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                    s32 = ((c2 ^ (r0 << 16)) & 0xFF000000)
                        | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                        | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                        | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                    s33 = ((c3 ^ (r0 << 24)) & 0xFF000000)
                        | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                        | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                        | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                    #endregion
                    #region CMIX36
                    s27 ^= s31;
                    s28 ^= s32;
                    s29 ^= s33;
                    s09 ^= s31;
                    s10 ^= s32;
                    s11 ^= s33;
                    #endregion
                    #region SMIX
                    c0 = 0;
                    c1 = 0;
                    c2 = 0;
                    c3 = 0;
                    r0 = 0;
                    r1 = 0;
                    r2 = 0;
                    r3 = 0;
                    tmp = mixtab0[s27 >> 24];
                    c0 ^= tmp;
                    tmp = mixtab1[(s27 >> 16) & 0xFF];
                    c0 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s27 >> 8) & 0xFF];
                    c0 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s27 & 0xFF];
                    c0 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s28 >> 24];
                    c1 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s28 >> 16) & 0xFF];
                    c1 ^= tmp;
                    tmp = mixtab2[(s28 >> 8) & 0xFF];
                    c1 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s28 & 0xFF];
                    c1 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s29 >> 24];
                    c2 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s29 >> 16) & 0xFF];
                    c2 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s29 >> 8) & 0xFF];
                    c2 ^= tmp;
                    tmp = mixtab3[s29 & 0xFF];
                    c2 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s30 >> 24];
                    c3 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s30 >> 16) & 0xFF];
                    c3 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s30 >> 8) & 0xFF];
                    c3 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s30 & 0xFF];
                    c3 ^= tmp;
                    s27 = ((c0 ^ r0) & 0xFF000000)
                        | ((c1 ^ r1) & 0x00FF0000)
                        | ((c2 ^ r2) & 0x0000FF00)
                        | ((c3 ^ r3) & 0x000000FF);
                    s28 = ((c1 ^ (r0 << 8)) & 0xFF000000)
                        | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                        | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                        | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                    s29 = ((c2 ^ (r0 << 16)) & 0xFF000000)
                        | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                        | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                        | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                    s30 = ((c3 ^ (r0 << 24)) & 0xFF000000)
                        | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                        | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                        | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                    #endregion
                    #region CMIX36
                    s24 ^= s28;
                    s25 ^= s29;
                    s26 ^= s30;
                    s06 ^= s28;
                    s07 ^= s29;
                    s08 ^= s30;
                    #endregion
                    #region SMIX
                    c0 = 0;
                    c1 = 0;
                    c2 = 0;
                    c3 = 0;
                    r0 = 0;
                    r1 = 0;
                    r2 = 0;
                    r3 = 0;
                    tmp = mixtab0[s24 >> 24];
                    c0 ^= tmp;
                    tmp = mixtab1[(s24 >> 16) & 0xFF];
                    c0 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s24 >> 8) & 0xFF];
                    c0 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s24 & 0xFF];
                    c0 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s25 >> 24];
                    c1 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s25 >> 16) & 0xFF];
                    c1 ^= tmp;
                    tmp = mixtab2[(s25 >> 8) & 0xFF];
                    c1 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s25 & 0xFF];
                    c1 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s26 >> 24];
                    c2 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s26 >> 16) & 0xFF];
                    c2 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s26 >> 8) & 0xFF];
                    c2 ^= tmp;
                    tmp = mixtab3[s26 & 0xFF];
                    c2 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s27 >> 24];
                    c3 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s27 >> 16) & 0xFF];
                    c3 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s27 >> 8) & 0xFF];
                    c3 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s27 & 0xFF];
                    c3 ^= tmp;
                    s24 = ((c0 ^ r0) & 0xFF000000)
                        | ((c1 ^ r1) & 0x00FF0000)
                        | ((c2 ^ r2) & 0x0000FF00)
                        | ((c3 ^ r3) & 0x000000FF);
                    s25 = ((c1 ^ (r0 << 8)) & 0xFF000000)
                        | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                        | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                        | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                    s26 = ((c2 ^ (r0 << 16)) & 0xFF000000)
                        | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                        | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                        | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                    s27 = ((c3 ^ (r0 << 24)) & 0xFF000000)
                        | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                        | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                        | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                    #endregion
                    #endregion
                    break;

                case 1:
                    rshift = 2;
                    #region
                    #region TIX4
                    s10 ^= s24;
                    s24 = p;
                    s32 ^= s24;
                    s25 ^= s12;
                    s28 ^= s15;
                    s31 ^= s18;
                    #endregion
                    #region CMIX36
                    s21 ^= s25;
                    s22 ^= s26;
                    s23 ^= s27;
                    s03 ^= s25;
                    s04 ^= s26;
                    s05 ^= s27;
                    #endregion
                    #region SMIX
                    c0 = 0;
                    c1 = 0;
                    c2 = 0;
                    c3 = 0;
                    r0 = 0;
                    r1 = 0;
                    r2 = 0;
                    r3 = 0;
                    tmp = mixtab0[s21 >> 24];
                    c0 ^= tmp;
                    tmp = mixtab1[(s21 >> 16) & 0xFF];
                    c0 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s21 >> 8) & 0xFF];
                    c0 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s21 & 0xFF];
                    c0 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s22 >> 24];
                    c1 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s22 >> 16) & 0xFF];
                    c1 ^= tmp;
                    tmp = mixtab2[(s22 >> 8) & 0xFF];
                    c1 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s22 & 0xFF];
                    c1 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s23 >> 24];
                    c2 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s23 >> 16) & 0xFF];
                    c2 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s23 >> 8) & 0xFF];
                    c2 ^= tmp;
                    tmp = mixtab3[s23 & 0xFF];
                    c2 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s24 >> 24];
                    c3 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s24 >> 16) & 0xFF];
                    c3 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s24 >> 8) & 0xFF];
                    c3 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s24 & 0xFF];
                    c3 ^= tmp;
                    s21 = ((c0 ^ r0) & 0xFF000000)
                        | ((c1 ^ r1) & 0x00FF0000)
                        | ((c2 ^ r2) & 0x0000FF00)
                        | ((c3 ^ r3) & 0x000000FF);
                    s22 = ((c1 ^ (r0 << 8)) & 0xFF000000)
                        | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                        | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                        | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                    s23 = ((c2 ^ (r0 << 16)) & 0xFF000000)
                        | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                        | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                        | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                    s24 = ((c3 ^ (r0 << 24)) & 0xFF000000)
                        | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                        | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                        | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                    #endregion
                    #region CMIX36
                    s18 ^= s22;
                    s19 ^= s23;
                    s20 ^= s24;
                    s00 ^= s22;
                    s01 ^= s23;
                    s02 ^= s24;
                    #endregion
                    #region SMIX
                    c0 = 0;
                    c1 = 0;
                    c2 = 0;
                    c3 = 0;
                    r0 = 0;
                    r1 = 0;
                    r2 = 0;
                    r3 = 0;
                    tmp = mixtab0[s18 >> 24];
                    c0 ^= tmp;
                    tmp = mixtab1[(s18 >> 16) & 0xFF];
                    c0 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s18 >> 8) & 0xFF];
                    c0 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s18 & 0xFF];
                    c0 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s19 >> 24];
                    c1 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s19 >> 16) & 0xFF];
                    c1 ^= tmp;
                    tmp = mixtab2[(s19 >> 8) & 0xFF];
                    c1 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s19 & 0xFF];
                    c1 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s20 >> 24];
                    c2 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s20 >> 16) & 0xFF];
                    c2 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s20 >> 8) & 0xFF];
                    c2 ^= tmp;
                    tmp = mixtab3[s20 & 0xFF];
                    c2 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s21 >> 24];
                    c3 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s21 >> 16) & 0xFF];
                    c3 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s21 >> 8) & 0xFF];
                    c3 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s21 & 0xFF];
                    c3 ^= tmp;
                    s18 = ((c0 ^ r0) & 0xFF000000)
                        | ((c1 ^ r1) & 0x00FF0000)
                        | ((c2 ^ r2) & 0x0000FF00)
                        | ((c3 ^ r3) & 0x000000FF);
                    s19 = ((c1 ^ (r0 << 8)) & 0xFF000000)
                        | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                        | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                        | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                    s20 = ((c2 ^ (r0 << 16)) & 0xFF000000)
                        | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                        | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                        | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                    s21 = ((c3 ^ (r0 << 24)) & 0xFF000000)
                        | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                        | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                        | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                    #endregion
                    #region CMIX36
                    s15 ^= s19;
                    s16 ^= s20;
                    s17 ^= s21;
                    s33 ^= s19;
                    s34 ^= s20;
                    s35 ^= s21;
                    #endregion
                    #region SMIX
                    c0 = 0;
                    c1 = 0;
                    c2 = 0;
                    c3 = 0;
                    r0 = 0;
                    r1 = 0;
                    r2 = 0;
                    r3 = 0;
                    tmp = mixtab0[s15 >> 24];
                    c0 ^= tmp;
                    tmp = mixtab1[(s15 >> 16) & 0xFF];
                    c0 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s15 >> 8) & 0xFF];
                    c0 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s15 & 0xFF];
                    c0 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s16 >> 24];
                    c1 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s16 >> 16) & 0xFF];
                    c1 ^= tmp;
                    tmp = mixtab2[(s16 >> 8) & 0xFF];
                    c1 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s16 & 0xFF];
                    c1 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s17 >> 24];
                    c2 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s17 >> 16) & 0xFF];
                    c2 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s17 >> 8) & 0xFF];
                    c2 ^= tmp;
                    tmp = mixtab3[s17 & 0xFF];
                    c2 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s18 >> 24];
                    c3 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s18 >> 16) & 0xFF];
                    c3 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s18 >> 8) & 0xFF];
                    c3 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s18 & 0xFF];
                    c3 ^= tmp;
                    s15 = ((c0 ^ r0) & 0xFF000000)
                        | ((c1 ^ r1) & 0x00FF0000)
                        | ((c2 ^ r2) & 0x0000FF00)
                        | ((c3 ^ r3) & 0x000000FF);
                    s16 = ((c1 ^ (r0 << 8)) & 0xFF000000)
                        | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                        | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                        | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                    s17 = ((c2 ^ (r0 << 16)) & 0xFF000000)
                        | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                        | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                        | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                    s18 = ((c3 ^ (r0 << 24)) & 0xFF000000)
                        | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                        | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                        | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                    #endregion
                    #region CMIX36
                    s12 ^= s16;
                    s13 ^= s17;
                    s14 ^= s18;
                    s30 ^= s16;
                    s31 ^= s17;
                    s32 ^= s18;
                    #endregion
                    #region SMIX
                    c0 = 0;
                    c1 = 0;
                    c2 = 0;
                    c3 = 0;
                    r0 = 0;
                    r1 = 0;
                    r2 = 0;
                    r3 = 0;
                    tmp = mixtab0[s12 >> 24];
                    c0 ^= tmp;
                    tmp = mixtab1[(s12 >> 16) & 0xFF];
                    c0 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s12 >> 8) & 0xFF];
                    c0 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s12 & 0xFF];
                    c0 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s13 >> 24];
                    c1 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s13 >> 16) & 0xFF];
                    c1 ^= tmp;
                    tmp = mixtab2[(s13 >> 8) & 0xFF];
                    c1 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s13 & 0xFF];
                    c1 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s14 >> 24];
                    c2 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s14 >> 16) & 0xFF];
                    c2 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s14 >> 8) & 0xFF];
                    c2 ^= tmp;
                    tmp = mixtab3[s14 & 0xFF];
                    c2 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s15 >> 24];
                    c3 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s15 >> 16) & 0xFF];
                    c3 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s15 >> 8) & 0xFF];
                    c3 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s15 & 0xFF];
                    c3 ^= tmp;
                    s12 = ((c0 ^ r0) & 0xFF000000)
                        | ((c1 ^ r1) & 0x00FF0000)
                        | ((c2 ^ r2) & 0x0000FF00)
                        | ((c3 ^ r3) & 0x000000FF);
                    s13 = ((c1 ^ (r0 << 8)) & 0xFF000000)
                        | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                        | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                        | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                    s14 = ((c2 ^ (r0 << 16)) & 0xFF000000)
                        | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                        | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                        | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                    s15 = ((c3 ^ (r0 << 24)) & 0xFF000000)
                        | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                        | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                        | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                    #endregion
                    #endregion
                    break;

                case 2:
                    rshift = 0;
                    #region
                    #region TIX4
                    s34 ^= s12;
                    s12 = p;
                    s20 ^= s12;
                    s13 ^= s00;
                    s16 ^= s03;
                    s19 ^= s06;
                    #endregion
                    #region CMIX36
                    s09 ^= s13;
                    s10 ^= s14;
                    s11 ^= s15;
                    s27 ^= s13;
                    s28 ^= s14;
                    s29 ^= s15;
                    #endregion
                    #region SMIX
                    c0 = 0;
                    c1 = 0;
                    c2 = 0;
                    c3 = 0;
                    r0 = 0;
                    r1 = 0;
                    r2 = 0;
                    r3 = 0;
                    tmp = mixtab0[s09 >> 24];
                    c0 ^= tmp;
                    tmp = mixtab1[(s09 >> 16) & 0xFF];
                    c0 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s09 >> 8) & 0xFF];
                    c0 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s09 & 0xFF];
                    c0 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s10 >> 24];
                    c1 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s10 >> 16) & 0xFF];
                    c1 ^= tmp;
                    tmp = mixtab2[(s10 >> 8) & 0xFF];
                    c1 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s10 & 0xFF];
                    c1 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s11 >> 24];
                    c2 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s11 >> 16) & 0xFF];
                    c2 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s11 >> 8) & 0xFF];
                    c2 ^= tmp;
                    tmp = mixtab3[s11 & 0xFF];
                    c2 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s12 >> 24];
                    c3 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s12 >> 16) & 0xFF];
                    c3 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s12 >> 8) & 0xFF];
                    c3 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s12 & 0xFF];
                    c3 ^= tmp;
                    s09 = ((c0 ^ r0) & 0xFF000000)
                        | ((c1 ^ r1) & 0x00FF0000)
                        | ((c2 ^ r2) & 0x0000FF00)
                        | ((c3 ^ r3) & 0x000000FF);
                    s10 = ((c1 ^ (r0 << 8)) & 0xFF000000)
                        | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                        | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                        | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                    s11 = ((c2 ^ (r0 << 16)) & 0xFF000000)
                        | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                        | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                        | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                    s12 = ((c3 ^ (r0 << 24)) & 0xFF000000)
                        | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                        | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                        | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                    #endregion
                    #region CMIX36
                    s06 ^= s10;
                    s07 ^= s11;
                    s08 ^= s12;
                    s24 ^= s10;
                    s25 ^= s11;
                    s26 ^= s12;
                    #endregion
                    #region SMIX
                    c0 = 0;
                    c1 = 0;
                    c2 = 0;
                    c3 = 0;
                    r0 = 0;
                    r1 = 0;
                    r2 = 0;
                    r3 = 0;
                    tmp = mixtab0[s06 >> 24];
                    c0 ^= tmp;
                    tmp = mixtab1[(s06 >> 16) & 0xFF];
                    c0 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s06 >> 8) & 0xFF];
                    c0 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s06 & 0xFF];
                    c0 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s07 >> 24];
                    c1 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s07 >> 16) & 0xFF];
                    c1 ^= tmp;
                    tmp = mixtab2[(s07 >> 8) & 0xFF];
                    c1 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s07 & 0xFF];
                    c1 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s08 >> 24];
                    c2 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s08 >> 16) & 0xFF];
                    c2 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s08 >> 8) & 0xFF];
                    c2 ^= tmp;
                    tmp = mixtab3[s08 & 0xFF];
                    c2 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s09 >> 24];
                    c3 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s09 >> 16) & 0xFF];
                    c3 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s09 >> 8) & 0xFF];
                    c3 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s09 & 0xFF];
                    c3 ^= tmp;
                    s06 = ((c0 ^ r0) & 0xFF000000)
                        | ((c1 ^ r1) & 0x00FF0000)
                        | ((c2 ^ r2) & 0x0000FF00)
                        | ((c3 ^ r3) & 0x000000FF);
                    s07 = ((c1 ^ (r0 << 8)) & 0xFF000000)
                        | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                        | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                        | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                    s08 = ((c2 ^ (r0 << 16)) & 0xFF000000)
                        | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                        | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                        | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                    s09 = ((c3 ^ (r0 << 24)) & 0xFF000000)
                        | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                        | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                        | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                    #endregion
                    #region CMIX36
                    s03 ^= s07;
                    s04 ^= s08;
                    s05 ^= s09;
                    s21 ^= s07;
                    s22 ^= s08;
                    s23 ^= s09;
                    #endregion
                    #region SMIX
                    c0 = 0;
                    c1 = 0;
                    c2 = 0;
                    c3 = 0;
                    r0 = 0;
                    r1 = 0;
                    r2 = 0;
                    r3 = 0;
                    tmp = mixtab0[s03 >> 24];
                    c0 ^= tmp;
                    tmp = mixtab1[(s03 >> 16) & 0xFF];
                    c0 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s03 >> 8) & 0xFF];
                    c0 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s03 & 0xFF];
                    c0 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s04 >> 24];
                    c1 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s04 >> 16) & 0xFF];
                    c1 ^= tmp;
                    tmp = mixtab2[(s04 >> 8) & 0xFF];
                    c1 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s04 & 0xFF];
                    c1 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s05 >> 24];
                    c2 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s05 >> 16) & 0xFF];
                    c2 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s05 >> 8) & 0xFF];
                    c2 ^= tmp;
                    tmp = mixtab3[s05 & 0xFF];
                    c2 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s06 >> 24];
                    c3 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s06 >> 16) & 0xFF];
                    c3 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s06 >> 8) & 0xFF];
                    c3 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s06 & 0xFF];
                    c3 ^= tmp;
                    s03 = ((c0 ^ r0) & 0xFF000000)
                        | ((c1 ^ r1) & 0x00FF0000)
                        | ((c2 ^ r2) & 0x0000FF00)
                        | ((c3 ^ r3) & 0x000000FF);
                    s04 = ((c1 ^ (r0 << 8)) & 0xFF000000)
                        | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                        | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                        | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                    s05 = ((c2 ^ (r0 << 16)) & 0xFF000000)
                        | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                        | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                        | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                    s06 = ((c3 ^ (r0 << 24)) & 0xFF000000)
                        | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                        | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                        | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                    #endregion
                    #region CMIX36
                    s00 ^= s04;
                    s01 ^= s05;
                    s02 ^= s06;
                    s18 ^= s04;
                    s19 ^= s05;
                    s20 ^= s06;
                    #endregion
                    #region SMIX
                    c0 = 0;
                    c1 = 0;
                    c2 = 0;
                    c3 = 0;
                    r0 = 0;
                    r1 = 0;
                    r2 = 0;
                    r3 = 0;
                    tmp = mixtab0[s00 >> 24];
                    c0 ^= tmp;
                    tmp = mixtab1[(s00 >> 16) & 0xFF];
                    c0 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s00 >> 8) & 0xFF];
                    c0 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s00 & 0xFF];
                    c0 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s01 >> 24];
                    c1 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s01 >> 16) & 0xFF];
                    c1 ^= tmp;
                    tmp = mixtab2[(s01 >> 8) & 0xFF];
                    c1 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s01 & 0xFF];
                    c1 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s02 >> 24];
                    c2 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s02 >> 16) & 0xFF];
                    c2 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s02 >> 8) & 0xFF];
                    c2 ^= tmp;
                    tmp = mixtab3[s02 & 0xFF];
                    c2 ^= tmp;
                    r3 ^= tmp;
                    tmp = mixtab0[s03 >> 24];
                    c3 ^= tmp;
                    r0 ^= tmp;
                    tmp = mixtab1[(s03 >> 16) & 0xFF];
                    c3 ^= tmp;
                    r1 ^= tmp;
                    tmp = mixtab2[(s03 >> 8) & 0xFF];
                    c3 ^= tmp;
                    r2 ^= tmp;
                    tmp = mixtab3[s03 & 0xFF];
                    c3 ^= tmp;
                    s00 = ((c0 ^ r0) & 0xFF000000)
                        | ((c1 ^ r1) & 0x00FF0000)
                        | ((c2 ^ r2) & 0x0000FF00)
                        | ((c3 ^ r3) & 0x000000FF);
                    s01 = ((c1 ^ (r0 << 8)) & 0xFF000000)
                        | ((c2 ^ (r1 << 8)) & 0x00FF0000)
                        | ((c3 ^ (r2 << 8)) & 0x0000FF00)
                        | ((c0 ^ (r3 >> 24)) & 0x000000FF);
                    s02 = ((c2 ^ (r0 << 16)) & 0xFF000000)
                        | ((c3 ^ (r1 << 16)) & 0x00FF0000)
                        | ((c0 ^ (r2 >> 16)) & 0x0000FF00)
                        | ((c1 ^ (r3 >> 16)) & 0x000000FF);
                    s03 = ((c3 ^ (r0 << 24)) & 0xFF000000)
                        | ((c0 ^ (r1 >> 8)) & 0x00FF0000)
                        | ((c1 ^ (r2 >> 8)) & 0x0000FF00)
                        | ((c2 ^ (r3 >> 8)) & 0x000000FF);
                    #endregion
                    #endregion
                    break;
            }

            this.s00 = s00;
            this.s01 = s01;
            this.s02 = s02;
            this.s03 = s03;
            this.s04 = s04;
            this.s05 = s05;
            this.s06 = s06;
            this.s07 = s07;
            this.s08 = s08;
            this.s09 = s09;
            this.s10 = s10;
            this.s11 = s11;
            this.s12 = s12;
            this.s13 = s13;
            this.s14 = s14;
            this.s15 = s15;
            this.s16 = s16;
            this.s17 = s17;
            this.s18 = s18;
            this.s19 = s19;
            this.s20 = s20;
            this.s21 = s21;
            this.s22 = s22;
            this.s23 = s23;
            this.s24 = s24;
            this.s25 = s25;
            this.s26 = s26;
            this.s27 = s27;
            this.s28 = s28;
            this.s29 = s29;
            this.s30 = s30;
            this.s31 = s31;
            this.s32 = s32;
            this.s33 = s33;
            this.s34 = s34;
            this.s35 = s35;
        }
    }
}