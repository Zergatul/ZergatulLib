﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1.Structures.X509
{
    class Validity
    {
        public DateTime NotBefore { get; private set; }
        public DateTime NotAfter { get; private set; }

        public static Validity Parse(Asn1Element element)
        {
            var result = new Validity();

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 2);

            var date = seq.Elements[0] as Asn1TimeElement;
            ParseException.ThrowIfNull(date);
            result.NotBefore = date.Date;

            date = seq.Elements[1] as Asn1TimeElement;
            ParseException.ThrowIfNull(date);
            result.NotAfter = date.Date;

            return result;
        }
    }
}