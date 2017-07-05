using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Ftp
{
    public static class FtpRepresentation
    {
        public static class Type
        {
            public const string ASCII = "A";
            public const string EBCDIC = "E";
            public const string Image = "I";
            
        }

        public static class Param
        {
            public const string NonPrint = "N";
            public const string Telnet = "T";
            public const string CarriageControl = "C";
        }
    }
}