using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Ftp
{
    public enum FtpTransferMode
    {
        Stream,
        Block,
        Compressed
    }

    internal static class FtpTransferModeExtensions
    {
        public static string ToCommand(this FtpTransferMode mode)
        {
            if (mode == FtpTransferMode.Stream)
                return "S";
            if (mode == FtpTransferMode.Block)
                return "B";
            if (mode == FtpTransferMode.Compressed)
                return "C";
            throw new Exception("Invalid mode");
        }
    }
}