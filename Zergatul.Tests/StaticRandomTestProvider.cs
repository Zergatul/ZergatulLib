using System;
using Zergatul.Security;

namespace Zergatul.Tests
{
    public class StaticRandomTestProvider : SecurityProvider
    {
        public override string Name => "StaticRandomTest";

        private byte[] _data;

        public StaticRandomTestProvider(byte[] data)
        {
            _data = data;
        }

        public override SecureRandom GetSecureRandom() => new StaticSecureRandom(_data);

        public static IDisposable Use(string hex) => Use(BitHelper.HexToBytes(hex));

        public static IDisposable Use(byte[] data)
        {
            Register(new StaticRandomTestProvider(data), 0);
            return new Disposable();
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