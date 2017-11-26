using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5915#section-3
    /// </summary>
    public class ECPrivateKey
    {
        public int Version { get; private set; }
        public byte[] PrivateKey { get; private set; }
        public ECParameters Parameters { get; private set; }
        public byte[] PublicKey { get; private set; }

        public static ECPrivateKey Parse(ASN1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotInRange(seq.Elements.Count, 2, 4);

            var ver = seq.Elements[0] as Integer;
            ParseException.ThrowIfNull(ver);

            var privkey = seq.Elements[1] as OctetString;
            ParseException.ThrowIfNull(privkey);

            ECParameters parameters = null;
            BitString pubkey = null;
            for (int i = 2; i < seq.Elements.Count; i++)
            {
                var cs = seq.Elements[i] as ContextSpecific;
                ParseException.ThrowIfNull(cs);
                ParseException.ThrowIfTrue(cs.IsImplicit);
                ParseException.ThrowIfNotEqual(cs.Elements.Count, 1);

                switch (cs.Tag.TagNumberEx)
                {
                    case 0:
                        parameters = ECParameters.Parse(cs.Elements[0]);
                        break;
                    case 1:
                        pubkey = cs.Elements[0] as BitString;
                        ParseException.ThrowIfNull(pubkey);
                        break;
                    default:
                        throw new ParseException();
                }
            }

            var result = new ECPrivateKey();

            result.Version = (int)ver.Value;
            result.PrivateKey = privkey.Data;
            result.Parameters = parameters;
            result.PublicKey = pubkey?.Data;

            return result;
        }
    }
}