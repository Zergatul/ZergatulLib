using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificate.Extensions
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.2.1.9
    /// </summary>
    public class BasicConstraints : X509Extension
    {
        public bool CA { get; private set; }
        public int? PathLenConstraint { get; private set; }

        protected override void Parse(byte[] data)
        {
            var element = ASN1Element.ReadFrom(data);

            var seq = element as Sequence;
            CertificateParseException.ThrowIfNull(seq);


            if (seq.Elements.Count >= 1)
            {
                CertificateParseException.ThrowIfFalse(seq.Elements[0] is Boolean);
                CA = ((Boolean)seq.Elements[0]).Value;
            }
            else
                CA = false;

            if (seq.Elements.Count >= 2)
            {
                CertificateParseException.ThrowIfFalse(seq.Elements[1] is Integer);
                PathLenConstraint = (int)((Integer)seq.Elements[1]).Value;
            }
        }
    }
}