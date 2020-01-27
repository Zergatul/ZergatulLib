using System;
using Zergatul.Security.Paddings;

namespace Zergatul.Security.OpenSsl.SymmetricCipher
{
    class AES : Security.SymmetricCipher
    {
        #region Private fields

        private IntPtr _ctx;
        private int _keyLen;
        private SymmetricPadding _padding;
        private int _position;
        private bool _enc;
        private bool _hasLastBlock;
        private byte[] _lastBlock = new byte[16];
        private byte[] _buffer = new byte[16];

        #endregion

        #region SymmetricCipher overrides

        public override int BlockSize => 16;

        public override void InitForEncryption(byte[] key, SymmetricCipherParameters parameters)
        {
            Init(key, true, parameters);
        }

        public override void InitForDecryption(byte[] key, SymmetricCipherParameters parameters)
        {
            Init(key, false, parameters);
        }

        public override int Update(byte[] input, int inputLength, byte[] output)
        {
            int outputLength = output.Length;
            if (Native.EVP_CipherUpdate(_ctx, output, ref outputLength, input, inputLength) != 1)
                throw new OpenSslException();

            _position = (_position + inputLength) & 0x0F;

            if (_padding == null)
                return outputLength;

            if (!_enc && outputLength >= 16)
            {
                if (_hasLastBlock)
                    Array.Copy(_lastBlock, 0, _buffer, 0, 16);
                Array.Copy(output, outputLength - 16, _lastBlock, 0, 16);
                outputLength -= 16;
                if (_hasLastBlock)
                {
                    for (int i = outputLength - 1; i >= 0; i--)
                        output[16 + i] = output[i];
                    Array.Copy(_buffer, 0, output, 0, 16);
                    outputLength += 16;
                }

                _hasLastBlock = true;
            }

            return outputLength;
        }

        public override int DoFinal(byte[] output)
        {
            if (_padding == null)
                return 0;

            if (output == null || output.Length < 16)
                throw new InvalidOperationException();

            if (_enc)
            {
                byte[] pad = _padding.GetPadding(_position, 16);
                int outputLength = output.Length;
                if (Native.EVP_CipherUpdate(_ctx, output, ref outputLength, pad, pad.Length) != 1)
                    throw new OpenSslException();
                return outputLength;
            }
            else
            {
                if (_position != 0)
                    throw new InvalidOperationException();
                if (!_hasLastBlock)
                    return 0;
                int index = _padding.RemovePadding(_lastBlock, 0, 16);
                if (index != 0)
                    Array.Copy(_lastBlock, 0, output, 0, index);
                return index;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_ctx != IntPtr.Zero)
            {
                Native.EVP_CIPHER_CTX_free(_ctx);
                _ctx = IntPtr.Zero;
            }
        }

        #endregion

        #region Private methods

        private void Init(byte[] key, bool enc, SymmetricCipherParameters parameters)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            _position = 0;
            _enc = enc;
            _keyLen = key.Length;
            switch (_keyLen)
            {
                case 16:
                case 24:
                case 32:
                    break;
                default:
                    throw new InvalidOperationException("Invalid key size");
            }

            _ctx = Native.EVP_CIPHER_CTX_new();
            if (_ctx == IntPtr.Zero)
                throw new OpenSslException();

            IntPtr type;
            switch (_keyLen)
            {
                case 16:
                    switch (parameters.Mode)
                    {
                        case BlockCipherMode.ECB: type = Native.EVP_aes_128_ecb(); break;
                        case BlockCipherMode.CBC: type = Native.EVP_aes_128_cbc(); break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case 24:
                    switch (parameters.Mode)
                    {
                        case BlockCipherMode.ECB: type = Native.EVP_aes_192_ecb(); break;
                        case BlockCipherMode.CBC: type = Native.EVP_aes_192_cbc(); break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case 32:
                    switch (parameters.Mode)
                    {
                        case BlockCipherMode.ECB: type = Native.EVP_aes_256_ecb(); break;
                        case BlockCipherMode.CBC: type = Native.EVP_aes_256_cbc(); break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }

            if (Native.EVP_CipherInit_ex(_ctx, type, IntPtr.Zero, null, null, enc ? 1 : 0) != 1)
                throw new OpenSslException();

            int ivLength = Native.EVP_CIPHER_CTX_iv_length(_ctx);
            int keyLength = Native.EVP_CIPHER_CTX_key_length(_ctx);

            if (key.Length != key.Length)
                throw new InvalidOperationException();

            if (ivLength > 0)
            {
                if (parameters.IV == null)
                {
                    if (parameters.Random == null)
                        throw new InvalidOperationException("Please fill IV or Random property");

                    parameters.IV = new byte[ivLength];
                    parameters.Random.GetNextBytes(parameters.IV);
                }
                else
                {
                    if (parameters.IV.Length != ivLength)
                        throw new InvalidOperationException();
                }
            }

            if (Native.EVP_CipherInit_ex(_ctx, IntPtr.Zero, IntPtr.Zero, key, parameters.IV, enc ? 1 : 0) != 1)
                throw new OpenSslException();

            if (Native.EVP_CIPHER_CTX_set_padding(_ctx, 0) != 1)
                throw new OpenSslException();

            switch (parameters.Padding)
            {
                case Padding.NoPadding: _padding = null; break;
                case Padding.PKCS7: _padding = new PKCS7Padding(); break;
                default:
                    throw new NotImplementedException();
            }
        }

        ~AES()
        {
            Dispose(false);
        }

        #endregion
    }
}