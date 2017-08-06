using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Test
{
    static class DownloadOIDs
    {
        public static void Go(string parent, string filename)
        {
            WebClient wc = new WebClient();
            byte[] data = wc.DownloadData($"http://oid-info.com/get/{parent}");
            var enc = Encoding.GetEncoding("ISO-8859-1");
            var html = enc.GetString(data);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            HtmlAgilityPack.HtmlNode strong;
            IEnumerable<string> links;

            var select = doc.DocumentNode.Descendants("select").Where(s => s.Attributes["name"].Value == "childNodes").SingleOrDefault();
            if (select != null)
            {
                links = select.Elements("option").Skip(1).Select(o => o.Attributes["value"].Value);
            }
            else
            {
                strong = doc.DocumentNode.Descendants("strong").Where(e => e.InnerText.Contains("child OIDs:")).Single();
                links = strong.ParentNode.Elements("a").Select(a => a.Attributes["href"].Value);
            }

            var xdoc = new XDocument();
            xdoc.Add(new XElement("root"));

            foreach (var link in links)
            {
                data = wc.DownloadData(link);
                var doc2 = new HtmlAgilityPack.HtmlDocument();
                doc2.LoadHtml(enc.GetString(data));

                var code = doc2.DocumentNode.Descendants("code").First();
                string text = code.InnerText.Trim();
                var parts = text.Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

                string id, value;
                if (parts.Length == 1)
                    id = value = parts[0];
                else if (parts.Length == 2)
                {
                    id = parts[0].Trim();
                    value = parts[1].Trim();
                }
                else
                    throw new InvalidOperationException();

                strong = doc2.DocumentNode.Descendants("strong").Where(e => e.InnerText.Contains("Description")).Last();
                HtmlAgilityPack.HtmlNode td = strong.ParentNode.NextSibling;
                do
                {
                    td = td.NextSibling;
                }
                while (td.Name != "td");

                string desc = td.InnerText.Trim();

                xdoc.Root.Add(new XElement("oid",
                    new XAttribute("value", value),
                    new XAttribute("id", id),
                    new XAttribute("class", id),
                    new XAttribute("desc", desc)));
            }

            File.WriteAllText(filename, xdoc.ToString(SaveOptions.None));
        }
    }
}