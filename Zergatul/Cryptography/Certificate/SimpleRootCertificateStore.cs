using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Certificate.Extensions;

namespace Zergatul.Cryptography.Certificate
{
    public class SimpleRootCertificateStore : IRootCertificateStore
    {
        private X509Certificate[] _certificates;

        public SimpleRootCertificateStore(params X509Certificate[] certificates)
        {
            this._certificates = certificates;
        }

        public X509Certificate FindBySubjectKeyId(byte[] subject)
        {
            return _certificates
                .SingleOrDefault(c => ByteArray.Equals(c.Extensions.Get<SubjectKeyIdentifier>()?.KeyIdentifier, subject));
        }
    }
}