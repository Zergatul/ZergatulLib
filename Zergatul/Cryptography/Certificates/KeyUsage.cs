using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificates
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.2.1.3
    /// </summary>
    public class KeyUsage : X509Extension
    {
        /// <summary>
        /// The digitalSignature bit is asserted when the subject public key
        /// is used for verifying digital signatures, other than signatures on
        /// certificates (KeyCertSign) and CRLs (CRLSign), such as those used in an
        /// entity authentication service, a data origin authentication
        /// service, and/or an integrity service.
        /// </summary>
        public bool DigitalSignature { get; private set; }

        /// <summary>
        /// The nonRepudiation bit is asserted when the subject public key is
        /// used to verify digital signatures, other than signatures on
        /// certificates (KeyCertSign) and CRLs (CRLSign), used to provide a non-
        /// repudiation service that protects against the signing entity
        /// falsely denying some action.In the case of later conflict, a
        /// reliable third party may determine the authenticity of the signed
        /// data. (Note that recent editions of X.509 have renamed the
        /// nonRepudiation bit to contentCommitment.)
        /// </summary>
        public bool ContentCommitment { get; private set; }

        /// <summary>
        /// The keyEncipherment bit is asserted when the subject public key is
        /// used for enciphering private or secret keys, i.e., for key
        /// transport. For example, this bit shall be set when an RSA public
        /// key is to be used for encrypting a symmetric content-decryption
        /// key or an asymmetric private key.
        /// </summary>
        public bool KeyEncipherment { get; private set; }

        /// <summary>
        /// The dataEncipherment bit is asserted when the subject public key
        /// is used for directly enciphering raw user data without the use of
        /// an intermediate symmetric cipher. Note that the use of this bit
        /// is extremely uncommon; almost all applications use key transport
        /// or key agreement to establish a symmetric key.
        /// </summary>
        public bool DataEncipherment { get; private set; }

        /// <summary>
        /// The keyAgreement bit is asserted when the subject public key is
        /// used for key agreement. For example, when a Diffie-Hellman key is
        /// to be used for key management, then this bit is set.
        /// </summary>
        public bool KeyAgreement { get; private set; }

        /// <summary>
        /// The keyCertSign bit is asserted when the subject public key is
        /// used for verifying signatures on public key certificates. If the
        /// keyCertSign bit is asserted, then the cA bit in the basic
        /// constraints extension  MUST also be asserted.
        /// </summary>
        public bool KeyCertSign { get; private set; }

        /// <summary>
        /// The cRLSign bit is asserted when the subject public key is used
        /// for verifying signatures on certificate revocation lists (e.g.,
        /// CRLs, delta CRLs, or ARLs).
        /// </summary>
        public bool CRLSign { get; private set; }

        /// <summary>
        /// The meaning of the encipherOnly bit is undefined in the absence of
        /// the keyAgreement bit. When the encipherOnly bit is asserted and
        /// the keyAgreement bit is also set, the subject public key may be
        /// used only for enciphering data while performing key agreement.
        /// </summary>
        public bool EncipherOnly { get; private set; }

        /// <summary>
        /// The meaning of the decipherOnly bit is undefined in the absence of
        /// the keyAgreement bit. When the decipherOnly bit is asserted and
        /// the keyAgreement bit is also set, the subject public key may be
        /// used only for deciphering data while performing key agreement.
        /// </summary>
        public bool DecipherOnly { get; private set; }

        protected override void Parse(OctetString data)
        {
            var element = ASN1Element.ReadFrom(data.Raw);

            var bs = element as BitString;
            CertificateParseException.ThrowIfFalse(bs != null);
            CertificateParseException.ThrowIfFalse(bs.Data.Length == 1);

            DigitalSignature = (bs.Data[0] & 0x01) != 0;
            ContentCommitment = (bs.Data[0] & 0x02) != 0;
            KeyEncipherment = (bs.Data[0] & 0x04) != 0;
            DataEncipherment = (bs.Data[0] & 0x08) != 0;
            KeyAgreement = (bs.Data[0] & 0x10) != 0;
            KeyCertSign = (bs.Data[0] & 0x20) != 0;
            EncipherOnly = (bs.Data[0] & 0x40) != 0;
            DecipherOnly = (bs.Data[0] & 0x80) != 0;
        }
    }
}