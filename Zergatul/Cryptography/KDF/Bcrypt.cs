using System;

namespace Zergatul.Cryptography.KDF
{
    public class Bcrypt
    {
        public byte[] DeriveKeyBytes(int cost, byte[] salt, byte[] password)
        {
            if (cost < 4 || cost > 31)
                throw new ArgumentOutOfRangeException(nameof(cost), "Cost must be 4..31");
            if (salt == null)
                throw new ArgumentNullException(nameof(salt));
            if (salt.Length != 16)
                throw new ArgumentException("salt must be 16 bytes");
            if (password == null)
                throw new ArgumentNullException(nameof(password));
            if (password.Length == 0 || password.Length > 72)
                throw new ArgumentException("password must be 1..72 bytes length");

            throw new NotImplementedException();
        }

        private void ExpensiveKeySetup(int cost, byte[] salt, byte[] password)
        {

        }
    }
}