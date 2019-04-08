using System;
using System.Runtime.InteropServices;

namespace Zergatul.Security.OpenSsl
{
    internal static class Native
    {
        private const string libcrypto = "libcrypto-1_1-x64.dll";
        private const string libssl = "libssl-1_1-x64.dll";

        #region libcrypto

        #region Memory

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CRYPTO_malloc(int num, string file, int line);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CRYPTO_free(IntPtr p);

        #endregion

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

        #region Constants

        #region BIO_CTRL

        public const int BIO_CTRL_RESET = 1;
        public const int BIO_CTRL_EOF = 2;
        public const int BIO_CTRL_INFO = 3;
        public const int BIO_CTRL_SET = 4;
        public const int BIO_CTRL_GET = 5;
        public const int BIO_CTRL_PUSH = 6;
        public const int BIO_CTRL_POP = 7;
        public const int BIO_CTRL_GET_CLOSE = 8;
        public const int BIO_CTRL_SET_CLOSE = 9;
        public const int BIO_CTRL_PENDING = 10;
        public const int BIO_CTRL_FLUSH = 11;
        public const int BIO_CTRL_DUP = 12;
        public const int BIO_CTRL_WPENDING = 13;
        public const int BIO_CTRL_SET_CALLBACK = 14;
        public const int BIO_CTRL_GET_CALLBACK = 15;
        public const int BIO_CTRL_PEEK = 29;
        public const int BIO_CTRL_SET_FILENAME = 30;
        public const int BIO_CTRL_DGRAM_CONNECT = 31;
        public const int BIO_CTRL_DGRAM_SET_CONNECTED = 32;
        public const int BIO_CTRL_DGRAM_SET_RECV_TIMEOUT = 33;
        public const int BIO_CTRL_DGRAM_GET_RECV_TIMEOUT = 34;
        public const int BIO_CTRL_DGRAM_SET_SEND_TIMEOUT = 35;
        public const int BIO_CTRL_DGRAM_GET_SEND_TIMEOUT = 36;
        public const int BIO_CTRL_DGRAM_GET_RECV_TIMER_EXP = 37;
        public const int BIO_CTRL_DGRAM_GET_SEND_TIMER_EXP = 38;
        public const int BIO_CTRL_DGRAM_MTU_DISCOVER = 39;
        public const int BIO_CTRL_DGRAM_QUERY_MTU = 40;
        public const int BIO_CTRL_DGRAM_GET_FALLBACK_MTU = 47;
        public const int BIO_CTRL_DGRAM_GET_MTU = 41;
        public const int BIO_CTRL_DGRAM_SET_MTU = 42;
        public const int BIO_CTRL_DGRAM_MTU_EXCEEDED = 43;
        public const int BIO_CTRL_DGRAM_GET_PEER = 46;
        public const int BIO_CTRL_DGRAM_SET_PEER = 44;
        public const int BIO_CTRL_DGRAM_SET_NEXT_TIMEOUT = 45;
        public const int BIO_CTRL_DGRAM_SET_DONT_FRAG = 48;
        public const int BIO_CTRL_DGRAM_GET_MTU_OVERHEAD = 49;
        public const int BIO_CTRL_DGRAM_SCTP_SET_IN_HANDSHAKE = 50;
        public const int BIO_CTRL_DGRAM_SCTP_ADD_AUTH_KEY = 51;
        public const int BIO_CTRL_DGRAM_SCTP_NEXT_AUTH_KEY = 52;
        public const int BIO_CTRL_DGRAM_SCTP_AUTH_CCS_RCVD = 53;
        public const int BIO_CTRL_DGRAM_SCTP_GET_SNDINFO = 60;
        public const int BIO_CTRL_DGRAM_SCTP_SET_SNDINFO = 61;
        public const int BIO_CTRL_DGRAM_SCTP_GET_RCVINFO = 62;
        public const int BIO_CTRL_DGRAM_SCTP_SET_RCVINFO = 63;
        public const int BIO_CTRL_DGRAM_SCTP_GET_PRINFO = 64;
        public const int BIO_CTRL_DGRAM_SCTP_SET_PRINFO = 65;
        public const int BIO_CTRL_DGRAM_SCTP_SAVE_SHUTDOWN = 70;
        public const int BIO_CTRL_DGRAM_SET_PEEK_MODE = 71;

        #endregion

        #endregion

        #region BIO

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BIO_new(IntPtr type);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BIO_set(IntPtr bio, IntPtr type);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BIO_up_ref(IntPtr bio);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BIO_set_init(IntPtr bio, int init);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BIO_get_init(IntPtr bio);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BIO_set_data(IntPtr bio, IntPtr ptr);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BIO_get_data(IntPtr bio);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BIO_set_shutdown(IntPtr bio, int shut);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BIO_get_shutdown(IntPtr bio);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BIO_read(IntPtr bio, IntPtr buf, int len);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BIO_gets(IntPtr bio, IntPtr buf, int size);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BIO_write(IntPtr bio, IntPtr buf, int len);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BIO_puts(IntPtr bio, IntPtr buf);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BIO_free(IntPtr bio);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BIO_vfree(IntPtr bio);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BIO_free_all(IntPtr bio);

        #region BIO_METHOD

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BIO_s_mem();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BIO_s_secmem();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BIO_get_new_index();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr BIO_meth_new(int type, string name);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long BIOCtrlDelegate(IntPtr bio, int cmd, long larg, IntPtr parg);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int BIO_meth_set_ctrl(IntPtr biom, [MarshalAs(UnmanagedType.FunctionPtr)] BIOCtrlDelegate ctrl);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int BIOWriteDelegate(IntPtr bio, IntPtr buffer, int count);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BIO_meth_set_write(IntPtr biom, [MarshalAs(UnmanagedType.FunctionPtr)] BIOWriteDelegate write);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int BIOReadDelegate(IntPtr bio, IntPtr buffer, int count);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int BIO_meth_set_read(IntPtr biom, [MarshalAs(UnmanagedType.FunctionPtr)] BIOReadDelegate read);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BIO_meth_free(IntPtr biom);

        #endregion

        #endregion

        #region EVP

        #region Constants

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

        #endregion

        #region EVP_CIPHER

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

        #endregion

        #region other

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

        #endregion

        #region MD

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_MD_CTX_new();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_MD_CTX_reset(IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void EVP_MD_CTX_free(IntPtr ctx);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_DigestInit_ex(IntPtr ctx, IntPtr type, IntPtr impl);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_DigestUpdate(IntPtr ctx, IntPtr d, int cnt);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_DigestUpdate(IntPtr ctx, byte[] d, int cnt);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_DigestFinal_ex(IntPtr ctx, byte[] md, IntPtr s);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_MD_size(IntPtr md);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EVP_MD_block_size(IntPtr md);

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_md4();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_md5();

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

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_ripemd160();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_blake2b512();

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EVP_blake2s256();

        #endregion

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

        #region X509_CTX

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr X509_STORE_CTX_get_current_cert(IntPtr ctx);

        #endregion

        #region X509

        [DllImport(libcrypto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int i2d_X509(IntPtr x509, ref IntPtr ppout);

        #endregion

        #endregion

        #region libssl

        #region Constants

        #region SSL_CTRL

        public const int SSL_CTRL_SET_TMP_DH = 3;
        public const int SSL_CTRL_SET_TMP_ECDH = 4;
        public const int SSL_CTRL_SET_TMP_DH_CB = 6;
        public const int SSL_CTRL_GET_CLIENT_CERT_REQUEST = 9;
        public const int SSL_CTRL_GET_NUM_RENEGOTIATIONS = 10;
        public const int SSL_CTRL_CLEAR_NUM_RENEGOTIATIONS = 11;
        public const int SSL_CTRL_GET_TOTAL_RENEGOTIATIONS = 12;
        public const int SSL_CTRL_GET_FLAGS = 13;
        public const int SSL_CTRL_EXTRA_CHAIN_CERT = 14;
        public const int SSL_CTRL_SET_MSG_CALLBACK = 15;
        public const int SSL_CTRL_SET_MSG_CALLBACK_ARG = 16;
        public const int SSL_CTRL_SET_MTU = 17;
        public const int SSL_CTRL_SESS_NUMBER = 20;
        public const int SSL_CTRL_SESS_CONNECT = 21;
        public const int SSL_CTRL_SESS_CONNECT_GOOD = 22;
        public const int SSL_CTRL_SESS_CONNECT_RENEGOTIATE = 23;
        public const int SSL_CTRL_SESS_ACCEPT = 24;
        public const int SSL_CTRL_SESS_ACCEPT_GOOD = 25;
        public const int SSL_CTRL_SESS_ACCEPT_RENEGOTIATE = 26;
        public const int SSL_CTRL_SESS_HIT = 27;
        public const int SSL_CTRL_SESS_CB_HIT = 28;
        public const int SSL_CTRL_SESS_MISSES = 29;
        public const int SSL_CTRL_SESS_TIMEOUTS = 30;
        public const int SSL_CTRL_SESS_CACHE_FULL = 31;
        public const int SSL_CTRL_MODE = 33;
        public const int SSL_CTRL_GET_READ_AHEAD = 40;
        public const int SSL_CTRL_SET_READ_AHEAD = 41;
        public const int SSL_CTRL_SET_SESS_CACHE_SIZE = 42;
        public const int SSL_CTRL_GET_SESS_CACHE_SIZE = 43;
        public const int SSL_CTRL_SET_SESS_CACHE_MODE = 44;
        public const int SSL_CTRL_GET_SESS_CACHE_MODE = 45;
        public const int SSL_CTRL_GET_MAX_CERT_LIST = 50;
        public const int SSL_CTRL_SET_MAX_CERT_LIST = 51;
        public const int SSL_CTRL_SET_MAX_SEND_FRAGMENT = 52;
        public const int SSL_CTRL_SET_TLSEXT_SERVERNAME_CB = 53;
        public const int SSL_CTRL_SET_TLSEXT_SERVERNAME_ARG = 54;
        public const int SSL_CTRL_SET_TLSEXT_HOSTNAME = 55;
        public const int SSL_CTRL_SET_TLSEXT_DEBUG_CB = 56;
        public const int SSL_CTRL_SET_TLSEXT_DEBUG_ARG = 57;
        public const int SSL_CTRL_GET_TLSEXT_TICKET_KEYS = 58;
        public const int SSL_CTRL_SET_TLSEXT_TICKET_KEYS = 59;
        public const int SSL_CTRL_SET_TLSEXT_STATUS_REQ_CB = 63;
        public const int SSL_CTRL_SET_TLSEXT_STATUS_REQ_CB_ARG = 64;
        public const int SSL_CTRL_SET_TLSEXT_STATUS_REQ_TYPE = 65;
        public const int SSL_CTRL_GET_TLSEXT_STATUS_REQ_EXTS = 66;
        public const int SSL_CTRL_SET_TLSEXT_STATUS_REQ_EXTS = 67;
        public const int SSL_CTRL_GET_TLSEXT_STATUS_REQ_IDS = 68;
        public const int SSL_CTRL_SET_TLSEXT_STATUS_REQ_IDS = 69;
        public const int SSL_CTRL_GET_TLSEXT_STATUS_REQ_OCSP_RESP = 70;
        public const int SSL_CTRL_SET_TLSEXT_STATUS_REQ_OCSP_RESP = 71;
        public const int SSL_CTRL_SET_TLSEXT_TICKET_KEY_CB = 72;
        public const int SSL_CTRL_SET_TLS_EXT_SRP_USERNAME_CB = 75;
        public const int SSL_CTRL_SET_SRP_VERIFY_PARAM_CB = 76;
        public const int SSL_CTRL_SET_SRP_GIVE_CLIENT_PWD_CB = 77;
        public const int SSL_CTRL_SET_SRP_ARG = 78;
        public const int SSL_CTRL_SET_TLS_EXT_SRP_USERNAME = 79;
        public const int SSL_CTRL_SET_TLS_EXT_SRP_STRENGTH = 80;
        public const int SSL_CTRL_SET_TLS_EXT_SRP_PASSWORD = 81;
        public const int SSL_CTRL_DTLS_EXT_SEND_HEARTBEAT = 85;
        public const int SSL_CTRL_GET_DTLS_EXT_HEARTBEAT_PENDING = 86;
        public const int SSL_CTRL_SET_DTLS_EXT_HEARTBEAT_NO_REQUESTS = 87;
        public const int DTLS_CTRL_GET_TIMEOUT = 73;
        public const int DTLS_CTRL_HANDLE_TIMEOUT = 74;
        public const int SSL_CTRL_GET_RI_SUPPORT = 76;
        public const int SSL_CTRL_CLEAR_MODE = 78;
        public const int SSL_CTRL_SET_NOT_RESUMABLE_SESS_CB = 79;
        public const int SSL_CTRL_GET_EXTRA_CHAIN_CERTS = 82;
        public const int SSL_CTRL_CLEAR_EXTRA_CHAIN_CERTS = 83;
        public const int SSL_CTRL_CHAIN = 88;
        public const int SSL_CTRL_CHAIN_CERT = 89;
        public const int SSL_CTRL_GET_GROUPS = 90;
        public const int SSL_CTRL_SET_GROUPS = 91;
        public const int SSL_CTRL_SET_GROUPS_LIST = 92;
        public const int SSL_CTRL_GET_SHARED_GROUP = 93;
        public const int SSL_CTRL_SET_SIGALGS = 97;
        public const int SSL_CTRL_SET_SIGALGS_LIST = 98;
        public const int SSL_CTRL_CERT_FLAGS = 99;
        public const int SSL_CTRL_CLEAR_CERT_FLAGS = 100;
        public const int SSL_CTRL_SET_CLIENT_SIGALGS = 101;
        public const int SSL_CTRL_SET_CLIENT_SIGALGS_LIST = 102;
        public const int SSL_CTRL_GET_CLIENT_CERT_TYPES = 103;
        public const int SSL_CTRL_SET_CLIENT_CERT_TYPES = 104;
        public const int SSL_CTRL_BUILD_CERT_CHAIN = 105;
        public const int SSL_CTRL_SET_VERIFY_CERT_STORE = 106;
        public const int SSL_CTRL_SET_CHAIN_CERT_STORE = 107;
        public const int SSL_CTRL_GET_PEER_SIGNATURE_NID = 108;
        public const int SSL_CTRL_GET_SERVER_TMP_KEY = 109;
        public const int SSL_CTRL_GET_RAW_CIPHERLIST = 110;
        public const int SSL_CTRL_GET_EC_POINT_FORMATS = 111;
        public const int SSL_CTRL_GET_CHAIN_CERTS = 115;
        public const int SSL_CTRL_SELECT_CURRENT_CERT = 116;
        public const int SSL_CTRL_SET_CURRENT_CERT = 117;
        public const int SSL_CTRL_SET_DH_AUTO = 118;
        public const int DTLS_CTRL_SET_LINK_MTU = 120;
        public const int DTLS_CTRL_GET_LINK_MIN_MTU = 121;
        public const int SSL_CTRL_GET_EXTMS_SUPPORT = 122;
        public const int SSL_CTRL_SET_MIN_PROTO_VERSION = 123;
        public const int SSL_CTRL_SET_MAX_PROTO_VERSION = 124;
        public const int SSL_CTRL_SET_SPLIT_SEND_FRAGMENT = 125;
        public const int SSL_CTRL_SET_MAX_PIPELINES = 126;
        public const int SSL_CTRL_GET_TLSEXT_STATUS_REQ_TYPE = 127;
        public const int SSL_CTRL_GET_TLSEXT_STATUS_REQ_CB = 128;
        public const int SSL_CTRL_GET_TLSEXT_STATUS_REQ_CB_ARG = 129;
        public const int SSL_CTRL_GET_MIN_PROTO_VERSION = 130;
        public const int SSL_CTRL_GET_MAX_PROTO_VERSION = 131;

        #endregion

        #region SSL_ERROR

        public const int SSL_ERROR_NONE = 0;
        public const int SSL_ERROR_SSL = 1;
        public const int SSL_ERROR_WANT_READ = 2;
        public const int SSL_ERROR_WANT_WRITE = 3;
        public const int SSL_ERROR_WANT_X509_LOOKUP = 4;
        public const int SSL_ERROR_SYSCALL = 5;
        public const int SSL_ERROR_ZERO_RETURN = 6;
        public const int SSL_ERROR_WANT_CONNECT = 7;
        public const int SSL_ERROR_WANT_ACCEPT = 8;
        public const int SSL_ERROR_WANT_ASYNC = 9;
        public const int SSL_ERROR_WANT_ASYNC_JOB = 10;
        public const int SSL_ERROR_WANT_CLIENT_HELLO_CB = 11;

        #endregion

        #region SSL_MODE

        /// <summary>
        /// Allow SSL_write(..., n) to return r with 0 &lt; r &lt; n
        /// (i.e. report success when just a single record has been written)
        /// </summary>
        public const int SSL_MODE_ENABLE_PARTIAL_WRITE = 0x00000001;

        /// <summary>
        /// Make it possible to retry SSL_write() with changed buffer location (buffer
        /// contents must stay the same!); this is not the default to avoid the
        /// misconception that non-blocking SSL_write() behaves like non-blocking
        /// write()
        /// </summary>
        public const int SSL_MODE_ACCEPT_MOVING_WRITE_BUFFER = 0x00000002;

        /// <summary>
        /// Never bother the application with retries if the transport is blocking
        /// </summary>
        public const int SSL_MODE_AUTO_RETRY = 0x00000004;

        /// <summary>
        /// Don't attempt to automatically build certificate chain
        /// </summary>
        public const int SSL_MODE_NO_AUTO_CHAIN = 0x00000008;

        /// <summary>
        /// Save RAM by releasing read and write buffers when they're empty. (SSL3 and
        /// TLS only.) Released buffers are freed.
        /// </summary>
        public const int SSL_MODE_RELEASE_BUFFERS = 0x00000010;

        /// <summary>
        /// Send the current time in the Random fields of the ClientHello
        /// record for compatibility with hypothetical implementations
        /// that require it
        /// </summary>
        public const int SSL_MODE_SEND_CLIENTHELLO_TIME = 0x00000020;

        /// <summary>
        /// Send the current time in the Random fields of the ServerHello
        /// record for compatibility with hypothetical implementations
        /// that require it
        /// </summary>
        public const int SSL_MODE_SEND_SERVERHELLO_TIME = 0x00000040;

        /// <summary>
        /// Send TLS_FALLBACK_SCSV in the ClientHello. To be set only by applications
        /// that reconnect with a downgraded protocol version; see
        /// draft-ietf-tls-downgrade-scsv-00 for details. DO NOT ENABLE THIS if your
        /// application attempts a normal handshake. Only use this in explicit
        /// fallback retries, following the guidance in
        /// draft-ietf-tls-downgrade-scsv-00
        /// </summary>
        public const int SSL_MODE_SEND_FALLBACK_SCSV = 0x00000080;

        /// <summary>
        /// Support Asynchronous operation
        /// </summary>
        public const int SSL_MODE_ASYNC = 0x00000100;

        #endregion

        #region SSL_VERIFY

        public const int SSL_VERIFY_NONE = 0x00;
        public const int SSL_VERIFY_PEER = 0x01;
        public const int SSL_VERIFY_FAIL_IF_NO_PEER_CERT = 0x02;
        public const int SSL_VERIFY_CLIENT_ONCE = 0x04;
        public const int SSL_VERIFY_POST_HANDSHAKE = 0x08;

        #endregion

        #region SSL_SHUTDOWN

        public const int SSL_SENT_SHUTDOWN = 1;
        public const int SSL_RECEIVED_SHUTDOWN = 2;

        #endregion

        #endregion

        #region SSL_CTX

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SSL_CTX_new(IntPtr method);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern long SSL_CTX_ctrl(IntPtr ctx, int cmd, long larg, IntPtr parg);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackDelegate();

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern long SSL_CTX_callback_ctrl(IntPtr ctx, int cmd, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackDelegate fp);

        public static long SSL_CTX_get_mode(IntPtr ctx) => SSL_CTX_ctrl(ctx, SSL_CTRL_MODE, 0, IntPtr.Zero);

        public static long SSL_CTX_set_mode(IntPtr ctx, long mode) => SSL_CTX_ctrl(ctx, SSL_CTRL_MODE, mode, IntPtr.Zero);

        public static long SSL_CTX_clear_mode(IntPtr ctx, long mode) => SSL_ctrl(ctx, SSL_CTRL_CLEAR_MODE, mode, IntPtr.Zero);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern long SSL_CTX_get_options(IntPtr ctx);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern long SSL_CTX_set_options(IntPtr ctx, long options);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern long SSL_CTX_clear_options(IntPtr ctx, long options);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int VerifyCallbackDelegate(int preverifyOk, IntPtr x509Ctx);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SSL_CTX_set_verify(IntPtr ctx, int mode, [MarshalAs(UnmanagedType.FunctionPtr)] VerifyCallbackDelegate verifyCallback);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SSL_CTX_set_verify_depth(IntPtr ctx, int depth);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SSL_CTX_free(IntPtr ctx);

        #endregion

        #region SSL_METHOD

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TLS_method();

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TLS_server_method();

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TLS_client_method();

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TLSv1_method();

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TLSv1_server_method();

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TLSv1_client_method();

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TLSv1_1_method();

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TLSv1_1_server_method();

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TLSv1_1_client_method();

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TLSv1_2_method();

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TLSv1_2_server_method();

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TLSv1_2_client_method();

        #endregion

        #region SSL

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SSL_new(IntPtr ctx);

        public static long SSL_set_min_proto_version(IntPtr ssl, int version) => SSL_ctrl(ssl, SSL_CTRL_SET_MIN_PROTO_VERSION, version, IntPtr.Zero);

        public static long SSL_set_max_proto_version(IntPtr ssl, int version) => SSL_ctrl(ssl, SSL_CTRL_SET_MAX_PROTO_VERSION, version, IntPtr.Zero);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SSL_set_cipher_list(IntPtr ssl, [MarshalAs(UnmanagedType.LPStr)] string str);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SSL_set_ciphersuites(IntPtr s, [MarshalAs(UnmanagedType.LPStr)] string str);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern long SSL_ctrl(IntPtr ssl, int cmd, long larg, IntPtr parg);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern long SSL_callback_ctrl(IntPtr ssl, int cmd, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackDelegate fp);

        public static long SSL_get_mode(IntPtr ssl) => SSL_ctrl(ssl, SSL_CTRL_MODE, 0, IntPtr.Zero);

        public static long SSL_set_mode(IntPtr ssl, long mode) => SSL_ctrl(ssl, SSL_CTRL_MODE, mode, IntPtr.Zero);

        public static long SSL_clear_mode(IntPtr ssl, long mode) => SSL_ctrl(ssl, SSL_CTRL_CLEAR_MODE, mode, IntPtr.Zero);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern long SSL_get_options(IntPtr ssl);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern long SSL_set_options(IntPtr ssl, long options);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern long SSL_clear_options(IntPtr ssl, long options);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SSL_set_verify(IntPtr ssl, int mode, [MarshalAs(UnmanagedType.FunctionPtr)] VerifyCallbackDelegate verifyCallback);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SSL_set_verify_depth(IntPtr ssl, int depth);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SSL_set_bio(IntPtr ssl, IntPtr rbio, IntPtr wbio);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SSL_set0_rbio(IntPtr s, IntPtr rbio);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SSL_set0_wbio(IntPtr s, IntPtr wbio);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SSL_accept(IntPtr ssl);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SSL_connect(IntPtr ssl);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SSL_read(IntPtr ssl, IntPtr buf, int num);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SSL_write(IntPtr ssl, IntPtr buf, int num);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SSL_shutdown(IntPtr ssl);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SSL_get_shutdown(IntPtr ssl);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SSL_free(IntPtr ssl);

        [DllImport(libssl, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SSL_get_error(IntPtr ssl, int ret);

        #endregion

        #endregion
    }
}