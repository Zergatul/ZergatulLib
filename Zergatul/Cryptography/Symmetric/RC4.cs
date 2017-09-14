using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric
{
    // Test Vectors for the Stream Cipher RC4
    // https://tools.ietf.org/html/rfc6229
    public class RC4 : AbstractStreamCipher
    {
        public override int BlockSize => 1;
        public override int KeySize => _keySize;
        public override int NonceSize => 0;

        private int _keySize;

        public RC4(int keySize)
        {
            this._keySize = keySize;
        }

        private static byte[] KeySchedule(byte[] key)
        {
            byte[] S = new byte[256];
            for (int i = 0; i < 256; i++)
                S[i] = (byte)i;

            int j = 0;
            for (int i = 0; i < 256; i++)
            {
                j = (j + S[i] + key[i % key.Length]) & 0xFF;

                byte buf = S[i];
                S[i] = S[j];
                S[j] = buf;
            }

            return S;
        }

        public override KeyStream InitKeyStream(byte[] key, byte[] nonce, uint counter)
        {
            byte[] S = KeySchedule(key);
            int i = 0;
            int j = 0;
            return new KeyStream(() =>
            {
                i = (i + 1) & 0xFF;
                j = (j + S[i]) & 0xFF;

                byte buf = S[i];
                S[i] = S[j];
                S[j] = buf;

                return new byte[1] { S[(S[i] + S[j]) & 0xFF] };
            });
        }
    }
}