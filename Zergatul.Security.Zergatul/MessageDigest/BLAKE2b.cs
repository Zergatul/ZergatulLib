using System;
using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class BLAKE2b : Security.MessageDigest
    {
        public override int BlockLength => 128;
        public override int DigestLength => _digestLength;

        private int _digestLength;
        private int _keyLength;
        private byte[] _key;

        ulong h0;
        ulong h1;
        ulong h2;
        ulong h3;
        ulong h4;
        ulong h5;
        ulong h6;
        ulong h7;
        byte[] buffer;
        int bufOffset;
        ulong lengthLo;
        ulong lengthHi;

        public BLAKE2b()
        {
            buffer = new byte[128];
            _digestLength = 64;
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
                if (digestLength < 0 || digestLength > 64)
                    throw new ArgumentOutOfRangeException("DigestLength");
                _digestLength = digestLength;
            }
            else
            {
                _digestLength = 64;
            }

            if (p.Key != null)
            {
                int keyLength = p.Key.Length;
                if (keyLength > 64)
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
            h0 = 0x6A09E667F3BCC908UL ^ 0x01010000 ^ ((ulong)_keyLength << 8) ^ (ulong)_digestLength;
            h1 = 0xBB67AE8584CAA73B;
            h2 = 0x3C6EF372FE94F82B;
            h3 = 0xA54FF53A5F1D36F1;
            h4 = 0x510E527FADE682D1;
            h5 = 0x9B05688C2B3E6C1F;
            h6 = 0x1F83D9ABFB41BD6B;
            h7 = 0x5BE0CD19137E2179;

            bufOffset = 0;

            if (_keyLength > 0)
            {
                Update(_key);
                while (bufOffset < 128)
                    buffer[bufOffset++] = 0;
            }

            lengthLo = 0;
            lengthHi = 0;
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
                if (bufOffset == 128)
                {
                    bufOffset = 0;
                    IncreaseLength(128);
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

            while (bufOffset < 128)
                buffer[bufOffset++] = 0;

            ProcessBlock(true);

            byte[] digest = new byte[64];
            GetBytes(h0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(h2, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(h3, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(h4, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(h5, ByteOrder.LittleEndian, digest, 0x28);
            GetBytes(h6, ByteOrder.LittleEndian, digest, 0x30);
            GetBytes(h7, ByteOrder.LittleEndian, digest, 0x38);

            if (_digestLength == 64)
                return digest;
            else
                return ByteArray.SubArray(digest, 0, _digestLength);
        }

        private void IncreaseLength(ulong value)
        {
            ulong oldHi = lengthHi;
            lengthLo += value;
            if (lengthLo < value)
            {
                lengthHi++;
                if (oldHi > lengthHi)
                    throw new InvalidOperationException("Maximum input length = 2^128 - 1 bytes");
            }
        }

        private void ProcessBlock(bool final)
        {
            ulong m0 = ToUInt64(buffer, 0x00, ByteOrder.LittleEndian);
            ulong m1 = ToUInt64(buffer, 0x08, ByteOrder.LittleEndian);
            ulong m2 = ToUInt64(buffer, 0x10, ByteOrder.LittleEndian);
            ulong m3 = ToUInt64(buffer, 0x18, ByteOrder.LittleEndian);
            ulong m4 = ToUInt64(buffer, 0x20, ByteOrder.LittleEndian);
            ulong m5 = ToUInt64(buffer, 0x28, ByteOrder.LittleEndian);
            ulong m6 = ToUInt64(buffer, 0x30, ByteOrder.LittleEndian);
            ulong m7 = ToUInt64(buffer, 0x38, ByteOrder.LittleEndian);
            ulong m8 = ToUInt64(buffer, 0x40, ByteOrder.LittleEndian);
            ulong m9 = ToUInt64(buffer, 0x48, ByteOrder.LittleEndian);
            ulong ma = ToUInt64(buffer, 0x50, ByteOrder.LittleEndian);
            ulong mb = ToUInt64(buffer, 0x58, ByteOrder.LittleEndian);
            ulong mc = ToUInt64(buffer, 0x60, ByteOrder.LittleEndian);
            ulong md = ToUInt64(buffer, 0x68, ByteOrder.LittleEndian);
            ulong me = ToUInt64(buffer, 0x70, ByteOrder.LittleEndian);
            ulong mf = ToUInt64(buffer, 0x78, ByteOrder.LittleEndian);

            ulong v0 = h0;
            ulong v1 = h1;
            ulong v2 = h2;
            ulong v3 = h3;
            ulong v4 = h4;
            ulong v5 = h5;
            ulong v6 = h6;
            ulong v7 = h7;
            ulong v8 = 0x6A09E667F3BCC908;
            ulong v9 = 0xBB67AE8584CAA73B;
            ulong va = 0x3C6EF372FE94F82B;
            ulong vb = 0xA54FF53A5F1D36F1;
            ulong vc = 0x510E527FADE682D1 ^ lengthLo;
            ulong vd = 0x9B05688C2B3E6C1F ^ lengthHi;
            ulong ve = 0x1F83D9ABFB41BD6B;
            ulong vf = 0x5BE0CD19137E2179;

            if (final)
                ve = ~ve;

            #region Loop

            // Iteration 0
            v0 = v0 + v4 + m0;
            vc = RotateRight(vc ^ v0, 32);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 24);
            v0 = v0 + v4 + m1;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 63);
            v1 = v1 + v5 + m2;
            vd = RotateRight(vd ^ v1, 32);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 24);
            v1 = v1 + v5 + m3;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 63);
            v2 = v2 + v6 + m4;
            ve = RotateRight(ve ^ v2, 32);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 24);
            v2 = v2 + v6 + m5;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 63);
            v3 = v3 + v7 + m6;
            vf = RotateRight(vf ^ v3, 32);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 24);
            v3 = v3 + v7 + m7;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 63);
            v0 = v0 + v5 + m8;
            vf = RotateRight(vf ^ v0, 32);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 24);
            v0 = v0 + v5 + m9;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 63);
            v1 = v1 + v6 + ma;
            vc = RotateRight(vc ^ v1, 32);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 24);
            v1 = v1 + v6 + mb;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 63);
            v2 = v2 + v7 + mc;
            vd = RotateRight(vd ^ v2, 32);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 24);
            v2 = v2 + v7 + md;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 63);
            v3 = v3 + v4 + me;
            ve = RotateRight(ve ^ v3, 32);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 24);
            v3 = v3 + v4 + mf;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 63);
            // Iteration 1
            v0 = v0 + v4 + me;
            vc = RotateRight(vc ^ v0, 32);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 24);
            v0 = v0 + v4 + ma;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 63);
            v1 = v1 + v5 + m4;
            vd = RotateRight(vd ^ v1, 32);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 24);
            v1 = v1 + v5 + m8;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 63);
            v2 = v2 + v6 + m9;
            ve = RotateRight(ve ^ v2, 32);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 24);
            v2 = v2 + v6 + mf;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 63);
            v3 = v3 + v7 + md;
            vf = RotateRight(vf ^ v3, 32);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 24);
            v3 = v3 + v7 + m6;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 63);
            v0 = v0 + v5 + m1;
            vf = RotateRight(vf ^ v0, 32);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 24);
            v0 = v0 + v5 + mc;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 63);
            v1 = v1 + v6 + m0;
            vc = RotateRight(vc ^ v1, 32);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 24);
            v1 = v1 + v6 + m2;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 63);
            v2 = v2 + v7 + mb;
            vd = RotateRight(vd ^ v2, 32);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 24);
            v2 = v2 + v7 + m7;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 63);
            v3 = v3 + v4 + m5;
            ve = RotateRight(ve ^ v3, 32);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 24);
            v3 = v3 + v4 + m3;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 63);
            // Iteration 2
            v0 = v0 + v4 + mb;
            vc = RotateRight(vc ^ v0, 32);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 24);
            v0 = v0 + v4 + m8;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 63);
            v1 = v1 + v5 + mc;
            vd = RotateRight(vd ^ v1, 32);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 24);
            v1 = v1 + v5 + m0;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 63);
            v2 = v2 + v6 + m5;
            ve = RotateRight(ve ^ v2, 32);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 24);
            v2 = v2 + v6 + m2;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 63);
            v3 = v3 + v7 + mf;
            vf = RotateRight(vf ^ v3, 32);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 24);
            v3 = v3 + v7 + md;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 63);
            v0 = v0 + v5 + ma;
            vf = RotateRight(vf ^ v0, 32);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 24);
            v0 = v0 + v5 + me;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 63);
            v1 = v1 + v6 + m3;
            vc = RotateRight(vc ^ v1, 32);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 24);
            v1 = v1 + v6 + m6;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 63);
            v2 = v2 + v7 + m7;
            vd = RotateRight(vd ^ v2, 32);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 24);
            v2 = v2 + v7 + m1;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 63);
            v3 = v3 + v4 + m9;
            ve = RotateRight(ve ^ v3, 32);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 24);
            v3 = v3 + v4 + m4;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 63);
            // Iteration 3
            v0 = v0 + v4 + m7;
            vc = RotateRight(vc ^ v0, 32);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 24);
            v0 = v0 + v4 + m9;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 63);
            v1 = v1 + v5 + m3;
            vd = RotateRight(vd ^ v1, 32);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 24);
            v1 = v1 + v5 + m1;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 63);
            v2 = v2 + v6 + md;
            ve = RotateRight(ve ^ v2, 32);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 24);
            v2 = v2 + v6 + mc;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 63);
            v3 = v3 + v7 + mb;
            vf = RotateRight(vf ^ v3, 32);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 24);
            v3 = v3 + v7 + me;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 63);
            v0 = v0 + v5 + m2;
            vf = RotateRight(vf ^ v0, 32);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 24);
            v0 = v0 + v5 + m6;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 63);
            v1 = v1 + v6 + m5;
            vc = RotateRight(vc ^ v1, 32);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 24);
            v1 = v1 + v6 + ma;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 63);
            v2 = v2 + v7 + m4;
            vd = RotateRight(vd ^ v2, 32);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 24);
            v2 = v2 + v7 + m0;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 63);
            v3 = v3 + v4 + mf;
            ve = RotateRight(ve ^ v3, 32);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 24);
            v3 = v3 + v4 + m8;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 63);
            // Iteration 4
            v0 = v0 + v4 + m9;
            vc = RotateRight(vc ^ v0, 32);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 24);
            v0 = v0 + v4 + m0;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 63);
            v1 = v1 + v5 + m5;
            vd = RotateRight(vd ^ v1, 32);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 24);
            v1 = v1 + v5 + m7;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 63);
            v2 = v2 + v6 + m2;
            ve = RotateRight(ve ^ v2, 32);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 24);
            v2 = v2 + v6 + m4;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 63);
            v3 = v3 + v7 + ma;
            vf = RotateRight(vf ^ v3, 32);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 24);
            v3 = v3 + v7 + mf;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 63);
            v0 = v0 + v5 + me;
            vf = RotateRight(vf ^ v0, 32);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 24);
            v0 = v0 + v5 + m1;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 63);
            v1 = v1 + v6 + mb;
            vc = RotateRight(vc ^ v1, 32);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 24);
            v1 = v1 + v6 + mc;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 63);
            v2 = v2 + v7 + m6;
            vd = RotateRight(vd ^ v2, 32);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 24);
            v2 = v2 + v7 + m8;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 63);
            v3 = v3 + v4 + m3;
            ve = RotateRight(ve ^ v3, 32);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 24);
            v3 = v3 + v4 + md;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 63);
            // Iteration 5
            v0 = v0 + v4 + m2;
            vc = RotateRight(vc ^ v0, 32);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 24);
            v0 = v0 + v4 + mc;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 63);
            v1 = v1 + v5 + m6;
            vd = RotateRight(vd ^ v1, 32);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 24);
            v1 = v1 + v5 + ma;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 63);
            v2 = v2 + v6 + m0;
            ve = RotateRight(ve ^ v2, 32);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 24);
            v2 = v2 + v6 + mb;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 63);
            v3 = v3 + v7 + m8;
            vf = RotateRight(vf ^ v3, 32);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 24);
            v3 = v3 + v7 + m3;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 63);
            v0 = v0 + v5 + m4;
            vf = RotateRight(vf ^ v0, 32);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 24);
            v0 = v0 + v5 + md;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 63);
            v1 = v1 + v6 + m7;
            vc = RotateRight(vc ^ v1, 32);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 24);
            v1 = v1 + v6 + m5;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 63);
            v2 = v2 + v7 + mf;
            vd = RotateRight(vd ^ v2, 32);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 24);
            v2 = v2 + v7 + me;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 63);
            v3 = v3 + v4 + m1;
            ve = RotateRight(ve ^ v3, 32);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 24);
            v3 = v3 + v4 + m9;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 63);
            // Iteration 6
            v0 = v0 + v4 + mc;
            vc = RotateRight(vc ^ v0, 32);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 24);
            v0 = v0 + v4 + m5;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 63);
            v1 = v1 + v5 + m1;
            vd = RotateRight(vd ^ v1, 32);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 24);
            v1 = v1 + v5 + mf;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 63);
            v2 = v2 + v6 + me;
            ve = RotateRight(ve ^ v2, 32);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 24);
            v2 = v2 + v6 + md;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 63);
            v3 = v3 + v7 + m4;
            vf = RotateRight(vf ^ v3, 32);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 24);
            v3 = v3 + v7 + ma;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 63);
            v0 = v0 + v5 + m0;
            vf = RotateRight(vf ^ v0, 32);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 24);
            v0 = v0 + v5 + m7;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 63);
            v1 = v1 + v6 + m6;
            vc = RotateRight(vc ^ v1, 32);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 24);
            v1 = v1 + v6 + m3;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 63);
            v2 = v2 + v7 + m9;
            vd = RotateRight(vd ^ v2, 32);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 24);
            v2 = v2 + v7 + m2;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 63);
            v3 = v3 + v4 + m8;
            ve = RotateRight(ve ^ v3, 32);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 24);
            v3 = v3 + v4 + mb;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 63);
            // Iteration 7
            v0 = v0 + v4 + md;
            vc = RotateRight(vc ^ v0, 32);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 24);
            v0 = v0 + v4 + mb;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 63);
            v1 = v1 + v5 + m7;
            vd = RotateRight(vd ^ v1, 32);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 24);
            v1 = v1 + v5 + me;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 63);
            v2 = v2 + v6 + mc;
            ve = RotateRight(ve ^ v2, 32);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 24);
            v2 = v2 + v6 + m1;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 63);
            v3 = v3 + v7 + m3;
            vf = RotateRight(vf ^ v3, 32);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 24);
            v3 = v3 + v7 + m9;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 63);
            v0 = v0 + v5 + m5;
            vf = RotateRight(vf ^ v0, 32);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 24);
            v0 = v0 + v5 + m0;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 63);
            v1 = v1 + v6 + mf;
            vc = RotateRight(vc ^ v1, 32);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 24);
            v1 = v1 + v6 + m4;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 63);
            v2 = v2 + v7 + m8;
            vd = RotateRight(vd ^ v2, 32);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 24);
            v2 = v2 + v7 + m6;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 63);
            v3 = v3 + v4 + m2;
            ve = RotateRight(ve ^ v3, 32);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 24);
            v3 = v3 + v4 + ma;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 63);
            // Iteration 8
            v0 = v0 + v4 + m6;
            vc = RotateRight(vc ^ v0, 32);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 24);
            v0 = v0 + v4 + mf;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 63);
            v1 = v1 + v5 + me;
            vd = RotateRight(vd ^ v1, 32);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 24);
            v1 = v1 + v5 + m9;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 63);
            v2 = v2 + v6 + mb;
            ve = RotateRight(ve ^ v2, 32);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 24);
            v2 = v2 + v6 + m3;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 63);
            v3 = v3 + v7 + m0;
            vf = RotateRight(vf ^ v3, 32);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 24);
            v3 = v3 + v7 + m8;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 63);
            v0 = v0 + v5 + mc;
            vf = RotateRight(vf ^ v0, 32);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 24);
            v0 = v0 + v5 + m2;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 63);
            v1 = v1 + v6 + md;
            vc = RotateRight(vc ^ v1, 32);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 24);
            v1 = v1 + v6 + m7;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 63);
            v2 = v2 + v7 + m1;
            vd = RotateRight(vd ^ v2, 32);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 24);
            v2 = v2 + v7 + m4;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 63);
            v3 = v3 + v4 + ma;
            ve = RotateRight(ve ^ v3, 32);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 24);
            v3 = v3 + v4 + m5;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 63);
            // Iteration 9
            v0 = v0 + v4 + ma;
            vc = RotateRight(vc ^ v0, 32);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 24);
            v0 = v0 + v4 + m2;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 63);
            v1 = v1 + v5 + m8;
            vd = RotateRight(vd ^ v1, 32);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 24);
            v1 = v1 + v5 + m4;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 63);
            v2 = v2 + v6 + m7;
            ve = RotateRight(ve ^ v2, 32);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 24);
            v2 = v2 + v6 + m6;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 63);
            v3 = v3 + v7 + m1;
            vf = RotateRight(vf ^ v3, 32);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 24);
            v3 = v3 + v7 + m5;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 63);
            v0 = v0 + v5 + mf;
            vf = RotateRight(vf ^ v0, 32);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 24);
            v0 = v0 + v5 + mb;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 63);
            v1 = v1 + v6 + m9;
            vc = RotateRight(vc ^ v1, 32);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 24);
            v1 = v1 + v6 + me;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 63);
            v2 = v2 + v7 + m3;
            vd = RotateRight(vd ^ v2, 32);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 24);
            v2 = v2 + v7 + mc;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 63);
            v3 = v3 + v4 + md;
            ve = RotateRight(ve ^ v3, 32);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 24);
            v3 = v3 + v4 + m0;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 63);
            // Iteration 10
            v0 = v0 + v4 + m0;
            vc = RotateRight(vc ^ v0, 32);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 24);
            v0 = v0 + v4 + m1;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 63);
            v1 = v1 + v5 + m2;
            vd = RotateRight(vd ^ v1, 32);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 24);
            v1 = v1 + v5 + m3;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 63);
            v2 = v2 + v6 + m4;
            ve = RotateRight(ve ^ v2, 32);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 24);
            v2 = v2 + v6 + m5;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 63);
            v3 = v3 + v7 + m6;
            vf = RotateRight(vf ^ v3, 32);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 24);
            v3 = v3 + v7 + m7;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 63);
            v0 = v0 + v5 + m8;
            vf = RotateRight(vf ^ v0, 32);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 24);
            v0 = v0 + v5 + m9;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 63);
            v1 = v1 + v6 + ma;
            vc = RotateRight(vc ^ v1, 32);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 24);
            v1 = v1 + v6 + mb;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 63);
            v2 = v2 + v7 + mc;
            vd = RotateRight(vd ^ v2, 32);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 24);
            v2 = v2 + v7 + md;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 63);
            v3 = v3 + v4 + me;
            ve = RotateRight(ve ^ v3, 32);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 24);
            v3 = v3 + v4 + mf;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 63);
            // Iteration 11
            v0 = v0 + v4 + me;
            vc = RotateRight(vc ^ v0, 32);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 24);
            v0 = v0 + v4 + ma;
            vc = RotateRight(vc ^ v0, 16);
            v8 = v8 + vc;
            v4 = RotateRight(v4 ^ v8, 63);
            v1 = v1 + v5 + m4;
            vd = RotateRight(vd ^ v1, 32);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 24);
            v1 = v1 + v5 + m8;
            vd = RotateRight(vd ^ v1, 16);
            v9 = v9 + vd;
            v5 = RotateRight(v5 ^ v9, 63);
            v2 = v2 + v6 + m9;
            ve = RotateRight(ve ^ v2, 32);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 24);
            v2 = v2 + v6 + mf;
            ve = RotateRight(ve ^ v2, 16);
            va = va + ve;
            v6 = RotateRight(v6 ^ va, 63);
            v3 = v3 + v7 + md;
            vf = RotateRight(vf ^ v3, 32);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 24);
            v3 = v3 + v7 + m6;
            vf = RotateRight(vf ^ v3, 16);
            vb = vb + vf;
            v7 = RotateRight(v7 ^ vb, 63);
            v0 = v0 + v5 + m1;
            vf = RotateRight(vf ^ v0, 32);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 24);
            v0 = v0 + v5 + mc;
            vf = RotateRight(vf ^ v0, 16);
            va = va + vf;
            v5 = RotateRight(v5 ^ va, 63);
            v1 = v1 + v6 + m0;
            vc = RotateRight(vc ^ v1, 32);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 24);
            v1 = v1 + v6 + m2;
            vc = RotateRight(vc ^ v1, 16);
            vb = vb + vc;
            v6 = RotateRight(v6 ^ vb, 63);
            v2 = v2 + v7 + mb;
            vd = RotateRight(vd ^ v2, 32);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 24);
            v2 = v2 + v7 + m7;
            vd = RotateRight(vd ^ v2, 16);
            v8 = v8 + vd;
            v7 = RotateRight(v7 ^ v8, 63);
            v3 = v3 + v4 + m5;
            ve = RotateRight(ve ^ v3, 32);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 24);
            v3 = v3 + v4 + m3;
            ve = RotateRight(ve ^ v3, 16);
            v9 = v9 + ve;
            v4 = RotateRight(v4 ^ v9, 63);

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