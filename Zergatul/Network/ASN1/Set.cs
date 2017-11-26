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

        public Set(params ASN1Element[] elements)
            : base(new ASN1Tag
            {
                Class = ASN1TagClass.Universal,
                ValueType = ASN1ValueType.Constructed,
                Number = ASN1TagNumber.SET
            })
        {
            this.Elements = new List<ASN1Element>();
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