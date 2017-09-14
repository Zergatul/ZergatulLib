using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Symmetric.CipherMode
{
    public class Poly1305
    {
        public AEADEncryptor CreateEncryptor(AbstractStreamCipher cipher, byte[] key, byte[] poly1305Key)
        {
            return new Poly1305Encryptor(cipher, key, poly1305Key);
        }

        public AEADDecryptor CreateDecryptor(AbstractStreamCipher cipher, byte[] key, byte[] poly1305Key)
        {
            return new Poly1305Decryptor(cipher, key, poly1305Key);
        }

        private static readonly BigInteger P = new BigInteger(new uint[] { 0x3, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFB }, ByteOrder.BigEndian);

        private static void ClampR(byte[] r)
        {
            r[3] &= 15;
            r[7] &= 15;
            r[11] &= 15;
            r[15] &= 15;
            r[4] &= 252;
            r[8] &= 252;
            r[12] &= 252;
        }

        private static void SplitKey(byte[] key, out BigInteger r, out BigInteger s)
        {
            byte[] rb = new byte[16];
            byte[] sb = new byte[16];

            Array.Copy(key, 0, rb, 0, 16);
            Array.Copy(key, 16, sb, 0, 16);

            ClampR(rb);

            r = new BigInteger(rb, ByteOrder.LittleEndian);
            s = new BigInteger(sb, ByteOrder.LittleEndian);
        }

        private class Poly1305Encryptor : AEADEncryptor
        {
            private AbstractStreamCipher _cipher;
            private Poly1305Function _func;
            private byte[] _cipherKey;
            private byte[] _poly1305Key;

            public Poly1305Encryptor(AbstractStreamCipher cipher, byte[] cipherKey, byte[] poly1305Key)
            {
                this._cipher = cipher;
                this._cipherKey = cipherKey;
                this._poly1305Key = poly1305Key;

                this._func = new Poly1305Function();
            }

            public override AEADCipherData Encrypt(byte[] IV, byte[] data, byte[] aad)
            {
                _func.Init(_poly1305Key);
                _func.Write(aad, 0, aad.Length);
                _func.PadZeros();

                var keyStream = _cipher.InitKeyStream(_cipherKey, IV, 1);
                byte[] keyBlock = new byte[16];

                var result = new AEADCipherData();
                result.CipherText = new byte[data.Length];

                int blockCount = (data.Length + 15) / 16;
                for (int i = 0; i < blockCount; i++)
                {
                    int blockLength = System.Math.Min(16, data.Length - i * 16);
                    keyStream.Read(keyBlock, 0, blockLength);
                    for (int b = 0; b < blockLength; b++)
                        result.CipherText[i * 16 + b] = (byte)(data[i * 16 + b] ^ keyBlock[b]);
                }

                _func.Write(result.CipherText, 0, data.Length);
                _func.PadZeros();

                byte[] lengths = new byte[16];
                BitHelper.GetBytes((ulong)aad.Length, ByteOrder.LittleEndian, lengths, 0);
                BitHelper.GetBytes((ulong)data.Length, ByteOrder.LittleEndian, lengths, 8);
                _func.Write(lengths, 0, 16);

                result.Tag = _func.ComputeResult();

                return result;
            }
        }

        private class Poly1305Decryptor : AEADDecryptor
        {
            private AbstractStreamCipher _cipher;
            private Poly1305Function _func;
            private byte[] _cipherKey;
            private byte[] _poly1305Key;

            public Poly1305Decryptor(AbstractStreamCipher cipher, byte[] cipherKey, byte[] poly1305Key)
            {
                this._cipher = cipher;
                this._cipherKey = cipherKey;
                this._poly1305Key = poly1305Key;

                this._func = new Poly1305Function();
            }

            public override byte[] Decrypt(byte[] IV, AEADCipherData data, byte[] aad)
            {
                _func.Init(_poly1305Key);
                _func.Write(aad, 0, aad.Length);
                _func.PadZeros();

                _func.Write(data.CipherText, 0, data.CipherText.Length);
                _func.PadZeros();

                byte[] lengths = new byte[16];
                BitHelper.GetBytes((ulong)aad.Length, ByteOrder.LittleEndian, lengths, 0);
                BitHelper.GetBytes((ulong)data.CipherText.Length, ByteOrder.LittleEndian, lengths, 8);
                _func.Write(lengths, 0, 16);

                byte[] tag = _func.ComputeResult();
                if (!tag.SequenceEqual(data.Tag))
                    throw new AuthenticationException();

                var keyStream = _cipher.InitKeyStream(_cipherKey, IV, 1);
                byte[] keyBlock = new byte[16];

                byte[] result = new byte[data.CipherText.Length];

                int blockCount = (result.Length + 15) / 16;
                for (int i = 0; i < blockCount; i++)
                {
                    int blockLength = System.Math.Min(16, result.Length - i * 16);
                    keyStream.Read(keyBlock, 0, blockLength);
                    for (int b = 0; b < blockLength; b++)
                        result[i * 16 + b] = (byte)(data.CipherText[i * 16 + b] ^ keyBlock[b]);
                }

                return result;
            }
        }
    }
}