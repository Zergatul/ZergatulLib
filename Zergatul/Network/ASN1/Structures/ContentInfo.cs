using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc2315#section-7
    /// </summary>
    public class ContentInfo
    {
        public OID ContentType { get; private set; }
        public byte[] Data { get; private set; }
        public EncryptedData EncryptedData { get; private set; }
        public SafeBag[] Bags { get; private set; }

        public static ContentInfo Parse(Asn1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 2);

            var oi = seq.Elements[0] as ObjectIdentifier;
            ParseException.ThrowIfNull(oi);

            var cs = seq.Elements[1] as ContextSpecific;
            ParseException.ThrowIfNull(cs);
            ParseException.ThrowIfTrue(cs.IsImplicit);
            ParseException.ThrowIfNotEqual(cs.Tag.TagNumberEx, 0);
            ParseException.ThrowIfNotEqual(cs.Elements.Count, 1);

            if (oi.OID == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS7.Data)
            {
                var os = cs.Elements[0] as OctetString;
                ParseException.ThrowIfNull(os);

                return new ContentInfo
                {
                    ContentType = oi.OID,
                    Data = os.Data
                };
            }
            else if (oi.OID == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS7.EncryptedData)
            {
                return new ContentInfo
                {
                    ContentType = oi.OID,
                    EncryptedData = EncryptedData.Parse(cs.Elements[0])
                };
            }
            else
                throw new NotSupportedException();
        }

        public void Decrypt(string password)
        {
            if (EncryptedData != null)
                Data = EncryptedData.Decrypt(password);

            var element = Asn1Element.ReadFrom(Data);
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);

            Bags = seq.Elements.Select(e => SafeBag.Parse(e)).ToArray();

            for (int i = 0; i < Bags.Length; i++)
                if (Bags[i].PKCS8ShroudedKeyBag != null)
                    Bags[i].PKCS8ShroudedKeyBag.Decrypt(password);
        }
    }
}