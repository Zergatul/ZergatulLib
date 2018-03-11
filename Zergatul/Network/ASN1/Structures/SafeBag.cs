using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1.Structures
{
    public class SafeBag
    {
        public OID Id { get; private set; }
        public EncryptedPrivateKeyInfo PKCS8ShroudedKeyBag { get; private set; }
        public CertBag CertBag { get; private set; }
        public IReadOnlyCollection<PKCS12Attribute> Attributes => _attributes?.AsReadOnly();

        private List<PKCS12Attribute> _attributes;

        public static SafeBag Parse(Asn1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotInRange(seq.Elements.Count, 2, 3);

            var result = new SafeBag();

            var oi = seq.Elements[0] as ObjectIdentifier;
            ParseException.ThrowIfNull(oi);
            result.Id = oi.OID;

            var cs = seq.Elements[1] as ContextSpecific;
            ParseException.ThrowIfNull(cs);
            ParseException.ThrowIfTrue(cs.IsImplicit);
            ParseException.ThrowIfNotEqual(cs.Elements.Count, 1);

            if (result.Id == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS12.Version1.BagIds.PKCS8ShroudedKeyBag)
                result.PKCS8ShroudedKeyBag = EncryptedPrivateKeyInfo.Parse(cs.Elements[0]);
            else if (result.Id == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS12.Version1.BagIds.CertBag)
                result.CertBag = CertBag.Parse(cs.Elements[0]);
            else
                throw new NotImplementedException();

            if (seq.Elements.Count == 3)
            {
                var set = seq.Elements[2] as Set;
                ParseException.ThrowIfNull(set);
                result._attributes = set.Elements.Select(e => PKCS12Attribute.Parse(e)).ToList();
            }

            return result;
        }
    }
}