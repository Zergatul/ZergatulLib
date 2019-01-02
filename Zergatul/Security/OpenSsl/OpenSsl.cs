using System;
using System.Runtime.InteropServices;

namespace Zergatul.Security.OpenSsl
{
    internal static class OpenSsl
    {
        private const string libcrypto = "libcrypto";
        private const string libssl = "libssl";

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CRYPTO_malloc(int num, string file, int line);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CRYPTO_free(IntPtr p);

        #region ERR

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ERR_get_error();

        [DllImport(libcrypto, EntryPoint = nameof(ERR_error_string), CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr _ERR_error_string(ulong e, byte[] buf);
        public static string ERR_error_string(ulong e)
        {
            IntPtr ptr = _ERR_error_string(e, null);
            return Marshal.PtrToStringAnsi(ptr);
        }

        #endregion

        #region MD4

        [StructLayout(LayoutKind.Sequential)]
        public struct MD4_CTX
        {
            public uint A, B, C, D;
            public uint Nl, Nh;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public uint[] data;

            public uint num;
        }

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void MD4_Init(IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void MD4_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void MD4_Final(byte[] md, IntPtr ctx);

        #endregion

        #region MD5

        [StructLayout(LayoutKind.Sequential)]
        public struct MD5_CTX
        {
            public uint A, B, C, D;
            public uint Nl, Nh;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public uint[] data;

            public uint num;
        }

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void MD5_Init(IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void MD5_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void MD5_Final(byte[] md, IntPtr ctx);

        #endregion

        #region SHA1

        [StructLayout(LayoutKind.Sequential)]
        public struct SHA_CTX
        {
            public uint h0, h1, h2, h3, h4;
            public uint Nl, Nh;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public uint[] data;

            public uint num;
        }

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA1_Init(IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA1_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA1_Final(byte[] md, IntPtr ctx);

        #endregion

        #region SHA256

        [StructLayout(LayoutKind.Sequential)]
        public struct SHA256_CTX
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] h;
            public uint Nl, Nh;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public uint[] data;

            public uint num, md_len;
        }

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA224_Init(IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA224_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA224_Final(byte[] md, IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA256_Init(IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA256_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA256_Final(byte[] md, IntPtr ctx);

        #endregion

        #region SHA512

        [StructLayout(LayoutKind.Sequential)]
        public struct SHA512_CTX
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public ulong[] h;
            public ulong Nl, Nh;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public ulong[] data;

            public uint num, md_len;
        }

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA384_Init(IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA384_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA384_Final(byte[] md, IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA512_Init(IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA512_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SHA512_Final(byte[] md, IntPtr ctx);

        #endregion

        #region RIPEMD160

        [StructLayout(LayoutKind.Sequential)]
        public struct RIPEMD160_CTX
        {
            public uint A, B, C, D, E;
            public uint Nl, Nh;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public uint[] data;

            public uint num;
        }

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RIPEMD160_Init(IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RIPEMD160_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RIPEMD160_Final(byte[] md, IntPtr ctx);

        #endregion

        #region EVP

        public const int EVP_PKEY_SCRYPT = 973;

        public const int EVP_PKEY_OP_DERIVE = 1 << 10;

        public const int EVP_PKEY_ALG_CTRL = 0x1000;

        public const int EVP_PKEY_CTRL_TLS_MD = EVP_PKEY_ALG_CTRL;
        public const int EVP_PKEY_CTRL_TLS_SECRET = EVP_PKEY_ALG_CTRL + 1;
        public const int EVP_PKEY_CTRL_TLS_SEED = EVP_PKEY_ALG_CTRL + 2;
        public const int EVP_PKEY_CTRL_HKDF_MD = EVP_PKEY_ALG_CTRL + 3;
        public const int EVP_PKEY_CTRL_HKDF_SALT = EVP_PKEY_ALG_CTRL + 4;
        public const int EVP_PKEY_CTRL_HKDF_KEY = EVP_PKEY_ALG_CTRL + 5;
        public const int EVP_PKEY_CTRL_HKDF_INFO = EVP_PKEY_ALG_CTRL + 6;
        public const int EVP_PKEY_CTRL_HKDF_MODE = EVP_PKEY_ALG_CTRL + 7;
        public const int EVP_PKEY_CTRL_PASS = EVP_PKEY_ALG_CTRL + 8;
        public const int EVP_PKEY_CTRL_SCRYPT_SALT = EVP_PKEY_ALG_CTRL + 9;
        public const int EVP_PKEY_CTRL_SCRYPT_N = EVP_PKEY_ALG_CTRL + 10;
        public const int EVP_PKEY_CTRL_SCRYPT_R = EVP_PKEY_ALG_CTRL + 11;
        public const int EVP_PKEY_CTRL_SCRYPT_P = EVP_PKEY_ALG_CTRL + 12;
        public const int EVP_PKEY_CTRL_SCRYPT_MAXMEM_BYTES = EVP_PKEY_ALG_CTRL + 13;

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_CIPHER_CTX_new();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void EVP_CIPHER_CTX_free(IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_CipherInit_ex(IntPtr ctx, IntPtr type, IntPtr impl, byte[] key, byte[] iv, int enc);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_CIPHER_CTX_set_padding(IntPtr ctx, int padding);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_CIPHER_CTX_iv_length(IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_CIPHER_CTX_key_length(IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_CipherUpdate(IntPtr ctx, byte[] @out, ref int outl, byte[] @in, int inl);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_CipherFinal_ex(IntPtr ctx, byte[] @out, ref int outl);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_aes_128_ecb();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_aes_128_cbc();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_aes_192_ecb();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_aes_192_cbc();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_aes_256_ecb();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_aes_256_cbc();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_PKEY_CTX_new_id(int id, IntPtr e);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_PKEY_CTX_ctrl(IntPtr ctx, int keytype, int optype, int cmd, int p1, byte[] p2);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_PKEY_CTX_ctrl_uint64(IntPtr ctx, int keytype, int optype, int cmd, ulong value);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_PKEY_derive(IntPtr ctx, byte[] key, ref int keylen);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_PBE_scrypt(byte[] pass, int passlen, byte[] salt, int saltlen, ulong N, ulong r, ulong p, ulong maxmem, byte[] key, int keylen);

        public static int EVP_PKEY_CTX_set1_pbe_pass(IntPtr ctx, byte[] pass, int passlen)
        {
            return EVP_PKEY_CTX_ctrl(ctx, -1, EVP_PKEY_OP_DERIVE, EVP_PKEY_CTRL_PASS, passlen, pass);
        }

        public static int EVP_PKEY_CTX_set1_scrypt_salt(IntPtr ctx, byte[] salt, int saltlen)
        {
            return EVP_PKEY_CTX_ctrl(ctx, -1, EVP_PKEY_OP_DERIVE, EVP_PKEY_CTRL_SCRYPT_SALT, saltlen, salt);
        }

        public static int EVP_PKEY_CTX_set_scrypt_N(IntPtr ctx, ulong n)
        {
            return EVP_PKEY_CTX_ctrl_uint64(ctx, -1, EVP_PKEY_OP_DERIVE, EVP_PKEY_CTRL_SCRYPT_N, n);
        }

        public static int EVP_PKEY_CTX_set_scrypt_r(IntPtr ctx, ulong r)
        {
            return EVP_PKEY_CTX_ctrl_uint64(ctx, -1, EVP_PKEY_OP_DERIVE, EVP_PKEY_CTRL_SCRYPT_R, r);
        }

        public static int EVP_PKEY_CTX_set_scrypt_p(IntPtr ctx, ulong p)
        {
            return EVP_PKEY_CTX_ctrl_uint64(ctx, -1, EVP_PKEY_OP_DERIVE, EVP_PKEY_CTRL_SCRYPT_P, p);
        }

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_sha1();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_sha224();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_sha256();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_sha384();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_sha512();

        public static IntPtr EVPByName(string algorithm)
        {
            switch (algorithm)
            {
                case MessageDigests.SHA1: return EVP_sha1();
                case MessageDigests.SHA224: return EVP_sha224();
                case MessageDigests.SHA256: return EVP_sha256();
                case MessageDigests.SHA384: return EVP_sha384();
                case MessageDigests.SHA512: return EVP_sha512();
                default:
                    throw new NotSupportedException();

            }
        }

        #endregion

        #region BN_CTX

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BN_CTX_new();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BN_CTX_free(IntPtr c);

        #endregion

        #region BIGNUM

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_is_zero(IntPtr a);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_is_one(IntPtr a);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_is_odd(IntPtr a);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BN_value_one();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_rand(IntPtr rnd, int bits, int top, int bottom);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_priv_rand(IntPtr rnd, int bits, int top, int bottom);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_rand_range(IntPtr rnd, IntPtr range);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_priv_rand_range(IntPtr rnd, IntPtr range);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_pseudo_rand(IntPtr rnd, int bits, int top, int bottom);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_pseudo_rand_range(IntPtr rnd, IntPtr range);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_num_bits(IntPtr a);

        public static int BN_num_bytes(IntPtr a) => (BN_num_bits(a) + 7) / 8;

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BN_new();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BN_swap(IntPtr a, IntPtr b);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="len"></param>
        /// <param name="ret"></param>
        /// <returns>the BIGNUM, NULL on error</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BN_bin2bn(byte[] s, int len, IntPtr ret);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_bn2bin(IntPtr a, byte[] to);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_sub(IntPtr r, IntPtr a, IntPtr b);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_usub(IntPtr r, IntPtr a, IntPtr b);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_uadd(IntPtr r, IntPtr a, IntPtr b);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_add(IntPtr r, IntPtr a, IntPtr b);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern  int BN_mul(IntPtr r, IntPtr a, IntPtr b, IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_sqr(IntPtr r, IntPtr a, IntPtr ctx);

        /// <summary>
        /// sets sign of a BIGNUM
        /// </summary>
        /// <param name="b">pointer to the BIGNUM object</param>
        /// <param name="n">0 if the BIGNUM b should be positive and a value != 0 otherwise</param>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BN_set_negative(IntPtr b, int n);

        /// <summary>
        /// returns 1 if the BIGNUM is negative
        /// </summary>
        /// <param name="b">pointer to the BIGNUM object</param>
        /// <returns>1 if a &lt; 0 and 0 otherwise</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_is_negative(IntPtr b);

        /// <summary>
        /// Divides a by d and places the result in dv and the remainder in rem (dv=a/d, rem=a%d). 
        /// </summary>
        /// <param name="dv"></param>
        /// <param name="rem"></param>
        /// <param name="a"></param>
        /// <param name="d"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_div(IntPtr dv, IntPtr rem, IntPtr a, IntPtr d, IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_mul_word(IntPtr a, ulong w);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_add_word(IntPtr a, ulong w);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_sub_word(IntPtr a, ulong w);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_set_word(IntPtr a, ulong w);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong BN_get_word(IntPtr a);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_cmp(IntPtr a, IntPtr b);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BN_free(IntPtr a);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_is_bit_set(IntPtr a, int n);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_lshift(IntPtr r, IntPtr a, int n);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_lshift1(IntPtr r, IntPtr a);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_exp(IntPtr r, IntPtr a, IntPtr p, IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_mod_exp(IntPtr r, IntPtr a, IntPtr p, IntPtr m, IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_rshift(IntPtr r, IntPtr a, int n);

        /// <summary>
        /// Shifts a right by one and places the result in r (r=a/2).
        /// </summary>
        /// <param name="r"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_rshift1(IntPtr r, IntPtr a);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BN_clear(IntPtr a);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BN_dup(IntPtr a);

        [DllImport(libcrypto, EntryPoint = nameof(BN_bn2hex), CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr _BN_bn2hex(IntPtr a);
        public static string BN_bn2hex(IntPtr a)
        {
            return Marshal.PtrToStringAnsi(_BN_bn2hex(a));
        }

        [DllImport(libcrypto, EntryPoint = nameof(BN_bn2dec), CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr _BN_bn2dec(IntPtr a);
        public static string BN_bn2dec(IntPtr a)
        {
            return Marshal.PtrToStringAnsi(_BN_bn2dec(a));
        }

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_hex2bn(ref IntPtr a, [MarshalAs(UnmanagedType.LPStr)] string str);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BN_dec2bn(ref IntPtr a, [MarshalAs(UnmanagedType.LPStr)] string str);

        /// <summary>
        /// Computes the inverse of a modulo n places the result in r ((a*r)%n==1). If r is NULL, a new BIGNUM is created.
        /// </summary>
        /// <param name="ret"></param>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <param name="ctx"></param>
        /// <returns>the BIGNUM containing the inverse, and NULL on error.</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BN_mod_inverse(IntPtr ret, IntPtr a, IntPtr n, IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BN_mod_sqrt(IntPtr ret, IntPtr a, IntPtr n, IntPtr ctx);

        #endregion

        #region Elliptic Curves

        #region EC_GROUP

        public const int NID_X9_62_prime192v1 = 409;
        public const int NID_X9_62_prime192v2 = 410;
        public const int NID_X9_62_prime192v3 = 411;
        public const int NID_X9_62_prime239v1 = 412;
        public const int NID_X9_62_prime239v2 = 413;
        public const int NID_X9_62_prime239v3 = 414;
        public const int NID_X9_62_prime256v1 = 415;
        public const int NID_secp112r1 = 704;
        public const int NID_secp112r2 = 705;
        public const int NID_secp128r1 = 706;
        public const int NID_secp128r2 = 707;
        public const int NID_secp160k1 = 708;
        public const int NID_secp160r1 = 709;
        public const int NID_secp160r2 = 710;
        public const int NID_secp192k1 = 711;
        public const int NID_secp224k1 = 712;
        public const int NID_secp224r1 = 713;
        public const int NID_secp256k1 = 714;
        public const int NID_secp384r1 = 715;
        public const int NID_secp521r1 = 716;

        /// <summary>
        /// Creates a EC_GROUP object with a curve specified by a NID
        /// </summary>
        /// <param name="nid">NID of the OID of the curve name</param>
        /// <returns>newly created EC_GROUP object with specified curve or NULL if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EC_GROUP_new_by_curve_name(int nid);

        /// <summary>
        /// Returns the generator of a EC_GROUP object.
        /// </summary>
        /// <param name="group">EC_GROUP object</param>
        /// <returns>the currently used generator (possibly NULL).</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EC_GROUP_get0_generator(IntPtr group);

        /// <summary>
        /// Gets the order of an EC_GROUP
        /// </summary>
        /// <param name="group">EC_GROUP object</param>
        /// <returns>the group order</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EC_GROUP_get0_order(IntPtr group);

        /// <summary>
        /// Compares two EC_GROUP objects
        /// </summary>
        /// <param name="a">first EC_GROUP object</param>
        /// <param name="b">second EC_GROUP object</param>
        /// <param name="ctx">BN_CTX object (optional)</param>
        /// <returns>0 if the groups are equal, 1 if not, or -1 on error</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EC_GROUP_cmp(IntPtr a, IntPtr b, IntPtr ctx);

        #endregion

        #region EC_POINT

        public enum PointConversionForm : int
        {
            Compressed = 2,
            Uncompressed = 4,
            Hybrid = 6
        }

        /// <summary>
        /// Creates a new EC_POINT object for the specified EC_GROUP
        /// </summary>
        /// <param name="group">EC_GROUP the underlying EC_GROUP object</param>
        /// <returns>newly created EC_POINT object or NULL if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EC_POINT_new(IntPtr group);

        /// <summary>
        /// Frees a EC_POINT object.
        /// </summary>
        /// <param name="point">EC_POINT object to be freed</param>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void EC_POINT_free(IntPtr point);

        /// <summary>
        /// Sets the affine coordinates of a EC_POINT over GFp
        /// </summary>
        /// <param name="group">underlying EC_GROUP object</param>
        /// <param name="p">EC_POINT object</param>
        /// <param name="x">BIGNUM with the x-coordinate</param>
        /// <param name="y">BIGNUM with the y-coordinate</param>
        /// <param name="ctx">BN_CTX object (optional)</param>
        /// <returns>1 on success and 0 if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EC_POINT_set_affine_coordinates_GFp(IntPtr group, IntPtr p, IntPtr x, IntPtr y, IntPtr ctx);

        /// <summary>
        /// Encodes a EC_POINT object to a octet string
        /// </summary>
        /// <param name="group">underlying EC_GROUP object</param>
        /// <param name="p">EC_POINT object</param>
        /// <param name="form">point conversion form</param>
        /// <param name="buf">memory buffer for the result. If NULL the function returns required buffer size.</param>
        /// <param name="len">length of the memory buffer</param>
        /// <param name="ctx">BN_CTX object (optional)</param>
        /// <returns>the length of the encoded octet string or 0 if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EC_POINT_point2oct(IntPtr group, IntPtr p, PointConversionForm form, byte[] buf, int len, IntPtr ctx);

        /// <summary>
        /// Decodes a EC_POINT from a octet string.
        /// </summary>
        /// <param name="group">underlying EC_GROUP object</param>
        /// <param name="p">EC_POINT object</param>
        /// <param name="buf">memory buffer with the encoded ec point</param>
        /// <param name="len">length of the encoded ec point</param>
        /// <param name="ctx">BN_CTX object (optional)</param>
        /// <returns>1 on success and 0 if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EC_POINT_oct2point(IntPtr group, IntPtr p, byte[] buf, int len, IntPtr ctx);

        /// <summary>
        /// Compares two EC_POINTs
        /// </summary>
        /// <param name="group">underlying EC_GROUP object</param>
        /// <param name="a">first EC_POINT object</param>
        /// <param name="b">second EC_POINT object</param>
        /// <param name="ctx">BN_CTX object (optional)</param>
        /// <returns>1 if the points are not equal, 0 if they are, or -1 on error</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EC_POINT_cmp(IntPtr group, IntPtr a, IntPtr b, IntPtr ctx);

        /// <summary>
        /// Computes the sum of two EC_POINT
        /// </summary>
        /// <param name="group">underlying EC_GROUP object</param>
        /// <param name="r">EC_POINT object for the result (r = a + b)</param>
        /// <param name="a">EC_POINT object with the first summand</param>
        /// <param name="b">EC_POINT object with the second summand</param>
        /// <param name="ctx">BN_CTX object (optional)</param>
        /// <returns>1 on success and 0 if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EC_POINT_add(IntPtr group, IntPtr r, IntPtr a, IntPtr b, IntPtr ctx);

        /// <summary>
        /// Computes r = generator * n + q * m
        /// </summary>
        /// <param name="group">underlying EC_GROUP object</param>
        /// <param name="r">EC_POINT object for the result</param>
        /// <param name="n">BIGNUM with the multiplier for the group generator (optional)</param>
        /// <param name="q">EC_POINT object with the first factor of the second summand</param>
        /// <param name="m">BIGNUM with the second factor of the second summand</param>
        /// <param name="ctx">BN_CTX object (optional)</param>
        /// <returns>1 on success and 0 if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EC_POINT_mul(IntPtr group, IntPtr r, IntPtr n, IntPtr q, IntPtr m, IntPtr ctx);

        /// <summary>
        /// Gets the affine coordinates of a EC_POINT over GFp
        /// </summary>
        /// <param name="group">underlying EC_GROUP object</param>
        /// <param name="p">EC_POINT object</param>
        /// <param name="x">BIGNUM for the x-coordinate</param>
        /// <param name="y">BIGNUM for the y-coordinate</param>
        /// <param name="ctx">BN_CTX object (optional)</param>
        /// <returns>1 on success and 0 if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EC_POINT_get_affine_coordinates_GFp(IntPtr group, IntPtr p, IntPtr x, IntPtr y, IntPtr ctx);

        #endregion

        #region EC_KEY

        /// <summary>
        /// Creates a new EC_KEY object.
        /// </summary>
        /// <returns>EC_KEY object or NULL if an error occurred.</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EC_KEY_new();

        /// <summary>
        /// Creates a new EC_KEY object using a named curve as underlying EC_GROUP object.
        /// </summary>
        /// <param name="nid">NID of the named curve</param>
        /// <returns>EC_KEY object or NULL if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EC_KEY_new_by_curve_name(int nid);

        /// <summary>
        /// Frees a EC_KEY object.
        /// </summary>
        /// <param name="key">EC_KEY object to be freed</param>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void EC_KEY_free(IntPtr key);

        /// <summary>
        /// Returns the EC_GROUP object of a EC_KEY object.
        /// </summary>
        /// <param name="key">EC_KEY object</param>
        /// <returns>the EC_GROUP object (possibly NULL)</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EC_KEY_get0_group(IntPtr key);

        /// <summary>
        /// Sets the EC_GROUP of a EC_KEY object.
        /// </summary>
        /// <param name="key">EC_KEY object</param>
        /// <param name="group">EC_GROUP to use in the EC_KEY object (note: the EC_KEY object will use an own copy of the EC_GROUP)</param>
        /// <returns>1 on success and 0 if an error occurred.</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EC_KEY_set_group(IntPtr key, IntPtr group);

        /// <summary>
        /// Sets the private key of a EC_KEY object.
        /// </summary>
        /// <param name="key">EC_KEY object</param>
        /// <param name="prv">BIGNUM with the private key (note: the EC_KEY object will use an own copy of the BIGNUM)</param>
        /// <returns>1 on success and 0 if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EC_KEY_set_private_key(IntPtr key, IntPtr prv);

        /// <summary>
        /// Sets the public key of a EC_KEY object.
        /// </summary>
        /// <param name="key">EC_KEY object</param>
        /// <param name="pub">EC_POINT object with the public key (note: the EC_KEY object will use an own copy of the EC_POINT object)</param>
        /// <returns>1 on success and 0 if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EC_KEY_set_public_key(IntPtr key, IntPtr pub);

        /// <summary>
        /// Verifies that a private and/or public key is valid.
        /// </summary>
        /// <param name="key">the EC_KEY object</param>
        /// <returns>1 on success and 0 otherwise</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EC_KEY_check_key(IntPtr key);

        /// <summary>
        /// Decodes a EC_KEY public key from a octet string.
        /// </summary>
        /// <param name="key">key to decode</param>
        /// <param name="buf">memory buffer with the encoded ec point</param>
        /// <param name="len">length of the encoded ec point</param>
        /// <param name="ctx">BN_CTX object (optional)</param>
        /// <returns>1 on success and 0 if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EC_KEY_oct2key(IntPtr key, byte[] buf, int len, IntPtr ctx);

        /// <summary>
        /// Returns the public key of a EC_KEY object.
        /// </summary>
        /// <param name="key">the EC_KEY object</param>
        /// <returns>a EC_POINT object with the public key (possibly NULL)</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EC_KEY_get0_public_key(IntPtr key);

        #endregion

        #region ECDSA

        /// <summary>
        /// Allocates and initialize a ECDSA_SIG structure
        /// </summary>
        /// <returns>pointer to a ECDSA_SIG structure or NULL if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ECDSA_SIG_new();

        /// <summary>
        /// Frees a ECDSA_SIG structure
        /// </summary>
        /// <param name="sig">pointer to the ECDSA_SIG structure</param>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ECDSA_SIG_free(IntPtr sig);

        /// <summary>
        /// DER encode content of ECDSA_SIG object (note: this function modifies *pp (*pp += length of the DER encoded signature)).
        /// </summary>
        /// <param name="sig">pointer to the ECDSA_SIG object</param>
        /// <param name="pp">pointer to a unsigned char pointer for the output or NULL</param>
        /// <returns>the length of the DER encoded ECDSA_SIG object or 0</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int i2d_ECDSA_SIG(IntPtr sig, ref IntPtr pp);

        /// <summary>
        /// Decodes a DER encoded ECDSA signature (note: this function changes *pp (*pp += len)).
        /// </summary>
        /// <param name="sig">pointer to ECDSA_SIG pointer (may be NULL)</param>
        /// <param name="pp">memory buffer with the DER encoded signature</param>
        /// <param name="len">length of the buffer</param>
        /// <returns>pointer to the decoded ECDSA_SIG structure (or NULL)</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr d2i_ECDSA_SIG(IntPtr sig, ref IntPtr pp, int len);

        /// <summary>
        /// Accessor for r and s fields of ECDSA_SIG
        /// </summary>
        /// <param name="sig">pointer to ECDSA_SIG pointer</param>
        /// <param name="pr">pointer to BIGNUM pointer for r (may be NULL)</param>
        /// <param name="ps">pointer to BIGNUM pointer for s (may be NULL)</param>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ECDSA_SIG_get0(IntPtr sig, ref IntPtr pr, ref IntPtr ps);

        /// <summary>
        /// Setter for r and s fields of ECDSA_SIG
        /// </summary>
        /// <param name="sig">pointer to ECDSA_SIG pointer</param>
        /// <param name="r">pointer to BIGNUM for r (may be NULL)</param>
        /// <param name="s">pointer to BIGNUM for s (may be NULL)</param>
        /// <returns>1 on success or 0 on failure</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ECDSA_SIG_set0(IntPtr sig, IntPtr r, IntPtr s);

        /// <summary>
        /// Computes the ECDSA signature of the given hash value using the supplied private key and returns the created signature.
        /// </summary>
        /// <param name="dgst">pointer to the hash value</param>
        /// <param name="dgst_len">length of the hash value</param>
        /// <param name="eckey">EC_KEY object containing a private EC key</param>
        /// <returns>pointer to a ECDSA_SIG structure or NULL if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ECDSA_do_sign(byte[] dgst, int dgst_len, IntPtr eckey);

        /// <summary>
        /// Computes ECDSA signature of a given hash value using the supplied private key (note: sig must point to ECDSA_size(eckey) bytes of memory).
        /// </summary>
        /// <param name="dgst">pointer to the hash value to sign</param>
        /// <param name="dgstlen">length of the hash value</param>
        /// <param name="kinv">BIGNUM with a pre-computed inverse k (optional)</param>
        /// <param name="rp">BIGNUM with a pre-computed rp value (optional), see ECDSA_sign_setup</param>
        /// <param name="eckey">EC_KEY object containing a private EC key</param>
        /// <returns>pointer to a ECDSA_SIG structure or NULL if an error occurred</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ECDSA_do_sign_ex(byte[] dgst, int dgstlen, IntPtr kinv, IntPtr rp, IntPtr eckey);

        /// <summary>
        /// Verifies that the supplied signature is a valid ECDSA signature of the supplied hash value using the supplied public key.
        /// </summary>
        /// <param name="dgst">pointer to the hash value</param>
        /// <param name="dgst_len">length of the hash value</param>
        /// <param name="sig">ECDSA_SIG structure</param>
        /// <param name="eckey">EC_KEY object containing a public EC key</param>
        /// <returns>1 if the signature is valid, 0 if the signature is invalid and -1 on error</returns>
        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ECDSA_do_verify(byte[] dgst, int dgst_len, IntPtr sig, IntPtr eckey);

        #endregion

        #endregion

        #region PKCS5

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PKCS5_PBKDF2_HMAC(
            byte[] pass, int passlen,
            byte[] salt, int saltlen,
            int iter,
            IntPtr digest, int keylen, byte[] @out);

        #endregion
    }

    public class OpenSslException : Exception
    {
        public ulong Code { get; private set; }
        public string ErrorMessage { get; private set; }

        public OpenSslException()
        {
            this.Code = OpenSsl.ERR_get_error();
            if (this.Code != 0)
            {
                this.ErrorMessage = OpenSsl.ERR_error_string(this.Code);
            }
        }
    }
}