using System;
using System.Collections.Generic;

namespace Zergatul.FileFormat.Pdf.Token
{
    internal class DictionaryToken : TokenBase
    {
        public override bool IsBasic => true;

        private Dictionary<string, TokenBase> _dictionary;

        public DictionaryToken(Dictionary<string, TokenBase> dictionary)
        {
            _dictionary = dictionary;
        }

        public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

        public bool Is<T>(string key)
            where T : TokenBase
        {
            return _dictionary[key] is T;
        }

        public ArrayToken GetArrayNullable(string key)
        {
            if (_dictionary.TryGetValue(key, out TokenBase token))
            {
                return (ArrayToken)_dictionary[key];
            }
            else
            {
                return null;
            }
        }

        public IndirectObject GetIndirectObject(string key)
        {
            return new IndirectObject((IndirectReferenceToken)_dictionary[key]);
        }

        public IndirectObject GetIndirectObjectNullable(string key)
        {
            if (_dictionary.TryGetValue(key, out TokenBase token))
            {
                return new IndirectObject((IndirectReferenceToken)_dictionary[key]);
            }
            else
            {
                return null;
            }
        }

        public long GetInteger(string key)
        {
            return ((IntegerToken)_dictionary[key]).Value;
        }

        public long? GetIntegerNullable(string key)
        {
            if (_dictionary.TryGetValue(key, out TokenBase token))
            {
                return ((IntegerToken)token).Value;
            }
            else
            {
                return null;
            }
        }

        public TokenBase GetToken(string key) => _dictionary[key];

        public TokenBase GetTokenNullable(string key) => _dictionary.ContainsKey(key) ? _dictionary[key] : null;
    }
}