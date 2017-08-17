using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Tls.Tests
{
    static class BCrypt
    {
        [DllImport("Bcrypt.dll", EntryPoint = "BCryptEnumContextFunctions", CharSet = CharSet.Unicode)]
        public static extern uint EnumContextFunctions(uint dwTable, string pszContext, uint dwInterface, ref uint pcbBuffer, ref IntPtr ppBuffer);

        [DllImport("Bcrypt.dll", EntryPoint = "BCryptFreeBuffer")]
        public static extern void FreeBuffer(IntPtr pvBuffer);

        [DllImport("Bcrypt.dll", EntryPoint = "BCryptAddContextFunction", CharSet = CharSet.Unicode)]
        public static extern uint AddContextFunction(uint dwTable, string pszContext, uint dwInterface, string pszFunction, uint dwPosition);

        [DllImport("Bcrypt.dll", EntryPoint = "BCryptRemoveContextFunction", CharSet = CharSet.Unicode)]
        public static extern uint RemoveContextFunction(uint dwTable, string pszContext, uint dwInterface, string pszFunction);

        [StructLayout(LayoutKind.Sequential)]
        public struct CRYPT_CONTEXT_FUNCTIONS
        {
            public uint cFunctions;
            public IntPtr rgpszFunctions;
        }

        public const uint CRYPT_LOCAL = 0x00000001;
        public const uint NCRYPT_SCHANNEL_INTERFACE = 0x00010002;
        public const uint CRYPT_PRIORITY_TOP = 0x00000000;
        public const uint CRYPT_PRIORITY_BOTTOM = 0xFFFFFFFF;
    }
}