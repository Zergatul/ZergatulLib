using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc2315#section-7
    /// </summary>
    public class ContentInfo
    {
        public OID ContentType { get; private set; }
        public byte[] Data { get; private set; }

        public static ContentInfo TryParse(ASN1Element element)
        {
            var seq = element as Sequence;
            if (seq == null || seq.Elements.Count < 2)
                return null;

            var oi = seq.Elements[0] as ObjectIdentifier;
            if (oi == null)
                return null;

            var cs = seq.Elements[1] as ContextSpecific;
            if (cs == null || cs.IsImplicit || cs.Tag.TagNumberEx != 0)
                return null;
            var content = cs.Elements[0];

            if (oi.OID == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS7.Data)
            {
                var os = content as OctetString;
                if (os == null)
                    return null;
                return new ContentInfo
                {
                    ContentType = oi.OID,
                    Data = os.Raw
                };
            }
            else
                throw new NotSupportedException();
        }
    }
}