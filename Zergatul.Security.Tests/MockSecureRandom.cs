using System;

namespace Zergatul.Security.Tests
{
    public static class MockSecureRandom
    {
        public static IDisposable Use(byte[] data)
        {
            return new Disposable(data);
        }

        private class Disposable : IDisposable
        {
            public Disposable(byte[] data)
            {
                SecurityProvider.Register(new MockSecureRandomProvider(data), 0);
            }

            public void Dispose()
            {
                SecurityProvider.Unregister(0);
            }
        }

        private class SecureRandomMock : SecureRandom
        {
            private byte[] _data;
            private int _index;

            public SecureRandomMock(byte[] data)
            {
                _data = data;
                _index = 0;
            }

            public override void GetNextBytes(byte[] bytes)
            {
                lock (this)
                {
                    if (_index + bytes.Length > _data.Length)
                        throw new InvalidOperationException();
                    Array.Copy(_data, _index, bytes, 0, bytes.Length);
                    _index += bytes.Length;
                }
            }

            public override void GetNextBytes(byte[] bytes, int offset, int count)
            {
                lock (this)
                {
                    if (_index + count > _data.Length)
                        throw new InvalidOperationException();
                    Array.Copy(_data, _index, bytes, offset, count);
                    _index += count;
                }
            }

            public override void SetSeed(byte[] seed)
            {
                throw new NotImplementedException();
            }
        }

        private class MockSecureRandomProvider : SecurityProvider
        {
            public override string Name => nameof(MockSecureRandomProvider);

            public MockSecureRandomProvider(byte[] data)
            {
                var random = new SecureRandomMock(data);
                RegisterSecureRandom(SecureRandoms.Default, () => random);
            }
        }
    }
}