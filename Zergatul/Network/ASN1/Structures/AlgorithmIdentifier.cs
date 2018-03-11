﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.Asn1;

namespace Zergatul.Network.Asn1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.1.1.2
    /// </summary>
    public class AlgorithmIdentifier
    {
        public OID Algorithm { get; private set; }
        public Asn1Element Parameters { get; private set; }

        private AlgorithmIdentifier()
        {

        }

        public AlgorithmIdentifier(OID algorithm, Asn1Element parameters)
        {
            this.Algorithm = algorithm;
            this.Parameters = parameters;
        }

        public Asn1Element ToASN1()
        {
            return new Sequence(new ObjectIdentifier(Algorithm), Parameters);
        }

        public static AlgorithmIdentifier Parse(Asn1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotInRange(seq.Elements.Count, 1, 2);

            var oi = seq.Elements[0] as ObjectIdentifier;
            ParseException.ThrowIfNull(oi);

            return new AlgorithmIdentifier
            {
                Algorithm = oi.OID,
                Parameters = seq.Elements.Count == 1 ? null : seq.Elements[1]
            };
        }
    }
}