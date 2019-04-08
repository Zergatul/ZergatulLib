using Zergatul.Security.DotNet;

namespace Zergatul.Security
{
    public class DotNetProvider : SecurityProvider
    {
        public override string Name => ".NET";

        public DotNetProvider()
        {
            
        }

        public override SecureRandom GetSecureRandom() => new DefaultSecureRandom();
    }
}