using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificate.Extensions
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.2.1.12
    /// </summary>
    public class ExtendedKeyUsage : X509Extension
    {
        public OID[] KeyPurposes { get; private set; }

        protected override void Parse(byte[] data)
        {
            var element = ASN1Element.ReadFrom(data);
            var seq = element as Sequence;
            if (seq != null)
            {
                KeyPurposes = seq.Elements.Cast<ObjectIdentifier>().Select(oi => oi.OID).ToArray();
                return;
            }

            throw new InvalidOperationException();
        }
    }
}