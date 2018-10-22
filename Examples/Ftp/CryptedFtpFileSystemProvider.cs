using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.IO;
using Zergatul.Network.Ftp;
using Zergatul.Security;

namespace Zergatul.Examples.Ftp
{
    class Cryptor
    {
        private const int BufferSize = 0x100000;

        public static void EncryptFile(string file, byte[] key)
        {
            byte[] rBuffer = new byte[BufferSize];
            byte[] wBuffer = new byte[BufferSize];

            var aes = SymmetricCipher.GetInstance(SymmetricCiphers.AES);
            var parameters = new SymmetricCipherParameters
            {
                Mode = BlockCipherMode.CBC,
                Padding = Padding.PKCS7,
                Random = SecureRandom.GetInstance(SecureRandoms.Default)
            };
            aes.InitForEncryption(key, parameters);

            using (var source = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (var dest = new FileStream(file + ".crypt", FileMode.Create, FileAccess.Write))
            {
                dest.Write(parameters.IV, 0, parameters.IV.Length);

                while (true)
                {
                    int read = source.Read(rBuffer, 0, rBuffer.Length);
                    if (read == 0)
                    {
                        byte[] last = new byte[16];
                        int count = aes.DoFinal(last);
                        dest.Write(last, 0, count);
                        break;
                    }

                    int write = aes.Update(rBuffer, read, wBuffer);

                    dest.Write(wBuffer, 0, write);
                }
            }
        }

        public static void DecryptFile(string file, string newfile, byte[] key)
        {
            byte[] rBuffer = new byte[BufferSize];
            byte[] wBuffer = new byte[BufferSize];

            var aes = SymmetricCipher.GetInstance(SymmetricCiphers.AES);
            var parameters = new SymmetricCipherParameters
            {
                Mode = BlockCipherMode.CBC,
                Padding = Padding.PKCS7,
                IV = new byte[16]
            };

            using (var source = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (var dest = new FileStream(newfile, FileMode.Create, FileAccess.Write))
            {
                StreamHelper.ReadArray(source, parameters.IV);
                aes.InitForDecryption(key, parameters);

                while (true)
                {
                    int read = source.Read(rBuffer, 0, rBuffer.Length);
                    if (read == 0)
                    {
                        byte[] last = new byte[16];
                        int count = aes.DoFinal(last);
                        dest.Write(last, 0, count);
                        break;
                    }

                    int write = aes.Update(rBuffer, read, wBuffer);

                    dest.Write(wBuffer, 0, write);
                }
            }
        }
    }

    class CryptedFtpFileSystemProvider : IFtpFileSystemProvider
    {
        private readonly byte[] _key;

        public CryptedFtpFileSystemProvider(byte[] key)
        {
            _key = key;
        }

        public string GetCurrentDirectory()
        {
            return ".";
        }

        public IFtpFile GetFile(string filename)
        {
            return new CryptedFtpFile(@"", _key);
        }
    }

    class CryptedFtpFile : IFtpFile
    {
        private readonly string _path;
        private readonly byte[] _key;

        public CryptedFtpFile(string path, byte[] key)
        {
            _path = path;
            _key = key;
        }

        public DateTime GetModifiedDate()
        {
            return new DateTime(2018, 1, 1);
        }

        public long GetSize()
        {
            using (var aes = SymmetricCipher.GetInstance(SymmetricCiphers.AES))
            {
                var parameters = new SymmetricCipherParameters
                {
                    Mode = BlockCipherMode.CBC,
                    Padding = Padding.PKCS7,
                    IV = new byte[16]
                };

                using (var fs = new FileStream(_path, FileMode.Open, FileAccess.Read))
                {
                    if (fs.Length % 16 != 0)
                        throw new InvalidOperationException();

                    fs.Position = fs.Length - 32;

                    StreamHelper.ReadArray(fs, parameters.IV);
                    aes.InitForDecryption(_key, parameters);

                    byte[] buffer = new byte[16];
                    StreamHelper.ReadArray(fs, buffer);

                    byte[] output = new byte[16];
                    if (aes.Update(buffer, 16, output) != 0)
                        throw new InvalidOperationException();

                    int len = aes.DoFinal(output);

                    return fs.Length - 32 + len;
                }
            }
        }

        public Stream GetStream()
        {
            return new CryptedFileStream(_path, _key);
        }
    }

    class CryptedFileStream : Stream
    {
        private FileStream _stream;
        private byte[] _key;
        private bool _initialized;
        private long _position;
        private SymmetricCipher _aes;
        private byte[] _cBuffer = new byte[0x100000];
        private byte[] _pBuffer = new byte[0x100000 + 16];
        private int _pBufPos;
        private int _pBufLen;
        private bool _end;

        public CryptedFileStream(string path, byte[] key)
        {
            _stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            _key = key;
            _initialized = false;
            _position = 0;
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;

        public override long Length => throw new InvalidOperationException();

        public override long Position
        {
            get => _position;
            set
            {
                _position = value;
                _initialized = false;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return 0;

            if (!_initialized)
            {
                _aes?.Dispose();

                _aes = SymmetricCipher.GetInstance(SymmetricCiphers.AES);
                var parameters = new SymmetricCipherParameters
                {
                    Mode = BlockCipherMode.CBC,
                    Padding = Padding.PKCS7,
                    IV = new byte[16]
                };

                _stream.Position = _position - _position % 16;
                StreamHelper.ReadArray(_stream, parameters.IV);

                _aes.InitForDecryption(_key, parameters);

                FillPlainBuffer();

                _initialized = true;
            }

            if (!_end && _pBufPos >= _pBufLen)
                FillPlainBuffer();

            int read = System.Math.Min(_pBufLen - _pBufPos, count);
            Array.Copy(_pBuffer, _pBufPos, buffer, offset, read);
            _position += read;

            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void Flush() => throw new InvalidOperationException();
        public override void SetLength(long value) => throw new InvalidOperationException();
        public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _aes?.Dispose();
                _stream.Dispose();
            }
        }

        private void FillPlainBuffer()
        {
            int read = _stream.Read(_cBuffer, 0, _cBuffer.Length);
            _pBufLen = _aes.Update(_cBuffer, read, _pBuffer);
            _pBufPos = (int)(_position % 16);

            if (_stream.Position == _stream.Length)
            {
                byte[] last = new byte[16];
                int len = _aes.DoFinal(last);
                Array.Copy(last, 0, _pBuffer, _pBufLen, len);
                _pBufLen += len;

                _end = true;
            }
        }
    }
}