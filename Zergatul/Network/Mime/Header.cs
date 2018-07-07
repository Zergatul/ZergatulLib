using System.Linq;

namespace Zergatul.Network.Mime
{
    public class Header
    {
        public string Key { get; set; }
        public string[] Values { get; set; }

        public Header(string key, string value)
        {
            this.Key = key;
            this.Values = new[] { value };
        }

        public Header(string key, string[] values)
        {
            this.Key = key;
            this.Values = values;
        }
    }
}