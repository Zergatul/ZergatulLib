using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public class ContextSpecific : ASN1Element
    {
        public List<ASN1Element> Elements { get; private set; }
        public byte[] Implicit { get; private set; }
        public bool IsImplicit => Implicit != null;

        public ContextSpecific()
        {
            Elements = new List<ASN1Element>();
        }

        protected override void ReadBody(Stream stream)
        {
            if (Tag.ValueType == ASN1ValueType.Primitive)
            {
                Implicit = ReadBuffer(stream, checked((int)Length));
                return;
            }

            if (Tag.ValueType == ASN1ValueType.Constructed)
            {
                while (GetElementsLength(Elements) < Length)
                    Elements.Add(ReadFrom(stream));
            }
        }

        public T As<T>()
            where T : ASN1Element, new()
        {
            if (!IsImplicit)
                throw new ASN1ParseException();

            T result = new T();
            StaticReadBody(result, Implicit);
            return result;
        }
    }
}