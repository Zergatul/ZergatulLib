using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;

namespace Zergatul.Math.Elliptic
{
    public abstract class Curve
    {
        #region Static members

        #region Curves

        public static class Names
        {
            public const string prime192v1 = nameof(prime192v1);
            public const string prime192v2 = nameof(prime192v2);
            public const string prime192v3 = nameof(prime192v3);
            public const string prime239v1 = nameof(prime239v1);
            public const string prime239v2 = nameof(prime239v2);
            public const string prime239v3 = nameof(prime239v3);
            public const string prime256v1 = nameof(prime256v1);
            public const string c2pnb163v1 = nameof(c2pnb163v1);
            public const string c2pnb163v2 = nameof(c2pnb163v2);
            public const string c2pnb163v3 = nameof(c2pnb163v3);
            public const string c2pnb176v1 = nameof(c2pnb176v1);
            public const string c2tnb191v1 = nameof(c2tnb191v1);
            public const string c2tnb191v2 = nameof(c2tnb191v2);
            public const string c2tnb191v3 = nameof(c2tnb191v3);
            public const string c2onb191v4 = nameof(c2onb191v4);
            public const string c2onb191v5 = nameof(c2onb191v5);
            public const string c2pnb208w1 = nameof(c2pnb208w1);
            public const string c2tnb239v1 = nameof(c2tnb239v1);
            public const string c2tnb239v2 = nameof(c2tnb239v2);
            public const string c2tnb239v3 = nameof(c2tnb239v3);
            public const string c2onb239v4 = nameof(c2onb239v4);
            public const string c2onb239v5 = nameof(c2onb239v5);
            public const string c2pnb272w1 = nameof(c2pnb272w1);
            public const string c2pnb304w1 = nameof(c2pnb304w1);
            public const string c2tnb359v1 = nameof(c2tnb359v1);
            public const string c2pnb368w1 = nameof(c2pnb368w1);
            public const string c2tnb431r1 = nameof(c2tnb431r1);
            public const string secp112r1 = nameof(secp112r1);
            public const string secp112r2 = nameof(secp112r2);
            public const string secp128r1 = nameof(secp128r1);
            public const string secp128r2 = nameof(secp128r2);
            public const string secp160k1 = nameof(secp160k1);
            public const string secp160r1 = nameof(secp160r1);
            public const string secp160r2 = nameof(secp160r2);
            public const string secp192k1 = nameof(secp192k1);
            public const string secp224k1 = nameof(secp224k1);
            public const string secp224r1 = nameof(secp224r1);
            public const string secp256k1 = nameof(secp256k1);
            public const string secp384r1 = nameof(secp384r1);
            public const string secp521r1 = nameof(secp521r1);
            public const string sect113r1 = nameof(sect113r1);
            public const string sect113r2 = nameof(sect113r2);
            public const string sect131r1 = nameof(sect131r1);
            public const string sect131r2 = nameof(sect131r2);
            public const string sect163k1 = nameof(sect163k1);
            public const string sect163r1 = nameof(sect163r1);
            public const string sect163r2 = nameof(sect163r2);
            public const string sect193r1 = nameof(sect193r1);
            public const string sect193r2 = nameof(sect193r2);
            public const string sect233k1 = nameof(sect233k1);
            public const string sect233r1 = nameof(sect233r1);
            public const string sect239k1 = nameof(sect239k1);
            public const string sect283k1 = nameof(sect283k1);
            public const string sect283r1 = nameof(sect283r1);
            public const string sect409k1 = nameof(sect409k1);
            public const string sect409r1 = nameof(sect409r1);
            public const string sect571k1 = nameof(sect571k1);
            public const string sect571r1 = nameof(sect571r1);
            public const string brainpoolP160r1 = nameof(brainpoolP160r1);
            public const string brainpoolP160t1 = nameof(brainpoolP160t1);
            public const string brainpoolP192r1 = nameof(brainpoolP192r1);
            public const string brainpoolP192t1 = nameof(brainpoolP192t1);
            public const string brainpoolP224r1 = nameof(brainpoolP224r1);
            public const string brainpoolP224t1 = nameof(brainpoolP224t1);
            public const string brainpoolP256r1 = nameof(brainpoolP256r1);
            public const string brainpoolP256t1 = nameof(brainpoolP256t1);
            public const string brainpoolP320r1 = nameof(brainpoolP320r1);
            public const string brainpoolP320t1 = nameof(brainpoolP320t1);
            public const string brainpoolP384r1 = nameof(brainpoolP384r1);
            public const string brainpoolP384t1 = nameof(brainpoolP384t1);
            public const string brainpoolP512r1 = nameof(brainpoolP512r1);
            public const string brainpoolP512t1 = nameof(brainpoolP512t1);
        }

        public static class NIDs
        {
            public const int prime192v1 = 409;
            public const int prime192v2 = 410;
            public const int prime192v3 = 411;
            public const int prime239v1 = 412;
            public const int prime239v2 = 413;
            public const int prime239v3 = 414;
            public const int prime256v1 = 415;
            public const int c2pnb163v1 = 684;
            public const int c2pnb163v2 = 685;
            public const int c2pnb163v3 = 686;
            public const int c2pnb176v1 = 687;
            public const int c2tnb191v1 = 688;
            public const int c2tnb191v2 = 689;
            public const int c2tnb191v3 = 690;
            public const int c2onb191v4 = 691;
            public const int c2onb191v5 = 692;
            public const int c2pnb208w1 = 693;
            public const int c2tnb239v1 = 694;
            public const int c2tnb239v2 = 695;
            public const int c2tnb239v3 = 696;
            public const int c2onb239v4 = 697;
            public const int c2onb239v5 = 698;
            public const int c2pnb272w1 = 699;
            public const int c2pnb304w1 = 700;
            public const int c2tnb359v1 = 701;
            public const int c2pnb368w1 = 702;
            public const int c2tnb431r1 = 703;
            public const int secp112r1 = 704;
            public const int secp112r2 = 705;
            public const int secp128r1 = 706;
            public const int secp128r2 = 707;
            public const int secp160k1 = 708;
            public const int secp160r1 = 709;
            public const int secp160r2 = 710;
            public const int secp192k1 = 711;
            public const int secp224k1 = 712;
            public const int secp224r1 = 713;
            public const int secp256k1 = 714;
            public const int secp384r1 = 715;
            public const int secp521r1 = 716;
            public const int sect113r1 = 717;
            public const int sect113r2 = 718;
            public const int sect131r1 = 719;
            public const int sect131r2 = 720;
            public const int sect163k1 = 721;
            public const int sect163r1 = 722;
            public const int sect163r2 = 723;
            public const int sect193r1 = 724;
            public const int sect193r2 = 725;
            public const int sect233k1 = 726;
            public const int sect233r1 = 727;
            public const int sect239k1 = 728;
            public const int sect283k1 = 729;
            public const int sect283r1 = 730;
            public const int sect409k1 = 731;
            public const int sect409r1 = 732;
            public const int sect571k1 = 733;
            public const int sect571r1 = 734;
            public const int brainpoolP160r1 = 921;
            public const int brainpoolP160t1 = 922;
            public const int brainpoolP192r1 = 923;
            public const int brainpoolP192t1 = 924;
            public const int brainpoolP224r1 = 925;
            public const int brainpoolP224t1 = 926;
            public const int brainpoolP256r1 = 927;
            public const int brainpoolP256t1 = 928;
            public const int brainpoolP320r1 = 929;
            public const int brainpoolP320t1 = 930;
            public const int brainpoolP384r1 = 931;
            public const int brainpoolP384t1 = 932;
            public const int brainpoolP512r1 = 933;
            public const int brainpoolP512t1 = 934;
        }

        public static class OIDs
        {
            public static OID prime192v1 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Prime.prime192v1;
            public static OID prime192v2 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Prime.prime192v2;
            public static OID prime192v3 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Prime.prime192v3;
            public static OID prime239v1 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Prime.prime239v1;
            public static OID prime239v2 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Prime.prime239v2;
            public static OID prime239v3 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Prime.prime239v3;
            public static OID prime256v1 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Prime.prime256v1;
            public static OID c2pnb163v1 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2pnb163v1;
            public static OID c2pnb163v2 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2pnb163v2;
            public static OID c2pnb163v3 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2pnb163v3;
            public static OID c2pnb176v1 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2pnb176v1;
            public static OID c2tnb191v1 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2tnb191v1;
            public static OID c2tnb191v2 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2tnb191v2;
            public static OID c2tnb191v3 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2tnb191v3;
            public static OID c2onb191v4 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2onb191v4;
            public static OID c2onb191v5 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2onb191v5;
            public static OID c2pnb208w1 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2pnb208w1;
            public static OID c2tnb239v1 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2tnb239v1;
            public static OID c2tnb239v2 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2tnb239v2;
            public static OID c2tnb239v3 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2tnb239v3;
            public static OID c2onb239v4 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2onb239v4;
            public static OID c2onb239v5 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2onb239v5;
            public static OID c2pnb272w1 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2pnb272w1;
            public static OID c2pnb304w1 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2pnb304w1;
            public static OID c2tnb359v1 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2tnb359v1;
            public static OID c2pnb368w1 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2pnb368w1;
            public static OID c2tnb431r1 => OID.ISO.MemberBody.US.ANSI_X962.Curves.Char2.c2tnb431r1;
            public static OID secp112r1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.secp112r1;
            public static OID secp112r2 => OID.ISO.IdentifiedOrganization.Certicom.Curve.secp112r2;
            public static OID secp128r1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.secp128r1;
            public static OID secp128r2 => OID.ISO.IdentifiedOrganization.Certicom.Curve.secp128r2;
            public static OID secp160k1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.secp160k1;
            public static OID secp160r1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.secp160r1;
            public static OID secp160r2 => OID.ISO.IdentifiedOrganization.Certicom.Curve.secp160r2;
            public static OID secp192k1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.secp192k1;
            public static OID secp224k1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.secp224k1;
            public static OID secp224r1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.secp224r1;
            public static OID secp256k1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.secp256k1;
            public static OID secp384r1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.secp384r1;
            public static OID secp521r1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.secp521r1;
            public static OID sect113r1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect113r1;
            public static OID sect113r2 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect113r2;
            public static OID sect131r1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect131r1;
            public static OID sect131r2 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect131r2;
            public static OID sect163k1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect163k1;
            public static OID sect163r1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect163r1;
            public static OID sect163r2 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect163r2;
            public static OID sect193r1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect193r1;
            public static OID sect193r2 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect193r2;
            public static OID sect233k1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect233k1;
            public static OID sect233r1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect233r1;
            public static OID sect239k1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect239k1;
            public static OID sect283k1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect283k1;
            public static OID sect283r1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect283r1;
            public static OID sect409k1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect409k1;
            public static OID sect409r1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect409r1;
            public static OID sect571k1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect571k1;
            public static OID sect571r1 => OID.ISO.IdentifiedOrganization.Certicom.Curve.sect571r1;
            public static OID brainpoolP160r1 => OID.ISO.IdentifiedOrganization.Teletrust.Algorithm.SignatureAlgorithm.ECSign.ECStdCurvesAndGeneration.EllipticCurve.VersionOne.brainpoolP160r1;
            public static OID brainpoolP160t1 => OID.ISO.IdentifiedOrganization.Teletrust.Algorithm.SignatureAlgorithm.ECSign.ECStdCurvesAndGeneration.EllipticCurve.VersionOne.brainpoolP160t1;
            public static OID brainpoolP192r1 => OID.ISO.IdentifiedOrganization.Teletrust.Algorithm.SignatureAlgorithm.ECSign.ECStdCurvesAndGeneration.EllipticCurve.VersionOne.brainpoolP192r1;
            public static OID brainpoolP192t1 => OID.ISO.IdentifiedOrganization.Teletrust.Algorithm.SignatureAlgorithm.ECSign.ECStdCurvesAndGeneration.EllipticCurve.VersionOne.brainpoolP192t1;
            public static OID brainpoolP224r1 => OID.ISO.IdentifiedOrganization.Teletrust.Algorithm.SignatureAlgorithm.ECSign.ECStdCurvesAndGeneration.EllipticCurve.VersionOne.brainpoolP224r1;
            public static OID brainpoolP224t1 => OID.ISO.IdentifiedOrganization.Teletrust.Algorithm.SignatureAlgorithm.ECSign.ECStdCurvesAndGeneration.EllipticCurve.VersionOne.brainpoolP224t1;
            public static OID brainpoolP256r1 => OID.ISO.IdentifiedOrganization.Teletrust.Algorithm.SignatureAlgorithm.ECSign.ECStdCurvesAndGeneration.EllipticCurve.VersionOne.brainpoolP256r1;
            public static OID brainpoolP256t1 => OID.ISO.IdentifiedOrganization.Teletrust.Algorithm.SignatureAlgorithm.ECSign.ECStdCurvesAndGeneration.EllipticCurve.VersionOne.brainpoolP256t1;
            public static OID brainpoolP320r1 => OID.ISO.IdentifiedOrganization.Teletrust.Algorithm.SignatureAlgorithm.ECSign.ECStdCurvesAndGeneration.EllipticCurve.VersionOne.brainpoolP320r1;
            public static OID brainpoolP320t1 => OID.ISO.IdentifiedOrganization.Teletrust.Algorithm.SignatureAlgorithm.ECSign.ECStdCurvesAndGeneration.EllipticCurve.VersionOne.brainpoolP320t1;
            public static OID brainpoolP384r1 => OID.ISO.IdentifiedOrganization.Teletrust.Algorithm.SignatureAlgorithm.ECSign.ECStdCurvesAndGeneration.EllipticCurve.VersionOne.brainpoolP384r1;
            public static OID brainpoolP384t1 => OID.ISO.IdentifiedOrganization.Teletrust.Algorithm.SignatureAlgorithm.ECSign.ECStdCurvesAndGeneration.EllipticCurve.VersionOne.brainpoolP384t1;
            public static OID brainpoolP512r1 => OID.ISO.IdentifiedOrganization.Teletrust.Algorithm.SignatureAlgorithm.ECSign.ECStdCurvesAndGeneration.EllipticCurve.VersionOne.brainpoolP512r1;
            public static OID brainpoolP512t1 => OID.ISO.IdentifiedOrganization.Teletrust.Algorithm.SignatureAlgorithm.ECSign.ECStdCurvesAndGeneration.EllipticCurve.VersionOne.brainpoolP512t1;
        }

        #endregion

        public static Curve ByNID(int id)
        {
            return null;
        }

        public static Curve ByName(string name)
        {
            return null;
        }

        public static Curve ByOID(OID oid)
        {
            return null;
        }

        #endregion

        #region Instance members

        public int? NID { get; protected set; }
        public string Name { get; protected set; }
        public OID OID { get; protected set; }

        public byte[] A { get; protected set; }
        public byte[] B { get; protected set; }
        public byte[] Order { get; protected set; }

        #endregion
    }

    public abstract class Curve<T> : Curve where T : Point
    {
        #region Instance members

        public T G { get; protected set; }

        public abstract T Multiplication(byte[] value);
        public abstract T Multiplication(T point, byte[] value);
        public abstract T Double(T point);

        #endregion
    }
}