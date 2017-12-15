using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.KDF
{
    /// <summary>
    /// HMAC-based Extract-and-Expand Key Derivation Function (HKDF)
    /// <para>https://tools.ietf.org/html/rfc5869</para>
    /// </summary>
    public class HKDF
    {
        private AbstractHash _hash;
        private byte[] _prk;

        public HKDF(AbstractHash hash)
        {
            this._hash = hash;
        }

        public void Init(byte[] salt, byte[] key)
        {
            var hmac = new HMAC(_hash, salt ?? new byte[0]);
            this._prk = hmac.ComputeHash(key);
        }

        /// <summary>
        /// Calculate keying material
        /// </summary>
        /// <param name="length">Length of output keying material (&lt;= 255*HashLen)</param>
        /// <param name="info">Optional context and application specific information</param>
        /// <returns></returns>
        public byte[] Expand(int length, byte[] info = null)
        {
            if (this._prk == null)
                throw new InvalidOperationException("Use Init first");

            int hashLen = _hash.HashSize;
            // N = ceil(L/HashLen)
            int n = (length + hashLen - 1) / hashLen;
            if (n > 255)
                throw new InvalidOperationException();
            int infoLen = info?.Length ?? 0;

            byte[] result = new byte[length];
            byte[] T = new byte[hashLen];
            byte[] data = new byte[hashLen + infoLen + 1];

            var hmac = new HMAC(_hash, _prk);

            for (int i = 0; i < n; i++)
            {
                if (i == 0)
                {
                    if (infoLen > 0)
                        Array.Copy(info, 0, data, 0, infoLen);
                    data[infoLen] = (byte)(i + 1);
                    T = hmac.ComputeHash(data, 0, infoLen + 1);
                }
                else
                {
                    Array.Copy(T, 0, data, 0, hashLen);
                    if (infoLen > 0)
                        Array.Copy(info, 0, data, hashLen, infoLen);
                    data[hashLen + infoLen] = (byte)(i + 1);
                    T = hmac.ComputeHash(data);
                }

                Array.Copy(T, 0, result, i * hashLen, System.Math.Min(length - i * hashLen, hashLen));
            }

            return result;
        }
    }
}