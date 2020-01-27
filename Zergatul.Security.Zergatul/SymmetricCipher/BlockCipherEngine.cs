namespace Zergatul.Security.Zergatul.SymmetricCipher
{
    abstract class BlockCipherEngine
    {
        public abstract void ProcessKey(int blockSize, byte[] key);
        public abstract void EncryptBlock(byte[] input, byte[] output);
        public abstract void DecryptBlock(byte[] input, byte[] output);
    }
}