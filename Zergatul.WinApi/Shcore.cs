using System;
using System.Runtime.InteropServices;

namespace Zergatul.WinApi
{
    public static class Shcore
    {
        private const string Dll = "Shcore.dll";

        #region Methods

        [DllImport(Dll, SetLastError = true)]
        public static extern HResult GetDpiForMonitor(IntPtr hMonitor, MONITOR_DPI_TYPE dpiType, out int dpiX, out int dpiY);

        [DllImport(Dll, SetLastError = true)]
        public static extern HResult GetScaleFactorForMonitor(IntPtr hMonitor, out int pScale);

        #endregion

        #region Enums

        public enum MONITOR_DPI_TYPE : int
        {
            MDT_EFFECTIVE_DPI = 0,
            MDT_ANGULAR_DPI = 1,
            MDT_RAW_DPI = 2,
            MDT_DEFAULT = MDT_EFFECTIVE_DPI
        }

        #endregion

        #region Structs

        #endregion
    }
}