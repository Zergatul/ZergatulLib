﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1
{
    public class Sequence : Asn1Element
    {
        public List<Asn1Element> Elements { get; private set; }

        public Sequence(params Asn1Element[] elements)
            : base(new Asn1Tag
            {
                Class = Asn1TagClass.Universal,
                ValueType = Asn1ValueType.Constructed,
                Number = Asn1TagNumber.SEQUENCE
            })
        {
            this.Elements = new List<Asn1Element>();
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
            {
                var element = ReadFrom(stream);
                _raw.AddRange(element.Raw);
                Elements.Add(element);
            }
        }
    }
}