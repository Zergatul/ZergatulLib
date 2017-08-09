using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher.CipherMode
{
    // https://tools.ietf.org/html/rfc3610
    public class CCM : AbstractAEADCipherMode
    {
        public int OctetLength { get; set; } = 2;

        protected override AEADEncryptor CreateEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
        {
            ValidateTagLength();
            ValidateOctetLength();
            return new CCMEncryptor(cipher, processBlock, TagLength, OctetLength);
        }

        protected override AEADDecryptor CreateDecryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
        {
            ValidateTagLength();
            ValidateOctetLength();
            return new CCMDecryptor(cipher, processBlock, TagLength, OctetLength);
        }

        private void ValidateTagLength()
        {
            if (TagLength < 4 || 16 < TagLength)
                throw new InvalidOperationException("TagLength must be positive and less or equal then 16");
            if (TagLength % 2 != 0)
                throw new InvalidOperationException("Invalid TagLength. Valid values: 4, 6, 8, 10, 12, 14, 16");
        }

        private void ValidateOctetLength()
        {
            if (OctetLength < 2 || 8 < OctetLength)
                throw new InvalidOperationException("Invalid OctetLength. Valid range: 2-8");
        }

        private static byte[] CalculateAuthField(int M, int L, byte[] nonce, byte[] P, byte[] A, Func<byte[], byte[]> E)
        {
            /*
                Octet Number   Contents
                ------------   ---------
                0              Flags
                1 ... 15-L     Nonce N
                16-L ... 15    l(m)
            */
            byte[] B = new byte[16];

            /*
                Bit Number   Contents
                ----------   ----------------------
                7            Reserved (always zero)
                6            Adata
                5 ... 3      M'
                2 ... 0      L'
            */
            B[0] = (byte)((L - 1) | (((M - 2) / 2) << 3) | (A.Length > 0 ? 64 : 0));

            Array.Copy(nonce, 0, B, 1, 15 - L);
            byte[] mLenBytes = BitHelper.GetBytes(P.LongLength, ByteOrder.BigEndian);
            Array.Copy(mLenBytes, 8 - L, B, 16 - L, L);

            byte[] X = E(B);

            int aceil = (L + A.Length + 15) / 16 * 16;
            int pceil = (P.Length + 15) / 16 * 16;
            B = new byte[aceil + pceil];

            // The blocks encoding a are formed by concatenating this string that
            // encodes l(a) with a itself, and splitting the result into 16 - octet
            // blocks, and then padding the last block with zeroes if necessary.
            byte[] aLenBytes = BitHelper.GetBytes(A.LongLength, ByteOrder.BigEndian);
            Array.Copy(aLenBytes, 8 - L, B, 0, L);
            Array.Copy(A, 0, B, L, A.Length);
            Array.Copy(P, 0, B, aceil, P.Length);

            byte[] xored = new byte[16];
            for (int i = 0; i < B.Length / 16; i++)
            {
                for (int b = 0; b < 16; b++)
                    xored[b] = (byte)(X[b] ^ B[i * 16 + b]);
                X = E(xored);
            }

            return X;
        }

        private static byte[] GetInitialCounter(byte[] nonce, int L)
        {
            /*
                    Octet Number   Contents
                    ------------   ---------
                    0              Flags
                    1 ... 15-L     Nonce N
                    16-L ... 15    Counter i
                */
            byte[] A = new byte[16];
            /*
                Bit Number   Contents
                ----------   ----------------------
                7            Reserved (always zero)
                6            Reserved (always zero)
                5 ... 3      Zero
                2 ... 0      L'
            */
            A[0] = (byte)(L - 1);
            Array.Copy(nonce, 0, A, 1, 15 - L);

            return A;
        }

        private class CCMEncryptor : AEADEncryptor
        {
            int M, L;
            private Func<byte[], byte[]> _processBlock;

            public CCMEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock, int tagLen, int octLen)
            {
                if (cipher.BlockSize != 16)
                    throw new NotSupportedException("Only 128-bit block ciphers are supported");

                this._processBlock = processBlock;
                this.M = tagLen;
                this.L = octLen;
            }

            public override AEADCipherData Encrypt(byte[] IV, byte[] data, byte[] authenticatedData)
            {
                if (IV == null)
                    throw new ArgumentNullException(nameof(IV));
                if (data == null)
                    throw new ArgumentNullException(nameof(data));
                if (authenticatedData == null)
                    throw new ArgumentNullException(nameof(authenticatedData));

                if (IV.Length != 15 - L)
                    throw new ArgumentException($"Nonce should be {15 - L} length", nameof(IV));

                byte[] T = CalculateAuthField(M, L, IV, data, authenticatedData, _processBlock);

                byte[] A = GetInitialCounter(IV, L);
                long counter = 0;

                byte[] S0 = _processBlock(A);
                byte[] U = new byte[M];
                for (int i = 0; i < M; i++)
                    U[i] = (byte)(T[i] ^ S0[i]);

                byte[] C = new byte[data.Length];
                int blockLength = (C.Length + 15) / 16;
                for (int i = 0; i < blockLength; i++)
                {
                    counter++;
                    Array.Copy(BitHelper.GetBytes(counter, ByteOrder.BigEndian), 8 - L, A, 16 - L, L);
                    byte[] S = _processBlock(A);
                    int last = System.Math.Min(16, C.Length - i * 16);
                    for (int b = 0; b < last; b++)
                        C[i * 16 + b] = (byte)(S[b] ^ data[i * 16 + b]);
                }

                return new AEADCipherData
                {
                    CipherText = C,
                    Tag = U
                };
            }
        }

        private class CCMDecryptor : AEADDecryptor
        {
            int M, L;
            private Func<byte[], byte[]> _processBlock;

            public CCMDecryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock, int tagLen, int octLen)
            {
                if (cipher.BlockSize != 16)
                    throw new NotSupportedException("Only 128-bit block ciphers are supported");

                this._processBlock = processBlock;
                this.M = tagLen;
                this.L = octLen;
            }

            public override byte[] Decrypt(byte[] IV, AEADCipherData data, byte[] authenticatedData)
            {
                if (IV == null)
                    throw new ArgumentNullException(nameof(IV));
                if (data == null)
                    throw new ArgumentNullException(nameof(data));
                if (authenticatedData == null)
                    throw new ArgumentNullException(nameof(authenticatedData));

                if (IV.Length != 15 - L)
                    throw new ArgumentException($"Nonce should be {15 - L} length", nameof(IV));
                if (data.CipherText == null)
                    throw new ArgumentException("data.CipherText is null", nameof(data));
                if (data.Tag == null)
                    throw new ArgumentException("data.CipherText is null", nameof(data));
                if (data.Tag.Length != M)
                    throw new ArgumentException($"Tag should be {M} length", nameof(data));

                byte[] A = GetInitialCounter(IV, L);
                long counter = 0;

                byte[] S0 = _processBlock(A);

                byte[] P = new byte[data.CipherText.Length];
                int blockLength = (P.Length + 15) / 16;
                for (int i = 0; i < blockLength; i++)
                {
                    counter++;
                    Array.Copy(BitHelper.GetBytes(counter, ByteOrder.BigEndian), 8 - L, A, 16 - L, L);
                    byte[] S = _processBlock(A);
                    int last = System.Math.Min(16, P.Length - i * 16);
                    for (int b = 0; b < last; b++)
                        P[i * 16 + b] = (byte)(S[b] ^ data.CipherText[i * 16 + b]);
                }

                byte[] T = CalculateAuthField(M, L, IV, P, authenticatedData, _processBlock);
                for (int i = 0; i < M; i++)
                    if ((T[i] ^ S0[i]) != data.Tag[i])
                        throw new AuthenticationException();

                return P;
            }
        }
    }
}