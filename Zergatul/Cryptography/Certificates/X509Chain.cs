using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Certificates
{
    public class X509Chain
    {
        public static X509Chain Build(IEnumerable<X509Certificate> certificates, ICertificateStore store = null)
        {
            return null;
        }

        public bool Validate()
        {
            throw new NotImplementedException();
        }
    }
}