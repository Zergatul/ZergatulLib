using Zergatul.Security.DotNet;

namespace Zergatul.Security
{
    public class DotNetProvider : SecurityProvider
    {
        public override string Name => ".NET";

        public DotNetProvider()
        {
            RegisterSecureRandom(SecureRandoms.Default, () => new DefaultSecureRandom());
        }
    }
}