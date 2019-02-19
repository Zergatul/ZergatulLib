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

        private class Disposable : IDisposable
        {
            public void Dispose()
            {
                Unregister(0);
            }
        }
    }
}