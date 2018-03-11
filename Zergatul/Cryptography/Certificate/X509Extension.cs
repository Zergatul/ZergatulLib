﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Certificate.Extensions;
using Zergatul.Network;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate
{
    public abstract class X509Extension
    {
        public OID ExtensionOID { get; protected set; }
        public bool Critical { get; protected set; }

        protected abstract void Parse(byte[] data);

        internal static X509Extension Parse(Network.Asn1.Structures.X509.Extension asn1raw)
        {
            OID oid = asn1raw.ID;

            X509Extension ext;
            if (oid == OID.ISO.IdentifiedOrganization.DOD.Internet.Security.Mechanisms.PKIX.PE.AuthorityInfoAccess)
                ext = new AuthorityInformationAccess();
            else if (oid.DotNotation == "1.3.6.1.4.1.11129.2.4.2")
                ext = new SignedCertificateTimestampList();
            else if (oid == OID.JointISOITUT.DS.CertificateExtension.SubjectKeyIdentifier)
                ext = new SubjectKeyIdentifier();
            else if (oid == OID.JointISOITUT.DS.CertificateExtension.KeyUsage)
                ext = new KeyUsage();
            else if (oid == OID.JointISOITUT.DS.CertificateExtension.SubjectAltName)
                ext = new SubjectAlternativeName();
            else if (oid == OID.JointISOITUT.DS.CertificateExtension.BasicConstraints)
                ext = new BasicConstraints();
            //else if (oid == OID.JointISOITUT.DS.CertificateExtension.NameConstraints)
            //    ext = new NameConstraints();
            else if (oid == OID.JointISOITUT.DS.CertificateExtension.CRLDistributionPoints)
                ext = new CRLDistributionPoints();
            else if (oid == OID.JointISOITUT.DS.CertificateExtension.CertificatePolicies)
                ext = new CertificatePolicies();
            else if (oid == OID.JointISOITUT.DS.CertificateExtension.AuthorityKeyIdentifier)
                ext = new AuthorityKeyIdentifier();
            else if (oid == OID.JointISOITUT.DS.CertificateExtension.ExtKeyUsage)
                ext = new ExtendedKeyUsage();
            else
                //throw new NotImplementedException();
                return null;

            ext.ExtensionOID = oid;
            ext.Critical = asn1raw.Critical;
            ext.Parse(asn1raw.Value);

            return ext;
        }
    }
}