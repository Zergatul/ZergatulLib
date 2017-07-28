using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography
{
    public class OID
    {
        public string DotNotation { get; private set; }
        public string Identifier { get; private set; }
        public string ShortName { get; private set; }

        public OID(string dotnotation)
        {
            this.DotNotation = dotnotation;
        }

        public OID(string dotnotation, string identifier)
            : this(dotnotation)
        {
            this.Identifier = identifier;
        }

        public OID(string dotnotation, string identifier, string shortname)
            : this(dotnotation, identifier)
        {
            this.ShortName = shortname;
        }

        public static OID SHA224 = new OID("2.16.840.1.101.3.4.2.4");
        public static OID SHA256 = new OID("2.16.840.1.101.3.4.2.1");
        public static OID SHA384 = new OID("2.16.840.1.101.3.4.2.2");
        public static OID SHA512 = new OID("2.16.840.1.101.3.4.2.3");

        /// <summary>
        /// iso(1)
        /// </summary>
        public static class ISO
        {
            /// <summary>
            /// iso(1) member-body(2)
            /// </summary>
            public static class MemberBody
            {
                /// <summary>
                /// iso(1) member-body(2) us(840)
                /// </summary>
                public static class US
                {
                    /// <summary>
                    /// iso(1) member-body(2) us(840) rsadsi(113549)
                    /// </summary>
                    public static class RSADSI
                    {
                        /// <summary>
                        /// iso(1) member-body(2) us(840) rsadsi(113549) digestAlgorithm(2)
                        /// </summary>
                        public static class DigestAlgorithm
                        {
                            public static OID MD2 = new OID("1.2.840.113549.2.2", "md2");
                            public static OID MD4 = new OID("1.2.840.113549.2.4", "md4");
                            public static OID MD5 = new OID("1.2.840.113549.2.5", "md5");
                            public static OID HMACWithSHA1 = new OID("1.2.840.113549.2.7", "hmacWithSHA1");

                            public static IEnumerable<OID> All => new OID[]
                            {
                                MD2,
                                MD4,
                                MD5,
                                HMACWithSHA1
                            };
                        }

                        public static IEnumerable<OID> All => DigestAlgorithm.All;
                    }

                    public static IEnumerable<OID> All => RSADSI.All;
                }

                public static IEnumerable<OID> All => US.All;
            }

            /// <summary>
            /// iso(1) identified-organization(3)
            /// </summary>
            public static class IdentifiedOrganization
            {
                /// <summary>
                /// iso(1) identified-organization(3) oiw(14)
                /// </summary>
                public static class OIW
                {
                    /// <summary>
                    /// iso(1) identified-organization(3) oiw(14) secsig(3)
                    /// </summary>
                    public static class SECSIG
                    {
                        /// <summary>
                        /// iso(1) identified-organization(3) oiw(14) secsig(3) algorithms(2)
                        /// </summary>
                        public static class Algorithms
                        {
                            public static OID RSA = new OID("1.3.14.3.2.1", "rsa");
                            public static OID RSASignature = new OID("1.3.14.3.2.11", "rsaSignature");
                            public static OID DSA = new OID("1.3.14.3.2.12", "dsa");
                            public static OID SHA = new OID("1.3.14.3.2.18", "sha");
                            public static OID SHA1 = new OID("1.3.14.3.2.26", "sha1");

                            public static IEnumerable<OID> All => new OID[]
                            {
                                RSA,
                                RSASignature,
                                DSA,
                                SHA,
                                SHA1
                            };
                        }

                        public static IEnumerable<OID> All => Algorithms.All;
                    }

                    public static IEnumerable<OID> All => SECSIG.All;
                }

                public static IEnumerable<OID> All => OIW.All;
            }

            public static IEnumerable<OID> All => MemberBody.All.Concat(IdentifiedOrganization.All);
        }

        /// <summary>
        /// joint-iso-itu-t(2)
        /// </summary>
        public static class JointISOITUT
        {
            /// <summary>
            /// joint-iso-itu-t(2) ds(5)
            /// </summary>
            public static class DS
            {
                /// <summary>
                /// joint-iso-itu-t(2) ds(5) attributeType(4)
                /// </summary>
                public static class AttributeType
                {
                    public static OID CommonName = new OID("2.5.4.3", "commonName", "CN");
                    public static OID CountryName = new OID("2.5.4.6", "countryName", "C");
                    public static OID OrganizationName = new OID("2.5.4.10", "organizationName", "O");
                    public static OID OrganizationalUnitName = new OID("2.5.4.11", "organizationalUnitName", "OU");

                    public static IEnumerable<OID> All => new OID[]
                    {
                        CommonName,
                        CountryName,
                        OrganizationName,
                        OrganizationalUnitName
                    };
                }

                public static IEnumerable<OID> All => AttributeType.All;
            }

            /// <summary>
            /// joint-iso-itu-t(2) country(16)
            /// </summary>
            public static class Country
            {
                /// <summary>
                /// joint-iso-itu-t(2) country(16) us(840)
                /// </summary>
                public static class US
                {
                    /// <summary>
                    /// joint-iso-itu-t(2) country(16) us(840) organization(1)
                    /// </summary>
                    public static class Organization
                    {
                        /// <summary>
                        /// joint-iso-itu-t(2) country(16) us(840) organization(1) gov(101)
                        /// </summary>
                        public static class Gov
                        {
                            /// <summary>
                            /// joint-iso-itu-t(2) country(16) us(840) organization(1) gov(101) csor(3)
                            /// <para>Computer Security Objects Register (CSOR)</para>
                            /// </summary>
                            public static class CSOR
                            {
                                /// <summary>
                                /// joint-iso-itu-t(2) country(16) us(840) organization(1) gov(101) csor(3) nistAlgorithm(4)
                                /// </summary>
                                public static class NISTAlgorithm
                                {
                                    /// <summary>
                                    /// joint-iso-itu-t(2) country(16) us(840) organization(1) gov(101) csor(3) nistAlgorithm(4) hashAlgs(2)
                                    /// </summary>
                                    public static class HashAlgs
                                    {
                                        public static OID SHA256 = new OID("2.16.840.1.101.3.4.2.1", "sha256");
                                        public static OID SHA384 = new OID("2.16.840.1.101.3.4.2.2", "sha384");
                                        public static OID SHA512 = new OID("2.16.840.1.101.3.4.2.3", "sha512");
                                        public static OID SHA224 = new OID("2.16.840.1.101.3.4.2.4", "sha224");
                                        public static OID SHA512_224 = new OID("2.16.840.1.101.3.4.2.5", "sha512-224");
                                        public static OID SHA512_256 = new OID("2.16.840.1.101.3.4.2.6", "sha512-256");

                                        public static IEnumerable<OID> All => new OID[]
                                        {
                                            SHA256,
                                            SHA384,
                                            SHA512,
                                            SHA224,
                                            SHA512_224,
                                            SHA512_256
                                        };
                                    }

                                    public static IEnumerable<OID> All => HashAlgs.All;
                                }

                                public static IEnumerable<OID> All => NISTAlgorithm.All;
                            }

                            public static IEnumerable<OID> All => CSOR.All;
                        }

                        public static IEnumerable<OID> All => Gov.All;
                    }

                    public static IEnumerable<OID> All => Organization.All;
                }

                public static IEnumerable<OID> All => US.All;
            }

            public static IEnumerable<OID> All => DS.All.Concat(Country.All);
        }

        public static IEnumerable<OID> All => ISO.All.Concat(JointISOITUT.All);
    }
}