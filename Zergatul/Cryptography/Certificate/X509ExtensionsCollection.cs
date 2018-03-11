using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Asn1.Structures.X509;

namespace Zergatul.Cryptography.Certificate
{
    public class X509ExtensionsCollection : IReadOnlyList<X509Extension>
    {
        private List<X509Extension> _extensions;

        internal X509ExtensionsCollection(IEnumerable<Extension> extensions)
        {
            if (extensions != null)
                this._extensions = extensions.Select(e => X509Extension.Parse(e)).Where(ext => ext != null).ToList();
            else
                this._extensions = new List<X509Extension>();
        }

        public T Get<T>() where T : X509Extension
        {
            return _extensions.OfType<T>().SingleOrDefault();
        }

        #region IReadOnlyList<X509Extension>

        public X509Extension this[int index] => _extensions[index];

        public int Count => _extensions.Count;

        public IEnumerator<X509Extension> GetEnumerator() => _extensions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}