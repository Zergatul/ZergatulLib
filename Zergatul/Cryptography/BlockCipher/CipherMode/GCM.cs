using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.BlockCipher.CipherMode
{
    public class GCM : AbstractBlockCipherMode
    {
        public int TagLength { get; set; }

        public override Encryptor CreateEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock) => new GCMEncryptor(cipher, processBlock, TagLength);

        public override Decryptor CreateDecryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock) => new GCMDecryptor(cipher, processBlock, TagLength);

        private static readonly BinaryPolynomial Polynomial = BinaryPolynomial.FromPowers(127, 7, 2, 1, 0);

        private static byte[] BlockMult(byte[] X, byte[] Y)
        {
            var xp = new BinaryPolynomial(X, ByteOrder.LittleEndian);
            var yp = new BinaryPolynomial(Y, ByteOrder.LittleEndian);
            var mult = BinaryPolynomial.ModularMultiplication(xp, yp, Polynomial);
            return mult.ToBytes(ByteOrder.LittleEndian, 16);
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

        private class GCMEncryptor : Encryptor
        {
            int _tagLen;

            public GCMEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock, int tagLen)
                : base(cipher, processBlock)
            {
                if (cipher.BlockSize != 16)
                    throw new NotSupportedException("Only 128-bit block ciphers are supported");

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
                    byte[] p1 = new byte[IV.Length + s + 8 + 8];
                    Array.Copy(IV, p1, IV.Length);
                    Array.Copy(BitHelper.GetBytes(IV.Length * 8, ByteOrder.BigEndian), 0, p1, p1.Length - 4, 4);
                    J0 = GHash(H, p1);
                }

                byte[] J = new byte[16];
                Array.Copy(J0, J, 16);
                Inc32(J);
                byte[] C = GCTR(J, data, _processBlock);

                int u = (C.Length + 15) / 16 * 16 - C.Length;
                int v = (A.Length + 15) / 16 * 16 - A.Length;

                byte[] p2 = new byte[A.Length + v + C.Length + u + 8 + 8];
                int index = 0;
                Array.Copy(A, 0, p2, index, A.Length);
                index += A.Length;
                index += v;
                Array.Copy(C, 0, p2, index, C.Length);
                index += C.Length;
                index += u;
                Array.Copy(BitHelper.GetBytes(A.Length * 8, ByteOrder.BigEndian), 0, p2, index + 4, 4);
                index += 8;
                Array.Copy(BitHelper.GetBytes(C.Length * 8, ByteOrder.BigEndian), 0, p2, index + 4, 4);
                byte[] S = GHash(H, p2);

                byte[] T = GCTR(J0, S, _processBlock);

                byte[] tag = new byte[_tagLen];
                Array.Copy(T, tag, _tagLen);

                return new AEADCipherData
                {
                    CipherText = C,
                    Tag = tag
                };
            }

            public override byte[] Encrypt(byte[] data)
            {
                throw new NotSupportedException("GCM is AEAD cipher mode");
            }

            public override byte[] Encrypt(byte[] IV, byte[] data)
            {
                throw new NotSupportedException("GCM is AEAD cipher mode");
            }
        }

        private class GCMDecryptor : Decryptor
        {
            int _tagLen;

            public GCMDecryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock, int tagLen)
                : base(cipher, processBlock)
            {
                if (cipher.BlockSize != 16)
                    throw new NotSupportedException("Only 128-bit block ciphers are supported");

                this._tagLen = tagLen;
            }

            public override byte[] Decrypt(byte[] IV, byte[] data, byte[] authenticatedData)
            {
                throw new NotImplementedException();
            }

            public override byte[] Decrypt(byte[] data)
            {
                throw new NotSupportedException("GCM is AEAD cipher mode");
            }

            public override byte[] Decrypt(byte[] IV, byte[] data)
            {
                throw new NotSupportedException("GCM is AEAD cipher mode");
            }
        }
    }
}