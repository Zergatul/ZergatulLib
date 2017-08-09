using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.BlockCipher.CipherMode
{
    public class GCM : AbstractAEADCipherMode
    {
        protected override AEADEncryptor CreateEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
        {
            return new GCMEncryptor(cipher, processBlock, TagLength);
        }

        protected override AEADDecryptor CreateDecryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
        {
            return new GCMDecryptor(cipher, processBlock, TagLength);
        }

        private static readonly BinaryPolynomial Polynomial = BinaryPolynomial.FromPowers(128, 7, 2, 1, 0);

        public static byte[] BitReverseTable =
{
    0x00, 0x80, 0x40, 0xc0, 0x20, 0xa0, 0x60, 0xe0,
    0x10, 0x90, 0x50, 0xd0, 0x30, 0xb0, 0x70, 0xf0,
    0x08, 0x88, 0x48, 0xc8, 0x28, 0xa8, 0x68, 0xe8,
    0x18, 0x98, 0x58, 0xd8, 0x38, 0xb8, 0x78, 0xf8,
    0x04, 0x84, 0x44, 0xc4, 0x24, 0xa4, 0x64, 0xe4,
    0x14, 0x94, 0x54, 0xd4, 0x34, 0xb4, 0x74, 0xf4,
    0x0c, 0x8c, 0x4c, 0xcc, 0x2c, 0xac, 0x6c, 0xec,
    0x1c, 0x9c, 0x5c, 0xdc, 0x3c, 0xbc, 0x7c, 0xfc,
    0x02, 0x82, 0x42, 0xc2, 0x22, 0xa2, 0x62, 0xe2,
    0x12, 0x92, 0x52, 0xd2, 0x32, 0xb2, 0x72, 0xf2,
    0x0a, 0x8a, 0x4a, 0xca, 0x2a, 0xaa, 0x6a, 0xea,
    0x1a, 0x9a, 0x5a, 0xda, 0x3a, 0xba, 0x7a, 0xfa,
    0x06, 0x86, 0x46, 0xc6, 0x26, 0xa6, 0x66, 0xe6,
    0x16, 0x96, 0x56, 0xd6, 0x36, 0xb6, 0x76, 0xf6,
    0x0e, 0x8e, 0x4e, 0xce, 0x2e, 0xae, 0x6e, 0xee,
    0x1e, 0x9e, 0x5e, 0xde, 0x3e, 0xbe, 0x7e, 0xfe,
    0x01, 0x81, 0x41, 0xc1, 0x21, 0xa1, 0x61, 0xe1,
    0x11, 0x91, 0x51, 0xd1, 0x31, 0xb1, 0x71, 0xf1,
    0x09, 0x89, 0x49, 0xc9, 0x29, 0xa9, 0x69, 0xe9,
    0x19, 0x99, 0x59, 0xd9, 0x39, 0xb9, 0x79, 0xf9,
    0x05, 0x85, 0x45, 0xc5, 0x25, 0xa5, 0x65, 0xe5,
    0x15, 0x95, 0x55, 0xd5, 0x35, 0xb5, 0x75, 0xf5,
    0x0d, 0x8d, 0x4d, 0xcd, 0x2d, 0xad, 0x6d, 0xed,
    0x1d, 0x9d, 0x5d, 0xdd, 0x3d, 0xbd, 0x7d, 0xfd,
    0x03, 0x83, 0x43, 0xc3, 0x23, 0xa3, 0x63, 0xe3,
    0x13, 0x93, 0x53, 0xd3, 0x33, 0xb3, 0x73, 0xf3,
    0x0b, 0x8b, 0x4b, 0xcb, 0x2b, 0xab, 0x6b, 0xeb,
    0x1b, 0x9b, 0x5b, 0xdb, 0x3b, 0xbb, 0x7b, 0xfb,
    0x07, 0x87, 0x47, 0xc7, 0x27, 0xa7, 0x67, 0xe7,
    0x17, 0x97, 0x57, 0xd7, 0x37, 0xb7, 0x77, 0xf7,
    0x0f, 0x8f, 0x4f, 0xcf, 0x2f, 0xaf, 0x6f, 0xef,
    0x1f, 0x9f, 0x5f, 0xdf, 0x3f, 0xbf, 0x7f, 0xff
};

        private static byte[] BlockMult(byte[] X, byte[] Y)
        {
            byte[] xr = new byte[16];
            byte[] yr = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                xr[i] = BitReverseTable[X[i]];
                yr[i] = BitReverseTable[Y[i]];
            }

            var xp = new BinaryPolynomial(xr, ByteOrder.LittleEndian);
            var yp = new BinaryPolynomial(yr, ByteOrder.LittleEndian);
            var mult = BinaryPolynomial.ModularMultiplication(xp, yp, Polynomial);

            byte[] result = mult.ToBytes(ByteOrder.LittleEndian, 16);
            for (int i = 0; i < 16; i++)
                result[i] = BitReverseTable[result[i]];
            return result;
        }

        private static byte[] GHash(byte[] H, byte[] X)
        {
            if (X.Length % 16 != 0)
                throw new InvalidOperationException();

            int m = X.Length / 16;
            byte[] Y = new byte[16];
            byte[] xored = new byte[16];

            for (int i = 0; i < m; i++)
            {
                for (int b = 0; b < 16; b++)
                    xored[b] = (byte)(X[i * 16 + b] ^ Y[b]);
                Y = BlockMult(xored, H);
            }

            return Y;
        }

        private static void Inc32(byte[] x)
        {
            if (x.Length != 16)
                throw new InvalidOperationException();

            int carry = 1;
            for (int i = 15; i >= 12; i--)
            {
                carry = carry + x[i];
                x[i] = (byte)carry;
                carry >>= 8;
            }
        }

        private static byte[] GCTR(byte[] ICB, byte[] X, Func<byte[], byte[]> processBlock)
        {
            if (X.Length == 0)
                return new byte[0];
            if (ICB.Length != 16)
                throw new InvalidOperationException();

            int n = (X.Length + 15) / 16;
            byte[] Y = new byte[X.Length];

            byte[] CB = ICB; // do not copy, keep in mind - ICB will be modified here

            for (int i = 0; i < n; i++)
            {
                var block = processBlock(CB);
                Inc32(CB);
                int last = System.Math.Min(16, X.Length - i * 16);
                for (int b = 0; b < last; b++)
                    Y[i * 16 + b] = (byte)(X[i * 16 + b] ^ block[b]);
            }

            return Y;
        }

        private static byte[] CalculateJ0(byte[] H, byte[] IV)
        {
            byte[] J0 = new byte[16];
            if (IV.Length == 12)
            {
                // If len(IV)=96, then let J0 = IV || 0^31 || 1
                Array.Copy(IV, J0, 12);
                J0[15] = 1;
            }
            else
            {
                // If len(IV) ≠ 96, then let s = 128 ⎡len(IV)/128⎤-len(IV),
                // and let J0 = GHASH(H)( IV || 0^(s+64) || [len(IV)](64))
                int s = (IV.Length + 15) / 16 * 16 - IV.Length;
                byte[] p = new byte[IV.Length + s + 8 + 8];
                Array.Copy(IV, p, IV.Length);
                Array.Copy(BitHelper.GetBytes(IV.Length * 8, ByteOrder.BigEndian), 0, p, p.Length - 4, 4);
                J0 = GHash(H, p);
            }

            return J0;
        }

        private static byte[] CalculateBlockForHash(byte[] A, byte[] C)
        {
            int u = (C.Length + 15) / 16 * 16 - C.Length;
            int v = (A.Length + 15) / 16 * 16 - A.Length;

            byte[] p = new byte[A.Length + v + C.Length + u + 8 + 8];
            int index = 0;
            Array.Copy(A, 0, p, index, A.Length);
            index += A.Length;
            index += v;
            Array.Copy(C, 0, p, index, C.Length);
            index += C.Length;
            index += u;
            Array.Copy(BitHelper.GetBytes(A.Length * 8, ByteOrder.BigEndian), 0, p, index + 4, 4);
            index += 8;
            Array.Copy(BitHelper.GetBytes(C.Length * 8, ByteOrder.BigEndian), 0, p, index + 4, 4);

            return p;
        }

        private class GCMEncryptor : AEADEncryptor
        {
            int _tagLen;
            private Func<byte[], byte[]> _processBlock;

            public GCMEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock, int tagLen)
            {
                if (cipher.BlockSize != 16)
                    throw new NotSupportedException("Only 128-bit block ciphers are supported");

                this._processBlock = processBlock;
                this._tagLen = tagLen;
            }

            public override AEADCipherData Encrypt(byte[] IV, byte[] data, byte[] authenticatedData)
            {
                if (IV == null)
                    throw new ArgumentNullException(nameof(IV));
                if (data == null)
                    throw new ArgumentNullException(nameof(data));
                if (authenticatedData == null)
                    throw new ArgumentNullException(nameof(authenticatedData));

                byte[] A = authenticatedData;
                byte[] H = _processBlock(new byte[16]);

                byte[] J0 = CalculateJ0(H, IV);

                byte[] J = new byte[16];
                Array.Copy(J0, J, 16);
                Inc32(J);
                byte[] C = GCTR(J, data, _processBlock);

                byte[] S = GHash(H, CalculateBlockForHash(A, C));

                byte[] T = GCTR(J0, S, _processBlock);

                byte[] tag = new byte[_tagLen];
                Array.Copy(T, tag, _tagLen);

                return new AEADCipherData
                {
                    CipherText = C,
                    Tag = tag
                };
            }
        }

        private class GCMDecryptor : AEADDecryptor
        {
            int _tagLen;
            private Func<byte[], byte[]> _processBlock;

            public GCMDecryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock, int tagLen)
            {
                if (cipher.BlockSize != 16)
                    throw new NotSupportedException("Only 128-bit block ciphers are supported");

                this._processBlock = processBlock;
                this._tagLen = tagLen;
            }

            public override byte[] Decrypt(byte[] IV, AEADCipherData data, byte[] authenticatedData)
            {
                if (IV == null)
                    throw new ArgumentNullException(nameof(IV));
                if (data == null)
                    throw new ArgumentNullException(nameof(data));
                if (authenticatedData == null)
                    throw new ArgumentNullException(nameof(authenticatedData));

                byte[] A = authenticatedData;
                byte[] C = data.CipherText;
                byte[] H = _processBlock(new byte[16]);

                byte[] J0 = CalculateJ0(H, IV);

                byte[] J = new byte[16];
                Array.Copy(J0, J, 16);
                Inc32(J);
                byte[] P = GCTR(J, C, _processBlock);

                byte[] S = GHash(H, CalculateBlockForHash(A, C));
                byte[] T = GCTR(J0, S, _processBlock);
                for (int i = 0; i < _tagLen; i++)
                    if (T[i] != data.Tag[i])
                        throw new AuthenticationException();

                return P;
            }
        }
    }
}