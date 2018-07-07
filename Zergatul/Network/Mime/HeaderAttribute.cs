using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Mime
{
    public class HeaderAttribute
    {
        public string Key;
        public string Value;

        public HeaderAttribute(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}