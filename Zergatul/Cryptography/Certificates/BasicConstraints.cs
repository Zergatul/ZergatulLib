using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificates
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.2.1.9
    /// </summary>
    public class BasicConstraints : X509Extension
    {
        public bool CA { get; private set; }
        public int? PathLenConstraint { get; private set; }

        protected override void Parse(OctetString data)
        {
            var element = ASN1Element.ReadFrom(data.Raw);
            var seq = element as Sequence;
            if (seq != null && (seq.Elements.Count == 1 || seq.Elements.Count == 2))
            {
                var b = seq.Elements[0] as Boolean;
                if (b != null)
                {
                    CA = b.Value;
                    if (seq.Elements.Count == 2)
                    {
                        var i = seq.Elements[1] as Integer;
                        if (i != null)
                        {
                            PathLenConstraint = (int)i.Value;
                            return;
                        }
                    }
                    else
                        return;
                }
            }

            throw new System.InvalidOperationException();
        }
    }
}
