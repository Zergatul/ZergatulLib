﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class BLAKE256 : Security.MessageDigest
    {
        public override int BlockLength => 64;
        public override int DigestLength => 32;

        protected byte[] buffer;
        protected int bufOffset;
        protected uint h0, h1, h2, h3, h4, h5, h6, h7;
        protected uint s0, s1, s2, s3;
        protected ulong length;
        protected bool appendBit;

        public BLAKE256()
        {
            buffer = new byte[64];
            Reset();
            appendBit = true;
        }

        public override void Reset()
        {
            h0 = 0x6A09E667;
            h1 = 0xBB67AE85;
            h2 = 0x3C6EF372;
            h3 = 0xA54FF53A;
            h4 = 0x510E527F;
            h5 = 0x9B05688C;
            h6 = 0x1F83D9AB;
            h7 = 0x5BE0CD19;
            s0 = 0;
            s1 = 0;
            s2 = 0;
            s3 = 0;

            bufOffset = 0;
            length = 0;
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
                    this.length += 512;
                    bufOffset = 0;
                    ProcessBlock();
                }
            }
        }

        public override byte[] Digest()
        {
            length += ((ulong)bufOffset << 3);

            bool nullT = bufOffset == 0 || bufOffset >= 56;

            buffer[bufOffset++] = 0x80;

            if (bufOffset > 56)
            {
                while (bufOffset < 64)
                    buffer[bufOffset++] = 0;
                bufOffset = 0;
                ProcessBlock();
            }

            while (bufOffset < 56)
                buffer[bufOffset++] = 0;
            if (appendBit)
                buffer[55] |= 0x01;

            GetBytes(length, ByteOrder.BigEndian, buffer, 56);

            if (nullT)
                length = 0;

            bufOffset = 0;
            ProcessBlock();

            return InternalStateToBytes();
        }

        protected virtual byte[] InternalStateToBytes()
        {
            byte[] digest = new byte[32];
            GetBytes(h0, ByteOrder.BigEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.BigEndian, digest, 0x04);
            GetBytes(h2, ByteOrder.BigEndian, digest, 0x08);
            GetBytes(h3, ByteOrder.BigEndian, digest, 0x0C);
            GetBytes(h4, ByteOrder.BigEndian, digest, 0x10);
            GetBytes(h5, ByteOrder.BigEndian, digest, 0x14);
            GetBytes(h6, ByteOrder.BigEndian, digest, 0x18);
            GetBytes(h7, ByteOrder.BigEndian, digest, 0x1C);
            return digest;
        }

        protected void ProcessBlock()
        {
            uint m0 = ToUInt32(buffer, 0x00, ByteOrder.BigEndian);
            uint m1 = ToUInt32(buffer, 0x04, ByteOrder.BigEndian);
            uint m2 = ToUInt32(buffer, 0x08, ByteOrder.BigEndian);
            uint m3 = ToUInt32(buffer, 0x0c, ByteOrder.BigEndian);
            uint m4 = ToUInt32(buffer, 0x10, ByteOrder.BigEndian);
            uint m5 = ToUInt32(buffer, 0x14, ByteOrder.BigEndian);
            uint m6 = ToUInt32(buffer, 0x18, ByteOrder.BigEndian);
            uint m7 = ToUInt32(buffer, 0x1c, ByteOrder.BigEndian);
            uint m8 = ToUInt32(buffer, 0x20, ByteOrder.BigEndian);
            uint m9 = ToUInt32(buffer, 0x24, ByteOrder.BigEndian);
            uint ma = ToUInt32(buffer, 0x28, ByteOrder.BigEndian);
            uint mb = ToUInt32(buffer, 0x2c, ByteOrder.BigEndian);
            uint mc = ToUInt32(buffer, 0x30, ByteOrder.BigEndian);
            uint md = ToUInt32(buffer, 0x34, ByteOrder.BigEndian);
            uint me = ToUInt32(buffer, 0x38, ByteOrder.BigEndian);
            uint mf = ToUInt32(buffer, 0x3c, ByteOrder.BigEndian);

            uint v0 = h0;
            uint v1 = h1;
            uint v2 = h2;
            uint v3 = h3;
            uint v4 = h4;
            uint v5 = h5;
            uint v6 = h6;
            uint v7 = h7;
            uint v8 = s0 ^ 0x243F6A88;
            uint v9 = s1 ^ 0x85A308D3;
            uint va = s2 ^ 0x13198A2E;
            uint vb = s3 ^ 0x03707344;
            uint vc = (uint)length ^ 0xA4093822;
            uint vd = (uint)length ^ 0x299F31D0;
            uint ve = (uint)(length >> 32) ^ 0x082EFA98;
            uint vf = (uint)(length >> 32) ^ 0xEC4E6C89;

            #region Loop

            // Iteration 1
            v0 += v4 + (m0 ^ 0x85A308D3);
            vc = RotateRight(vc ^ v0, 16);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 += v4 + (m1 ^ 0x243F6A88);
            vc = RotateRight(vc ^ v0, 8);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 += v5 + (m2 ^ 0x03707344);
            vd = RotateRight(vd ^ v1, 16);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 += v5 + (m3 ^ 0x13198A2E);
            vd = RotateRight(vd ^ v1, 8);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 += v6 + (m4 ^ 0x299F31D0);
            ve = RotateRight(ve ^ v2, 16);
            va += ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 += v6 + (m5 ^ 0xA4093822);
            ve = RotateRight(ve ^ v2, 8);
            va += ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 += v7 + (m6 ^ 0xEC4E6C89);
            vf = RotateRight(vf ^ v3, 16);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 += v7 + (m7 ^ 0x082EFA98);
            vf = RotateRight(vf ^ v3, 8);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 += v5 + (m8 ^ 0x38D01377);
            vf = RotateRight(vf ^ v0, 16);
            va += vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 += v5 + (m9 ^ 0x452821E6);
            vf = RotateRight(vf ^ v0, 8);
            va += vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 += v6 + (ma ^ 0x34E90C6C);
            vc = RotateRight(vc ^ v1, 16);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 += v6 + (mb ^ 0xBE5466CF);
            vc = RotateRight(vc ^ v1, 8);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 += v7 + (mc ^ 0xC97C50DD);
            vd = RotateRight(vd ^ v2, 16);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 += v7 + (md ^ 0xC0AC29B7);
            vd = RotateRight(vd ^ v2, 8);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 += v4 + (me ^ 0xB5470917);
            ve = RotateRight(ve ^ v3, 16);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 += v4 + (mf ^ 0x3F84D5B5);
            ve = RotateRight(ve ^ v3, 8);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 7);

            // Iteration 2
            v0 += v4 + (me ^ 0xBE5466CF);
            vc = RotateRight(vc ^ v0, 16);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 += v4 + (ma ^ 0x3F84D5B5);
            vc = RotateRight(vc ^ v0, 8);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 += v5 + (m4 ^ 0x452821E6);
            vd = RotateRight(vd ^ v1, 16);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 += v5 + (m8 ^ 0xA4093822);
            vd = RotateRight(vd ^ v1, 8);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 += v6 + (m9 ^ 0xB5470917);
            ve = RotateRight(ve ^ v2, 16);
            va += ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 += v6 + (mf ^ 0x38D01377);
            ve = RotateRight(ve ^ v2, 8);
            va += ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 += v7 + (md ^ 0x082EFA98);
            vf = RotateRight(vf ^ v3, 16);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 += v7 + (m6 ^ 0xC97C50DD);
            vf = RotateRight(vf ^ v3, 8);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 += v5 + (m1 ^ 0xC0AC29B7);
            vf = RotateRight(vf ^ v0, 16);
            va += vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 += v5 + (mc ^ 0x85A308D3);
            vf = RotateRight(vf ^ v0, 8);
            va += vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 += v6 + (m0 ^ 0x13198A2E);
            vc = RotateRight(vc ^ v1, 16);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 += v6 + (m2 ^ 0x243F6A88);
            vc = RotateRight(vc ^ v1, 8);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 += v7 + (mb ^ 0xEC4E6C89);
            vd = RotateRight(vd ^ v2, 16);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 += v7 + (m7 ^ 0x34E90C6C);
            vd = RotateRight(vd ^ v2, 8);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 += v4 + (m5 ^ 0x03707344);
            ve = RotateRight(ve ^ v3, 16);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 += v4 + (m3 ^ 0x299F31D0);
            ve = RotateRight(ve ^ v3, 8);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 7);

            // Iteration 3
            v0 += v4 + (mb ^ 0x452821E6);
            vc = RotateRight(vc ^ v0, 16);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 += v4 + (m8 ^ 0x34E90C6C);
            vc = RotateRight(vc ^ v0, 8);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 += v5 + (mc ^ 0x243F6A88);
            vd = RotateRight(vd ^ v1, 16);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 += v5 + (m0 ^ 0xC0AC29B7);
            vd = RotateRight(vd ^ v1, 8);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 += v6 + (m5 ^ 0x13198A2E);
            ve = RotateRight(ve ^ v2, 16);
            va += ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 += v6 + (m2 ^ 0x299F31D0);
            ve = RotateRight(ve ^ v2, 8);
            va += ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 += v7 + (mf ^ 0xC97C50DD);
            vf = RotateRight(vf ^ v3, 16);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 += v7 + (md ^ 0xB5470917);
            vf = RotateRight(vf ^ v3, 8);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 += v5 + (ma ^ 0x3F84D5B5);
            vf = RotateRight(vf ^ v0, 16);
            va += vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 += v5 + (me ^ 0xBE5466CF);
            vf = RotateRight(vf ^ v0, 8);
            va += vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 += v6 + (m3 ^ 0x082EFA98);
            vc = RotateRight(vc ^ v1, 16);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 += v6 + (m6 ^ 0x03707344);
            vc = RotateRight(vc ^ v1, 8);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 += v7 + (m7 ^ 0x85A308D3);
            vd = RotateRight(vd ^ v2, 16);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 += v7 + (m1 ^ 0xEC4E6C89);
            vd = RotateRight(vd ^ v2, 8);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 += v4 + (m9 ^ 0xA4093822);
            ve = RotateRight(ve ^ v3, 16);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 += v4 + (m4 ^ 0x38D01377);
            ve = RotateRight(ve ^ v3, 8);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 7);

            // Iteration 4
            v0 += v4 + (m7 ^ 0x38D01377);
            vc = RotateRight(vc ^ v0, 16);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 += v4 + (m9 ^ 0xEC4E6C89);
            vc = RotateRight(vc ^ v0, 8);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 += v5 + (m3 ^ 0x85A308D3);
            vd = RotateRight(vd ^ v1, 16);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 += v5 + (m1 ^ 0x03707344);
            vd = RotateRight(vd ^ v1, 8);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 += v6 + (md ^ 0xC0AC29B7);
            ve = RotateRight(ve ^ v2, 16);
            va += ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 += v6 + (mc ^ 0xC97C50DD);
            ve = RotateRight(ve ^ v2, 8);
            va += ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 += v7 + (mb ^ 0x3F84D5B5);
            vf = RotateRight(vf ^ v3, 16);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 += v7 + (me ^ 0x34E90C6C);
            vf = RotateRight(vf ^ v3, 8);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 += v5 + (m2 ^ 0x082EFA98);
            vf = RotateRight(vf ^ v0, 16);
            va += vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 += v5 + (m6 ^ 0x13198A2E);
            vf = RotateRight(vf ^ v0, 8);
            va += vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 += v6 + (m5 ^ 0xBE5466CF);
            vc = RotateRight(vc ^ v1, 16);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 += v6 + (ma ^ 0x299F31D0);
            vc = RotateRight(vc ^ v1, 8);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 += v7 + (m4 ^ 0x243F6A88);
            vd = RotateRight(vd ^ v2, 16);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 += v7 + (m0 ^ 0xA4093822);
            vd = RotateRight(vd ^ v2, 8);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 += v4 + (mf ^ 0x452821E6);
            ve = RotateRight(ve ^ v3, 16);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 += v4 + (m8 ^ 0xB5470917);
            ve = RotateRight(ve ^ v3, 8);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 7);

            // Iteration 5
            v0 += v4 + (m9 ^ 0x243F6A88);
            vc = RotateRight(vc ^ v0, 16);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 += v4 + (m0 ^ 0x38D01377);
            vc = RotateRight(vc ^ v0, 8);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 += v5 + (m5 ^ 0xEC4E6C89);
            vd = RotateRight(vd ^ v1, 16);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 += v5 + (m7 ^ 0x299F31D0);
            vd = RotateRight(vd ^ v1, 8);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 += v6 + (m2 ^ 0xA4093822);
            ve = RotateRight(ve ^ v2, 16);
            va += ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 += v6 + (m4 ^ 0x13198A2E);
            ve = RotateRight(ve ^ v2, 8);
            va += ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 += v7 + (ma ^ 0xB5470917);
            vf = RotateRight(vf ^ v3, 16);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 += v7 + (mf ^ 0xBE5466CF);
            vf = RotateRight(vf ^ v3, 8);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 += v5 + (me ^ 0x85A308D3);
            vf = RotateRight(vf ^ v0, 16);
            va += vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 += v5 + (m1 ^ 0x3F84D5B5);
            vf = RotateRight(vf ^ v0, 8);
            va += vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 += v6 + (mb ^ 0xC0AC29B7);
            vc = RotateRight(vc ^ v1, 16);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 += v6 + (mc ^ 0x34E90C6C);
            vc = RotateRight(vc ^ v1, 8);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 += v7 + (m6 ^ 0x452821E6);
            vd = RotateRight(vd ^ v2, 16);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 += v7 + (m8 ^ 0x082EFA98);
            vd = RotateRight(vd ^ v2, 8);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 += v4 + (m3 ^ 0xC97C50DD);
            ve = RotateRight(ve ^ v3, 16);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 += v4 + (md ^ 0x03707344);
            ve = RotateRight(ve ^ v3, 8);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 7);

            // Iteration 6
            v0 += v4 + (m2 ^ 0xC0AC29B7);
            vc = RotateRight(vc ^ v0, 16);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 += v4 + (mc ^ 0x13198A2E);
            vc = RotateRight(vc ^ v0, 8);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 += v5 + (m6 ^ 0xBE5466CF);
            vd = RotateRight(vd ^ v1, 16);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 += v5 + (ma ^ 0x082EFA98);
            vd = RotateRight(vd ^ v1, 8);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 += v6 + (m0 ^ 0x34E90C6C);
            ve = RotateRight(ve ^ v2, 16);
            va += ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 += v6 + (mb ^ 0x243F6A88);
            ve = RotateRight(ve ^ v2, 8);
            va += ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 += v7 + (m8 ^ 0x03707344);
            vf = RotateRight(vf ^ v3, 16);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 += v7 + (m3 ^ 0x452821E6);
            vf = RotateRight(vf ^ v3, 8);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 += v5 + (m4 ^ 0xC97C50DD);
            vf = RotateRight(vf ^ v0, 16);
            va += vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 += v5 + (md ^ 0xA4093822);
            vf = RotateRight(vf ^ v0, 8);
            va += vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 += v6 + (m7 ^ 0x299F31D0);
            vc = RotateRight(vc ^ v1, 16);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 += v6 + (m5 ^ 0xEC4E6C89);
            vc = RotateRight(vc ^ v1, 8);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 += v7 + (mf ^ 0x3F84D5B5);
            vd = RotateRight(vd ^ v2, 16);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 += v7 + (me ^ 0xB5470917);
            vd = RotateRight(vd ^ v2, 8);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 += v4 + (m1 ^ 0x38D01377);
            ve = RotateRight(ve ^ v3, 16);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 += v4 + (m9 ^ 0x85A308D3);
            ve = RotateRight(ve ^ v3, 8);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 7);

            // Iteration 7
            v0 += v4 + (mc ^ 0x299F31D0);
            vc = RotateRight(vc ^ v0, 16);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 += v4 + (m5 ^ 0xC0AC29B7);
            vc = RotateRight(vc ^ v0, 8);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 += v5 + (m1 ^ 0xB5470917);
            vd = RotateRight(vd ^ v1, 16);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 += v5 + (mf ^ 0x85A308D3);
            vd = RotateRight(vd ^ v1, 8);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 += v6 + (me ^ 0xC97C50DD);
            ve = RotateRight(ve ^ v2, 16);
            va += ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 += v6 + (md ^ 0x3F84D5B5);
            ve = RotateRight(ve ^ v2, 8);
            va += ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 += v7 + (m4 ^ 0xBE5466CF);
            vf = RotateRight(vf ^ v3, 16);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 += v7 + (ma ^ 0xA4093822);
            vf = RotateRight(vf ^ v3, 8);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 += v5 + (m0 ^ 0xEC4E6C89);
            vf = RotateRight(vf ^ v0, 16);
            va += vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 += v5 + (m7 ^ 0x243F6A88);
            vf = RotateRight(vf ^ v0, 8);
            va += vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 += v6 + (m6 ^ 0x03707344);
            vc = RotateRight(vc ^ v1, 16);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 += v6 + (m3 ^ 0x082EFA98);
            vc = RotateRight(vc ^ v1, 8);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 += v7 + (m9 ^ 0x13198A2E);
            vd = RotateRight(vd ^ v2, 16);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 += v7 + (m2 ^ 0x38D01377);
            vd = RotateRight(vd ^ v2, 8);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 += v4 + (m8 ^ 0x34E90C6C);
            ve = RotateRight(ve ^ v3, 16);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 += v4 + (mb ^ 0x452821E6);
            ve = RotateRight(ve ^ v3, 8);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 7);

            // Iteration 8
            v0 += v4 + (md ^ 0x34E90C6C);
            vc = RotateRight(vc ^ v0, 16);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 += v4 + (mb ^ 0xC97C50DD);
            vc = RotateRight(vc ^ v0, 8);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 += v5 + (m7 ^ 0x3F84D5B5);
            vd = RotateRight(vd ^ v1, 16);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 += v5 + (me ^ 0xEC4E6C89);
            vd = RotateRight(vd ^ v1, 8);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 += v6 + (mc ^ 0x85A308D3);
            ve = RotateRight(ve ^ v2, 16);
            va += ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 += v6 + (m1 ^ 0xC0AC29B7);
            ve = RotateRight(ve ^ v2, 8);
            va += ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 += v7 + (m3 ^ 0x38D01377);
            vf = RotateRight(vf ^ v3, 16);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 += v7 + (m9 ^ 0x03707344);
            vf = RotateRight(vf ^ v3, 8);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 += v5 + (m5 ^ 0x243F6A88);
            vf = RotateRight(vf ^ v0, 16);
            va += vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 += v5 + (m0 ^ 0x299F31D0);
            vf = RotateRight(vf ^ v0, 8);
            va += vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 += v6 + (mf ^ 0xA4093822);
            vc = RotateRight(vc ^ v1, 16);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 += v6 + (m4 ^ 0xB5470917);
            vc = RotateRight(vc ^ v1, 8);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 += v7 + (m8 ^ 0x082EFA98);
            vd = RotateRight(vd ^ v2, 16);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 += v7 + (m6 ^ 0x452821E6);
            vd = RotateRight(vd ^ v2, 8);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 += v4 + (m2 ^ 0xBE5466CF);
            ve = RotateRight(ve ^ v3, 16);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 += v4 + (ma ^ 0x13198A2E);
            ve = RotateRight(ve ^ v3, 8);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 7);

            // Iteration 9
            v0 += v4 + (m6 ^ 0xB5470917);
            vc = RotateRight(vc ^ v0, 16);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 += v4 + (mf ^ 0x082EFA98);
            vc = RotateRight(vc ^ v0, 8);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 += v5 + (me ^ 0x38D01377);
            vd = RotateRight(vd ^ v1, 16);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 += v5 + (m9 ^ 0x3F84D5B5);
            vd = RotateRight(vd ^ v1, 8);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 += v6 + (mb ^ 0x03707344);
            ve = RotateRight(ve ^ v2, 16);
            va += ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 += v6 + (m3 ^ 0x34E90C6C);
            ve = RotateRight(ve ^ v2, 8);
            va += ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 += v7 + (m0 ^ 0x452821E6);
            vf = RotateRight(vf ^ v3, 16);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 += v7 + (m8 ^ 0x243F6A88);
            vf = RotateRight(vf ^ v3, 8);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 += v5 + (mc ^ 0x13198A2E);
            vf = RotateRight(vf ^ v0, 16);
            va += vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 += v5 + (m2 ^ 0xC0AC29B7);
            vf = RotateRight(vf ^ v0, 8);
            va += vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 += v6 + (md ^ 0xEC4E6C89);
            vc = RotateRight(vc ^ v1, 16);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 += v6 + (m7 ^ 0xC97C50DD);
            vc = RotateRight(vc ^ v1, 8);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 += v7 + (m1 ^ 0xA4093822);
            vd = RotateRight(vd ^ v2, 16);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 += v7 + (m4 ^ 0x85A308D3);
            vd = RotateRight(vd ^ v2, 8);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 += v4 + (ma ^ 0x299F31D0);
            ve = RotateRight(ve ^ v3, 16);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 += v4 + (m5 ^ 0xBE5466CF);
            ve = RotateRight(ve ^ v3, 8);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 7);

            // Iteration 10
            v0 += v4 + (ma ^ 0x13198A2E);
            vc = RotateRight(vc ^ v0, 16);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 += v4 + (m2 ^ 0xBE5466CF);
            vc = RotateRight(vc ^ v0, 8);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 += v5 + (m8 ^ 0xA4093822);
            vd = RotateRight(vd ^ v1, 16);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 += v5 + (m4 ^ 0x452821E6);
            vd = RotateRight(vd ^ v1, 8);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 += v6 + (m7 ^ 0x082EFA98);
            ve = RotateRight(ve ^ v2, 16);
            va += ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 += v6 + (m6 ^ 0xEC4E6C89);
            ve = RotateRight(ve ^ v2, 8);
            va += ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 += v7 + (m1 ^ 0x299F31D0);
            vf = RotateRight(vf ^ v3, 16);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 += v7 + (m5 ^ 0x85A308D3);
            vf = RotateRight(vf ^ v3, 8);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 += v5 + (mf ^ 0x34E90C6C);
            vf = RotateRight(vf ^ v0, 16);
            va += vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 += v5 + (mb ^ 0xB5470917);
            vf = RotateRight(vf ^ v0, 8);
            va += vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 += v6 + (m9 ^ 0x3F84D5B5);
            vc = RotateRight(vc ^ v1, 16);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 += v6 + (me ^ 0x38D01377);
            vc = RotateRight(vc ^ v1, 8);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 += v7 + (m3 ^ 0xC0AC29B7);
            vd = RotateRight(vd ^ v2, 16);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 += v7 + (mc ^ 0x03707344);
            vd = RotateRight(vd ^ v2, 8);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 += v4 + (md ^ 0x243F6A88);
            ve = RotateRight(ve ^ v3, 16);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 += v4 + (m0 ^ 0xC97C50DD);
            ve = RotateRight(ve ^ v3, 8);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 7);

            // Iteration 11
            v0 += v4 + (m0 ^ 0x85A308D3);
            vc = RotateRight(vc ^ v0, 16);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 += v4 + (m1 ^ 0x243F6A88);
            vc = RotateRight(vc ^ v0, 8);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 += v5 + (m2 ^ 0x03707344);
            vd = RotateRight(vd ^ v1, 16);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 += v5 + (m3 ^ 0x13198A2E);
            vd = RotateRight(vd ^ v1, 8);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 += v6 + (m4 ^ 0x299F31D0);
            ve = RotateRight(ve ^ v2, 16);
            va += ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 += v6 + (m5 ^ 0xA4093822);
            ve = RotateRight(ve ^ v2, 8);
            va += ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 += v7 + (m6 ^ 0xEC4E6C89);
            vf = RotateRight(vf ^ v3, 16);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 += v7 + (m7 ^ 0x082EFA98);
            vf = RotateRight(vf ^ v3, 8);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 += v5 + (m8 ^ 0x38D01377);
            vf = RotateRight(vf ^ v0, 16);
            va += vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 += v5 + (m9 ^ 0x452821E6);
            vf = RotateRight(vf ^ v0, 8);
            va += vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 += v6 + (ma ^ 0x34E90C6C);
            vc = RotateRight(vc ^ v1, 16);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 += v6 + (mb ^ 0xBE5466CF);
            vc = RotateRight(vc ^ v1, 8);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 += v7 + (mc ^ 0xC97C50DD);
            vd = RotateRight(vd ^ v2, 16);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 += v7 + (md ^ 0xC0AC29B7);
            vd = RotateRight(vd ^ v2, 8);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 += v4 + (me ^ 0xB5470917);
            ve = RotateRight(ve ^ v3, 16);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 += v4 + (mf ^ 0x3F84D5B5);
            ve = RotateRight(ve ^ v3, 8);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 7);

            // Iteration 12
            v0 += v4 + (me ^ 0xBE5466CF);
            vc = RotateRight(vc ^ v0, 16);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 += v4 + (ma ^ 0x3F84D5B5);
            vc = RotateRight(vc ^ v0, 8);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 += v5 + (m4 ^ 0x452821E6);
            vd = RotateRight(vd ^ v1, 16);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 += v5 + (m8 ^ 0xA4093822);
            vd = RotateRight(vd ^ v1, 8);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 += v6 + (m9 ^ 0xB5470917);
            ve = RotateRight(ve ^ v2, 16);
            va += ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 += v6 + (mf ^ 0x38D01377);
            ve = RotateRight(ve ^ v2, 8);
            va += ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 += v7 + (md ^ 0x082EFA98);
            vf = RotateRight(vf ^ v3, 16);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 += v7 + (m6 ^ 0xC97C50DD);
            vf = RotateRight(vf ^ v3, 8);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 += v5 + (m1 ^ 0xC0AC29B7);
            vf = RotateRight(vf ^ v0, 16);
            va += vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 += v5 + (mc ^ 0x85A308D3);
            vf = RotateRight(vf ^ v0, 8);
            va += vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 += v6 + (m0 ^ 0x13198A2E);
            vc = RotateRight(vc ^ v1, 16);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 += v6 + (m2 ^ 0x243F6A88);
            vc = RotateRight(vc ^ v1, 8);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 += v7 + (mb ^ 0xEC4E6C89);
            vd = RotateRight(vd ^ v2, 16);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 += v7 + (m7 ^ 0x34E90C6C);
            vd = RotateRight(vd ^ v2, 8);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 += v4 + (m5 ^ 0x03707344);
            ve = RotateRight(ve ^ v3, 16);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 += v4 + (m3 ^ 0x299F31D0);
            ve = RotateRight(ve ^ v3, 8);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 7);

            // Iteration 13
            v0 += v4 + (mb ^ 0x452821E6);
            vc = RotateRight(vc ^ v0, 16);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 += v4 + (m8 ^ 0x34E90C6C);
            vc = RotateRight(vc ^ v0, 8);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 += v5 + (mc ^ 0x243F6A88);
            vd = RotateRight(vd ^ v1, 16);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 += v5 + (m0 ^ 0xC0AC29B7);
            vd = RotateRight(vd ^ v1, 8);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 += v6 + (m5 ^ 0x13198A2E);
            ve = RotateRight(ve ^ v2, 16);
            va += ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 += v6 + (m2 ^ 0x299F31D0);
            ve = RotateRight(ve ^ v2, 8);
            va += ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 += v7 + (mf ^ 0xC97C50DD);
            vf = RotateRight(vf ^ v3, 16);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 += v7 + (md ^ 0xB5470917);
            vf = RotateRight(vf ^ v3, 8);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 += v5 + (ma ^ 0x3F84D5B5);
            vf = RotateRight(vf ^ v0, 16);
            va += vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 += v5 + (me ^ 0xBE5466CF);
            vf = RotateRight(vf ^ v0, 8);
            va += vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 += v6 + (m3 ^ 0x082EFA98);
            vc = RotateRight(vc ^ v1, 16);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 += v6 + (m6 ^ 0x03707344);
            vc = RotateRight(vc ^ v1, 8);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 += v7 + (m7 ^ 0x85A308D3);
            vd = RotateRight(vd ^ v2, 16);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 += v7 + (m1 ^ 0xEC4E6C89);
            vd = RotateRight(vd ^ v2, 8);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 += v4 + (m9 ^ 0xA4093822);
            ve = RotateRight(ve ^ v3, 16);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 += v4 + (m4 ^ 0x38D01377);
            ve = RotateRight(ve ^ v3, 8);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 7);

            // Iteration 14
            v0 += v4 + (m7 ^ 0x38D01377);
            vc = RotateRight(vc ^ v0, 16);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 += v4 + (m9 ^ 0xEC4E6C89);
            vc = RotateRight(vc ^ v0, 8);
            v8 += vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 += v5 + (m3 ^ 0x85A308D3);
            vd = RotateRight(vd ^ v1, 16);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 += v5 + (m1 ^ 0x03707344);
            vd = RotateRight(vd ^ v1, 8);
            v9 += vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 += v6 + (md ^ 0xC0AC29B7);
            ve = RotateRight(ve ^ v2, 16);
            va += ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 += v6 + (mc ^ 0xC97C50DD);
            ve = RotateRight(ve ^ v2, 8);
            va += ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 += v7 + (mb ^ 0x3F84D5B5);
            vf = RotateRight(vf ^ v3, 16);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 += v7 + (me ^ 0x34E90C6C);
            vf = RotateRight(vf ^ v3, 8);
            vb += vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 += v5 + (m2 ^ 0x082EFA98);
            vf = RotateRight(vf ^ v0, 16);
            va += vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 += v5 + (m6 ^ 0x13198A2E);
            vf = RotateRight(vf ^ v0, 8);
            va += vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 += v6 + (m5 ^ 0xBE5466CF);
            vc = RotateRight(vc ^ v1, 16);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 += v6 + (ma ^ 0x299F31D0);
            vc = RotateRight(vc ^ v1, 8);
            vb += vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 += v7 + (m4 ^ 0x243F6A88);
            vd = RotateRight(vd ^ v2, 16);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 += v7 + (m0 ^ 0xA4093822);
            vd = RotateRight(vd ^ v2, 8);
            v8 += vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 += v4 + (mf ^ 0x452821E6);
            ve = RotateRight(ve ^ v3, 16);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 += v4 + (m8 ^ 0xB5470917);
            ve = RotateRight(ve ^ v3, 8);
            v9 += ve;
            v4 = RotateRight(v4 ^ v9, 7);

            #endregion

            h0 ^= s0 ^ v0 ^ v8;
            h1 ^= s1 ^ v1 ^ v9;
            h2 ^= s2 ^ v2 ^ va;
            h3 ^= s3 ^ v3 ^ vb;
            h4 ^= s0 ^ v4 ^ vc;
            h5 ^= s1 ^ v5 ^ vd;
            h6 ^= s2 ^ v6 ^ ve;
            h7 ^= s3 ^ v7 ^ vf;

        }
    }
}