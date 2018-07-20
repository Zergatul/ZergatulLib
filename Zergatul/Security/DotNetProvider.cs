using Zergatul.Security.DotNet;

namespace Zergatul.Security
{
    public class DotNetProvider : Provider
    {
        public override string Name => ".NET";

        public DotNetProvider()
        {
            RegisterSecureRandom(SecureRandoms.Default, () => new DefaultSecureRandom());
        }
    }
}