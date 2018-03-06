using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Smtp
{
    public static class SmtpCommands
    {
        /// <summary>
        /// Extended Hello
        /// </summary>
        public const string EHLO = "EHLO";

        public const string MAIL = "MAIL";
        public const string RCPT = "RCPT";
        public const string DATA = "DATA";

        public const string QUIT = "QUIT";

        public const string STARTTLS = "STARTTLS";
        public const string AUTH = "AUTH";
    }
}