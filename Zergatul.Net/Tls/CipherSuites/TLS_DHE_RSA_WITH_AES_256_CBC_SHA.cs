using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal class TLS_DHE_RSA_WITH_AES_256_CBC_SHA : DHECipherSuite
    {
        public TLS_DHE_RSA_WITH_AES_256_CBC_SHA() :
            base(CipherSuiteType.TLS_DHE_RSA_WITH_AES_256_CBC_SHA)
        {

        }
    }
}
