using System;
using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class BLAKE2s : Security.MessageDigest
    {
        public override int BlockLength => 64;
        public override int DigestLength => _digestLength;

        private int _digestLength;
        private int _keyLength;
        private byte[] _key;

        uint h0;
        uint h1;
        uint h2;
        uint h3;
        uint h4;
        uint h5;
        uint h6;
        uint h7;
        byte[] buffer;
        int bufOffset;
        ulong length;

        public BLAKE2s()
        {
            buffer = new byte[64];
            _digestLength = 32;
            _keyLength = 0;
            Reset();
        }

        public override void Init(MDParameters parameters)
        {
            var p = parameters as BLAKE2Parameters;
            if (p == null)
                throw new ArgumentException(nameof(parameters));

            if (p.DigestLength != null)
            {
                int digestLength = p.DigestLength.Value;
                if (digestLength < 0 || digestLength > 32)
                    throw new ArgumentOutOfRangeException("DigestLength");
                _digestLength = digestLength;
            }
            else
            {
                _digestLength = 32;
            }

            if (p.Key != null)
            {
                int keyLength = p.Key.Length;
                if (keyLength > 32)
                    throw new ArgumentOutOfRangeException("Key.Length");
                _key = p.Key;
                _keyLength = keyLength;
            }
            else
            {
                _key = null;
                _keyLength = 0;
            }

            Reset();
        }

        public override void Reset()
        {
            h0 = 0x6A09E667 ^ 0x01010000 ^ ((uint)_keyLength << 8) ^ (uint)_digestLength;
            h1 = 0xBB67AE85;
            h2 = 0x3C6EF372;
            h3 = 0xA54FF53A;
            h4 = 0x510E527F;
            h5 = 0x9B05688C;
            h6 = 0x1F83D9AB;
            h7 = 0x5BE0CD19;

            bufOffset = 0;

            if (_keyLength > 0)
            {
                Update(_key);
                while (bufOffset < 64)
                    buffer[bufOffset++] = 0;
            }

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
                if (bufOffset == 64)
                {
                    bufOffset = 0;
                    IncreaseLength(64);
                    ProcessBlock(false);
                }

                int copy = System.Math.Min(buffer.Length - bufOffset, length);
                Buffer.BlockCopy(data, offset, buffer, bufOffset, copy);

                offset += copy;
                length -= copy;
                bufOffset += copy;
            }
        }

        public override byte[] Digest()
        {
            IncreaseLength((ulong)bufOffset);

            while (bufOffset < 64)
                buffer[bufOffset++] = 0;

            ProcessBlock(true);

            byte[] digest = new byte[32];
            GetBytes(h0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(h2, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(h3, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(h4, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(h5, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(h6, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(h7, ByteOrder.LittleEndian, digest, 0x1C);

            if (_digestLength == 32)
                return digest;
            else
                return ByteArray.SubArray(digest, 0, _digestLength);
        }

        private void IncreaseLength(ulong value)
        {
            ulong old = length;
            length += value;
            if (old > length)
                throw new InvalidOperationException("Maximum input length = 2^64 - 1 bytes");
        }

        private void ProcessBlock(bool final)
        {
            uint m0 = ToUInt32(buffer, 0x00, ByteOrder.LittleEndian);
            uint m1 = ToUInt32(buffer, 0x04, ByteOrder.LittleEndian);
            uint m2 = ToUInt32(buffer, 0x08, ByteOrder.LittleEndian);
            uint m3 = ToUInt32(buffer, 0x0C, ByteOrder.LittleEndian);
            uint m4 = ToUInt32(buffer, 0x10, ByteOrder.LittleEndian);
            uint m5 = ToUInt32(buffer, 0x14, ByteOrder.LittleEndian);
            uint m6 = ToUInt32(buffer, 0x18, ByteOrder.LittleEndian);
            uint m7 = ToUInt32(buffer, 0x1C, ByteOrder.LittleEndian);
            uint m8 = ToUInt32(buffer, 0x20, ByteOrder.LittleEndian);
            uint m9 = ToUInt32(buffer, 0x24, ByteOrder.LittleEndian);
            uint ma = ToUInt32(buffer, 0x28, ByteOrder.LittleEndian);
            uint mb = ToUInt32(buffer, 0x2C, ByteOrder.LittleEndian);
            uint mc = ToUInt32(buffer, 0x30, ByteOrder.LittleEndian);
            uint md = ToUInt32(buffer, 0x34, ByteOrder.LittleEndian);
            uint me = ToUInt32(buffer, 0x38, ByteOrder.LittleEndian);
            uint mf = ToUInt32(buffer, 0x3C, ByteOrder.LittleEndian);

            uint v0 = h0;
            uint v1 = h1;
            uint v2 = h2;
            uint v3 = h3;
            uint v4 = h4;
            uint v5 = h5;
            uint v6 = h6;
            uint v7 = h7;
            uint v8 = 0x6A09E667;
            uint v9 = 0xBB67AE85;
            uint va = 0x3C6EF372;
            uint vb = 0xA54FF53A;
            uint vc = 0x510E527F ^ (uint)(length & 0xFFFFFFFF);
            uint vd = 0x9B05688C ^ (uint)(length >> 32);
            uint ve = 0x1F83D9AB;
            uint vf = 0x5BE0CD19;

            if (final)
                ve = ~ve;

            #region Loop

            // Iteration 0
            v0 = v0 + v4 + m0;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 = v0 + v4 + m1;
            vc = RotateRight(vc ^ v0, 8);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 = v1 + v5 + m2;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 = v1 + v5 + m3;
            vd = RotateRight(vd ^ v1, 8);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 = v2 + v6 + m4;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 = v2 + v6 + m5;
            ve = RotateRight(ve ^ v2, 8);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 = v3 + v7 + m6;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 = v3 + v7 + m7;
            vf = RotateRight(vf ^ v3, 8);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 = v0 + v5 + m8;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 = v0 + v5 + m9;
            vf = RotateRight(vf ^ v0, 8);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 = v1 + v6 + ma;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 = v1 + v6 + mb;
            vc = RotateRight(vc ^ v1, 8);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 = v2 + v7 + mc;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 = v2 + v7 + md;
            vd = RotateRight(vd ^ v2, 8);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 = v3 + v4 + me;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 = v3 + v4 + mf;
            ve = RotateRight(ve ^ v3, 8);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 7);
            // Iteration 1
            v0 = v0 + v4 + me;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 = v0 + v4 + ma;
            vc = RotateRight(vc ^ v0, 8);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 = v1 + v5 + m4;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 = v1 + v5 + m8;
            vd = RotateRight(vd ^ v1, 8);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 = v2 + v6 + m9;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 = v2 + v6 + mf;
            ve = RotateRight(ve ^ v2, 8);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 = v3 + v7 + md;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 = v3 + v7 + m6;
            vf = RotateRight(vf ^ v3, 8);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 = v0 + v5 + m1;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 = v0 + v5 + mc;
            vf = RotateRight(vf ^ v0, 8);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 = v1 + v6 + m0;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 = v1 + v6 + m2;
            vc = RotateRight(vc ^ v1, 8);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 = v2 + v7 + mb;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 = v2 + v7 + m7;
            vd = RotateRight(vd ^ v2, 8);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 = v3 + v4 + m5;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 = v3 + v4 + m3;
            ve = RotateRight(ve ^ v3, 8);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 7);
            // Iteration 2
            v0 = v0 + v4 + mb;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 = v0 + v4 + m8;
            vc = RotateRight(vc ^ v0, 8);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 = v1 + v5 + mc;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 = v1 + v5 + m0;
            vd = RotateRight(vd ^ v1, 8);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 = v2 + v6 + m5;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 = v2 + v6 + m2;
            ve = RotateRight(ve ^ v2, 8);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 = v3 + v7 + mf;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 = v3 + v7 + md;
            vf = RotateRight(vf ^ v3, 8);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 = v0 + v5 + ma;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 = v0 + v5 + me;
            vf = RotateRight(vf ^ v0, 8);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 = v1 + v6 + m3;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 = v1 + v6 + m6;
            vc = RotateRight(vc ^ v1, 8);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 = v2 + v7 + m7;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 = v2 + v7 + m1;
            vd = RotateRight(vd ^ v2, 8);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 = v3 + v4 + m9;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 = v3 + v4 + m4;
            ve = RotateRight(ve ^ v3, 8);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 7);
            // Iteration 3
            v0 = v0 + v4 + m7;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 = v0 + v4 + m9;
            vc = RotateRight(vc ^ v0, 8);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 = v1 + v5 + m3;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 = v1 + v5 + m1;
            vd = RotateRight(vd ^ v1, 8);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 = v2 + v6 + md;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 = v2 + v6 + mc;
            ve = RotateRight(ve ^ v2, 8);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 = v3 + v7 + mb;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 = v3 + v7 + me;
            vf = RotateRight(vf ^ v3, 8);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 = v0 + v5 + m2;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 = v0 + v5 + m6;
            vf = RotateRight(vf ^ v0, 8);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 = v1 + v6 + m5;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 = v1 + v6 + ma;
            vc = RotateRight(vc ^ v1, 8);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 = v2 + v7 + m4;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 = v2 + v7 + m0;
            vd = RotateRight(vd ^ v2, 8);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 = v3 + v4 + mf;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 = v3 + v4 + m8;
            ve = RotateRight(ve ^ v3, 8);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 7);
            // Iteration 4
            v0 = v0 + v4 + m9;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 = v0 + v4 + m0;
            vc = RotateRight(vc ^ v0, 8);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 = v1 + v5 + m5;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 = v1 + v5 + m7;
            vd = RotateRight(vd ^ v1, 8);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 = v2 + v6 + m2;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 = v2 + v6 + m4;
            ve = RotateRight(ve ^ v2, 8);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 = v3 + v7 + ma;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 = v3 + v7 + mf;
            vf = RotateRight(vf ^ v3, 8);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 = v0 + v5 + me;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 = v0 + v5 + m1;
            vf = RotateRight(vf ^ v0, 8);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 = v1 + v6 + mb;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 = v1 + v6 + mc;
            vc = RotateRight(vc ^ v1, 8);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 = v2 + v7 + m6;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 = v2 + v7 + m8;
            vd = RotateRight(vd ^ v2, 8);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 = v3 + v4 + m3;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 = v3 + v4 + md;
            ve = RotateRight(ve ^ v3, 8);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 7);
            // Iteration 5
            v0 = v0 + v4 + m2;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 = v0 + v4 + mc;
            vc = RotateRight(vc ^ v0, 8);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 = v1 + v5 + m6;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 = v1 + v5 + ma;
            vd = RotateRight(vd ^ v1, 8);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 = v2 + v6 + m0;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 = v2 + v6 + mb;
            ve = RotateRight(ve ^ v2, 8);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 = v3 + v7 + m8;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 = v3 + v7 + m3;
            vf = RotateRight(vf ^ v3, 8);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 = v0 + v5 + m4;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 = v0 + v5 + md;
            vf = RotateRight(vf ^ v0, 8);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 = v1 + v6 + m7;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 = v1 + v6 + m5;
            vc = RotateRight(vc ^ v1, 8);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 = v2 + v7 + mf;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 = v2 + v7 + me;
            vd = RotateRight(vd ^ v2, 8);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 = v3 + v4 + m1;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 = v3 + v4 + m9;
            ve = RotateRight(ve ^ v3, 8);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 7);
            // Iteration 6
            v0 = v0 + v4 + mc;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 = v0 + v4 + m5;
            vc = RotateRight(vc ^ v0, 8);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 = v1 + v5 + m1;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 = v1 + v5 + mf;
            vd = RotateRight(vd ^ v1, 8);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 = v2 + v6 + me;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 = v2 + v6 + md;
            ve = RotateRight(ve ^ v2, 8);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 = v3 + v7 + m4;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 = v3 + v7 + ma;
            vf = RotateRight(vf ^ v3, 8);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 = v0 + v5 + m0;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 = v0 + v5 + m7;
            vf = RotateRight(vf ^ v0, 8);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 = v1 + v6 + m6;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 = v1 + v6 + m3;
            vc = RotateRight(vc ^ v1, 8);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 = v2 + v7 + m9;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 = v2 + v7 + m2;
            vd = RotateRight(vd ^ v2, 8);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 = v3 + v4 + m8;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 = v3 + v4 + mb;
            ve = RotateRight(ve ^ v3, 8);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 7);
            // Iteration 7
            v0 = v0 + v4 + md;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 = v0 + v4 + mb;
            vc = RotateRight(vc ^ v0, 8);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 = v1 + v5 + m7;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 = v1 + v5 + me;
            vd = RotateRight(vd ^ v1, 8);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 = v2 + v6 + mc;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 = v2 + v6 + m1;
            ve = RotateRight(ve ^ v2, 8);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 = v3 + v7 + m3;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 = v3 + v7 + m9;
            vf = RotateRight(vf ^ v3, 8);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 = v0 + v5 + m5;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 = v0 + v5 + m0;
            vf = RotateRight(vf ^ v0, 8);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 = v1 + v6 + mf;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 = v1 + v6 + m4;
            vc = RotateRight(vc ^ v1, 8);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 = v2 + v7 + m8;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 = v2 + v7 + m6;
            vd = RotateRight(vd ^ v2, 8);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 = v3 + v4 + m2;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 = v3 + v4 + ma;
            ve = RotateRight(ve ^ v3, 8);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 7);
            // Iteration 8
            v0 = v0 + v4 + m6;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 = v0 + v4 + mf;
            vc = RotateRight(vc ^ v0, 8);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 = v1 + v5 + me;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 = v1 + v5 + m9;
            vd = RotateRight(vd ^ v1, 8);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 = v2 + v6 + mb;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 = v2 + v6 + m3;
            ve = RotateRight(ve ^ v2, 8);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 = v3 + v7 + m0;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 = v3 + v7 + m8;
            vf = RotateRight(vf ^ v3, 8);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 = v0 + v5 + mc;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 = v0 + v5 + m2;
            vf = RotateRight(vf ^ v0, 8);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 = v1 + v6 + md;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 = v1 + v6 + m7;
            vc = RotateRight(vc ^ v1, 8);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 = v2 + v7 + m1;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 = v2 + v7 + m4;
            vd = RotateRight(vd ^ v2, 8);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 = v3 + v4 + ma;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 = v3 + v4 + m5;
            ve = RotateRight(ve ^ v3, 8);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 7);
            // Iteration 9
            v0 = v0 + v4 + ma;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 12);
            v0 = v0 + v4 + m2;
            vc = RotateRight(vc ^ v0, 8);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 7);
            v1 = v1 + v5 + m8;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 12);
            v1 = v1 + v5 + m4;
            vd = RotateRight(vd ^ v1, 8);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 7);
            v2 = v2 + v6 + m7;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 12);
            v2 = v2 + v6 + m6;
            ve = RotateRight(ve ^ v2, 8);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 7);
            v3 = v3 + v7 + m1;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 12);
            v3 = v3 + v7 + m5;
            vf = RotateRight(vf ^ v3, 8);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 7);
            v0 = v0 + v5 + mf;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 12);
            v0 = v0 + v5 + mb;
            vf = RotateRight(vf ^ v0, 8);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 7);
            v1 = v1 + v6 + m9;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 12);
            v1 = v1 + v6 + me;
            vc = RotateRight(vc ^ v1, 8);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 7);
            v2 = v2 + v7 + m3;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 12);
            v2 = v2 + v7 + mc;
            vd = RotateRight(vd ^ v2, 8);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 7);
            v3 = v3 + v4 + md;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 12);
            v3 = v3 + v4 + m0;
            ve = RotateRight(ve ^ v3, 8);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 7);

            #endregion

            h0 = h0 ^ v0 ^ v8;
            h1 = h1 ^ v1 ^ v9;
            h2 = h2 ^ v2 ^ va;
            h3 = h3 ^ v3 ^ vb;
            h4 = h4 ^ v4 ^ vc;
            h5 = h5 ^ v5 ^ vd;
            h6 = h6 ^ v6 ^ ve;
            h7 = h7 ^ v7 ^ vf;
        }
    }
}