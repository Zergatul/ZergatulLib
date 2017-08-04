using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Certificate
{
    public class X509Chain
    {
        public IReadOnlyCollection<X509Certificate> List => _list.AsReadOnly();

        private List<X509Certificate> _list;

        public static X509Chain Build(IEnumerable<X509Certificate> certificates, IRootCertificateStore store = null)
        {
            if (certificates.Count() == 1 && certificates.First().Extensions == null)
            {
                // self-signed certificate
                return new X509Chain { _list = certificates.ToList() };
            }

            var list = new List<X509Certificate>();

            var selfSigned = certificates.Where(c => c.Extensions.OfType<AuthorityKeyIdentifier>().Count() == 0);
            if (selfSigned.Count() > 1)
                throw new InvalidOperationException("There can be only 1 self signed certificate in chain");

            if (certificates.Any(c => c.Extensions.OfType<AuthorityKeyIdentifier>().Count() > 1))
                throw new NotSupportedException();
            if (certificates.Any(c => c.Extensions.OfType<SubjectKeyIdentifier>().Count() != 1))
                throw new NotSupportedException();

            // skip self-signed, they should be grabbed from store
            if (store != null)
                certificates = certificates.Where(c => c.Extensions.OfType<AuthorityKeyIdentifier>().Count() == 1).ToArray();

            if (certificates.Any(c => c.Extensions.OfType<AuthorityKeyIdentifier>().Single().KeyIdentifier == null))
                throw new NotSupportedException();

            if (store != null)
            {
                var rootLinked = certificates.Where(c => FindBySubjectKeyId(certificates, c.Extensions.OfType<AuthorityKeyIdentifier>().Single().KeyIdentifier).Count() == 0).ToArray();
                if (rootLinked.Length != 1)
                    throw new InvalidOperationException("1 certificate should point to root");

                list.Add(rootLinked[0]);
            }
            else
                list.Add(selfSigned.Single());

            while (list.Count < certificates.Count())
            {
                var certs = FindByAuthorityKeyId(certificates, list.Last().Extensions.OfType<SubjectKeyIdentifier>().Single().KeyIdentifier).ToArray();
                if (certs.Length != 1)
                    throw new InvalidOperationException("Cannot form chain");
                list.Add(certs[0]);
            }

            if (store != null)
            {
                var root = store.FindBySubjectKeyId(list.First().Extensions.OfType<AuthorityKeyIdentifier>().Single().KeyIdentifier);
                if (root == null)
                    throw new InvalidOperationException("Cannot find root certificate in store");
                list.Insert(0, root);
            }

            return new X509Chain { _list = list };
        }

        private static IEnumerable<X509Certificate> FindBySubjectKeyId(IEnumerable<X509Certificate> certificates, byte[] subject)
        {
            return certificates.Where(c => c.Extensions.OfType<SubjectKeyIdentifier>().Single().KeyIdentifier.SequenceEqual(subject));
        }

        private static IEnumerable<X509Certificate> FindByAuthorityKeyId(IEnumerable<X509Certificate> certificates, byte[] subject)
        {
            return certificates.Where(c => c.Extensions.OfType<AuthorityKeyIdentifier>().Single().KeyIdentifier.SequenceEqual(subject));
        }

        public bool Validate()
        {
            return true;
        }
    }
}