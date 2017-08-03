using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Certificate
{
    public class WindowsRootCertificateStore : IRootCertificateStore
    {
        public X509Certificate FindBySubjectKeyId(byte[] subject)
        {
            var store = new System.Security.Cryptography.X509Certificates.X509Store(System.Security.Cryptography.X509Certificates.StoreName.Root, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine);
            store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly);
            var coll = store.Certificates.Find(
                System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectKeyIdentifier,
                BitHelper.BytesToHex(subject),
                false);
            if (coll.Count > 1)
                throw new InvalidOperationException();
            if (coll.Count == 0)
                return null;
            else
                return new X509Certificate(coll[0].RawData);
        }
    }
}