using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public class Set : ASN1Element
    {
        public List<ASN1Element> Elements { get; private set; }

        public Set()
        {
            Elements = new List<ASN1Element>();
        }

        protected override void ReadBody(Stream stream)
        {
            while (GetElementsLength(Elements) < Length)
            {
                var element = ReadFrom(stream);
                if (Elements.Count == 0 || element.GetType() == Elements[0].GetType())
                    Elements.Add(element);
                else
                    throw new InvalidOperationException("Type of subelement of SET is invalid");
            }
        }
    }
}