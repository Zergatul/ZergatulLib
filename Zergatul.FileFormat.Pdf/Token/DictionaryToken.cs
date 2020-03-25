using System;
using System.Collections.Generic;
using System.Linq;

namespace Zergatul.FileFormat.Pdf.Token
{
    internal class DictionaryToken : TokenBase
    {
        public override bool IsBasic => true;

        private Dictionary<string, DictionaryEntry> _dictionary;

        public DictionaryToken(Dictionary<string, DictionaryEntry> dictionary)
        {
            _dictionary = dictionary;
        }

        public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

        public bool Is<T>(string key)
            where T : TokenBase
        {
            return _dictionary[key].Token is T;
        }

        public ArrayToken GetArray(string key) => (ArrayToken)_dictionary[key].Token;

        public ArrayToken GetArrayNullable(string key)
        {
            if (_dictionary.TryGetValue(key, out DictionaryEntry entry))
            {
                return (ArrayToken)entry.Token;
            }
            else
            {
                return null;
            }
        }

        public IndirectObject GetIndirectObject(string key)
        {
            return new IndirectObject((IndirectReferenceToken)_dictionary[key].Token);
        }

        public IndirectObject GetIndirectObjectNullable(string key)
        {
            if (_dictionary.TryGetValue(key, out DictionaryEntry entry))
            {
                return new IndirectObject((IndirectReferenceToken)entry.Token);
            }
            else
            {
                return null;
            }
        }

        public long GetInteger(string key)
        {
            return ((IntegerToken)_dictionary[key].Token).Value;
        }

        public long? GetIntegerNullable(string key)
        {
            if (_dictionary.TryGetValue(key, out DictionaryEntry entry))
            {
                return ((IntegerToken)entry.Token).Value;
            }
            else
            {
                return null;
            }
        }

        public string GetName(string key)
        {
            return ((NameToken)_dictionary[key].Token).Value;
        }

        public TokenBase GetToken(string key) => _dictionary[key].Token;

        public TokenBase GetTokenNullable(string key) => _dictionary.ContainsKey(key) ? _dictionary[key].Token : null;

        public bool TryGet<T>(string key, out T result)
            where T : TokenBase
        {
            if (_dictionary.TryGetValue(key, out DictionaryEntry entry))
                result = entry.Token as T;
            else
                result = null;

            return result != null;
        }

        public bool ValidateName(string key, string value)
        {
            if (_dictionary.TryGetValue(key, out DictionaryEntry entry))
            {
                var name = entry.Token as NameToken;
                return name?.Value == value;
            }
            else
            {
                return false;
            }
        }

#if DEBUG
        public override string ToString()
        {
            var entries = _dictionary.OrderBy(kv => kv.Value.Order).Select(kv => $"{{ /{kv.Key} {kv.Value.Token} }}");
            return "<< " + string.Join(", ", entries) + " >>";
        }
#endif
    }
}