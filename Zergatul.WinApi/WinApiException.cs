using System;
using System.Runtime.InteropServices;

namespace Zergatul.WinApi
{
    public class WinApiException : Exception
    {
        public int WinApiCode { get; set; }

        public WinApiException()
        {
            WinApiCode = Marshal.GetLastWin32Error();
        }
    }
}