using System;
using static Zergatul.BitHelper;
using static Zergatul.Security.Zergatul.Common.AESHelper;

namespace Zergatul.Security.Zergatul.SymmetricCipher
{
    class RijndaelEngine : BlockCipherEngine
    {
        private uint[] _roundKey;
        private int _rounds;

        public override void ProcessKey(int blockSize, byte[] key)
        {
            if (blockSize != 16)
                throw new BlockCipherException("Invalid block size", ErrorCodes.BlockCipherInvalidBlockSize);

            int keySize = key.Length;
            if (keySize != 16 && keySize != 24 && keySize != 32)
                throw new BlockCipherException("Invalid key size", ErrorCodes.BlockCipherInvalidKeySize);

            _roundKey = new uint[16];
            _roundKey[0] = ToUInt32(key, 0x00, ByteOrder.LittleEndian);
            _roundKey[1] = ToUInt32(key, 0x04, ByteOrder.LittleEndian);
            _roundKey[2] = ToUInt32(key, 0x08, ByteOrder.LittleEndian);
            _roundKey[3] = ToUInt32(key, 0x0C, ByteOrder.LittleEndian);

            if (keySize == 16)
            {
                for (int i = 0, index = 0; i < 10; i++, index += 4)
                {
                    uint temp = _roundKey[index + 3];
                    _roundKey[index + 4] =
                        _roundKey[index] ^
                        TE4[(temp >> 0x08) & 0xFF] ^
                        (uint)(TE4[(temp >> 0x10) & 0xFF] << 0x08) ^
                        (uint)(TE4[(temp >> 0x18)] << 0x10) ^
                        (uint)(TE4[(temp) & 0xFF] << 0x18) ^
                        RCon[i];
                    _roundKey[index + 5] = _roundKey[index + 1] ^ _roundKey[index + 4];
                    _roundKey[index + 6] = _roundKey[index + 2] ^ _roundKey[index + 5];
                    _roundKey[index + 7] = _roundKey[index + 3] ^ _roundKey[index + 6];
                }

                return;
            }

            throw new NotImplementedException();
        }

        public override void EncryptBlock(byte[] input, byte[] output)
        {
            throw new NotImplementedException();
        }

        public override void DecryptBlock(byte[] input, byte[] output)
        {
            throw new NotImplementedException();
        }
    }
}