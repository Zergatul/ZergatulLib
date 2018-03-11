using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1
{
    public class ContextSpecific : Asn1Element
    {
        public List<Asn1Element> Elements { get; private set; }
        public byte[] Implicit { get; private set; }
        public bool IsImplicit => Implicit != null;

        public ContextSpecific()
            : base(new Asn1Tag
            {
                /*Class = ASN1TagClass.Universal,
                ValueType = ASN1ValueType.Primitive,
                Number = ASN1TagNumber.*/
            })
        {
            Elements = new List<Asn1Element>();
        }

        protected override byte[] BodyToBytes()
        {
            throw new NotImplementedException();
        }

        protected override void ReadBody(Stream stream)
        {
            if (Tag.ValueType == Asn1ValueType.Primitive)
            {
                Implicit = ReadBuffer(stream, checked((int)Length));
                _raw.AddRange(Implicit);
                return;
            }

            if (Tag.ValueType == Asn1ValueType.Constructed)
            {
                while (GetElementsLength(Elements) < Length)
                {
                    var element = ReadFrom(stream);
                    _raw.AddRange(element.Raw);
                    Elements.Add(element);
                }
            }
        }

        public T As<T>()
            where T : Asn1Element, new()
        {
            if (!IsImplicit)
                throw new Asn1ParseException();

            T result = new T();
            StaticReadBody(result, Implicit);
            return result;
        }
    }
}