using System;

namespace Zergatul.Network
{
    public static class Constants
    {
        public static readonly string TelnetEndOfLine = new string(new[] { (char)0x0D, (char)0x0A });
        public static readonly byte[] TelnetEndOfLineBytes = new byte[] { 0x0D, 0x0A };
        public static readonly DateTime UnixTimeStart = new DateTime(1970, 1, 1);
    }
}