namespace Zergatul.Security.Zergatul.Tls
{
    static class TlsConstants
    {
        public const int RecordLayerHeaderLength = 5;
        public const int RecordLayerLimit = 0x4000;
        public const int TlsCiphertextMaxLength = 0x4800;
    }
}