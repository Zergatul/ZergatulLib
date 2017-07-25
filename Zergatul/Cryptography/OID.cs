using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography
{
    public class OID
    {
        public string Text { get; private set; }

        public static OID MD5 = new OID { Text = "1.2.840.113549.2.5" };
        public static OID SHA1 = new OID { Text = "1.3.14.3.2.26" };
        public static OID SHA224 = new OID { Text = "2.16.840.1.101.3.4.2.4" };
        public static OID SHA256 = new OID { Text = "2.16.840.1.101.3.4.2.1" };
        public static OID SHA384 = new OID { Text = "2.16.840.1.101.3.4.2.2" };
        public static OID SHA512 = new OID { Text = "2.16.840.1.101.3.4.2.3" };
    }
}