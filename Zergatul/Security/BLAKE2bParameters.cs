namespace Zergatul.Security
{
    public class BLAKE2bParameters : MDParameters
    {
        public int HashSizeBytes = 64;
        public byte[] Key;
    }
}