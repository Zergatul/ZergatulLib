using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.ASN1;
using Zergatul.Network.ASN1.Structures.PKCS7;

namespace Zergatul.Network.ASN1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5652
    /// </summary>
    public class CryptographicMessageSyntax
    {
        private SignedData _signedData;

        public static CryptographicMessageSyntax Parse(ASN1Element element)
        {
            var result = new CryptographicMessageSyntax();

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 2);

            var oid = seq.Elements[0] as ObjectIdentifier;
            ParseException.ThrowIfNull(oid);
            var contentType = oid.OID;

            if (contentType == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS7.Data)
            {
                throw new NotImplementedException();
            }
            else if (contentType == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS7.SignedData)
            {
                var cs = seq.Elements[1] as ContextSpecific;
                ParseException.ThrowIfNull(cs);
                ParseException.ThrowIfTrue(cs.IsImplicit);
                result._signedData = SignedData.Parse(cs.Elements[0]);
            }
            else
                throw new NotImplementedException();

            return result;
        }

        public bool ValidateSignatures()
        {
            if (_signedData == null || _signedData.SignerInfos.Length == 0)
                throw new InvalidOperationException("Cannot validate signature, since message doesn't contain it");

            foreach (var si in _signedData.SignerInfos)
                if (!ValidateSignerInfo(si))
                    return false;

            return true;
        }

        private bool ValidateSignerInfo(SignerInfo si)
        {
            var cert = _signedData.Certificates.SingleOrDefault(c => c.SerialNumber.SequenceEqual(si.SID.IssuerAndSerialNumber.SerialNumber));
            if (cert == null)
                throw new InvalidOperationException("Cannot find certificate");

            if (!ValidateMessageDigest(si))
                return false;

            var algorithm = cert.PublicKey.ResolveAlgorithm();
            if (si.SignatureAlgorithm.Algorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.RSA)
            {
                var rsa = algorithm as Cryptography.Asymmetric.RSA;
                var scheme = rsa.Signature.GetScheme("EMSA-PKCS1-v1.5");
                scheme.SetParameter(si.DigestAlgorithm.Algorithm);
                byte[] digest = Digest(si.DigestAlgorithm, si.SignedAttributesRaw);
                return scheme.Verify(si.Signature, digest);
            }
            else
                throw new NotImplementedException();
        }

        private bool ValidateMessageDigest(SignerInfo si)
        {
            var messageDigestAttribute = si.SignedAttributes
                .SingleOrDefault(a => a.Type == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS9.MessageDigest);
            if (messageDigestAttribute == null)
                return false;

            byte[] digest1 = Digest(si.DigestAlgorithm, _signedData.EncapContentInfo.Content);
            byte[] digest2 = ((OctetString)messageDigestAttribute.Values[0]).Data;
            return ByteArray.Equals(digest1, digest2);
        }

        private byte[] Digest(AlgorithmIdentifier ai, byte[] data)
        {
            Cryptography.Hash.AbstractHash hash;
            if (ai.Algorithm == OID.JointISOITUT.Country.US.Organization.Gov.CSOR.NISTAlgorithm.HashAlgs.SHA256)
            {
                hash = new Cryptography.Hash.SHA256();
            }
            else
                throw new NotImplementedException();

            hash.Update(data);
            return hash.ComputeHash();
        }
    }
}