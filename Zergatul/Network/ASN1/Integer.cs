﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Network.ASN1
{
    public class Integer : ASN1Element
    {
        public byte[] Raw { get; private set; }
        public BigInteger Value => new BigInteger(Raw, ByteOrder.BigEndian);

        public Integer()
            : base(new ASN1Tag
            {
                Class = ASN1TagClass.Universal,
                ValueType = ASN1ValueType.Primitive,
                Number = ASN1TagNumber.INTEGER
            })
        {

        }

        public Integer(BigInteger value)
            : this()
        {
            Raw = value.ToBytes(ByteOrder.BigEndian);
        }

        protected override byte[] BodyToBytes()
        {
            return Raw;
        }

        protected override void ReadBody(byte[] data)
        {
            Raw = data;
        }
    }
}