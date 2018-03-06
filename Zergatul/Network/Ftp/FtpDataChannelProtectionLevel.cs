using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Ftp
{
    public enum FtpDataChannelProtectionLevel
    {
        /// <summary>
        /// The data channel will carry the raw data of the file transfer, with no security applied.
        /// </summary>
        Clear,

        /// <summary>
        /// The data channel will be integrity protected.
        /// </summary>
        Safe,

        /// <summary>
        /// The data channel will be confidentiality protected.
        /// </summary>
        Confidential,

        /// <summary>
        /// The data channel will be integrity and confidentiality protected.
        /// </summary>
        Private
    }

    internal static class FtpDataChannelProtectionLevelExtensions
    {
        public static string ToCommand(this FtpDataChannelProtectionLevel level)
        {
            switch (level)
            {
                case FtpDataChannelProtectionLevel.Clear: return "C";
                case FtpDataChannelProtectionLevel.Safe: return "S";
                case FtpDataChannelProtectionLevel.Confidential: return "E";
                case FtpDataChannelProtectionLevel.Private: return "P";
                default:
                    throw new Exception("Invalid level");
            }
        }
    }
}