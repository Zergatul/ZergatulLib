using System.Collections.Generic;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificate
{
    // TODO: change parsing like other elements
    // https://tools.ietf.org/html/rfc5280
    internal class ASN1CertificateSyntax
    {
        public TBSCertificate TbsCertificate;
        public AlgorithmIdentifier SignatureAlgorithm;
        public BitString SignatureValue;

        public static ASN1CertificateSyntax FromASN1Element(ASN1Element element)
        {
            if (element is Sequence)
            {
                var certificate = (Sequence)element;
                if (certificate.Elements.Count == 3)
                {
                    var tbs = ParseTBSCertificate(certificate.Elements[0]);
                    var sa = new AlgorithmIdentifier(certificate.Elements[1]);
                    var sv = certificate.Elements[2] as BitString;
                    if (tbs != null && sa != null && sv != null)
                        return new ASN1CertificateSyntax
                        {
                            TbsCertificate = tbs,
                            SignatureAlgorithm = sa,
                            SignatureValue = sv
                        };
                }
            }

            return null;
        }

        private static TBSCertificate ParseTBSCertificate(ASN1Element element)
        {
            if (element is Sequence)
            {
                var tbs = (Sequence)element;
                if (7 <= tbs.Elements.Count && tbs.Elements.Count <= 10)
                {
                    int index = 0;
                    var ver = GetContextSpecificInnerElement(tbs.Elements[index++]) as Integer; // ?????
                    var sn = tbs.Elements[index++] as Integer;
                    var sign = new AlgorithmIdentifier(tbs.Elements[index++]);
                    var iss = ParseName(tbs.Elements[index++]);
                    var valid = ParseValidity(tbs.Elements[index++]);
                    var subj = ParseName(tbs.Elements[index++]);
                    var pubkey = ParseSubjectPublicKeyInfo(tbs.Elements[index++]);

                    BitString issUID = null;
                    BitString subjUID = null;
                    Extension[] exts = null;
                    bool extPresent = false;

                    while (index < tbs.Elements.Count)
                    {
                        var cs = tbs.Elements[index++] as ContextSpecific;
                        if (cs.Tag.TagNumberEx == 3)
                        {
                            exts = ParseExtensions(GetContextSpecificInnerElement(cs));
                            extPresent = true;
                        }
                        else
                            throw new System.NotImplementedException();
                    }

                    bool correct =
                        ver != null &&
                        sn != null &&
                        sign != null &&
                        iss != null &&
                        valid != null &&
                        subj != null &&
                        pubkey != null &&
                        (!extPresent || exts != null);
                    if (correct)
                        return new TBSCertificate
                        {
                            Version = ver,
                            SerialNumber = sn,
                            Signature = sign,
                            Issuer = iss,
                            Validity = valid,
                            Subject = subj,
                            SubjectPublicKeyInfo = pubkey,
                            IssuerUniqueID = issUID,
                            SubjectUniqueID = subjUID,
                            Extensions = exts
                        };
                }
            }

            return null;
        }

        private static Name ParseName(ASN1Element element)
        {
            if (element is Sequence)
            {
                var n = (Sequence)element;
                bool allSet = true;
                for (int i = 0; i < n.Elements.Count; i++)
                    if (!(n.Elements[i] is Set))
                    {
                        allSet = false;
                        break;
                    }
                if (allSet)
                {
                    var sets = new List<RelativeDistinguishedName[]>();
                    for (int i = 0; i < n.Elements.Count; i++)
                    {
                        var rdns = new List<RelativeDistinguishedName>();
                        var set = (Set)n.Elements[i];
                        for (int j = 0; j < set.Elements.Count; j++)
                        {
                            var seq = set.Elements[j] as Sequence;
                            if (seq == null)
                                return null;
                            if (seq.Elements.Count != 2)
                                return null;
                            var type = seq.Elements[0] as ObjectIdentifier;
                            if (type == null)
                                return null;
                            var value = seq.Elements[1] as ASN1StringElement;
                            if (value == null)
                                return null;
                            rdns.Add(new RelativeDistinguishedName
                            {
                                Type = type,
                                Value = value
                            });
                        }
                        sets.Add(rdns.ToArray());
                    }

                    return new Name
                    {
                        RDN = sets.ToArray()
                    };
                }
            }

            return null;
        }

        private static Validity ParseValidity(ASN1Element element)
        {
            if (element is Sequence)
            {
                var v = (Sequence)element;
                if (v.Elements.Count == 2)
                {
                    var notbefore = v.Elements[0] as ASN1TimeElement;
                    var notafter = v.Elements[1] as ASN1TimeElement;
                    if (notbefore != null && notafter != null)
                        return new Validity
                        {
                            NotBefore = notbefore,
                            NotAfter = notafter
                        };
                }
            }

            return null;
        }

        private static SubjectPublicKeyInfo ParseSubjectPublicKeyInfo(ASN1Element element)
        {
            if (element is Sequence)
            {
                var spki = (Sequence)element;
                if (spki.Elements.Count == 2)
                {
                    var algo = new AlgorithmIdentifier(spki.Elements[0]);
                    var pubkey = spki.Elements[1] as BitString;
                    if (algo != null && pubkey != null)
                        return new SubjectPublicKeyInfo
                        {
                            Algorithm = algo,
                            SubjectPublicKey = pubkey
                        };
                }
            }

            return null;
        }

        private static Extension[] ParseExtensions(ASN1Element element)
        {
            var seq = element as Sequence;
            if (seq != null && seq.Elements.Count > 0)
            {
                var extensions = new Extension[seq.Elements.Count];
                for (int i = 0; i < extensions.Length; i++)
                {
                    var extension = ParseExtension(seq.Elements[i]);
                    if (extension == null)
                        return null;
                    extensions[i] = extension;
                }
                return extensions;
            }

            return null;
        }

        private static Extension ParseExtension(ASN1Element element)
        {
            var seq = element as Sequence;

            if (seq != null && seq.Elements.Count == 2)
            {
                var extnID = seq.Elements[0] as ObjectIdentifier;
                var extnValue = seq.Elements[1] as OctetString;
                if (extnID != null && extnValue != null)
                    return new Extension
                    {
                        ExtnID = extnID,
                        Critical = new Boolean(false),
                        ExtnValue = extnValue
                    };
            }

            if (seq != null && seq.Elements.Count == 3)
            {
                var extnID = seq.Elements[0] as ObjectIdentifier;
                var critical = seq.Elements[1] as Boolean;
                var extnValue = seq.Elements[2] as OctetString;
                if (extnID != null && critical != null && extnValue != null)
                    return new Extension
                    {
                        ExtnID = extnID,
                        Critical = critical,
                        ExtnValue = extnValue
                    };
            }

            return null;
        }

        private static ASN1Element GetContextSpecificInnerElement(ASN1Element element)
        {
            var cs = element as ContextSpecific;
            if (cs != null && cs.Elements.Count > 0)
                return cs.Elements[0];

            return null;
        }

        #region Nested Classes

        internal class TBSCertificate
        {
            public Integer Version;
            public Integer SerialNumber;
            public AlgorithmIdentifier Signature;
            public Name Issuer;
            public Validity Validity;
            public Name Subject;
            public SubjectPublicKeyInfo SubjectPublicKeyInfo;
            public BitString IssuerUniqueID;
            public BitString SubjectUniqueID;
            public Extension[] Extensions;
        }

        internal class Name
        {
            public RelativeDistinguishedName[][] RDN;
        }

        internal class RelativeDistinguishedName
        {
            public ObjectIdentifier Type;
            public ASN1StringElement Value;
        }

        internal class Validity
        {
            public ASN1TimeElement NotBefore;
            public ASN1TimeElement NotAfter;
        }

        internal class SubjectPublicKeyInfo
        {
            public AlgorithmIdentifier Algorithm;
            public BitString SubjectPublicKey;
        }

        internal class Extension
        {
            public ObjectIdentifier ExtnID;
            public Boolean Critical;
            public OctetString ExtnValue;
        }

        #endregion
    }
}