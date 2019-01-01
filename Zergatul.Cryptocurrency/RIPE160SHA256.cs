using Zergatul.Security;

namespace Zergatul.Cryptocurrency
{
    public static class RIPE160SHA256
    {
        public static byte[] Hash(byte[] data)
        {
            using (var sha256 = SecurityProvider.GetMessageDigestInstance(MessageDigests.SHA256))
            using (var ripemd = SecurityProvider.GetMessageDigestInstance(MessageDigests.RIPEMD160))
            {
                return ripemd.Digest(sha256.Digest(data));
            }
        }

        public static byte[] Hash(params byte[][] data)
        {
            using (var sha256 = SecurityProvider.GetMessageDigestInstance(MessageDigests.SHA256))
            using (var ripemd = SecurityProvider.GetMessageDigestInstance(MessageDigests.RIPEMD160))
            {
                foreach (byte[] array in data)
                    sha256.Update(array);
                return ripemd.Digest(sha256.Digest());
            }
        }
    }
}