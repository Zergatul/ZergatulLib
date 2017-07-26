using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Certificates
{
    public class X509v3Certificate
    {
        public X509v3Certificate(byte[] data)
        {
            using (var ms = new MemoryStream(data))
                ReadFromStream(ms);
        }

        public X509v3Certificate(Stream stream)
        {
            ReadFromStream(stream);
        }

        private void ReadFromStream(Stream stream)
        {

        }
    }
}