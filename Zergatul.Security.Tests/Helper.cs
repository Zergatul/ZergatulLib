using System;

namespace Zergatul.Security.Tests
{
    static class Helper
    {
        public static void ForEachProvider(Provider[] providers, Action<Provider> action)
        {
            foreach (var provider in providers)
                action(provider);
        }
    }
}