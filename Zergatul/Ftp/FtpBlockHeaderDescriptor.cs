using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Ftp
{
    public static class FtpBlockHeaderDescriptor
    {
        public const byte EOR = 128;
        public const byte EOF = 64;
        public const byte Errors = 32;
        public const byte RestartMarker = 16;
    }
}