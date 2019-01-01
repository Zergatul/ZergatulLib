using System;

namespace Zergatul.Security.Tests
{
    static class Helper
    {
        public static void ForEachProvider(SecurityProvider[] providers, Action<SecurityProvider> action)
        {
            foreach (var provider in providers)
                action(provider);
        }
    }
}