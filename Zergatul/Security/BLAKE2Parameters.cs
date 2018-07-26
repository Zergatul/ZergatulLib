namespace Zergatul.Security
{
    public class BLAKE2Parameters : MDParameters
    {
        /// <summary>
        /// Leave null for default hash size. BLAKE2s - 32 bytes, BLAKE2b - 64 bytes
        /// </summary>
        public int? HashSizeBytes;

        public byte[] Key;
    }
}