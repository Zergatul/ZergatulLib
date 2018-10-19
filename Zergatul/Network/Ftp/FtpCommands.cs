namespace Zergatul.Network.Ftp
{
    public static class FtpCommands
    {
        public const string USER = "USER";
        public const string PASS = "PASS";
        public const string ACCT = "ACCT";
        public const string CWD = "CWD";
        public const string CDUP = "CDUP";
        public const string SMNT = "SMNT";
        public const string QUIT = "QUIT";
        public const string REIN = "REIN";
        public const string PORT = "PORT";
        public const string PASV = "PASV";
        public const string TYPE = "TYPE";
        public const string STRU = "STRU";
        public const string MODE = "MODE";
        public const string RETR = "RETR";
        public const string STOR = "STOR";
        public const string STOU = "STOU";
        public const string APPE = "APPE";
        public const string ALLO = "ALLO";
        public const string REST = "REST";
        public const string RNFR = "RNFR";
        public const string RNTO = "RNTO";
        public const string ABOR = "ABOR";
        public const string DELE = "DELE";
        public const string RMD = "RMD";
        public const string MKD = "MKD";
        public const string PWD = "PWD";
        public const string LIST = "LIST";
        public const string NLST = "NLST";
        public const string SITE = "SITE";
        public const string SYST = "SYST";
        public const string STAT = "STAT";
        public const string HELP = "HELP";
        public const string NOOP = "NOOP";

        // RFC 2428
        public const string EPRT = "EPRT";
        public const string EPSV = "EPSV";

        //
        public const string FEAT = "FEAT";
        public const string OPTS = "OPTS";
        public const string MLST = "MLST";
        public const string MLSD = "MLSD";

        public const string AUTH = "AUTH";
        public const string PROT = "PROT";
        public const string PBSZ = "PBSZ";
        public const string CCC = "CCC";

        // RFC 3659
        public const string SIZE = "SIZE";
    }
}