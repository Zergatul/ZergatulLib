using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificates
{
    internal class ASN1CertificateSyntax
    {
        public object TBSCertificate;
        public AlgorithmIdentifier SignatureAlgorithm;
        public BitString SignatureValue;

        public static ASN1CertificateSyntax FromASN1Element(ASN1Element element)
        {
            return null;
        }
    }

    internal class TBSCertificate
    {
        public Integer Version;
        public Integer SerialNumber;
        public AlgorithmIdentifier Signature;
        public object Issuer;
        public Validity Validity;
        public object Subject;
        public SubjectPublicKeyInfo SubjectPublicKeyInfo;
        public BitString IssuerUniqueID;
        public BitString SubjectUniqueID;
        public Extension[] Extensions;
    }

    internal class AlgorithmIdentifier
    {

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

    }
}