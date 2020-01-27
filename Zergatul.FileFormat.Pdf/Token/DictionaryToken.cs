using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.FileFormat.Pdf.Token
{
    internal class DictionaryToken : TokenBase
    {
        private Dictionary<string, TokenBase> _dictionary;

        public DictionaryToken(Dictionary<string, TokenBase> dictionary)
        {
            _dictionary = dictionary;
        }
    }
}