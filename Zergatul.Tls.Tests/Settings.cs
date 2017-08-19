using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Tls.Tests
{
    static class Settings
    {
        public static string RSA4096CertName = "rsa-4096.pfx";
        public static string RSA4096CertPwd = "hh87$-Jqo";

        public static string DSA3072CertName = "dsa-3072.pfx";
        public static string DSA3072CertPwd = ";qIU8*1m,q3";

        public static string ECDSAp256r1CertName = "ecdsa-secp256r1.pfx";
        public static string ECDSAp256r1CertPwd = @"l(*qqqJ;q30[]e";

        public static string ECDSAp521r1CertName = "ecdsa-secp521r1.pfx";
        public static string ECDSAp521r1CertPwd = @"\7FoIK*f1{\q";

        public static string DHCertName = "dh-2048.pfx";
        public static string DHCertPwd = @"kJ==+`!j8qzm&";

        public static int Port = 32032;
    }
}