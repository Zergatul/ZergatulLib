using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.ASN1.Structures.X509;

namespace Zergatul.Cryptography.Certificate
{
    public class AttributesCollection : IReadOnlyDictionary<string, string>
    {
        private Dictionary<string, string> _strDictionary;
        private Dictionary<OID, string> _oidDictionary;
        private string _value;

        internal AttributesCollection(Name name)
        {
            this._strDictionary = new Dictionary<string, string>();
            this._oidDictionary = new Dictionary<OID, string>();

            foreach (var rnd in name.RDN)
                foreach (var attr in rnd.Attributes)
                {
                    string key = attr.Type.ShortName ?? "OID." + attr.Type.DotNotation;
                    _strDictionary.Add(key, attr.Value.Value);
                    _oidDictionary.Add(attr.Type, attr.Value.Value);
                }

            this._value = string.Join(", ",
                name.RDN.SelectMany(rdn => rdn.Attributes).Reverse().Select(r => (r.Type.ShortName ?? "OID." + r.Type.DotNotation) + "=" + r.Value.Value));
        }

        public override string ToString() => _value;

        #region IReadOnlyDictionary<string, string>

        public string this[string key] => _strDictionary[key];

        public int Count => _strDictionary.Count;

        public IEnumerable<string> Keys => _strDictionary.Keys;

        public IEnumerable<string> Values => _strDictionary.Values;

        public bool ContainsKey(string key) => _strDictionary.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _strDictionary.GetEnumerator();

        public bool TryGetValue(string key, out string value) => _strDictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}