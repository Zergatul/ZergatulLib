using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network
{
    public static class Constants
    {
        public static readonly string TelnetEndOfLine = new string(new[] { (char)0x0D, (char)0x0A });
        public static readonly DateTime UnixTimeStart = new DateTime(1970, 1, 1);
    }
}