﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#page-47
    /// </summary>
    public class DistributionPointName
    {
        public GeneralNames FullName { get; private set; }
        public RelativeDistinguishedName NameRelativeToCRLIssuer { get; private set; }

        internal DistributionPointName(Asn1Element element)
        {
            var cs = element as ContextSpecific;
            CertificateParseException.ThrowIfFalse(cs != null);

            switch (cs.Tag.TagNumberEx)
            {
                case 0:
                    FullName = new GeneralNames(cs.Elements[0]);
                    break;
                case 1:
                    NameRelativeToCRLIssuer = new RelativeDistinguishedName(cs.Elements[0]);
                    break;
                default:
                    throw new CertificateParseException();
            }
        }
    }
}