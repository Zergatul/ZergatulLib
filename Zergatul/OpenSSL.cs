using System;
using System.Runtime.InteropServices;

namespace Zergatul
{
#if UseOpenSSL

    internal static class OpenSSL
    {
        private const string libeay = "libeay32";
        private const string ssleay = "ssleay32";

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr CRYPTO_malloc(int num, string file, int line);


        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void CRYPTO_free(IntPtr p);

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

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void MD5_Init(IntPtr ctx);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void MD5_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void MD5_Final(byte[] md, IntPtr ctx);

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

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA1_Init(IntPtr ctx);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA1_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA1_Final(byte[] md, IntPtr ctx);

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

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA224_Init(IntPtr ctx);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA224_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA224_Final(byte[] md, IntPtr ctx);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA256_Init(IntPtr ctx);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA256_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA256_Final(byte[] md, IntPtr ctx);

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

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA384_Init(IntPtr ctx);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA384_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA384_Final(byte[] md, IntPtr ctx);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA512_Init(IntPtr ctx);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA512_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SHA512_Final(byte[] md, IntPtr ctx);

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

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void RIPEMD160_Init(IntPtr ctx);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void RIPEMD160_Update(IntPtr ctx, byte[] data, int len);

        [DllImport(libeay, CallingConvention = CallingConvention.Cdecl)]
        public extern static void RIPEMD160_Final(byte[] md, IntPtr ctx);

        #endregion
    }

#endif
}