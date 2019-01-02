using Zergatul.Cryptography.Hash.Base;
using Zergatul.Network;

namespace Zergatul.Cryptography.Hash
{
    public class SHA224 : SHA2_32Bit
    {
        public override int HashSize => 28;
        public override OID OID => OID.JointISOITUT.Country.US.Organization.Gov.CSOR.NISTAlgorithm.HashAlgs.SHA224;

        protected override void Init()
        {
            h0 = 0xC1059ED8;
            h1 = 0x367CD507;
            h2 = 0x3070DD17;
            h3 = 0xF70E5939;
            h4 = 0xFFC00B31;
            h5 = 0x68581511;
            h6 = 0x64F98FA7;
            h7 = 0xBEFA4FA4;
        }
    }
}