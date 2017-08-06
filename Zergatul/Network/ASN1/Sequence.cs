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

        public Sequence(params ASN1Element[] elements)
            : base(new ASN1Tag
            {
                Class = ASN1TagClass.Universal,
                ValueType = ASN1ValueType.Constructed,
                Number = ASN1TagNumber.SEQUENCE
            })
        {
            this.Elements = new List<ASN1Element>();
            if (elements != null)
                this.Elements.AddRange(elements);
        }

        protected override byte[] BodyToBytes()
        {
            using (var ms = new MemoryStream())
            {
                for (int i = 0; i < Elements.Count; i++)
                {
                    byte[] buffer = Elements[i].ToBytes();
                    ms.Write(buffer, 0, buffer.Length);
                }
                return ms.ToArray();
            }
        }

        protected override void ReadBody(Stream stream)
        {
            while (GetElementsLength(Elements) < Length)
                Elements.Add(ReadFrom(stream));
        }
    }
}
