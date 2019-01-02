namespace Zergatul.Security
{
    public class SymmetricCipherParameters
    {
        public BlockCipherMode Mode { get; set; }
        public Padding Padding { get; set; }
        public SecureRandom Random { get; set; }
        public byte[] IV { get; set; }
    }
}