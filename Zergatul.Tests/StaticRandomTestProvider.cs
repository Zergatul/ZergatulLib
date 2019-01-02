using System;
using Zergatul.Security;

namespace Zergatul.Tests
{
    public class StaticRandomTestProvider : SecurityProvider
    {
        public override string Name => "StaticRandomTest";

        public StaticRandomTestProvider(byte[] data)
        {
            RegisterSecureRandom(SecureRandoms.Default, () => new StaticSecureRandom(data));
        }

        public static IDisposable Use(string hex) => Use(BitHelper.HexToBytes(hex));

        public static IDisposable Use(byte[] data)
        {
            Register(new StaticRandomTestProvider(data), 0);
            return new Disposable();
        }

        private class StaticSecureRandom : SecureRandom
        {
            private byte[] _data;
            private int _index;

            public StaticSecureRandom(byte[] data)
            {
                this._data = data;
                this._index = 0;
            }

            public override void GetNextBytes(byte[] bytes)
            {
                if (_data.Length - _index < bytes.Length)
                    throw new InvalidOperationException();

                Array.Copy(_data, _index, bytes, 0, bytes.Length);
                _index += bytes.Length;
            }

            public override void SetSeed(byte[] seed)
            {
                throw new NotImplementedException();
            }
        }

        private class Disposable : IDisposable
        {
            public void Dispose()
            {
                Unregister(0);
            }
        }
    }
}