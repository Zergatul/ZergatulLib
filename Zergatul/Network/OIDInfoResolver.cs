using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;
using Zergatul.Math.EllipticCurves;

namespace Zergatul.Network
{
    public static class OIDInfoResolver
    {
        private static HashSet<OID> _sha1 = new HashSet<OID>
        {
            OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.SHA1WithRSA
        };
        private static HashSet<OID> _sha224 = new HashSet<OID>
        {
            OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.SHA224WithRSA,
            OID.ISO.MemberBody.US.ANSI_X962.Signatures.ECDSAWithSHA2.ECDSAWithSHA224,
        };
        private static HashSet<OID> _sha256 = new HashSet<OID>
        {
            OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.SHA256WithRSA,
            OID.ISO.MemberBody.US.ANSI_X962.Signatures.ECDSAWithSHA2.ECDSAWithSHA256,
        };
        private static HashSet<OID> _sha384 = new HashSet<OID>
        {
            OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.SHA384WithRSA,
            OID.ISO.MemberBody.US.ANSI_X962.Signatures.ECDSAWithSHA2.ECDSAWithSHA384,
        };
        private static HashSet<OID> _sha512 = new HashSet<OID>
        {
            OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.SHA512WithRSA,
            OID.ISO.MemberBody.US.ANSI_X962.Signatures.ECDSAWithSHA2.ECDSAWithSHA512,
        };
        public static AbstractHash GetHash(OID oid)
        {
            if (_sha1.Contains(oid))
                return new SHA1();
            if (_sha224.Contains(oid))
                return new SHA224();
            if (_sha256.Contains(oid))
                return new SHA256();
            if (_sha384.Contains(oid))
                return new SHA384();
            if (_sha512.Contains(oid))
                return new SHA512();

            return null;
        }

        private static HashSet<OID> _rsa = new HashSet<OID>
        {
            OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.SHA1WithRSA,
            OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.SHA224WithRSA,
            OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.SHA256WithRSA,
            OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.SHA384WithRSA,
            OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.SHA512WithRSA,
        };
        public static bool IsRSA(OID oid) => _rsa.Contains(oid);

        private static HashSet<OID> _ecdsa = new HashSet<OID>
        {
            OID.ISO.MemberBody.US.ANSI_X962.Signatures.ECDSAWithSHA2.ECDSAWithSHA224,
            OID.ISO.MemberBody.US.ANSI_X962.Signatures.ECDSAWithSHA2.ECDSAWithSHA256,
            OID.ISO.MemberBody.US.ANSI_X962.Signatures.ECDSAWithSHA2.ECDSAWithSHA384,
            OID.ISO.MemberBody.US.ANSI_X962.Signatures.ECDSAWithSHA2.ECDSAWithSHA512,
        };
        public static bool IsECDSA(OID oid) => _ecdsa.Contains(oid);

        private static Dictionary<OID, IEllipticCurve> _curves = new Dictionary<OID, IEllipticCurve>
        {
            [OID.ISO.MemberBody.US.ANSI_X962.Curves.Prime.Prime256v1] = Math.EllipticCurves.PrimeField.EllipticCurve.secp256r1,
            [OID.ISO.IdentifiedOrganization.Certicom.Curve.secp384r1] = Math.EllipticCurves.PrimeField.EllipticCurve.secp384r1,
            [OID.ISO.IdentifiedOrganization.Certicom.Curve.secp521r1] = Math.EllipticCurves.PrimeField.EllipticCurve.secp521r1,
        };
        public static IEllipticCurve GetCurve(OID oid)
        {
            IEllipticCurve curve;
            if (_curves.TryGetValue(oid, out curve))
                return curve;
            else
                return null;
        }
    }
}