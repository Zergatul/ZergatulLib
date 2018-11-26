using Zergatul.Security;

namespace Zergatul.Cryptocurrency
{
    public static class DoubleSHA256
    {
        public static byte[] Hash(byte[] data)
        {
            using (var sha1 = Provider.GetMessageDigestInstance(MessageDigests.SHA256))
            using (var sha2 = Provider.GetMessageDigestInstance(MessageDigests.SHA256))
            {
                return sha2.Digest(sha1.Digest(data));
            }
        }

        public static byte[] Hash(byte[] data, int offset, int length)
        {
            using (var sha1 = Provider.GetMessageDigestInstance(MessageDigests.SHA256))
            using (var sha2 = Provider.GetMessageDigestInstance(MessageDigests.SHA256))
            {
                return sha2.Digest(sha1.Digest(data, offset, length));
            }
        }

        public static byte[] Hash(params byte[][] data)
        {
            using (var sha1 = Provider.GetMessageDigestInstance(MessageDigests.SHA256))
            using (var sha2 = Provider.GetMessageDigestInstance(MessageDigests.SHA256))
            {
                foreach (byte[] array in data)
                    sha1.Update(array);
                return sha2.Digest(sha1.Digest());
            }
        }
    }
}