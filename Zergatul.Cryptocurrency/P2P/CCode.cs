namespace Zergatul.Cryptocurrency.P2P
{
    public enum CCode
    {
        REJECT_MALFORMED = 0x01,
        REJECT_INVALID = 0x10,
        REJECT_OBSOLETE = 0x11,
        REJECT_DUPLICATE = 0x12,
        REJECT_NONSTANDARD = 0x40,
        REJECT_DUST = 0x41,
        REJECT_INSUFFICIENTFEE = 0x42,
        REJECT_CHECKPOINT = 0x43
    }
}