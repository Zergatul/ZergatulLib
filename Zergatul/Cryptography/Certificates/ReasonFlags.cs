using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificates
{
    public class ReasonFlags
    {
        public bool KeyCompromise { get; private set; }
        public bool CACompromise { get; private set; }
        public bool AffiliationChanged { get; private set; }
        public bool Superseded { get; private set; }
        public bool CessationOfOperation { get; private set; }
        public bool CertificateHold { get; private set; }
        public bool PrivilegeWithdrawn { get; private set; }
        public bool AACompromise { get; private set; }

        internal ReasonFlags(ASN1Element element)
        {
            var bs = element as BitString;
            CertificateParseException.ThrowIfFalse(bs != null);
            CertificateParseException.ThrowIfFalse(bs.Data.Length == 2);

            KeyCompromise = (bs.Data[0] & 0x02) != 0;
            CACompromise = (bs.Data[0] & 0x04) != 0;
            AffiliationChanged = (bs.Data[0] & 0x08) != 0;
            Superseded = (bs.Data[0] & 0x10) != 0;
            CessationOfOperation = (bs.Data[0] & 0x20) != 0;
            CertificateHold = (bs.Data[0] & 0x40) != 0;
            PrivilegeWithdrawn = (bs.Data[0] & 0x80) != 0;

            AACompromise = (bs.Data[1] & 0x01) != 0;
        }
    }
}