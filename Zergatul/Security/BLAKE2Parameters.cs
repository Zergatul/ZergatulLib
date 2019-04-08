namespace Zergatul.Security
{
    public class BLAKE2Parameters : MDParameters
    {
        /// <summary>
        /// Leave null for default hash length. BLAKE2s - 32 bytes, BLAKE2b - 64 bytes
        /// </summary>
        public int? DigestLength;

        public byte[] Key;
    }
}