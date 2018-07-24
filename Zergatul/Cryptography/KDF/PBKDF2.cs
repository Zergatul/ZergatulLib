using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.KDF
{
    /// <summary>
    /// PKCS #5: Password-Based Cryptography Specification Version 2.0
    /// <para>https://tools.ietf.org/html/rfc2898#section-5.2</para>
    /// </summary>
    public class PBKDF2
    {
        private AbstractHash _hash;

        public PBKDF2(AbstractHash hash)
        {
            this._hash = hash;
        }

        public PBKDF2(string hashname)
        {
            switch (hashname)
            {
                case "SHA1": _hash = new SHA1(); break;
                case "SHA256": _hash = new SHA256(); break;
                default:
                    throw new NotImplementedException();
            }
        }

        public byte[] DeriveKeyBytes(byte[] password, byte[] salt, int iterations, ulong keyLength)
        {
            uint hLength = checked((uint)_hash.HashSize);
            if (keyLength > uint.MaxValue * (ulong)hLength)
                throw new ArgumentException("Derived key too long");

            ulong l = checked((uint)((keyLength + hLength - 1) / hLength));
            var hmac = new HMAC(_hash, password);
            byte[] buffer = new byte[hLength];
            byte[] keys = new byte[keyLength];

            for (uint i = 0; i < l; i++)
            {
                F(hmac, salt, iterations, i + 1, buffer);
                long copyLen = i < l - 1 ? (long)hLength : (long)(keyLength - i * hLength);
                Array.Copy(buffer, 0, keys, (long)i * hLength, copyLen);
            }

            return keys;
        }

        private void F(HMAC hmac, byte[] S, int c, uint i, byte[] output)
        {
            for (int j = 0; j < output.Length; j++)
                output[j] = 0;

            byte[] U = ByteArray.Concat(S, BitHelper.GetBytes(i, ByteOrder.BigEndian));
            for (int j = 0; j < c; j++)
            {
                U = hmac.ComputeHash(U);
                ByteArray.Xor(output, U);
            }
        }
    }
}