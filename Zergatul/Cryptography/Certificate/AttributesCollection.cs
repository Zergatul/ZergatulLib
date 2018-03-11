using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zergatul.Network;
using Zergatul.Network.Asn1.Structures.X509;

namespace Zergatul.Cryptography.Certificate
{
    public class AttributesCollection : IReadOnlyCollection<AttributePair<OID, string>>
    {
        #region Private

        private List<AttributePair<string, string>> _strList;
        private List<AttributePair<OID, string>> _oidList;
        private string _value;

        #endregion

        #region .ctor

        internal AttributesCollection(Name name)
        {
            this._strList = new List<AttributePair<string, string>>();
            this._oidList = new List<AttributePair<OID, string>>();

            foreach (var rnd in name.RDN)
                foreach (var attr in rnd.Attributes)
                {
                    string key = attr.Type.ShortName ?? "OID." + attr.Type.DotNotation;
                    _strList.Add(new AttributePair<string, string>(key, attr.Value.Value));
                    _oidList.Add(new AttributePair<OID, string>(attr.Type, attr.Value.Value));
                }

            this._value = string.Join(", ",
                name.RDN.SelectMany(rdn => rdn.Attributes).Reverse().Select(r => (r.Type.ShortName ?? "OID." + r.Type.DotNotation) + "=" + r.Value.Value));
        }

        #endregion

        #region Public

        public override string ToString() => _value;

        public IEnumerable<string> GetValuesByString(string key)
        {
            return _strList.Where(p => p.Key == key).Select(p => p.Value);
        }

        public IEnumerable<string> GetValuesByOID(OID key)
        {
            return _oidList.Where(p => p.Key == key).Select(p => p.Value);
        }

        public string this[string key] => GetValuesByString(key).SingleOrDefault();

        public string this[OID key] => GetValuesByOID(key).SingleOrDefault();

        #endregion

        #region IReadOnlyCollection

        public int Count => _strList.Count;
        public IEnumerator<AttributePair<OID, string>> GetEnumerator() => _oidList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}