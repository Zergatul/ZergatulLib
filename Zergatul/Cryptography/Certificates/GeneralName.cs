using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificates
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#page-38
    /// </summary>
    public class GeneralName
    {
        public OtherName OtherName { get; private set; }
        public string RFC822Name { get; private set; }
        public string DNSName { get; private set; }
        public object X400Address { get; private set; }
        public object DirectoryName { get; private set; }
        public object EDIPartyName { get; private set; }
        public string UniformResourceIdentifier { get; private set; }
        public IPAddress IPAddress { get; private set; }
        public OID RegisteredID { get; private set; }

        internal GeneralName(ASN1Element element)
        {
            var cs = element as ContextSpecific;

            CertificateParseException.ThrowIfFalse(cs != null);
            CertificateParseException.ThrowIfFalse(cs.IsImplicit);

            switch (cs.Tag.TagNumberEx)
            {
                case 0:
                    CertificateParseException.ThrowIfFalse(cs.IsImplicit);
                    OtherName = new OtherName(ASN1Element.ReadFrom(cs.Implicit));
                    break;
                case 1:
                    CertificateParseException.ThrowIfTrue(cs.IsImplicit);
                    CertificateParseException.ThrowIfFalse(cs.Elements.Count != 1);
                    CertificateParseException.ThrowIfFalse(cs.Elements[0] is IA5String);
                    RFC822Name = ((IA5String)cs.Elements[0]).Value;
                    break;
                case 2:
                    CertificateParseException.ThrowIfTrue(cs.IsImplicit);
                    CertificateParseException.ThrowIfFalse(cs.Elements.Count != 1);
                    CertificateParseException.ThrowIfFalse(cs.Elements[0] is IA5String);
                    DNSName = ((IA5String)cs.Elements[0]).Value;
                    break;
                case 3:
                    throw new NotImplementedException();
                case 4:
                    throw new NotImplementedException();
                case 5:
                    throw new NotImplementedException();
                case 6:
                    CertificateParseException.ThrowIfTrue(cs.IsImplicit);
                    CertificateParseException.ThrowIfFalse(cs.Elements.Count != 1);
                    CertificateParseException.ThrowIfFalse(cs.Elements[0] is IA5String);
                    UniformResourceIdentifier = ((IA5String)cs.Elements[0]).Value;
                    break;
                case 7:
                    CertificateParseException.ThrowIfTrue(cs.IsImplicit);
                    CertificateParseException.ThrowIfFalse(cs.Elements.Count != 1);
                    CertificateParseException.ThrowIfFalse(cs.Elements[0] is OctetString);
                    throw new NotImplementedException();
                case 8:
                    CertificateParseException.ThrowIfTrue(cs.IsImplicit);
                    CertificateParseException.ThrowIfFalse(cs.Elements.Count != 1);
                    CertificateParseException.ThrowIfFalse(cs.Elements[0] is ObjectIdentifier);
                    RegisteredID = ((ObjectIdentifier)cs.Elements[0]).OID;
                    break;
                case 9:
                    throw new CertificateParseException();
            }
        }
    }
}