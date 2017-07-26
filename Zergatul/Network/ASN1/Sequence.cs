using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public class Sequence : ASN1Element
    {
        public List<ASN1Element> Elements { get; private set; }

        public Sequence()
        {
            Elements = new List<ASN1Element>();
        }

        protected override void ReadBody(Stream stream)
        {
            while (GetElementsLength() < Length)
                Elements.Add(ReadFrom(stream));
        }

        private ulong GetElementsLength()
        {
            ulong length = 0;
            for (int i = 0; i < Elements.Count; i++)
                length += Elements[i].Length;
            return length;
        }
    }
}
