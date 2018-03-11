using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1
{
    public class Set : Asn1Element
    {
        public List<Asn1Element> Elements { get; private set; }

        public Set(params Asn1Element[] elements)
            : base(new Asn1Tag
            {
                Class = Asn1TagClass.Universal,
                ValueType = Asn1ValueType.Constructed,
                Number = Asn1TagNumber.SET
            })
        {
            this.Elements = new List<Asn1Element>();
            if (elements != null)
                this.Elements.AddRange(elements);
        }

        protected override byte[] BodyToBytes()
        {
            var list = new List<byte>();
            foreach (var element in Elements)
                list.AddRange(element.Raw);
            return list.ToArray();
        }

        protected override void ReadBody(Stream stream)
        {
            while (GetElementsLength(Elements) < Length)
            {
                var element = ReadFrom(stream);
                _raw.AddRange(element.Raw);
                if (Elements.Count == 0 || element.GetType() == Elements[0].GetType())
                    Elements.Add(element);
                else
                    throw new InvalidOperationException("Type of subelement of SET is invalid");
            }
        }
    }
}